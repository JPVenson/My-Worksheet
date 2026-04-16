using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Shared.Data.Display.Views;

public partial class DataDisplayTableView<T>
{
    [Parameter]
    public DataDisplay<T> DataDisplay { get; set; }
}