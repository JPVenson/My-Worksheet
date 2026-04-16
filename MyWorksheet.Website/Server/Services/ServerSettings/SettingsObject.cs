using System;

namespace MyWorksheet.Website.Server.Services.ServerSettings;

public class SettingsObject : ISettingsObject
{
    public virtual string Path { get; set; }
    public virtual string Name { get; set; }
    public virtual object Value { get; set; }
    public virtual bool ReadOnly { get; set; } = false;

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="InvalidCastException"></exception>
    /// <returns></returns>
    public T As<T>()
    {
        if (Value == null)
            return (T)Value;

        if (Value.Equals(default(T)))
            return (T)Value;

        if (Value is T)
            return (T)Value;

        if (Value is IConvertible convertebalValue)
        {
            return (T)Convert.ChangeType(Value, typeof(T));
        }

        return (T)Value;
    }
}
