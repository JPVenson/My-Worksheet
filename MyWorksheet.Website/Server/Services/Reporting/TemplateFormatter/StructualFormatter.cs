using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Morestachio.Formatter.Framework.Attributes;
using Newtonsoft.Json;

namespace MyWorksheet.Website.Server.Services.Reporting.TemplateFormatter;

public static class StructualFormatter
{
    [MorestachioFormatter("json", "Transforms any object into Json")]
    public static string JsonFromObject(object value, string arg)
    {
        return JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings()
        {
            Context = new StreamingContext(StreamingContextStates.File),
            ConstructorHandling = ConstructorHandling.Default,
        });
    }

    [MorestachioFormatter("xml", "Transforms any object into Xml")]
    public static string XmlFromObject(object value, string arg)
    {
        var serilizer = new XmlSerializer(value.GetType());
        using (var ms = new MemoryStream())
        {
            serilizer.Serialize(ms, value);
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }

    [MorestachioFormatter("xml2", "Transforms any object into a Data Contract Xml")]
    public static string DataContactFromObject(object value, string arg)
    {
        var serilizer = new DataContractSerializer(value.GetType());
        using (var ms = new MemoryStream())
        {
            serilizer.WriteObject(ms, value);
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}