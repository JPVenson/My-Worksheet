using System;
using MyWorksheet.Website.Server.Services.Reporting.Morestachio;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Server.Services.NumberRangeService;

public class WorksheetNumberRangeFactory : NumberRangeFactoryBase<WorksheetModel>
{
    public const string NrCode = "WorksheetNr";
    public override string Code { get; } = NrCode;
    public override string Description { get; } = "NumberRange/Description.Worksheet";

    public override string GetDefaultTemplate()
    {
        return "Ws{{Counter}} {{Data.StartTime.ToString('d')}} - {{Data.EndTime.ToString('d')}}";
    }

    public override WorksheetModel GetTestDataInternal()
    {
        return new WorksheetModel()
        {
            StartTime = DateTimeOffset.UtcNow,
            EndTime = DateTimeOffset.UtcNow.AddDays(30),
            ServiceDescription = "Test Description",
        };
    }

    public WorksheetNumberRangeFactory(MustachioFormatterService mustachioFormatterService) : base(mustachioFormatterService)
    {
    }
}
