namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Statistics
{
    public class DisplayTypes : ViewModelBase
    {
        private DisplayTypes(string typeName)
        {
            TypeName = typeName;
        }

        public string TypeName { get; }

        public static DisplayTypes Line { get; } = new DisplayTypes("line");
        public static DisplayTypes Bar { get; } = new DisplayTypes("bar");
        public static DisplayTypes Pie { get; } = new DisplayTypes("pie");
        public static DisplayTypes Radar { get; } = new DisplayTypes("radar");
        public static DisplayTypes PolarArea { get; } = new DisplayTypes("polarArea");
        public static DisplayTypes Doughnut { get; } = new DisplayTypes("doughnut");
        public static DisplayTypes HorizontalBar { get; } = new DisplayTypes("horizontalBar");

        public override string ToString()
        {
            return TypeName;
        }
    }
}