using System;

namespace MyWorksheet.Website.Client.Components;

public interface IBindingConverter<TFrom, TTo>
{
    TTo ConvertTo(TFrom value, Type type);
    TFrom ConvertFrom(TTo value, Type type);
}
