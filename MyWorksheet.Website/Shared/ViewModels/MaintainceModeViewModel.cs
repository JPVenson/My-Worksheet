using System;

namespace MyWorksheet.Website.Shared.ViewModels;

public class MaintainceModeViewModel : ViewModelBase
{
    public bool Strong
    {
        get { return _strong; }
        set { SetProperty(ref _strong, value); }
    }

    private bool _strong;
    public string Reason
    {
        get { return _reason; }
        set { SetProperty(ref _reason, value); }
    }

    private string _reason;
    public DateTime From
    {
        get { return _from; }
        set { SetProperty(ref _from, value); }
    }

    private DateTime _from;
    public DateTime Until
    {
        get { return _until; }
        set { SetProperty(ref _until, value); }
    }

    private DateTime _until;
}

public class Maintainance
{
    public Guid MaintainaceId { get; set; }
    public string Reason { get; set; }
    public DateTime From { get; set; }
    public DateTime Until { get; set; }
    public string CallerIp { get; set; }
    public string CompiledView { get; set; }
    public bool Completed { get; set; }
}