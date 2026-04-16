using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Components.Dialog;
using MyWorksheet.Website.Client.Components.Form;
using MyWorksheet.Website.Client.Util.View;

namespace MyWorksheet.Website.Client.Pages.Shared.Dialog;

public class MessageBoxDialogViewModel : DialogViewModelBase
{
    public MessageBoxDialogViewModel()
    {
        Commands = new List<MessageBoxButton>();
    }

    public LocalizableString Header { get; set; }
    public LocalizableString Message { get; set; }
    public ICollection<MessageBoxButton> Commands { get; set; }

    public object Result { get; set; }

    public static MessageBoxDialogViewModel YesNo(LocalizableString header, LocalizableString message)
    {
        return new MessageBoxDialogViewModel()
        {
            Header = header,
            Message = message,
            Commands =
            {
                new MessageBoxButton()
                {
                    Style = "btn btn-outline-primary",
                    Value = true,
                    Title = "Common/Yes"
                },
                new MessageBoxButton()
                {
                    Style = "btn btn-outline-secondary",
                    Value = false,
                    Title = "Common/No"
                }
            }
        };
    }

    public async Task SetStatus(MessageBoxButton btn)
    {
        Result = btn.Value;
        await Close();
    }
}

public class MessageBoxButton
{
    public string Style { get; set; }
    public object Value { get; set; }
    public LocalizableString Title { get; set; }
}