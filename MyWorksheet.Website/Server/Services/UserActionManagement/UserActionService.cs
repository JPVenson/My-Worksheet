using System.Collections.Generic;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Webpage.Services.UserActionManagement;

public class UserActionService
{
    public IDictionary<int, UserAction> UserActions { get; private set; }
}