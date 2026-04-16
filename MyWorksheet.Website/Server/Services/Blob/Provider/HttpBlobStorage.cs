using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MyWorksheet.Helper;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.ExternalDomainValidator;
using MyWorksheet.Website.Server.Services.StreamPool;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Services.Blob.Provider;

public class HttpBlobStorageProvider : BlobProviderBase
{
    private readonly ILocalFileStreamPoolService _fileStreamPoolService;
    private readonly IExternalDomainValidator _externalDomainValidator;
    public HttpBlobStorageProvider(Guid storageInstance, StorageProviderData[] data,
        IDbContextFactory<MyworksheetContext> dbContextFactory,
        IExternalDomainValidator externalDomainValidator,
        ILocalFileStreamPoolService fileStreamPoolService) : base(storageInstance, data, dbContextFactory)
    {
        _fileStreamPoolService = fileStreamPoolService;
        _externalDomainValidator = externalDomainValidator;
    }

    private enum HttpTypes
    {
        Post,
        Get,
        Delete
    }

    public override string HelpText(Guid appUserId)
    {
        return "This Provider will use another Http capable server to store data. " +
               "Please be aware that your url might be subject to blacklisting if any abuse is detected. " +
               "If the server will return one of the following Http codes while doing any action, " +
               "the Storage item means the file will be deleted from my-worksheet. " +
               "It might remain on your http server but it will not be accessible from my-worksheet anymore:" +
               _leadsToDelete.Select(e => e.ToString()).Aggregate((e, f) => e + Environment.NewLine + f);
    }

    private string GetUrlFor(HttpTypes type)
    {
        var baseUrl = GetDataFromStore(nameof(HttpBlobStorageArguments.BaseUrl)).Trim();
        var url = "";
        switch (type)
        {
            case HttpTypes.Post:
                url = GetDataFromStore(nameof(HttpBlobStorageArguments.PostUrl)).Trim();
                break;
            case HttpTypes.Get:
                url = GetDataFromStore(nameof(HttpBlobStorageArguments.GetUrl)).Trim();
                break;
            case HttpTypes.Delete:
                url = GetDataFromStore(nameof(HttpBlobStorageArguments.DeleteUrl)).Trim();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        if (baseUrl.EndsWith("/"))
        {
            baseUrl = baseUrl.Remove(baseUrl.Length - 1);
        }

        if (url.StartsWith("/"))
        {
            url = url.Remove(0, 1);
        }

        var constructedUrl = baseUrl + "/" + url;
        return constructedUrl;
    }

    private Task<Uri> GetAndCheck(HttpTypes type, Guid appUserId, string id)
    {
        var url = GetUrlFor(HttpTypes.Get);
        if (url == null || !url.Contains("{{id}}"))
        {
            throw new InvalidOperationException($"Missing Id parameter in Url '{url}'");
        }

        var uri = new Uri(url.Replace("{{id}}", id.ToString()), UriKind.RelativeOrAbsolute);

        var checkedUrl = _externalDomainValidator.ValidateUrl(uri.Host, "http", "https");

        if (checkedUrl != null)
        {
            throw new InvalidOperationException($"{checkedUrl}");
        }

        return Task.FromResult(uri);
    }

    private readonly HttpStatusCode[] _leadsToDelete = new HttpStatusCode[]
    {
        HttpStatusCode.BadRequest,
        HttpStatusCode.ExpectationFailed,
        HttpStatusCode.Gone,
        HttpStatusCode.NotFound,
        HttpStatusCode.Redirect,
        HttpStatusCode.RedirectKeepVerb,
        HttpStatusCode.RedirectMethod,
        HttpStatusCode.TemporaryRedirect,
        HttpStatusCode.Moved,
        HttpStatusCode.MovedPermanently,
        HttpStatusCode.NotAcceptable,
        HttpStatusCode.SeeOther,
        HttpStatusCode.Unauthorized,
        HttpStatusCode.UnsupportedMediaType,
        HttpStatusCode.UseProxy,
    };



    public static void SetCommonHeaders(HttpClient client)
    {
        client.Timeout = TimeSpan.FromSeconds(12);
        client.DefaultRequestHeaders.MaxForwards = 10;
        client.DefaultRequestHeaders.Host = "www.my-worksheet.com";
        client.DefaultRequestHeaders.Referrer = new Uri("https://www.my-worksheet.com");
        client.DefaultRequestHeaders.From = "abuse@my-worksheet.com";
        client.DefaultRequestHeaders.Date = DateTimeOffset.UtcNow;
    }

    public override async Task<BlobProviderGetResult> GetData(Guid id, Guid appUserId, MyworksheetContext db)
    {
        var storageEntry = db.StorageEntries.Find(id);
        var uri = await GetAndCheck(HttpTypes.Get, appUserId, storageEntry.StorageKey);

        using (var httpClient = new HttpClient())
        {
            SetCommonHeaders(httpClient);
            HttpResponseMessage httpResponseMessage = null;
            try
            {
                using (httpResponseMessage = await httpClient.GetAsync(uri))
                {
                    httpResponseMessage.EnsureSuccessStatusCode();
                    var streamingType = _fileStreamPoolService.GetLocalStream("HTTP_BLOB", appUserId, 0);
                    var openProducerStream = streamingType.OpenProducerStream(id.ToString());
                    using (var stream = await httpResponseMessage.Content.ReadAsStreamAsync())
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        await stream.CopyToAsync(openProducerStream);
                    }

                    openProducerStream.Seek(0, SeekOrigin.Begin);
                    return new BlobProviderGetResult(openProducerStream);
                }
            }
            catch (Exception)
            {
                if (httpResponseMessage != null)
                {
                    if (_leadsToDelete.Contains(httpResponseMessage.StatusCode))
                    {
                        return new BlobProviderGetResult(true);
                    }
                }
                await _externalDomainValidator.TryCallDomain(uri.Host, appUserId);
                return new BlobProviderGetResult(false);
            }
        }
    }

    public override async Task<StorageEntry> SetData(BlobData data, Guid appUserId, StorageEntityType entityType, MyworksheetContext db)
    {
        StorageEntry storageEntry = null;
        storageEntry = CreateFromBlob(db, data, entityType, appUserId);
        db.Add(storageEntry);
        var uri = await GetAndCheck(HttpTypes.Get, appUserId, storageEntry.StorageKey);
        using (var httpClient = new HttpClient())
        {
            SetCommonHeaders(httpClient);
            HttpResponseMessage httpResponseMessage = null;
            try
            {
                using (httpResponseMessage = await httpClient.PostAsync(uri, new StreamContent(data.DataStream)))
                {
                    httpResponseMessage.EnsureSuccessStatusCode();
                }
            }
            catch (Exception)
            {
                if (httpResponseMessage != null)
                {
                    if (_leadsToDelete.Contains(httpResponseMessage.StatusCode))
                    {
                        DeleteStorageItem(storageEntry.StorageEntryId);
                    }
                }
                await _externalDomainValidator.TryCallDomain(uri.Host, appUserId);
            }
        }
        return storageEntry;
    }

    public override async Task DeleteData(Guid id, Guid appUserId, MyworksheetContext db)
    {
        var storageEntry = db.StorageEntries.Find(id);
        var uri = await GetAndCheck(HttpTypes.Get, appUserId, storageEntry.StorageKey);
        using (var httpClient = new HttpClient())
        {
            SetCommonHeaders(httpClient);
            HttpResponseMessage httpResponseMessage = null;
            try
            {
                using (httpResponseMessage = await httpClient.DeleteAsync(uri))
                {
                    httpResponseMessage.EnsureSuccessStatusCode();
                    DeleteStorageItem(storageEntry.StorageEntryId);
                }
            }
            catch (Exception)
            {
                if (httpResponseMessage != null)
                {
                    if (_leadsToDelete.Contains(httpResponseMessage.StatusCode))
                    {
                        DeleteStorageItem(storageEntry.StorageEntryId);
                    }
                }
                await _externalDomainValidator.TryCallDomain(uri.Host, appUserId);
            }
        }
    }

    public override long MaxSizeInBytes { get; protected set; }
    public override IObjectSchema GetSchema()
    {
        return JsonSchemaExtensions.JsonSchema(new HttpBlobStorageArguments());
    }
}

public class HttpBlobStorageArguments : ArgumentsBase
{
    [JsonComment("Storage/Http.Arguments.Comments.BaseUrl")]
    public string BaseUrl { get; set; } = "https://test.com";

    [JsonComment("Storage/Http.Arguments.Comments.GetUrl")]
    public string GetUrl { get; set; } = "/Get?externalId={{id}}";

    [JsonComment("Storage/Http.Arguments.Comments.DeleteUrl")]
    public string DeleteUrl { get; set; } = "/Delete?externalId={{id}}";

    [JsonComment("Storage/Http.Arguments.Comments.PostUrl")]
    public string PostUrl { get; set; } = "/Create?externalId={{id}}";
}