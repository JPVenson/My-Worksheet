using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Resources;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Shared.ViewModels;

namespace MyWorksheet.Website.Server.Services.Text;

public interface ITextService
{
    object Compile(ServerProvidedTranslation translation, CultureInfo culture);
    object Compile(ServerProvidedTranslation translation);
    string RunTransformation(CultureInfo culture, string transformationKey, string text);
    IEnumerable<TextResourceEntity> GetByGroupName(string groupName, string locale);
    Collection<ResourceManager> TranslationResources { get; }
    void LoadTexts();
    UiResourceState[] GetCacheStatus();
    IEnumerable<TextResourceEntity> GetCache(string culture);
}