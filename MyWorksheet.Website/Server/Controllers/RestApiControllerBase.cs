using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Models;
using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Util.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyWorksheet.Website.Server.Controllers;

public abstract class RestApiControllerBase<TEntity, TGetModel> : ApiControllerBase
    where TEntity : class
{
    private static readonly MethodInfo _containsMethodGenericCache = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static).First(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2);
    private static readonly ConcurrentDictionary<Type, MethodInfo> _containsQueryCache = new();

    public IMapperService MapperService { get; }
    public IDbContextFactory<MyworksheetContext> EntitiesFactory { get; }

    protected RestApiControllerBase(IDbContextFactory<MyworksheetContext> contextFactory,
        IMapperService mapperMapperService)
    {
        MapperService = mapperMapperService;
        EntitiesFactory = contextFactory;

        //  EntitiesFactory.GetDefaultConfig().GetOrCreateClassInfoCache(typeof(TEntity))
        // 	.Propertys.Any(f => f.Value.PropertyType == typeof(AppUser));
    }

    public bool? SearchWithUserReference { get; set; }

    [NonAction]
    protected virtual TEntity GetByUser(Guid id)
    {
        using var db = EntitiesFactory.CreateDbContext();

        var entityType = db.Model.FindEntityType(typeof(TEntity));
        var appUserType = db.Model.FindEntityType(typeof(AppUser));
        var primaryKey = entityType.FindPrimaryKey();
        var forignKey = appUserType.FindPrimaryKey().GetReferencingForeignKeys().Single(e => e.PrincipalEntityType == entityType);

        var equalExpressionParam = Expression.Parameter(typeof(TEntity));
        var equalExpression = Expression.And(
            Expression.Equal(Expression.Property(equalExpressionParam, primaryKey.GetName()), Expression.Constant(id)),
            Expression.Equal(Expression.Property(equalExpressionParam, forignKey.DependentToPrincipal.Name), Expression.Constant(User.GetUserId()))
        );
        var entity = db.Set<TEntity>()
            .FirstOrDefault(Expression.Lambda<Func<TEntity, bool>>(equalExpression));

        return entity;

        // return EntitiesFactory.CreateDbContext()
        // 	.Query()
        // 	.Select.Table<TEntity>()
        // 	.Where
        // 	.PrimaryKey().Is.EqualsTo(id)
        // 	.And
        // 	.ForeignKey<AppUser>().Is.EqualsTo(User.GetUserId())
        // 	.FirstOrDefault();
    }

    [NonAction]
    protected virtual TEntity[] GetAllByUser()
    {
        using var db = EntitiesFactory.CreateDbContext();

        var entityType = db.Model.FindEntityType(typeof(TEntity));
        var appUserType = db.Model.FindEntityType(typeof(AppUser));
        var primaryKey = entityType.FindPrimaryKey();
        var forignKey = appUserType.FindPrimaryKey().GetReferencingForeignKeys().Single(e => e.DeclaringEntityType == entityType);

        var equalExpressionParam = Expression.Parameter(typeof(TEntity));
        var equalExpression =
            Expression.Equal(Expression.Property(equalExpressionParam, forignKey.Properties.Single().Name), Expression.Constant(User.GetUserId()));
        return db.Set<TEntity>().Where(Expression.Lambda<Func<TEntity, bool>>(equalExpression, equalExpressionParam)).ToArray();
    }

    [NonAction]
    protected virtual TEntity SearchSingle(Guid id)
    {
        if (SearchWithUserReference == true)
        {
            return GetByUser(id);
        }
        using var db = EntitiesFactory.CreateDbContext();
        return db.Set<TEntity>().Find(id);
    }

    [NonAction]
    protected virtual TEntity[] GetByUser(int[] id)
    {
        using var db = EntitiesFactory.CreateDbContext();

        var entityType = db.Model.FindEntityType(typeof(TEntity));
        var appUserType = db.Model.FindEntityType(typeof(AppUser));
        var primaryKey = entityType.FindPrimaryKey();
        var forignKey = appUserType.FindPrimaryKey().GetReferencingForeignKeys().Single(e => e.PrincipalEntityType == entityType);

        var equalExpressionParam = Expression.Parameter(typeof(TEntity));

        var containsMethodInfo = _containsQueryCache.GetOrAdd(typeof(TEntity), static (key) => _containsMethodGenericCache.MakeGenericMethod(key));

        var equalExpression = Expression.And(
            Expression.Call(null, containsMethodInfo, Expression.Constant(id), Expression.Property(equalExpressionParam, primaryKey.GetName())),
            Expression.Equal(Expression.Property(equalExpressionParam, forignKey.DependentToPrincipal.Name), Expression.Constant(User.GetUserId()))
        );
        var entities = db.Set<TEntity>()
            .Where(Expression.Lambda<Func<TEntity, bool>>(equalExpression))
            .ToArray();
        return entities;
    }

    [NonAction]
    protected virtual TEntity[] SearchMany(int[] id)
    {
        if (SearchWithUserReference == true)
        {
            return GetByUser(id);
        }

        using var db = EntitiesFactory.CreateDbContext();

        var entityType = db.Model.FindEntityType(typeof(TEntity));
        var appUserType = db.Model.FindEntityType(typeof(AppUser));
        var primaryKey = entityType.FindPrimaryKey();
        var forignKey = appUserType.FindPrimaryKey().GetReferencingForeignKeys().Single(e => e.PrincipalEntityType == entityType);
        var containsMethodInfo = _containsQueryCache.GetOrAdd(typeof(TEntity), static (key) => _containsMethodGenericCache.MakeGenericMethod(key));

        var equalExpressionParam = Expression.Parameter(typeof(TEntity));
        var equalExpression =
            Expression.Call(null, containsMethodInfo, Expression.Constant(id), Expression.Property(equalExpressionParam, primaryKey.GetName()));
        var entities = db.Set<TEntity>()
            .Where(Expression.Lambda<Func<TEntity, bool>>(equalExpression))
            .ToArray();
        return entities;
    }

    [HttpGet("GetSingle")]
    [Authorize]
    public virtual IActionResult GetSingle(Guid id)
    {
        var entity = SearchSingle(id);
        if (entity == null)
        {
            return NoContent();
        }

        return Data(MapperService.ViewModelMapper.Map<TGetModel>(entity));
    }

    [HttpGet()]
    [Authorize]
    [Route("GetMany")]
    public virtual IActionResult GetMany([FromQuery] int[] ids)
    {
        if (!ids.Any())
        {
            return NoContent();
        }

        var entity = SearchMany(ids.ToArray());
        if (entity.Length == 0)
        {
            return NoContent();
        }

        return Data(MapperService.ViewModelMapper.Map<TGetModel[]>(entity));
    }

    [HttpGet()]
    [Authorize]
    [Route("GetList")]
    public virtual IActionResult GetList()
    {
        var entity = GetAllByUser();
        if (entity.Length == 0)
        {
            return NoContent();
        }

        return Data(MapperService.ViewModelMapper.Map<TGetModel[]>(entity));
    }
}

public abstract class RestApiControllerBase<TEntity, TGetModel, TPostModel> : RestApiControllerBase<TEntity, TGetModel>
    where TEntity : class
{
    public RestApiControllerBase(IDbContextFactory<MyworksheetContext> contextFactory, IMapperService mapperMapperService)
        : base(contextFactory, mapperMapperService)
    {
    }

    [Route("Update")]
    [HttpPost()]
    [Authorize]
    public abstract ValueTask<IActionResult> Update([FromBody] TPostModel model, [FromQuery] Guid id);

    [Route("Create")]
    [HttpPost()]
    [Authorize]
    public abstract ValueTask<IActionResult> Create([FromBody] TPostModel model);
}