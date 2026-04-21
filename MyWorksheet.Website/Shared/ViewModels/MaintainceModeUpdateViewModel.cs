namespace MyWorksheet.Website.Shared.ViewModels;

public class MaintainceModeUpdateViewModel : MaintainceModeViewModel
{
    public bool Scheduled
    {
        get { return _scheduled; }
        set { SetProperty(ref _scheduled, value); }
    }

    private bool _scheduled;
    public string CallerIp
    {
        get { return _callerIp; }
        set { SetProperty(ref _callerIp, value); }
    }

    private string _callerIp;
}