using System;
using System.ComponentModel.DataAnnotations;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Asserts;

[ObjectTracking("WorksheetAssert")]
public class WorksheetAssertGetViewModel : ViewModelBase
{
    private string _description;

    private string _name;

    private decimal _tax;

    private decimal _value;

    private Guid _worksheetAssertId;

    public Guid WorksheetAssertId
    {
        get { return _worksheetAssertId; }
        set { SetProperty(ref _worksheetAssertId, value); }
    }

    [Required]
    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }

    [Required]
    public decimal Value
    {
        get { return _value; }
        set { SetProperty(ref _value, value); }
    }

    [Required]
    public decimal Tax
    {
        get { return _tax; }
        set { SetProperty(ref _tax, value); }
    }

    [Required]
    public string Description
    {
        get { return _description; }
        set { SetProperty(ref _description, value); }
    }

    public override Guid? GetModelIdentifier()
    {
        return WorksheetAssertId;
    }
}