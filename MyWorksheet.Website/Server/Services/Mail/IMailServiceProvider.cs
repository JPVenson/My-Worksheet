using System;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.Mail;

public interface IMailServiceProvider
{
    IMailService ApplicationMailService { get; }
    IUserMailService UserMailService(MyworksheetContext db, Guid userMailId);
}