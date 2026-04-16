namespace MyWorksheet.Website.Client.Util.View;

public class LocalizableString
{
    public string Text { get; }
    public object[] Arguments { get; }

    public LocalizableString(string text, params object[] arguments)
    {
        Text = text;
        Arguments = arguments;
    }

    public static implicit operator LocalizableString(string text)
    {
        return new LocalizableString(text);
    }
}