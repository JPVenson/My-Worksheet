using System;
using JetBrains.Annotations;
using MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater.Contracts;

namespace MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	[BaseTypeRequired(typeof(IPriorityQueueAction))]
	public sealed class PriorityQueueItemAttribute : Attribute
	{
		public PriorityQueueItemAttribute(string key)
		{
			Key = key;
		}

		public string Key { get; private set; }
	}
}