namespace MyWorksheet.Public.Models.ObjectSchema;

public interface IFunctionInfo
{
    string Name { get; }
    string ReturnValue { get; }
    string[] Arguments { get; }
}