using System;
using System.ComponentModel.DataAnnotations;
using MyWorksheet.Public.Models.ObjectSchema;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

[ObjectTracking("Project")]
public class PostProjectModel : ViewModelBase
{
    private Guid? _idDefaultRate;
    private Guid? _idOrganisation;
    private Guid? _idWorksheetWorkflow;
    private Guid? _idWorksheetWorkflowDataMap;
    private string _name;
    private int _userOrderNo;
    private string _projectReference;
    private bool _hidden;
    private Guid? _idPaymentCondition;

    [Required]
    [JsonComment("Project.Model.Description/IdPaymentTarget")]
    [Display(Name = "Links/PaymentTarget")]
    public Guid? IdPaymentCondition
    {
        get { return _idPaymentCondition; }
        set { SetProperty(ref _idPaymentCondition, value); }
    }

    [Required]
    [JsonComment("Project.Model.Description/Name")]
    [Display(Name = "Common/Name")]
    public string Name
    {
        get { return _name; }
        set { SetProperty(ref _name, value); }
    }

    [Required]
    [JsonComment("Project.Model.Description/ProjectReference")]
    [Display(Name = "Project/ProjectReference")]
    public string ProjectReference
    {
        get { return _projectReference; }
        set { SetProperty(ref _projectReference, value); }
    }

    [Required]
    [Obsolete]
    public int UserOrderNo
    {
        get { return _userOrderNo; }
        set { SetProperty(ref _userOrderNo, value); }
    }

    [JsonComment("Project.Model.Description/IdOrganisation")]
    public Guid? IdOrganisation
    {
        get { return _idOrganisation; }
        set { SetProperty(ref _idOrganisation, value); }
    }

    public Guid? IdWorksheetWorkflow
    {
        get { return _idWorksheetWorkflow; }
        set { SetProperty(ref _idWorksheetWorkflow, value); }
    }

    public Guid? IdWorksheetWorkflowDataMap
    {
        get { return _idWorksheetWorkflowDataMap; }
        set { SetProperty(ref _idWorksheetWorkflowDataMap, value); }
    }

    public Guid? IdDefaultRate
    {
        get { return _idDefaultRate; }
        set { SetProperty(ref _idDefaultRate, value); }
    }

    public bool Hidden
    {
        get { return _hidden; }
        set { SetProperty(ref _hidden, value); }
    }
}