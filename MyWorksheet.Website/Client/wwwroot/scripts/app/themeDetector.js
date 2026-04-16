function GetThemeType() {
    return window.matchMedia("(prefers-color-scheme: dark)").matches;
}

MyWorksheet.Blazor.GetThemeType = GetThemeType;