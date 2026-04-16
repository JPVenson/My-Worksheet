using System;
using System.Collections.Generic;
using MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater.ArgumentStore;

namespace MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater
{
	public class PriorityQueueOptions
	{
		public PriorityQueueOptions(string loggerCategory, Func<PriorityQueueElement, bool> filter)
		{
			LoggerCategory = loggerCategory;
			Filter = filter;
			QueueSize = new Dictionary<IComparable, QueueCreationOptions>();
		}

		public string LoggerCategory { get; private set; }
		public IDictionary<IComparable, QueueCreationOptions> QueueSize { get; private set; }

		public Func<PriorityQueueElement, bool> Filter { get; private set; }
	}
}