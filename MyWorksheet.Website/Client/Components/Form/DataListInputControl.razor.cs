using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using MyWorksheet.Website.Client.Util.View;
using Microsoft.AspNetCore.Components;

namespace MyWorksheet.Website.Client.Components.Form;

public partial class DataListInputControl<TValue>
{
    public DataListInputControl()
    {
        DisplayValue = d => d.ToString();
        EditorValue = d => d.ToString();
    }

    [CascadingParameter]
    public InputControl<TValue> FieldControl { get; set; }

    [Parameter]
    public Func<TValue, string> DisplayValue { get; set; }
    [Parameter]
    public Func<TValue, string> EditorValue { get; set; }

    [Parameter]
    public Func<TValue, Task<IEnumerable<TValue>>> Options { get; set; }

    public IEnumerable<TValue> GeneratedOptions { get; set; }

    private Timer _debouceOptionsTimer;

    private void SetupOrRenewOptionsTimer()
    {
        if (_debouceOptionsTimer == null)
        {
            _debouceOptionsTimer = new Timer(550);
            _debouceOptionsTimer.Elapsed += _debouceOptionsTimer_Elapsed;
        }
        else
        {
            _debouceOptionsTimer.Stop();
        }
        _debouceOptionsTimer.Start();
    }

    private void _debouceOptionsTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
        _debouceOptionsTimer.Stop();
        InvokeAsync(async () =>
        {
            GeneratedOptions = await Options(FieldControl.Value);
            StateHasChanged();
        });
    }

    protected override Task OnInitializedAsync()
    {
        var old = FieldControl.OnValueChanged;
        FieldControl.OnValueChanged = new EventCallback<ValueChangedEventArgs<TValue>>(this,
            new Func<ValueChangedEventArgs<TValue>, Task>(async (d) =>
            {
                await old.RaiseAsync(d);
                SetupOrRenewOptionsTimer();
            }));
        return base.OnInitializedAsync();
    }
}