namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Dashboard
{
    public class DashboardPluginInfoViewModel : ViewModelBase
    {
        public string ArgumentsQuery { get; set; }
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }
        public int GridX { get; set; }
        public int GridY { get; set; }
    }
}