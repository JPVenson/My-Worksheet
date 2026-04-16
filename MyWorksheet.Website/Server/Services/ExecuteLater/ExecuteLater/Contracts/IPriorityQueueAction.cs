using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater.ArgumentStore;

namespace MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater.Contracts
{
	public interface IPriorityQueueAction
	{
		string Name { get; }
		string Key { get; }
		Version Version { get; set; }
		bool ValidateArguments(IDictionary<string, object> arguments);
		Task Execute(PriorityQueueElement queueElement);
	}
}