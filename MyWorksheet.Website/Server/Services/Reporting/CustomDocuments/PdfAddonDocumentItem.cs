using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.Reporting.Morestachio;
using MyWorksheet.Website.Server.Services.Templating.Pdf;
using Morestachio;
using Morestachio.Document;
using Morestachio.Document.Contracts;
using Morestachio.Document.Custom;
using Morestachio.Document.Items;
using Morestachio.Document.Items.Base;
using Morestachio.Document.Visitor;
using Morestachio.Framework.Context;
using Morestachio.Framework.Expression;
using Morestachio.Framework.Expression.Parser;
using Morestachio.Framework.IO;
using Morestachio.Framework.Tokenizing;
using Morestachio.Helper;
using Morestachio.Parsing.ParserErrors;

namespace MyWorksheet.Website.Server.Services.Reporting.CustomDocuments;

public class PdfAddonDocumentItemProvider : BlockDocumentItemProviderBase
{
    /// <inheritdoc />
    public PdfAddonDocumentItemProvider(string tagOpen, string tagClose, bool isHeader) : base(tagOpen, tagClose)
    {
        IsHeader = isHeader;
    }

    public bool IsHeader { get; set; }

    public static PdfAddonDocumentItemProvider Header()
    {
        return new PdfAddonDocumentItemProvider("#PdfHeader", "/PdfHeader", true);
    }

    public static PdfAddonDocumentItemProvider Footer()
    {
        return new PdfAddonDocumentItemProvider("#PdfFooter", "/PdfFooter", false);
    }

    /// <inheritdoc />
    public override IEnumerable<TokenPair> Tokenize(TokenInfo token, ParserOptions options)
    {
        var trim = token.Token;
        if (trim.StartsWith(TagOpen, StringComparison.InvariantCultureIgnoreCase))
        {
            var locToken = token.Token.Remove(0, TagOpen.Length).Trim();
            var tokenOptions = new List<ITokenOption>();
            if (locToken.StartsWith("#HEIGHT ", StringComparison.OrdinalIgnoreCase))
            {
                locToken = locToken.Substring("#HEIGHT ".Length);
                tokenOptions.Add(new TokenOption("HEIGHT", ExpressionParser.ParseExpression(locToken, token.TokenizerContext).Expression));
            }

            yield return new TokenPair(TagOpen.Trim(),
                trim,
                token.Location,
                tokenOptions);
        }
        if (string.Equals(trim, TagClose, StringComparison.InvariantCultureIgnoreCase))
        {
            yield return new TokenPair(TagClose, trim, token.Location);
        }
    }

    public override IBlockDocumentItem CreateDocumentItem(string tag, string value, TokenPair token, ParserOptions options,
        IEnumerable<ITokenOption> tagCreationOptions)
    {
        tagCreationOptions = (tagCreationOptions ?? Array.Empty<ITokenOption>()).Concat(new ITokenOption[]
        {
            new PersistantTokenOption("TARGET", IsHeader ? "header" : "footer"),
            new PersistantTokenOption("HEIGHT", token.FindOption<IMorestachioExpression>("HEIGHT")),
        });

        return new PdfAddonDocumentItem(token.TokenRange, tagCreationOptions);
    }
}

public class PdfAddonDocumentItem : BlockDocumentItemBase
{
    /// <inheritdoc />
    public PdfAddonDocumentItem(in TextRange location, IEnumerable<ITokenOption> tagCreationOptions) : base(in location, tagCreationOptions)
    {
    }

    /// <inheritdoc />
    public PdfAddonDocumentItem(SerializationInfo info, StreamingContext c) : base(info, c)
    {
    }

    /// <inheritdoc />
    public override async ValueTask<IEnumerable<DocumentItemExecution>> Render(IByteCounterStream outputStream,
        ContextObject context,
        ScopeData scopeData)
    {
        using (var childStream = outputStream.GetSubStream())
        {
            await MorestachioDocument.ProcessItemsAndChildren(Children, childStream, context, scopeData);

            var targetOption = TagCreationOptions.First(e => e.Name == "TARGET");
            var heightExpression = TagCreationOptions.First(e => e.Name == "HEIGHT").Value as IMorestachioExpression;
            var heightOption = (Number)(await heightExpression.GetValue(context, scopeData)).Value;

            var userContext = scopeData.ParserOptions.Formatters.Services.GetRequiredService<UserContext>();
            if ((string)targetOption.Value == "header")
            {
                userContext.Header = new PdfAddon()
                {
                    Html = childStream.Read(),
                    Height = (int)heightOption.Value
                };
            }
            else if ((string)targetOption.Value == "footer")
            {
                userContext.Footer = new PdfAddon()
                {
                    Html = childStream.Read(),
                    Height = (int)heightOption.Value
                };
            }
        }

        return Enumerable.Empty<DocumentItemExecution>();
    }

    /// <inheritdoc />
    public override void Accept(IDocumentItemVisitor visitor)
    {
        visitor.Visit(this);
    }
}