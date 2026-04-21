using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.NumberRange;

public class NumberRangeModel : CreateNumberRangeModel
{
    public Guid AppNumberRangeId { get; set; }

    public override Guid? GetModelIdentifier()
    {
        return AppNumberRangeId;
    }
}