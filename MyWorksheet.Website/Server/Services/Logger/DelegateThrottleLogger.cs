//using System;
//using WebApiThrottle;

//namespace MyWorksheet.Webpage.Services.Logger
//{
//	public class DelegateThrottleLogger : IThrottleLogger
//	{
//		private readonly Action<ThrottleLogEntry> _callback;

//		public DelegateThrottleLogger(Action<ThrottleLogEntry> callback)
//		{
//			_callback = callback;
//		}

//		public void Log(ThrottleLogEntry entry)
//		{
//			_callback(entry);
//		}
//	}
//}