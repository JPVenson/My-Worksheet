using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Morestachio.Formatter.Framework;
using Morestachio.Helper;

namespace MyWorksheet.ReportService.Services.Templating.Text.FormatterFramework.javascript;

public class FormatterWrapper
{
    private readonly IEnumerable<MorestachioFormatterModel> _formatter;

    public FormatterWrapper(IEnumerable<MorestachioFormatterModel> formatter)
    {
        _formatter = formatter;
    }

    [JsCallable]
    public object Format(string name, params object[] args)
    {
        var formatter = _formatter.FirstOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        var result = formatter?.Function.Invoke(null, args);

        if (result is Task task2)
        {
            result = result.UnpackFormatterTask();
        }

        return result;
    }
}