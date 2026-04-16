namespace MyWorksheet.Website.Server.Util.Auth;

public class ChallangeResult
{
    public string ResultText { get; set; }
    public bool Success { get; set; }

    public static ChallangeResult NoChallangeFound()
    {
        return new ChallangeResult()
        {
            ResultText = "The session was canceled",
            Success = false
        };
    }

    public static ChallangeResult InvalidPassword()
    {
        return new ChallangeResult()
        {
            Success = false,
            ResultText = "Ether the password or the Username is invalid"
        };
    }

    public static ChallangeResult Data()
    {
        return new ChallangeResult()
        {
            ResultText = "Success",
            Success = true
        };
    }
}