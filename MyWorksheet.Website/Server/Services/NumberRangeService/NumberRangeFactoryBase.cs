using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Reporting.Morestachio;
using MyWorksheet.Website.Server.Shared.ObjectSchema;
using Morestachio;
using Morestachio.Helper;

namespace MyWorksheet.Website.Server.Services.NumberRangeService;

public abstract class NumberRangeFactoryBase<TData> : INumberRangeFactory
    where TData : class
{
    private readonly MustachioFormatterService _mustachioFormatterService;

    public NumberRangeFactoryBase(MustachioFormatterService mustachioFormatterService)
    {
        _mustachioFormatterService = mustachioFormatterService;
    }

    public async Task<string> GetNumberEntry(object additonalData, string template, long counter)
    {
        //var parsingOptions = new ParserOptions(template, null, Encoding.UTF8, true);
        var parsingOptions = ParserOptionsBuilder.New()
            .WithTemplate(template)
            .WithEncoding(Encoding.UTF8)
            .WithTimeout(TimeSpan.FromSeconds(2))
            .WithDisableContentEscaping(true)
            .WithFormatterService(_mustachioFormatterService).Build();

        var morestachioDocumentInfo = await Parser.ParseWithOptionsAsync(parsingOptions);
        return (await morestachioDocumentInfo.CreateRenderer().RenderAsync(new NumberRangeData()
        {
            Counter = counter,
            Data = additonalData as TData
        }, CancellationToken.None)).Stream.Stringify(true, parsingOptions.Encoding);
    }

    public abstract string Code { get; }
    public abstract string Description { get; }

    public IObjectSchemaInfo GetSchema(MyworksheetContext db)
    {
        return JsonSchemaExtensions.JsonSchema(typeof(NumberRangeData));
    }

    public abstract string GetDefaultTemplate();
    public object GetTestData()
    {
        return GetTestDataInternal();
    }

    public abstract TData GetTestDataInternal();

    public class NumberRangeData
    {
        public long Counter { get; set; }
        public TData Data { get; set; }
    }
}