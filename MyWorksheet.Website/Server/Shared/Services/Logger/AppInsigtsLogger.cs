using System;
using System.Collections.Generic;
using System.Threading;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;

namespace MyWorksheet.Website.Server.Shared.Services.Logger;
//public class DatabaseLogger : ILoggerDelegation
//{
//	public DatabaseLogger(string filterCategory, string filterLevel, string filterMessage, string logInsertCommand)
//		: this(logInsertCommand)
//	{
//		LogFilterCategory = (f) => filterCategory == null || filterCategory.Contains(f);
//		LogFilterLevel = (f) => filterLevel == null || filterLevel.Contains(f);
//		LogFilterMessage = (f) => filterMessage == null || filterMessage.Contains(f);
//	}

//	public DatabaseLogger(Func<string, bool> logFilterCategory, Func<string, bool> logFilterLevel, Func<string, bool> logFilterMessage, string logInsertCommand)
//		: this(logInsertCommand)
//	{
//		LogFilterCategory = logFilterCategory ?? ((f) => true);
//		LogFilterLevel = logFilterLevel ?? ((f) => true);
//		LogFilterMessage = logFilterMessage ?? ((f) => true);
//	}

//	private DatabaseLogger(string logInsertCommand)
//	{
//		LogInsertCommand = logInsertCommand;
//		Db = IoC.Resolve<DbEntities>();
//	}

//	public Func<string, bool> LogFilterCategory { get; private set; }
//	public Func<string, bool> LogFilterLevel { get; private set; }
//	public Func<string, bool> LogFilterMessage { get; private set; }

//	public string LogInsertCommand { get; private set; }
//	public DbEntities Db { get; set; }

//	public void Log(string message, string category, string level, IDictionary<string, string> optionalData, DateTime dateCreated)
//	{
//		if (!LogFilterCategory(category) || !LogFilterLevel(level) || !LogFilterMessage(message))
//		{
//			return;
//		}
//		Db.Database.Run((d) =>
//		{
//			var query = d.CreateCommand(LogInsertCommand, new SqlParameter("@message", SqlDbType.NVarChar) {Value = message},
//			new SqlParameter("@category", SqlDbType.NVarChar) {Value = category},
//			new SqlParameter("@level", SqlDbType.NVarChar) {Value = level},
//			new SqlParameter("@dateCreated", SqlDbType.DateTime2) {Value = dateCreated},
//			new SqlParameter("@optData", SqlDbType.NVarChar) {Value = JsonConvert.SerializeObject(optionalData)});

//			d.ExecuteNonQuery(query);
//		});
//	}

//	public void Log(string message, string category, string level, IDictionary<string, string> optionalData)
//	{
//		Log(message, category, level, optionalData, DateTime.UtcNow);
//	}

//	public void Log(string message, string category, string level)
//	{
//		Log(message, category, level, new Dictionary<string, string>(), DateTime.UtcNow);
//	}

//	public void Log(string message, string category)
//	{
//		Log(message, category, null);
//	}

//	public void Log(string message)
//	{
//		Log(message, null);
//	}

//	public void Started()
//	{
//	}
//}

public class AppInsigtsLogger : ILoggerDelegation
{
    private readonly IAppInsightsProviderService _client;

    private static AsyncLocal<AppInsightsConstFrame<ITelemetry>> _current;

    public AppInsigtsLogger(IAppInsightsProviderService appInsightsProviderService)
    {
        _client = appInsightsProviderService;
    }

    static AppInsigtsLogger()
    {
        _current = new AsyncLocal<AppInsightsConstFrame<ITelemetry>>();
    }

    public AppInsightsConstFrame<ITelemetry> BeginFrame(Action<ITelemetry> frameAction)
    {
        _current.Value = new AppInsightsConstFrame<ITelemetry>(frameAction, () => _current.Value = null);
        return _current.Value;
    }

    public void Log(string message, string category, string level, IDictionary<string, string> optionalData, DateTime dateCreated)
    {
        if (optionalData == null)
        {
            optionalData = new Dictionary<string, string>();
        }
        if (category != null)
        {
            optionalData.Add("Category", category);
        }

        LoggerCategories categoryEnum;
        var canParseCat = Enum.TryParse(category, out categoryEnum);
        if (canParseCat)
        {
            if ((((int)TrackOrTraceCategory.Trace) & ((int)categoryEnum)) != 0)
            {
                LogTrace(message, category, level, optionalData, dateCreated);
            }
            else
            {
                LogEvent(message, category, level, optionalData, dateCreated);
            }
        }
        else
        {
            LogTrace(message, category, level, optionalData, dateCreated);
        }
    }

    private void LogTrace(string message, string category, string level, IDictionary<string, string> optionalData, DateTime dateCreated)
    {
        var lvl = SeverityLevel.Information;
        if (level != null)
        {
            lvl = (SeverityLevel)Enum.Parse(typeof(SeverityLevel), level);
        }
        var telemetry = new TraceTelemetry(message, lvl);
        telemetry.Timestamp = dateCreated;

        foreach (var variable in optionalData)
        {
            telemetry.Properties.Add(variable);
        }

        if (_current.Value != null)
        {
            _current.Value.Invoke(telemetry);
        }

        _client.TelemetryClient?.TrackTrace(telemetry);
    }

    private void LogEvent(string message, string category, string level, IDictionary<string, string> optionalData,
        DateTime dateCreated)
    {
        var telemetry = new EventTelemetry(category);
        telemetry.Timestamp = dateCreated;
        telemetry.Properties.Add("Level", level);
        telemetry.Properties.Add("Message", message);

        foreach (var variable in optionalData)
        {
            telemetry.Properties.Add(variable);
        }

        if (_current.Value != null)
        {
            _current.Value.Invoke(telemetry);
        }

        _client.TelemetryClient?.TrackEvent(telemetry);
    }

    public void Log(string message, string category, string level, IDictionary<string, string> optionalData)
    {
        Log(message, category, level, optionalData, DateTime.UtcNow);
    }

    public void Log(string message, string category, string level)
    {
        Log(message, category, level, new Dictionary<string, string>(), DateTime.UtcNow);
    }

    public void Log(string message, string category)
    {
        Log(message, category, null);
    }

    public void Log(string message)
    {
        Log(message, null);
    }

    public void ReplacedWith(ILoggerDelegation logger)
    {
    }

    public void Started()
    {
    }

    public ILoggerDelegation Copy()
    {
        return new AppInsigtsLogger(_client);
    }
}