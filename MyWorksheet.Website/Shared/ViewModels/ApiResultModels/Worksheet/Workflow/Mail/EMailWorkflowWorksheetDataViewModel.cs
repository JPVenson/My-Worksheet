using System;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Workflow.Mail;

public class EMailWorkflowWorksheetDataViewModel : ViewModelBase
{
    private string _receiver;

    private string _signer;

    private DateTimeOffset _worksheetEnd;

    private DateTimeOffset _worksheetStart;

    public string Signer
    {
        get { return _signer; }
        set { SetProperty(ref _signer, value); }
    }

    public DateTimeOffset WorksheetStart
    {
        get { return _worksheetStart; }
        set { SetProperty(ref _worksheetStart, value); }
    }

    public DateTimeOffset WorksheetEnd
    {
        get { return _worksheetEnd; }
        set { SetProperty(ref _worksheetEnd, value); }
    }

    public string Receiver
    {
        get { return _receiver; }
        set { SetProperty(ref _receiver, value); }
    }
}