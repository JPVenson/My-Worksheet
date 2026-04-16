using System;

using System.Collections.Generic;
using MyWorksheet.Helper;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Shared.ObjectSchema;

namespace MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;

public sealed class ProjectDataSource : ReportingDataSourceBase<ProjectReporting, ProjectReporting>
{
    class ProjectReportArguments : ArgumentsBase
    {
        public List<ProjectReporting> Projects { get; set; }
    }

    public ProjectDataSource(IMapperService mapperService) : base(mapperService)
    {
        Name = "Projects";
        Key = "projectsDataSource";
        Id = new Guid("00000000-0000-0000-0002-000000000001");
        DataName = "Projects";
    }

    public override Guid Id { get; set; }
    public override string Key { get; set; }
    public override string Name { get; set; }
    public override string DataName { get; set; }

    public override IObjectSchema QuerySchema()
    {
        return JsonSchemaExtensions.JsonSchema(typeof(ReportingDataStructureBase<ProjectReportArguments>));
    }
}