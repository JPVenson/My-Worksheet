using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.ChangeTracking;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Signal;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Services.Repository;

public class LocalCacheRepository<TEntity> : ICacheRepository<TEntity> where TEntity : IEntityObject
{
    private readonly RestHttpAccessBase<TEntity> _repository;

    public LocalCacheRepository(ChangeTrackingService changeTrackingService, HttpService httpService)
    {
        changeTrackingService.RegisterTracking(typeof(TEntity), EntityChanged);
        _repository = httpService.For<TEntity>();
        Cache = new PartialExternalList<TEntity>(_repository);
    }

    private async void EntityChanged(EntityChangedEventArguments eventArguments)
    {
        var type = eventArguments.ChangeEventTypes;

        //Console.WriteLine($"{typeof(TEntity)} - {type}:{id}");
        if (type == ChangeEventTypes.Removed)
        {
            foreach (var eventArgumentsId in eventArguments.Ids)
            {
                Cache.RemoveId(eventArgumentsId);
            }
        }
        else if (Cache.FullyLoaded)
        {
            if (type == ChangeEventTypes.Added)
            {
                var unknownIds = eventArguments.Ids.Where(e => !Cache.ContainsId(e)).ToArray();
                if (unknownIds.Length == 1)
                {
                    var addApiResult = await _repository.GetSingle(unknownIds[0]);
                    if (addApiResult.Success)
                    {
                        Cache.Add(addApiResult.Object);
                    }
                }
                else if (unknownIds.Length > 1)
                {
                    var unknownEntities = await _repository.GetList(unknownIds);
                    if (unknownEntities.Success)
                    {
                        foreach (var entityObject in unknownEntities.Object)
                        {
                            Cache.Add(entityObject);
                        }
                    }
                }
            }
            else if (type == ChangeEventTypes.Changed)
            {
                if (eventArguments.Ids.Length == 1)
                {
                    var addApiResult = await _repository.GetSingle(eventArguments.Ids[0]);
                    if (addApiResult.Success)
                    {
                        Cache.RemoveId(eventArguments.Ids[0]);
                        Cache.Add(addApiResult.Object);
                    }
                }
                else if (eventArguments.Ids.Length > 1)
                {
                    var unknownEntities = await _repository.GetList(eventArguments.Ids);
                    if (unknownEntities.Success)
                    {
                        foreach (var entityObject in unknownEntities.Object)
                        {
                            Cache.Overwrite(entityObject);
                        }
                    }
                }
            }
        }
        else if (type == ChangeEventTypes.Changed)
        {
            var idsToReplace = eventArguments.Ids.Where(e => Cache.ContainsId(e)).ToArray();
            if (idsToReplace.Length == 1)
            {
                var id = idsToReplace[0];
                var updateApiResult = await _repository.GetSingle(id);
                if (updateApiResult.Success)
                {
                    Cache.RemoveId(id);
                    Cache.Add(updateApiResult.Object);
                }
            }
            else if (idsToReplace.Length > 1)
            {
                var unknownEntities = await _repository.GetList(idsToReplace);
                if (unknownEntities.Success)
                {
                    foreach (var entityObject in unknownEntities.Object)
                    {
                        Cache.Overwrite(entityObject);
                    }
                }
            }
        }
    }

    public PartialExternalList<TEntity> Cache { get; set; }
}

public class PartialExternalList<TEntity> : ICollection<TEntity>, ILazyLoadedService
    where TEntity : IEntityObject
{
    private readonly RestHttpAccessBase<TEntity> _repository;
    private readonly IList<TEntity> _cache;

    public PartialExternalList(RestHttpAccessBase<TEntity> repository)
    {
        _repository = repository;
        _cache = new List<TEntity>();
        _innerFilterCache = new Dictionary<string, IEnumerable<TEntity>>();
        IdComparer = new EntityObjectComparer();
    }

    public IEqualityComparer<TEntity> IdComparer { get; set; }

    private class EntityObjectComparer : IEqualityComparer<TEntity>
    {
        public bool Equals(TEntity? x, TEntity? y)
        {
            return x?.GetId() == y?.GetId();
        }

        public int GetHashCode(TEntity obj)
        {
            return obj.State();
        }
    }

    public void Add(TEntity item)
    {
        if (Contains(item))
        {
            return;
        }
        _cache.Add(item);
        OnDataLoaded();
    }

    public void Clear()
    {
        _cache.Clear();
        OnDataLoaded();
    }

    public bool Contains(TEntity item)
    {
        return _cache.Any(e => IdComparer.Equals(e, item));
    }

    public void CopyTo(TEntity[] array, int arrayIndex)
    {
        _cache.CopyTo(array, arrayIndex);
    }

    public bool Remove(TEntity item)
    {
        if (_cache.Remove(item))
        {
            OnDataLoaded();
            return true;
        }

        return false;
    }

    public int IndexOf(TEntity item)
    {
        return _cache.IndexOf(item);
    }

    public async ValueTask<TEntity> Find(Guid id)
    {
        var entity = _cache.FirstOrDefault(e => e.GetModelIdentifier() == id);
        if (entity == null)
        {
            Console.WriteLine($"Entity {typeof(TEntity)}:{id} not in cache");
            entity = (await _repository.GetSingle(id)).UnpackOrThrow().Object;
            _cache.Add(entity);
            OnDataLoaded();
        }

        return entity;
    }

    public async ValueTask<TEntity> FindBy(Func<TEntity, bool> condition, Func<RestHttpAccessBase<TEntity>, ValueTask<ApiResult<TEntity>>> loader)
    {
        var entity = _cache.FirstOrDefault(condition);
        if (entity == null)
        {
            entity = (await loader(_repository)).UnpackOrThrow().Object;
            if (entity != null)
            {
                _cache.Add(entity);
                OnDataLoaded();
            }
        }

        return entity;
    }

    private IDictionary<string, IEnumerable<TEntity>> _innerFilterCache;

    public async ValueTask<IEnumerable<TEntity>> FindBy(Expression<Func<TEntity, bool>> condition,
        Func<RestHttpAccessBase<TEntity>, ValueTask<ApiResult<TEntity[]>>> loader)
    {
        if (FullyLoaded)
        {
            return _cache.Where(condition.Compile());
        }

        condition = SubstituteParameter(condition);
        var key = condition.Reduce().ToString();
        if (_innerFilterCache.TryGetValue(key, out var subset))
        {
            return subset;
        }

        subset = (await loader(_repository)).UnpackOrThrow().Object;
        foreach (var entityObject in subset)
        {
            Add(entityObject);
        }

        _innerFilterCache[key] = _cache.Where(condition.Compile());
        return subset;
    }

    private Expression<Func<TEntity, bool>> SubstituteParameter(Expression<Func<TEntity, bool>> condition)
    {
        var visitor = new SubVisitor<TEntity>(condition);
        return visitor.Visit() as Expression<Func<TEntity, bool>>;
    }

    public bool FullyLoaded { get; private set; }

    public int Count
    {
        get
        {
            return _cache.Count;
        }
    }

    public bool IsReadOnly
    {
        get { return (_cache as ICollection<TEntity>).IsReadOnly; }
    }

    public async ValueTask LoadAll()
    {
        if (FullyLoaded)
        {
            return;
        }

        FullyLoaded = true;
        var apiResult = await _repository.GetList();
        if (apiResult.Success)
        {
            _cache.Clear();
            foreach (var item in apiResult.Object)
            {
                _cache.Add(item);
            }

            OnDataLoaded();
        }
    }

    public IEnumerator<TEntity> GetEnumerator()
    {
        return _cache.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_cache).GetEnumerator();
    }

    public void RemoveId(Guid id)
    {
        var hasItem = _cache.FirstOrDefault(e => e.GetId() == id);
        if (hasItem != null)
        {
            _cache.Remove(hasItem);
            OnDataLoaded();
        }
    }

    public bool ContainsId(Guid id)
    {
        return _cache.Any(f => f.GetId() == id);
    }

    public event EventHandler DataLoaded;

    protected virtual void OnDataLoaded()
    {
        DataLoaded?.Invoke(this, EventArgs.Empty);
    }

    public void Overwrite(TEntity entityObject)
    {
        RemoveId(entityObject.GetId());
        Add(entityObject);
    }
}

public class SubVisitor<TEntity> : ExpressionVisitor
{
    private readonly Expression<Func<TEntity, bool>> _expression;

    public SubVisitor(Expression<Func<TEntity, bool>> expression)
    {
        _expression = expression;
    }

    public Expression Visit()
    {
        return base.Visit(_expression);
    }

    private Expression CheckMemberAccess(Expression node)
    {
        if (node is MemberExpression memberExpression)
        {
            if (_expression.Parameters.Contains(memberExpression.Expression))
            {
                return node;
            }

            var value = (memberExpression.Expression as ConstantExpression).Value;
            if (memberExpression.Member is PropertyInfo prop)
            {
                value = prop.GetValue(value);
            }
            else if (memberExpression.Member is FieldInfo field)
            {
                value = field.GetValue(value);
            }

            return Expression.Constant(value);
        }

        return node;
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        var left = CheckMemberAccess(node.Left);
        var right = CheckMemberAccess(node.Right);

        return node.Update(left, node.Conversion, right);
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        return Expression.Constant(node.Value);
    }
}