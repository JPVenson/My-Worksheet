using System;
using System.ComponentModel.DataAnnotations;
using MyWorksheet.Public.Models.ObjectSchema;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

public class GetProjectModel : PostProjectModel
{
    private bool _noModifications;

    private Guid _projectId;

    [JsonComment("Project.Model.Description/ProjectId")]
    public Guid ProjectId
    {
        get { return _projectId; }
        set { SetProperty(ref _projectId, value); }
    }

    [JsonComment("Project.Model.Description/NoModifications")]
    [IgnoreState]
    public bool NoModifications
    {
        get { return _noModifications; }
        set { SetProperty(ref _noModifications, value); }
    }

    [JsonComment("Project.Model.Description/NumberRangeEntry")]
    [Display(Name = "Common/NumberRange")]
    public string NumberRangeEntry { get; set; }

    public override Guid? GetModelIdentifier()
    {
        return ProjectId;
    }
}