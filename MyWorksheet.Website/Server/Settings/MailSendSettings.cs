namespace MyWorksheet.Website.Server.Settings;

public class MailSendSettings
{
    public string Realm { get; set; }
    public int Port { get; set; }
    public string Sender { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool Dns { get; set; }
    public int Protocol { get; set; }
}
