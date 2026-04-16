using System;
namespace MyWorksheet.Website.Server.Models;

public partial class ProcessorCapability
{
    public Guid ProcessorCapabilityId { get; set; }

    public string Name { get; set; }

    public string Value { get; set; }

    public bool IsEnabled { get; set; }

    public Guid IdProcessor { get; set; }

    public virtual Processor IdProcessorNavigation { get; set; }
}
