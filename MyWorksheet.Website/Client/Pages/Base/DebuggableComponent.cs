using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Pages.Base;

public class DebuggableComponent : ComponentBase
{
    [Parameter]
    public bool Log { get; set; }

    public void WriteLine(object content)
    {
        if (Log)
        {
            Console.WriteLine(content);
        }
    }
}