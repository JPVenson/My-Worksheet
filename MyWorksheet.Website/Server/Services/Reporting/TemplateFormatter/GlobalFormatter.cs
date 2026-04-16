using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Blob;
using MyWorksheet.Website.Server.Services.Blob.Thumbnail;
using MyWorksheet.Website.Server.Services.Reporting.Morestachio;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Asserts;
using Microsoft.EntityFrameworkCore;
using Morestachio;
using Morestachio.Formatter.Framework.Attributes;
using Morestachio.Helper;

namespace MyWorksheet.Website.Server.Services.Reporting.TemplateFormatter;

public static class GlobalFormatter
{
    //[MorestachioFormatter("const", "creates an invokable object")]
    //public static object Const([SourceObject] object value, object valueElement, string type)
    //{
    //	return Convert.ChangeType(valueElement, (TypeCode)Enum.Parse(typeof(TypeCode), type));
    //}

    //[MorestachioFormatter("let", "Stores the current value or result of a formatting expression by the name. Returns the value if argument 'print' is true")]
    //[MorestachioFormatterInput("The variable name")]
    //[MorestachioFormatterInput("should the variable also be printet")]
    //public static object StoreVariable(object value, object name, string print)
    //{
    //	UserContext.CurrentContext.Value.Variables[name.ToString()] = (value);
    //	if (FormatBoolean(print))
    //	{
    //		return value;
    //	}

    //	return null;
    //}

    //[MorestachioFormatter("get", "get the variable")]
    //[MorestachioFormatterInput("The variable name")]
    //public static object StoreVariable(object value, object name)
    //{
    //	if (UserContext.CurrentContext.Value.Variables.TryGetValue(name.ToString(), out var val))
    //	{
    //		return val;
    //	}

    //	return null;
    //}	

    [MorestachioFormatter("ToString", "Formats a value according to the structure set by the argument")]
    public static string Formattable(IFormattable source, string argument, [ExternalData] ParserOptions options)
    {
        return source?.ToString(argument, options.CultureInfo);
    }

    [MorestachioFormatter("ToString", "Formats the value according to the build in rules for that object")]
    public static string Formattable(object source)
    {
        return source?.ToString();
    }

    [MorestachioFormatter("ValueOrNA", "Converts the default of N/A in address fields to and empty string")]
    [MorestachioFormatterInput("Must be 'value or null'")]
    public static string ValueOrNull(string value)
    {
        return value.Equals("N/A") ? string.Empty : value;
    }

    [MorestachioFormatter("WeekOfDate", "Gets the week of the given DateTime")]
    [MorestachioFormatterInput("Must be 'value or null'")]
    public static int WeekOfDate(DateTimeOffset time)
    {
        // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
        // be the same week# as whatever Thursday, Friday or Saturday are,
        // and we always get those right
        DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time.DateTime);
        if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
        {
            time = time.AddDays(3);
        }

        // Return the week of our adjusted day
        return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time.DateTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
    }

    [MorestachioFormatter("AssertToImage", "Pulls the Uploaded Data from this Assert and returns an array of" +
                                           " {Id:number; ImgDataBase64: string; Assert: WorksheetAssertGetViewModel}")]
    public static async Task<PngAssertUpload[]> PullAssertImages(IEnumerable<WorksheetAssertGetViewModel> value,
        [ExternalData] IBlobManagerService blobManagerService,
        [ExternalData] IThumbnailService thumbnailService,
        [ExternalData] UserContext userContext,
        [ExternalData] IDbContextFactory<MyworksheetContext> dbFactory)
    {
        var assertUploads = new List<PngAssertUpload>();
        foreach (var worksheetAssertGetViewModel in value)
        {
            assertUploads.AddRange(await PullAssertImage(worksheetAssertGetViewModel, blobManagerService, thumbnailService, userContext, dbFactory));
        }

        return assertUploads.ToArray();
    }

    [MorestachioFormatter("AssertToImage", "Pulls the Uploaded Data from this Assert and returns an array of" +
                                           " {Id:number; ImgDataBase64: string; Assert: WorksheetAssertGetViewModel}")]
    public static async Task<PngAssertUpload[]> PullAssertImage(WorksheetAssertGetViewModel value,
        [ExternalData] IBlobManagerService blobManagerService,
        [ExternalData] IThumbnailService thumbnailService,
        [ExternalData] UserContext userContext,
        [ExternalData] IDbContextFactory<MyworksheetContext> dbFactory)
    {
        var cache = userContext.Cache;
        if (cache.ContainsKey("ASSERTTOIMG_" + value.WorksheetAssertId))
        {
            return (PngAssertUpload[])cache["ASSERTTOIMG_" + value.WorksheetAssertId];
        }

        var imgs = new List<PngAssertUpload>();
        var db = await dbFactory.CreateDbContextAsync();
        await using (db.ConfigureAwait(false))
        {
            var worksheetAssertsFilesMaps = db.WorksheetAssertsFilesMaps
            .Where(f => f.IdWorksheetAssert == value.WorksheetAssertId)
            .ToArray();

            if (worksheetAssertsFilesMaps.Any())
            {
                foreach (var worksheetAssertsFilesMap in worksheetAssertsFilesMaps)
                {
                    var storageEntry = db.StorageEntries.Find(worksheetAssertsFilesMap.IdStorageEntry);

                    if (storageEntry.IdAppUser != userContext.UserId)
                    {
                        throw new InvalidOperationException(
                            "Critical Framework Exception. Code 0x01 SqlInjection Detected");
                    }

                    if (!thumbnailService.CanCreateThumbnail(storageEntry.FileName))
                    {
                        continue;
                    }

                    var blobManagerGetOperationResult = await blobManagerService.GetData(
                        worksheetAssertsFilesMap.IdStorageEntry,
                        userContext.UserId);
                    if (blobManagerGetOperationResult.Success)
                    {
                        using (blobManagerGetOperationResult.Object)
                        {
                            var convertedImage = await thumbnailService.CreateThumbnailFromFileDetailed(
                                blobManagerGetOperationResult.Object,
                                storageEntry.ContentType,
                                storageEntry.FileName,
                                "xl");

                            if (convertedImage == null)
                            {
                                continue;
                            }

                            imgs.Add(new PngAssertUpload
                            {
                                Id = worksheetAssertsFilesMap.IdStorageEntry,
                                ImgDataBase64 = Convert.ToBase64String(convertedImage),
                                Assert = value
                            });
                        }
                    }
                }
            }
        }


        return (PngAssertUpload[])(cache["ASSERTTOIMG_" + value.WorksheetAssertId] = imgs.ToArray());
    }

    public static bool FormatBoolean(string input)
    {
        if (input == null)
        {
            return false;
        }

        return input.Equals("true", StringComparison.OrdinalIgnoreCase)
               || input.Equals("yes", StringComparison.OrdinalIgnoreCase)
               || input.Equals("1", StringComparison.OrdinalIgnoreCase);
    }

    //[MorestachioFormatter("Time", "Formats a Time field", OutputType = typeof(string))]
    //[MorestachioFormatterInput("Must be d", OutputType = typeof(decimal), Output = "hh.dd")]
    //[MorestachioFormatterInput("Must be t", OutputType = typeof(string), Output = "hh,mm")]
    //[MorestachioFormatterInput(
    //	"if otherwise formatter as seen here: https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings",
    //	OutputType = typeof(string))]
    //public static object FormatTime(int value, string arg, [ExternalData] ParserOptions parserOptions)
    //{
    //	return FormatTime((decimal) value, arg, parserOptions);
    //}

    //[MorestachioFormatter("Time", "Formats a Time field", OutputType = typeof(string))]
    //[MorestachioFormatterInput("Must be d", OutputType = typeof(decimal), Output = "hh.dd")]
    //[MorestachioFormatterInput("Must be t", OutputType = typeof(string), Output = "hh,mm")]
    //[MorestachioFormatterInput(
    //	"if otherwise formatter as seen here: https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings",
    //	OutputType = typeof(string))]
    //public static object FormatTime(double value, string arg, [ExternalData] ParserOptions parserOptions)
    //{
    //	return FormatTime((decimal) value, arg, parserOptions);
    //}

    [MorestachioFormatter("Time", "Formats a Time field", OutputType = typeof(string))]
    [MorestachioFormatterInput("Must be d", OutputType = typeof(decimal), Output = "hh.dd")]
    [MorestachioFormatterInput("Must be t", OutputType = typeof(string), Output = "hh,mm")]
    [MorestachioFormatterInput("Must be d2", OutputType = typeof(decimal), Output = "hh.ddd")]
    [MorestachioFormatterInput("Must be t2", OutputType = typeof(string), Output = "hh,mmm")]
    [MorestachioFormatterInput(
        "if otherwise formatter as seen here: https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings",
        OutputType = typeof(string))]
    public static object FormatTime(Number value, string arg, [ExternalData] ParserOptions parserOptions)
    {
        if (arg == "d")
        {
            return Math.Round((double)value / 60, 2).ToString("00.00", parserOptions.CultureInfo);
        }

        if (arg == "h")
        {
            var hours = (int)value / 60;
            var minutes = value.Modulo(60).Round(2);
            return $"{hours:00}:{minutes:00}";
        }

        if (arg == "d2")
        {
            return Math.Round((double)value / 60, 3).ToString("00.000", parserOptions.CultureInfo);
        }

        if (arg == "h2")
        {
            var hours = (int)value / 60;
            var minutes = value.Modulo(60).Round(3);
            return $"{hours:00}:{minutes:000}";
        }

        return value.ToString(arg, parserOptions.CultureInfo);
    }

    public class PngAssertUpload
    {
        public Guid Id { get; set; }
        public string ImgDataBase64 { get; set; }
        public WorksheetAssertGetViewModel Assert { get; set; }
    }
}