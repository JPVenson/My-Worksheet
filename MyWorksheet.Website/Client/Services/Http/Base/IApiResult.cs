using System.Net;

namespace MyWorksheet.Website.Client.Services.Http.Base;

public interface IApiResult
{
    HttpStatusCode StatusCode { get; }
    bool Success { get; }
    string StatusMessage { get; }
    object ErrorResult { get; }
}