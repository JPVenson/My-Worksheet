using System;
using MyWorksheet.Website.Server.Services.Reporting.Morestachio;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Server.Services.NumberRangeService;

public class InvoiceNumberRangeFactory : NumberRangeFactoryBase<WorksheetModel>
{
    public const string NrCode = "InvoiceNr";
    public override string Code { get; } = NrCode;
    public override string Description { get; } = "NumberRange/Description.Invoice";

    public override string GetDefaultTemplate()
    {
        return "{{DateTimeNow().ToString('yyyMMdd')}}-{{Counter.ToString('0000')}}";
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

    public InvoiceNumberRangeFactory(MustachioFormatterService mustachioFormatterService) : base(mustachioFormatterService)
    {
    }
}

