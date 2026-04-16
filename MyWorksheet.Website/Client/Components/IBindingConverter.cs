using System;

namespace MyWorksheet.Website.Client.Components;

public interface IBindingConverter
{
    object ConvertTo(object value, Type type);
    object ConvertFrom(object value, Type type);
}