namespace MyWorksheet.Website.Client.Util.View;

public static class LocalizationHelper
{
    public static LocalizableString AsLocString(this string text, params object[] arguments)
    {
        return new LocalizableString(text, arguments);
    }
}