//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Katana.CommonTasks.Services.Logging.Contracts;
//using Microsoft.ApplicationInsights.DataContracts;

//namespace MyWorksheet.Webpage.Services.Logger
//{
//	public class DelegateLogger : IAppLogger
//	{
//		public DelegateLogger()
//		{
//		}

//		private ILoggerDelegation _logger;

//		public void Log(string message, string category, SeverityLevel level, IDictionary<string, string> optionalData = null)
//		{
//			Log(message, category, level.ToString(), optionalData);
//		}

//		public void LogVerbose(string message, string category, IDictionary<string, string> optionalData = null)
//		{
//			Log(message, category, SeverityLevel.Verbose, optionalData);
//		}

//		public void LogInformation(string message, string category, IDictionary<string, string> optionalData = null)
//		{
//			Log(message, category, SeverityLevel.Information, optionalData);
//		}

//		public void LogWarning(string message, string category, IDictionary<string, string> optionalData = null)
//		{
//			Log(message, category, SeverityLevel.Warning, optionalData);
//		}

//		public void LogError(string message, string category, IDictionary<string, string> optionalData = null)
//		{
//			Log(message, category, SeverityLevel.Error, optionalData);
//		}

//		public event Action<string, string, IDictionary<string, string>> OnLog;

//		public void LogCritical(string message, string category, IDictionary<string, string> optionalData = null)
//		{
//			Log(message, category, SeverityLevel.Critical, optionalData);
//		}

//		public void Log(string message, string category, string level, IDictionary<string, string> optionalData, DateTime dateCreated)
//		{
//			OnOnLog(message, category, optionalData);
//			_logger.Log(message, category, level, optionalData, dateCreated);
//		}

//		public void Log(string message, string category, string level, IDictionary<string, string> optionalData)
//		{
//			Log(message, category, level, optionalData, DateTime.UtcNow);
//		}

//		public void Log(string message, string category, string level)
//		{
//			Log(message, category, level, null);
//		}

//		public void Log(string message, string category)
//		{
//			Log(message, category, null);
//		}

//		public void Log(string message)
//		{
//			Log(message, null);
//		}

//		public void ReplacedWith(ILoggerDelegation logger)
//		{
//			if (_logger != null)
//			{
//				_logger.ReplacedWith(logger);
//			}
//			_logger = logger;
//		}

//		public void Started()
//		{
//			_logger.Started();
//		}

//		protected virtual void OnOnLog(string arg1, string arg2, IDictionary<string, string> arg3)
//		{
//			var events = OnLog;
//			if (events != null)
//			{
//				Task.Run(() =>
//				{
//					events.Invoke(arg1, arg2, arg3);
//				});
//			}
//		}
//	}
//}