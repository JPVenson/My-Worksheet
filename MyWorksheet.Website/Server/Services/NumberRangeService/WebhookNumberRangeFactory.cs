using System;
using MyWorksheet.Website.Server.Services.Reporting.Morestachio;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Webhooks.Out;

namespace MyWorksheet.Website.Server.Services.NumberRangeService;

public class WebhookNumberRangeFactory : NumberRangeFactoryBase<OutgoingWebhookModelGet>
{
    public const string NrCode = "WebHook";
    public override string Code { get; } = NrCode;
    public override string Description { get; } = "NumberRange/Description.Webhook";

    public override string GetDefaultTemplate()
    {
        return "WebHook{{Counter.ToString('0000')}}";
    }

    public override OutgoingWebhookModelGet GetTestDataInternal()
    {
        return new OutgoingWebhookModelGet()
        {
            CallingUrl = "https://test.com",
            IsActive = true,
            Secret = "TestSecret",
            IdOutgoingWebhookCase = new Guid("00000000-0000-0000-0001-000000000001")
        };
    }

    public WebhookNumberRangeFactory(MustachioFormatterService mustachioFormatterService) : base(mustachioFormatterService)
    {
    }
}
