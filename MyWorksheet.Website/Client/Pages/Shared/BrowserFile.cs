using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Client.Util.View;
using Microsoft.AspNetCore.Components.Forms;

namespace MyWorksheet.Website.Client.Pages.Shared;

public class BrowserFile
{
    public IBrowserFile File { get; set; }
    public long CurrentProgress { get; set; }
    public long MaxProgress { get; set; }
    public string FileThumbnail { get; set; }
}