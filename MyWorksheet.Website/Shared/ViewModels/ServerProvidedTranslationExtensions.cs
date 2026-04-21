namespace MyWorksheet.Website.Shared.ViewModels;

public static class ServerProvidedTranslationExtensions
{
    public static ServerProvidedTranslation AsTranslation(this string text)
    {
        return new ServerProvidedTranslation() { Key = text };
    }
}