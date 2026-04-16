using System;

namespace MyWorksheet.Website.Client.Pages.Home.Widgets
{
    /// <summary>
    /// Attribute to mark a class as a dashboard widget.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DashboardWidgetAttribute : Attribute
    {
        /// <summary>
        /// Gets the name of the dashboard widget.
        /// </summary>
        public string Name { get; }

        public string Key { get; set; }

        public string Icon { get; set; }

        public bool DisplayOnce { get; set; }

        /// <summary>
        /// Gets the description of the dashboard widget.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets or sets the display order of the widget on the dashboard.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardWidgetAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the widget.</param>
        /// <param name="description">The description of the widget.</param>
        public DashboardWidgetAttribute(string key, string name, string description, string icon, bool displayOnce)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Icon = icon;
            DisplayOnce = displayOnce;
            Order = 0;
        }
    }
}