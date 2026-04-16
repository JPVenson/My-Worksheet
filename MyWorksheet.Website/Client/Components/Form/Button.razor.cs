using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MyWorksheet.Website.Client.Pages.Base;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MyWorksheet.Website.Client.Components.Form;

public partial class Button : ComponentViewBase
{
    private ICommand _command;

    [Parameter]
    [Required]
    public ICommand Command
    {
        get { return _command; }
        set
        {
            SetProperty(ref _command, value, CommandChanged);
        }
    }

    [Parameter]
    public EventCallback<ICommand> CommandChanged { get; set; }

    [Parameter]
    public object Parameter { get; set; }

    [Parameter]
    public bool CanExecute { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object> Arguments { get; set; }

    private void ExecuteCommand(MouseEventArgs obj)
    {
        if (Command.CanExecute(Parameter))
        {
            Command.Execute(Parameter);
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Command.CanExecuteChanged += Command_CanExecuteChanged;
    }

    private void Command_CanExecuteChanged(object sender, EventArgs e)
    {
        CanExecute = Command.CanExecute(Parameter);
        StateHasChanged();
    }
}

public class DelegateCommand : ICommand
{
    private readonly Func<object, bool> _canExecute;
    private readonly Action<object> _execute;

    public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public DelegateCommand(Action<object> execute) : this(execute, (d) => true)
    {
    }

    public DelegateCommand(Action execute, Func<bool> canExecute) : this((d) => execute(), (d) => canExecute())
    {
    }

    public DelegateCommand(Action execute) : this(execute, () => true)
    {
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
        _execute(parameter);
    }

    public event EventHandler CanExecuteChanged;

    public virtual void OnCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}