using System;

namespace MyWorksheet.Public.Models.ObjectSchema
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class JsonIgnoreAttribute : Attribute
    {
    }
}