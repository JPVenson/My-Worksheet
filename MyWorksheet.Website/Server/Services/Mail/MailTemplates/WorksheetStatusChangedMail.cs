namespace MyWorksheet.Website.Server.Services.Mail.MailTemplates;

public class WorksheetStatusChangedMail : Mail
{
    private readonly string _content;

    public WorksheetStatusChangedMail(string content, string username, string project, string worksheet)
    {
        _content = content;
        Subject = $"User {username} has requested a Status change for Project {project}";
    }

    /// <inheritdoc />
    public override string RenderBody()
    {
        return _content;
    }
}