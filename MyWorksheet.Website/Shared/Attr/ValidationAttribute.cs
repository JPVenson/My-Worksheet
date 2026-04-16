using System;
using System.Text.RegularExpressions;

namespace MyWorksheet.Public.Models.ObjectSchema
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ValidationAttribute : Attribute
    {
        public Regex Regex { get; set; }

        public ValueValidator BuildValidator()
        {
            return new ValueValidator()
            {
                ValidatorRegex = Regex
            };
        }
    }
}