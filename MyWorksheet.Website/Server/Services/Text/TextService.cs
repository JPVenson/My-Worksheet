using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using MyWorksheet.Website.Client.Services.Auth;
using MyWorksheet.Website.Server.Shared.Locale.ServerResources;
using MyWorksheet.Website.Server.Shared.Locale.UiResources;
using MyWorksheet.Website.Server.Shared.Services.Logger;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;
using MyWorksheet.Website.Shared.Services;
using MyWorksheet.Website.Shared.ViewModels;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Server.Services.Text;

[SingletonService(typeof(ITextService))]
public class TextService : RequireInit, ITextService
{
    private static readonly Regex RefRegex = new(@"([{][^\d}]+[}]+)");
    private readonly ILogger<TextService> _logger;

    private readonly List<Exception> _loaderExceptions = [];

    public TextService(ILogger<TextService> logger)
    {
        _logger = logger;
        SupportedCultures = new KeyedDictionary<string, CultureInfo>(e => e.Name)
        {
            CultureInfo.GetCultureInfo("en-us"),
            CultureInfo.GetCultureInfo("de-de")
        };

        TranslationResources = [];

        TranslationCacheState = new Dictionary<CultureInfo, string>();

        TextCache = [];
        TextTransformations = new Dictionary<CultureInfo, IDictionary<string, Func<string, string>>>();
    }

    public IDictionary<CultureInfo, string> TranslationCacheState { get; }

    public KeyedDictionary<string, CultureInfo> SupportedCultures { get; set; }

    public IDictionary<CultureInfo, IDictionary<string, Func<string, string>>> TextTransformations { get; set; }

    public Dictionary<string, TextResourceEntity[]> TextCache { get; set; }

    public object Compile(ServerProvidedTranslation translationInfo)
    {
        return Compile(translationInfo, SupportedCultures.First());
    }

    public object Compile(ServerProvidedTranslation translationInfo, CultureInfo culture)
    {
        if (culture == null)
        {
            throw new ArgumentNullException(nameof(culture));
        }

        var translation = TextCache.SelectMany(e => e.Value)
        .FirstOrDefault(e => e.Key.Equals(translationInfo.Key, StringComparison.OrdinalIgnoreCase) && Equals(e.Lang, culture));

        if (translation.Text == null)
        {
            _logger.LogError("Missing Translation", LoggerCategories.Server.ToString(), new Dictionary<string, string>
            {
                { "Key", translationInfo.Key }
            });
            return translationInfo.Key;
        }

        var text = translation.Text.ToString();

        for (var index = 0; index < translationInfo.Arguments.Length; index++)
        {
            var translationInfoArgument = translationInfo.Arguments[index];
            text = text.Replace("{" + index + "}", Compile(translationInfoArgument).ToString());
        }

        return text;
    }

    public string RunTransformation(CultureInfo culture, string transformationKey, string text)
    {
        var transformations = TextTransformations[culture];

        var transformation =
            transformations.FirstOrDefault(e =>
                                                e.Key.Equals(transformationKey, StringComparison.OrdinalIgnoreCase));

        if (transformation.Key == null)
        {
            _loaderExceptions.Add(new InvalidOperationException(
                                                                $"{culture.Name}:Could not find \'{transformationKey}\' transformation"));
            return "";
        }

        return transformation.Value(text);
    }

    public Collection<ResourceManager> TranslationResources { get; set; }

    public void LoadTexts()
    {
        var textResources = new Dictionary<string, List<TextResourceEntity>>();

        foreach (var cultureInfo in SupportedCultures)
        {
            var resources = new Dictionary<string, object>();
            var translationCache = new StringBuilder();

            foreach (var translationResource in TranslationResources)
            {
                var resource = translationResource.GetResourceSet(cultureInfo, true, true);

                if (resource == null)
                {
                    throw new InvalidOperationException("Translations for requested " + cultureInfo.Name + " could not be loaded");
                }

                foreach (var dictionaryEntry in resource
                        .OfType<DictionaryEntry>())
                {
                    resources[dictionaryEntry.Key.ToString().ToUpper()] = dictionaryEntry.Value;
                }
            }

            var localResources = new List<TextResourceEntity>();

            foreach (var textResource in resources)
            {
                var key = textResource.Key;
                var keyParts = key.Split('/');

                localResources.Add(new TextResourceEntity
                {
                    Key = key,
                    Page = keyParts[0],
                    Text = textResource.Value,
                    Lang = cultureInfo
                });
            }

            var transformedResources = localResources
            .Select(localResource => new TextResourceEntity
            {
                Key = localResource.Key,
                Text = TransformText(localResource, localResources),
                Lang = localResource.Lang,
                Page = localResource.Page
            }).ToList();

            //Process References
            foreach (var group in transformedResources.GroupBy(e => e.Page))
            {
                foreach (var textResourceEntity in group)
                {
                    translationCache.AppendLine(textResourceEntity.Page + "-" + textResourceEntity.Text);
                }

                List<TextResourceEntity> cache;

                if (!textResources.ContainsKey(group.Key))
                {
                    textResources[group.Key] = cache = [];
                }
                else
                {
                    cache = textResources[group.Key];
                }

                cache.AddRange(group.ToArray());
            }

            var hash = MD5
            .Create()
            .ComputeHash(Encoding.Unicode.GetBytes(translationCache.ToString()))
            .Select(e => e.ToString("X2").ToUpper())
            .Aggregate((e, f) => e + f);
            TranslationCacheState.Add(cultureInfo, hash);
        }

        if (_loaderExceptions.Any())
        {
            throw new AggregateException(_loaderExceptions.GroupBy(e => e.Message).Select(e => e.First()).ToArray()).Flatten();
        }

        foreach (var textResource in textResources)
        {
            TextCache[textResource.Key] = textResource.Value.ToArray();
        }
    }

    public UiResourceState[] GetCacheStatus()
    {
        return TranslationCacheState.Select(e => new UiResourceState
        {
            CultureName = e.Key.Name,
            State = e.Value
        }).ToArray();
    }

    public IEnumerable<TextResourceEntity> GetCache(string culture)
    {
        return TextCache.SelectMany(f => f.Value.Where(e => e.Lang.Name.ToLower() == culture.ToLower()));
    }

    public IEnumerable<TextResourceEntity> GetByGroupName(string groupName, string locale)
    {
        if (TextCache.ContainsKey(groupName))
        {
            return TextCache[groupName].Where(e =>
                                                e.Lang.Name.Equals(locale, StringComparison.InvariantCultureIgnoreCase)
                                                || e.Lang.TwoLetterISOLanguageName.Equals(locale, StringComparison.InvariantCultureIgnoreCase)
                                                || e.Lang.ThreeLetterWindowsLanguageName.Equals(locale, StringComparison.InvariantCultureIgnoreCase));
        }

        return Enumerable.Empty<TextResourceEntity>();
    }

    public override void Init()
    {
        TranslationResources.Add(TextResources.ResourceManager);
        TranslationResources.Add(ServerResources.ResourceManager);
        LoadTexts();
    }

    private object TransformText(TextResourceEntity textResourceEntity,
                                IEnumerable<TextResourceEntity> fromResources,
                                Stack<string> transformationChain = null)
    {
        transformationChain = transformationChain ?? new Stack<string>();

        if (!(textResourceEntity.Text is string text))
        {
            return textResourceEntity.Text;
        }

        foreach (Match match in RefRegex.Matches(text))
        {
            var textPart = match.Value.Trim();

            if (textPart.Trim('{', '}').StartsWith("!"))
            {
                text = textPart.Replace("!", "");
                continue;
            }

            var transformationResource = fromResources.FirstOrDefault(e =>
                                                                        e.Lang == textResourceEntity.Lang &&
                                                                        e.Key.Equals(textPart.ToUpper().Trim('{', '}')));

            if (transformationResource.Key == null)
            {
                _loaderExceptions.Add(new InvalidOperationException($"{textResourceEntity.Lang.Name}: The requested Transformation in '{textResourceEntity.Key.ToUpper()}' for '{match.Value}' could not found"));
                return "";
            }

            if (transformationChain.Contains(transformationResource.Key))
            {
                _loaderExceptions.Add(new InvalidOperationException($"{textResourceEntity.Lang.Name}: Endless requsion detected for: " + transformationChain.Aggregate((e, f) => e + " | " + f)));
                return "";
            }

            transformationChain.Push(transformationResource.Key);

            var textReplacement = TransformText(transformationResource, fromResources, transformationChain);

            transformationChain.Pop();

            foreach (var textTransformationOperator in textPart.Split('|').Skip(1))
            {
                var transformations = TextTransformations[textResourceEntity.Lang];
                var transformation = transformations.First(e => e.Key.Equals(textTransformationOperator));
                textPart = transformation.Value(textPart);
            }

            text = text.Replace(textPart, textReplacement?.ToString());
        }

        return text;
    }
}