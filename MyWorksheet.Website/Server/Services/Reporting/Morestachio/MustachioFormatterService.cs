using System;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.Blob.Thumbnail;
using MyWorksheet.Website.Server.Services.ExternalDomainValidator;
using MyWorksheet.Website.Server.Services.Reporting.TemplateFormatter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Morestachio.Formatter.Framework;
using Morestachio.Linq;
using ServiceLocator.Attributes;
using HtmlFormatter = MyWorksheet.Website.Server.Services.Reporting.TemplateFormatter.HtmlFormatter;

namespace MyWorksheet.Website.Server.Services.Reporting.Morestachio;

[ScopedService(typeof(MorestachioFormatterService))]
public class MustachioFormatterService : MorestachioFormatterService
{
    public MustachioFormatterService(IServiceProvider provider)
        : base(false)
    {
        AddGlobalFormatter();
        Services.AddService(provider.GetService<IBlobManagerService>);
        Services.AddService(provider.GetService<IThumbnailService>);
        Services.AddService(provider.GetService<IExternalDomainValidator>);
        Services.AddService(provider.GetService<IDbContextFactory<MyworksheetContext>>);
    }

    public void AddGlobalFormatter()
    {
        this.AddFromType(typeof(GlobalFormatter));
        this.AddFromType(typeof(MonetaryCalculatorFormatters));
        this.AddFromType(typeof(LocProvider));
        this.AddFromType(typeof(TemplateFormatter.ListFormatter));
        this.AddFromType(typeof(ListFormatter));
        this.AddFromType(typeof(StructualFormatter));
        this.AddFromType(typeof(HtmlFormatter));
        this.AddFromType(typeof(NumericFormatter));
        this.AddFromType(typeof(PdfOptionsSetterFormatter));
        this.AddFromType(typeof(DynamicLinq));
        AddValueExchangeService();
    }

    private void AddValueExchangeService()
    {
        //var valueExchangeService = IoC.Resolve<IValueExchangeService>();

        //valueExchangeService.WaitForRefresh().ContinueWith((t) =>
        //{
        //	var valueExchangeTypes = valueExchangeService.ValueExchangeRates.Select(e => e.IsoName)
        //		.Aggregate((e, f) => e + ", " + f);

        //	var exp = new Func<decimal, string, string, decimal>((value, From, To) => valueExchangeService.Exchange(new Tuple<string, decimal>(From, value), To));

        //	this.AddSingle(new MorestachioFormatterModel("exchange", "Converts the amount of currency to the daily exchange rate provided by the EZB", typeof(decimal), typeof(decimal), new[]
        //	{
        //		new global::Morestachio.Formatter.Framework.InputDescription("[From]Must be one of this currency: " + valueExchangeTypes, typeof(decimal), ""),
        //		new global::Morestachio.Formatter.Framework.InputDescription("[To]Must be one of this currency: " + valueExchangeTypes, typeof(decimal), ""),
        //	}, "number", exp.Method));
        //});
    }
}