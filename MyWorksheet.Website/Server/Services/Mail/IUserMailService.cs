using System.Threading.Tasks;
using System;
using Katana.CommonTasks.Models;

namespace MyWorksheet.Website.Server.Services.Mail;

public interface IUserMailService : IMailService
{
    Task<QuestionableBoolean> Test(Guid userId);
}