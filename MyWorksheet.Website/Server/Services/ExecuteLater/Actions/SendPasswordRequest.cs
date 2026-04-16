using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services.Auth;
using MyWorksheet.Website.Server.Services.FileSystem;
using MyWorksheet.Website.Server.Services.Mail;
using MyWorksheet.Website.Server.Services.Mail.MailTemplates;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;
using MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.Contracts;
using MyWorksheet.Website.Server.Settings;
using Microsoft.Extensions.Options;

namespace MyWorksheet.Website.Server.Services.ExecuteLater.Actions;

[PriorityQueueItem(ActionKey)]
public class SendPasswordRequest : IPriorityQueueAction
{
    private readonly IResetPasswordManager _resetPasswordManager;
    private readonly IMailServiceProvider _mailServiceProvider;
    private ILocalFileProvider _localFileProvider;
    private readonly IOptions<AppServerSettings> _serverSettings;

    public SendPasswordRequest(IResetPasswordManager resetPasswordManager,
        IMailServiceProvider mailServiceProvider,
        ILocalFileProvider localFileProvider,
        IOptions<AppServerSettings> serverSettings)
    {
        _resetPasswordManager = resetPasswordManager;
        _mailServiceProvider = mailServiceProvider;
        _localFileProvider = localFileProvider;
        _serverSettings = serverSettings;
    }

    public const string ActionKey = "SEND_PASSWORD_MAIL";
    public string Name => "Sends a Password Request";
    public string Key => ActionKey;
    public Version Version { get; set; }

    /// <inheritdoc />
    public bool ValidateArguments(IDictionary<string, object> arguments)
    {
        return new DictionaryElementsValidator<string, object>(arguments)
            .ContainsKey("name")
            .ContainsKey("operating_system")
            .ContainsKey("browser_name")
            .ContainsKey("date")
            .ContainsKey("mailAddress")
            .Result;
    }

    public async Task Execute(PriorityQueueElement queueElement)
    {
        var arguments = queueElement.Arguments;

        var passwordResetToken = _resetPasswordManager
            .IssueResetToken(queueElement.UserId, (string)arguments.GetOrNull("name"), (string)arguments.GetOrNull("mailAddress"));
        if (passwordResetToken == null)
        {
            return;
        }

        var mailService = _mailServiceProvider.ApplicationMailService;
        await mailService.SendMail(new OutgingResetPasswordMail(
                (string)arguments.GetOrNull("name"),
                (string)arguments.GetOrNull("operating_system"),
                (string)arguments.GetOrNull("browser_name"),
                passwordResetToken,
                DateTime.Parse(arguments.GetOrNull("date").ToString()),
                _localFileProvider,
                _serverSettings.Value),
            queueElement.UserId,
            (string)arguments.GetOrNull("mailAddress"));
    }
}