using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.Text;
using MyWorksheet.Website.Shared.ViewModels;
using Morestachio.Helper.Localization;
using ServiceLocator.Attributes;
using TextResourceEntity = Morestachio.Helper.Localization.TextResourceEntity;

namespace MyWorksheet.Website.Server.Services.Reporting.Morestachio;

[SingletonService(typeof(IMorestachioLocalizationService))]
public class MorestachioTranslationAdapterService : IMorestachioLocalizationService
{
    private readonly ITextService _textService;

    public MorestachioTranslationAdapterService(ITextService textService)
    {
        _textService = textService;
    }

    /// <inheritdoc />
    public Task<TextResourceEntity?> GetEntryOrLoad(string key, CultureInfo culture = null)
    {
        var compile = _textService.Compile(new ServerProvidedTranslation()
        {
            Key = key,
        }, culture);
        return Task.FromResult<TextResourceEntity?>(new TextResourceEntity(culture, key, compile, ""));
    }

    /// <inheritdoc />
    public object GetTranslationOrNull(string key, CultureInfo culture = null, params object[] arguments)
    {
        return _textService.Compile(new ServerProvidedTranslation()
        {
            Key = key,
            Arguments = arguments.Select(f => f.ToString()).ToArray()
        }, culture);
    }
}