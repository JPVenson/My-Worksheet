using MyWorksheet.Website.Server.Services.Reporting.Morestachio;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Server.Services.NumberRangeService;

public class ProjectNumberRangeFactory : NumberRangeFactoryBase<PostProjectModel>
{
    public const string NrCode = "ProjectNr";
    public override string Code { get; } = NrCode;
    public override string Description { get; } = "NumberRange/Description.Project";

    public override string GetDefaultTemplate()
    {
        return "Proj{{Counter}}";
    }

    public override PostProjectModel GetTestDataInternal()
    {
        return new PostProjectModel()
        {
            Name = "Test Project",
            UserOrderNo = 0,
        };
    }

    public ProjectNumberRangeFactory(MustachioFormatterService mustachioFormatterService) : base(mustachioFormatterService)
    {
    }
}
