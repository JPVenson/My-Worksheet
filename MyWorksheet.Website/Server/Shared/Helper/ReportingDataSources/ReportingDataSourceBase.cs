using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MyWorksheet.Private.Models.ObjectSchema;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.Reporting;
using MyWorksheet.Website.Server.Services.Reporting.Models;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine;

namespace MyWorksheet.Website.Server.Shared.Helper.ReportingDataSources;

public abstract class ReportingDataSourceBase<TEntity, TVm> : IReportingDataSource
    where TEntity : class
{
    protected IMapperService MapperService { get; }

    protected ReportingDataSourceBase(IMapperService mapperService)
    {
        MapperService = mapperService;
        Purpose = Array.Empty<ReportPurpose>();
    }

    public abstract Guid Id { get; set; }
    public abstract string Key { get; set; }
    public abstract string Name { get; set; }
    public ReportPurpose[] Purpose { get; set; }
    public abstract string DataName { get; set; }
    public abstract IObjectSchema QuerySchema();


    /// <inheritdoc />
    public virtual IObjectSchema ArgumentSchema(MyworksheetContext db, Guid userId)
    {
        return JsonSchema.EmptyNotNull;
    }


    public virtual IDictionary<string, object> GetData(MyworksheetContext db, Guid userId,
        ReportingExecutionParameterValue[] query, IDictionary<string, object> arguments)
    {

        var entityType = db.Model.FindEntityType(typeof(TEntity));
        var appUserType = db.Model.FindEntityType(typeof(AppUser));
        var primaryKey = entityType.FindPrimaryKey();
        var forignKey = appUserType.FindPrimaryKey().GetReferencingForeignKeys().Single(e => e.PrincipalEntityType == entityType);

        var equalExpressionParam = Expression.Parameter(typeof(TEntity));
        var equalExpression =
            Expression.Equal(Expression.Property(equalExpressionParam, forignKey.DependentToPrincipal.Name), Expression.Constant(userId));
        var projectsOfUserQuery = db.Set<TEntity>()
            .Where(Expression.Lambda<Func<TEntity, bool>>(equalExpression));

        projectsOfUserQuery = DataSourceHelper.TranslateQuery(projectsOfUserQuery, query);
        return new Dictionary<string, object>
        {
            {
                "DataSource", new Dictionary<string, object>
                {
                    {
                        DataName, MapperService.ViewModelMapper.Map<TVm[]>(projectsOfUserQuery.ToArray())
                    }
                }
            }
        };
    }

    public virtual ReportingParameterInfo[] GetParameterInfos(MyworksheetContext config, Guid userId)
    {
        return DataSourceHelper.FillRemaining<TEntity>(new List<ReportingParameterInfo>(), userId, "IdCreator").ToArray();
    }
}