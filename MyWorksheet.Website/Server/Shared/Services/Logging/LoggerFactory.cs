using MyWorksheet.Website.Server.Shared.Services.Logging.Contracts;

namespace MyWorksheet.Website.Server.Shared.Services.Logging;

public class LoggerFactory
{
    private static DelegateLogger _logger;

    public static DelegateLogger Logger
    {
        get
        {
            if (_logger == null)
            {
                InitLogger();
            }

            return _logger;
        }
        set { _logger = value; }
    }

    private static void InitLogger()
    {
        Logger = new DelegateLogger();
    }
}