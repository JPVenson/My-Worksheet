namespace MyWorksheet.Website.Shared.ViewModels;

public class ServerProvidedTranslation : ViewModelBase
{
    public string Key
    {
        get { return _key; }
        set { SetProperty(ref _key, value); }
    }

    public string[] Arguments
    {
        get { return _arguments; }
        set
        {
            SetProperty(ref _arguments, value);
        }
    }

    private string _key;
    private string[] _arguments;


    public static implicit operator ServerProvidedTranslation(string text)
    {
        return new ServerProvidedTranslation() { Key = text };
    }
}
