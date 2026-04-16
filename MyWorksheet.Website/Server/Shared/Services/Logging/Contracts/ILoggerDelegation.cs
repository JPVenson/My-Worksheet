using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

public interface ILoggerDelegation
{
    void Log(string message, string category, string level, IDictionary<string, string> optionalData, DateTime dateCreated);
    //void Log(string message, string category, string level, IDictionary<string, string> optionalData);
    //void Log(string message, string category, string level);
    //void Log(string message, string category);
    //void Log(string message);

    void Started();

    ILoggerDelegation Copy();
}