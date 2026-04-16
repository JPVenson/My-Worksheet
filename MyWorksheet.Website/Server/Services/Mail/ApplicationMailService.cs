using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Katana.CommonTasks.Extentions;
using Katana.CommonTasks.Models;
using MyWorksheet.Website.Server.Services.Mail.MailTemplates;
using MyWorksheet.Website.Server.Settings;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.EMail;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using ServiceLocator.Attributes;
using Microsoft.Extensions.Options;

namespace MyWorksheet.Website.Server.Services.Mail;

[SingletonService(typeof(IMailService))]
public class ApplicationMailService : IMailService
{
    public string Mail { get; set; }
    public string UserName { get; set; }
    public string Server { get; set; }
    public int Port { get; set; }
    public string Password { get; set; }
    public bool IsTestRealm { get; set; }
    public int Protocol { get; set; }

    protected IOptions<MailSettings> MailSettings { get; }
    public ApplicationMailService(IOptions<MailSettings> mailSettings, IOptions<TransformationSettings> transformationSettings)
    {
        MailSettings = mailSettings;

        Server = mailSettings.Value.Send.Realm;
        UserName = mailSettings.Value.Send.Username;
        Mail = mailSettings.Value.Send.Sender;
        Port = mailSettings.Value.Send.Port;
        Password = mailSettings.Value.Send.Password;
        Protocol = mailSettings.Value.Send.Protocol;
        IsTestRealm = transformationSettings.Value.Realm == "test";
    }

    public ApplicationMailService(string mail, string password, string userName, string server, int port, int protocol)
    {
        Password = password;
        Port = port;
        Protocol = protocol;
        Mail = mail;
        UserName = userName;
        Server = server;
    }

    public virtual void PreSendMail(Mail mail, MimeMessage mailMessage, params string[] recipients)
    {

    }

    public virtual void PreProcessMail(Mail mail)
    {
        if (mail.Body != null)
        {
            mail.Body = mail.Body;
        }
    }

    public virtual async Task<QuestionableBoolean> SendMail(Mail mail, Guid userId, params string[] recipients)
    {
        var attachments = new List<Stream>();
        try
        {
            var templateMail = mail as TemplateMail;
            if (templateMail != null)
            {
                await templateMail.Init();
            }

            PreProcessMail(mail);
            var mailMessage = new MimeMessage();
            foreach (var recipient in recipients)
            {
                mailMessage.To.Add(MailboxAddress.Parse(recipient));
            }

            mailMessage.From.Add(MailboxAddress.Parse(Mail));
            mailMessage.Subject = mail.Subject;
            if (IsTestRealm)
            {
                mailMessage.Subject = "TEST REALM" + mail.Subject;
            }

            var body = new TextPart(TextFormat.Html)
            {
                Text = mail.RenderBody(),
            };

            if (mail.Attachments.Any())
            {
                var mailMultipartMessage = new Multipart("mixed")
                {
                    body
                };
                foreach (var mailAttachment in mail.Attachments)
                {
                    attachments.Add(mailAttachment.Content);
                    mailMultipartMessage.Add(new MimePart(mailAttachment.FileType)
                    {
                        Content = new MimeContent(mailAttachment.Content),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Default,
                        FileName = mailAttachment.Name,

                    });
                }

                mailMessage.Body = mailMultipartMessage;
            }
            else
            {
                mailMessage.Body = body;
            }

            PreSendMail(mail, mailMessage, recipients);
            if (MailSettings?.Value.Send.Dns == true)
            {
                return true;
            }
            return await SendPerSmtp(mailMessage);
        }
        catch (Exception ex)
        {
            return false.Because(ex.Message);
        }
        finally
        {
            foreach (var mailAttachment in attachments)
            {
                mailAttachment.Dispose();
            }
        }
    }

    public string MailType { get; set; }

    public static IEnumerable<EMailProtcol> GetProtocols()
    {
        yield return new EMailProtcol()
        {
            Id = 0,
            Name = "None"
        };
        yield return new EMailProtcol()
        {
            Id = 1,
            Name = "STARTTLS"
        };
        yield return new EMailProtcol()
        {
            Id = 2,
            Name = "SSL"
        };
    }

    public static SecureSocketOptions Map(int id)
    {
        var firstOrDefault = GetProtocols().FirstOrDefault(e => e.Id == id);
        switch (firstOrDefault.Id)
        {
            case 0:
                return SecureSocketOptions.None;
            case 1:
                return SecureSocketOptions.StartTls;
            case 2:
                return SecureSocketOptions.SslOnConnect;
            default:
                return SecureSocketOptions.None;
        }
    }

    protected virtual async Task<bool> SendPerSmtp(MimeMessage mailMessage)
    {
        using (var client = new SmtpClient())
        {
            client.Timeout = 30000;
            client.SslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
            client.ServerCertificateValidationCallback = ServerCertificateValidationCallback;

            await client.ConnectAsync(Server, Port, Map(Protocol))
                .ConfigureAwait(false);
            await client.AuthenticateAsync(UserName, Password)
                .ConfigureAwait(false);

            await client.SendAsync(mailMessage);
            await client.DisconnectAsync(true);
            return true;
        }
    }

    private bool ServerCertificateValidationCallback(object sender,
        X509Certificate certificate,
        X509Chain chain,
        SslPolicyErrors sslpolicyerrors)
    {
        if (sslpolicyerrors == SslPolicyErrors.None)
        {
            return true;
        }
        var serialNumberString = certificate.GetSerialNumberString();
        return serialNumberString == "";
    }
}