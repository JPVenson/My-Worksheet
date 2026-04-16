using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MyWorksheet.Website.Server.Services.Templating.Pdf;

namespace MyWorksheet.Website.Server.Services.Reporting.Morestachio;

public class UserContext : IDisposable
{
    public UserContext(Guid userId)
    {
        UserId = userId;
        Cache = new ConcurrentDictionary<string, object>();
    }

    public Guid UserId { get; private set; }
    public IDictionary<string, object> Cache { get; private set; }
    public SizeF? PaperSize { get; set; }
    public bool Grayscale { get; set; }
    public string Title { get; set; }

    public PdfAddon Header { get; set; }
    public PdfAddon Footer { get; set; }

    public void Dispose()
    {
        foreach (var maybeDisposable in Cache.OfType<IDisposable>())
        {
            maybeDisposable.Dispose();
        }

        Cache = null;
    }
}