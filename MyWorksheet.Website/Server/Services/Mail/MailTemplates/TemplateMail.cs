using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.FileSystem;
using Morestachio.Helper;

namespace MyWorksheet.Website.Server.Services.Mail.MailTemplates;

public abstract class TemplateMail : Mail
{
    protected ILocalFileProvider LocalFileProvider { get; }

    protected TemplateMail(ILocalFileProvider localFileProvider)
    {
        LocalFileProvider = localFileProvider;
        Values = new Dictionary<string, object>();
    }

    public virtual string Template { get; set; }

    public string TemplatePath { get; set; }

    public IDictionary<string, object> Values { get; }

    public virtual async Task Init()
    {
        foreach (var mailAttachment in Attachments)
        {
            await mailAttachment.Init();
        }
        await LoadFromFile();
    }

    public async Task LoadFromFile()
    {
        if (Template != null || TemplatePath == null)
        {
            return;
        }

        Template = (await LocalFileProvider.ReadAllAsync(TemplatePath).ConfigureAwait(false)).Stringify(false, Encoding.UTF8);
    }

    public override string RenderBody()
    {
        var body = Template;

        //var templateModel = Parser.ParseWithOptions(new ParserOptions(body, null, Encoding.UTF8));
        //body = templateModel.CreateAndStringify(Values);

        //foreach (var templateValue in Values)
        //{
        //	body = body.Replace("{{" + templateValue.Key + "}}", templateValue.Value?.ToString());
        //}

        return body;
    }
}