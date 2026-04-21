using System;

namespace MyWorksheet.Public.Models.ObjectSchema;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
public sealed class JsonCommentAttribute : Attribute
{
    public JsonCommentAttribute(string codeComment)
    {
        CodeComment = codeComment;
    }
    public string CodeComment { get; set; }
}