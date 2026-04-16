namespace MyWorksheet.Website.Server.Services.ServerSettings;

public interface ISettingsObject
{
    string Path { get; set; }
    string Name { get; set; }
    object Value { get; set; }
    bool ReadOnly { get; set; }
    T As<T>();
}
