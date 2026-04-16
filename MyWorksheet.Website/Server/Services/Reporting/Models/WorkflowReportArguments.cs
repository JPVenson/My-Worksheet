using System;
using System.Collections.Generic;
using MyWorksheet.Helper;
using MyWorksheet.Public.Models.ObjectSchema;

namespace MyWorksheet.Website.Server.Services.Reporting.Models;

public class WorkflowReportArguments : ArgumentsBase
{
    public static WorkflowReportArguments Parse(IDictionary<string, object> arguments)
    {
        var opts = new WorkflowReportArguments();
        opts.SetOrAbort<Guid>(arguments, e => opts.Worksheet = e, nameof(Worksheet));
        opts.SetOrAbort<Guid>(arguments, e => opts.PaymentInfos = e, nameof(PaymentInfos));
        opts.Set<DateTime?>(arguments, e => opts.Date = e, "Date");
        opts.Set<string>(arguments, e => opts.AdditionalInfos = e, nameof(AdditionalInfos));
        opts.Set<int>(arguments, e => opts.RoundBy = e, nameof(RoundBy));
        opts.Set<bool>(arguments, e => opts.UseParentOrg = e, nameof(UseParentOrg));
        return opts.GetIfValid() as WorkflowReportArguments;
    }

    [JsonDisplayKey("Entity/Worksheet")]
    public Guid Worksheet { get; set; }

    [JsonDisplayKey("Reporting/ProjectSpec.Form.Comment.UseParentOrgAsAddress")]
    public bool UseParentOrg { get; set; }

    [JsonDisplayKey("Reporting/ProjectSpec.Form.Comment.RoundBy")]
    public int RoundBy { get; set; } = 2;

    [JsonDisplayKey("Reporting/ProjectSpec.Form.Comment.PaymentInfos")]
    public Guid PaymentInfos { get; set; }

    [JsonDisplayKey("Reporting/ProjectSpec.Form.Comment.Date")]
    public DateTime? Date { get; set; }

    [JsonDisplayKey("Reporting/ProjectSpec.Form.Comment.AdditionalInfos")]
    [JsonCanBeNull]
    public string AdditionalInfos { get; set; }

    [JsonDisplayKey("Reporting/ProjectSpec.Form.Comment.TimeDisplayArg")]
    [JsonCanBeNull]
    public string TimeDisplayArg { get; set; }
}