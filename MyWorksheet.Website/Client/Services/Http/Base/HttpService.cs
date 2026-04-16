using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using MyWorksheet.Website.Client.Services.Navigation;
using MyWorksheet.Website.Client.Services.WaiterIndicator;
using MyWorksheet.Website.Shared.Services.Activation;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.Http.Base;

[SingletonService()]
public class HttpService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationService _navigationService;

    private JsonSerializerSettings _jsonSerializerSettings;

    public HttpService(HttpClient httpClient, NavigationService navigationService)
    {
        _jsonSerializerSettings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        };

        _formatters = new[]
        {
            JsonFormatter = new JsonMediaTypeFormatter()
            {
                SerializerSettings = _jsonSerializerSettings
            }
        };

        _httpClient = httpClient;
        _navigationService = navigationService;
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _accessesors = new List<HttpAccessBase>();
        foreach (var propertyInfo in GetType().GetProperties().Where(e => typeof(HttpAccessBase).IsAssignableFrom(e.PropertyType)))
        {
            var instance = Activator.CreateInstance(propertyInfo.PropertyType, this) as HttpAccessBase;
            _accessesors.Add(instance);
            propertyInfo.SetValue(this, instance);
        }
    }

    private IList<HttpAccessBase> _accessesors;

    public SchedulerApiAccess SchedulerApiAccess { get; set; }
    public RegionApiAccess RegionApiAccess { get; set; }
    public ProjectTrackerApiAccess ProjectTrackerApiAccess { get; set; }
    public LoggerApiAccess LoggerApiAccess { get; set; }
    public NumberRangeApiAccess NumberRangeApiAccess { get; set; }
    public PaymentInfoApiAccess PaymentInfoApiAccess { get; set; }
    public UserApiAccess UserApiAccess { get; set; }
    public AuthApiAccess AuthApiAccess { get; set; }
    public TextApiAccess TextApiAccess { get; set; }
    public ProjectApiAccess ProjectApiAccess { get; set; }
    public OrganizationApiAccess OrganizationApiAccess { get; set; }
    public UserWorkloadApiAccess UserWorkloadApiAccess { get; set; }
    public ProjectItemRateApiAccess ProjectItemRateApiAccess { get; set; }
    public WorksheetWorkflowApiAccess WorksheetWorkflowApiAccess { get; set; }
    public WorksheetWorkflowDataApiAccess WorksheetWorkflowDataApiAccess { get; set; }
    public WorksheetApiAccess WorksheetApiAccess { get; set; }
    public WorksheetItemApiAccess WorksheetItemApiAccess { get; set; }
    public WorksheetHistoryApiAccess WorksheetHistoryApiAccess { get; set; }
    public StorageApiAccess StorageApiAccess { get; set; }
    public WorksheetAssertApiAccess WorksheetAssertApiAccess { get; set; }
    public AccountApiAccess AccountApiAccess { get; set; }
    public AddressApiAccess AddressApiAccess { get; set; }
    public AccountAssociationUserApiAccess AccountAssociationUserApiAccess { get; set; }
    public UserAppSettingsApiAccess UserAppSettingsApiAccess { get; set; }
    public WebhookApiAccess WebhookApiAccess { get; set; }
    public ActivityApiAccess ActivityApiAccess { get; set; }
    public TemplateManagementApiAccess TemplateManagementApiAccess { get; set; }
    public ProjectBudgetApiAccess ProjectBudgetApiAccess { get; set; }
    public ReportManagementApiAccess ReportManagementApiAccess { get; set; }
    public EMailAccountApiAccess EMailAccountApiAccess { get; set; }
    public StatisticsApiAccess StatisticsApiAccess { get; set; }
    public DashboardApiAccess DashboardApiAccess { get; set; }

    public HttpClient Client()
    {
        return _httpClient;
    }

    public MediaTypeFormatter JsonFormatter { get; }
    private IEnumerable<MediaTypeFormatter> _formatters;
    public IEnumerable<MediaTypeFormatter> GetSupportedMediaTypeFormatters()
    {
        return _formatters;
    }

    public JsonSerializerSettings GetJsonSerializerSettings()
    {
        return _jsonSerializerSettings;
    }

    public void SetToken(string tokenToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", tokenToken);
        //_client.DefaultRequestHeaders.TryAddWithoutValidation("Authentification", "Bearer " + tokenToken);
    }

    public RestHttpAccessBase<TEntity> For<TEntity>()
    {
        return _accessesors.OfType<RestHttpAccessBase<TEntity>>().FirstOrDefault() ?? throw new InvalidOperationException($"Cannot get an repository for {typeof(TEntity)}");
    }

    public void CheckForUnauthorizedAccess(HttpResponseMessage message)
    {
        if (message.StatusCode == HttpStatusCode.Unauthorized)
        {
            _navigationService.NavigateTo("/Unauthorized");
        }
    }

    public void SignalChangeId(string connectionId)
    {
        _httpClient.DefaultRequestHeaders.Remove("x-mw-changeTrackingId");
        _httpClient.DefaultRequestHeaders.Add("x-mw-changeTrackingId", connectionId);
    }
}