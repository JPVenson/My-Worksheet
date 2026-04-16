using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Morestachio;
using Morestachio.Framework.IO.SingleStream;
using Morestachio.Helper;

namespace MyWorksheet.Website.Server.Services.Reporting.Morestachio;

public class MorestachioTemplate : ITemplate
{
    private readonly MorestachioDocumentInfo _model;

    public MorestachioTemplate(MorestachioDocumentInfo model)
    {
        _model = model;
    }

    public async Task<string> RenderTemplateAsync(IDictionary<string, object> arguments, UserContext userContext)
    {
        using (var template = await RenderTemplateStreamAsync(arguments, userContext))
        {
            return template.Stringify(true, _model.ParserOptions.Encoding);
        }
    }

    public async Task<Stream> RenderTemplateStreamAsync(IDictionary<string, object> arguments, UserContext userContext)
    {
        _model.ParserOptions.Formatters.Services.AddService(userContext);
        var stringBuilder = new StringBuilder();
        var targetOutput = new ByteCounterStringBuilder(stringBuilder, _model.ParserOptions);
        await _model.CreateRenderer().RenderAsync(arguments, CancellationToken.None, targetOutput);
        var stream = new MemoryStream(); // Get output stream from somewhere.
        using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            foreach (var chunk in stringBuilder.GetChunks())
            {
                await streamWriter.WriteAsync(chunk);
            }
        }

        return stream;
    }
}