using System;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Util.View;

public static class CssHelper
{
    public static MarkupString When(bool condition, string style)
    {
        return (MarkupString)(condition ? style : "");
    }
    public static MarkupString When(bool condition, string style, string elseStyle)
    {
        return (MarkupString)(condition ? style : elseStyle);
    }
}