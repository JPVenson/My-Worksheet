using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Shared.Services.Logging;

public class FormattedOutputLogger : ILoggerDelegation
{
    private readonly Action<string> _writeToOutput;

    public string MessageFormat { get; set; }

    public Func<bool> Enabled { get; set; }

    public static StreamLoggerMessageParts Message { get; }
    public static StreamLoggerMessageParts Category { get; }
    public static StreamLoggerMessageParts Level { get; }
    public static StreamLoggerMessageParts DateCreated { get; }
    public static KeyValueStreamLoggerMessagePart AdditonalData { get; }
    public static string DefaultFormat { get; set; }

    static FormattedOutputLogger()
    {
        Message = new StreamLoggerMessageParts("Message", "mess");
        Category = new StreamLoggerMessageParts("Category", "cat");
        Level = new StreamLoggerMessageParts("Level", "lvl");
        DateCreated = new StreamLoggerMessageParts("DateCreated", "date");
        AdditonalData = new KeyValueStreamLoggerMessagePart("AdditonalData", "key", "value");
        DefaultFormat = string.Format("{2}!{0}->{1} = \"{3}\"\r\n {4}", Level, Category, DateCreated, Message, AdditonalData);
    }

    public FormattedOutputLogger(TextWriter stream) : this()
    {
        _writeToOutput = stream.WriteLine;
    }

    public FormattedOutputLogger(StreamWriter stream) : this()
    {
        _writeToOutput = stream.WriteLine;
    }

    public FormattedOutputLogger(Action<string> stream) : this()
    {
        _writeToOutput = stream;
    }

    protected FormattedOutputLogger()
    {
        MessageFormat = DefaultFormat;
        Enabled = () => true;
    }

    public void Log(string message, string category, string level, IDictionary<string, string> optionalData, DateTime dateCreated)
    {
        if (!Enabled())
            return;
        var templateString = MessageFormat
            .Replace(Message.ToString(), message)
            .Replace(Category.ToString(), category)
            .Replace(Level.ToString(), level)
            .Replace(DateCreated.ToString(), dateCreated.ToString(CultureInfo.InvariantCulture));
        var additonalValues = "";
        if (optionalData != null)
        {
            foreach (var item in optionalData)
            {
                additonalValues += AdditonalData.ToString().Replace(AdditonalData.KeyName, item.Key)
                    .Replace(AdditonalData.ValueName, item.Value);
            }

            templateString = templateString.Replace(AdditonalData.ToString(), additonalValues);
        }
        else
        {
            templateString = templateString.Replace(AdditonalData.ToString(), "");
        }


        _writeToOutput(templateString);
    }

    public void Log(string message, string category, string level, IDictionary<string, string> optionalData)
    {
        Log(message, category, level, optionalData, DateTime.UtcNow);
    }

    public void Log(string message, string category, string level)
    {
        Log(message, category, level, new Dictionary<string, string>());
    }

    public void Log(string message, string category)
    {
        Log(message, category, null);
    }

    public void Log(string message)
    {
        Log(message, null);
    }

    public void Started()
    {
        throw new NotImplementedException();
    }

    public ILoggerDelegation Copy()
    {
        return new BufferLogger();
    }
}
