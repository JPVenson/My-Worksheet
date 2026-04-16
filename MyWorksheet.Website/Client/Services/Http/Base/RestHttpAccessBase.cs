using System;
using System.Threading.Tasks;

namespace MyWorksheet.Website.Client.Services.Http.Base;

public abstract class RestHttpAccessBase<TEntity> : HttpAccessBase
{
    protected RestHttpAccessBase(HttpService httpService, string url) : base(httpService, url)
    {
    }

    public virtual ValueTask<ApiResult<TEntity>> GetSingle(Guid id)
    {
        return Get<TEntity>(BuildApi("GetSingle", new
        {
            id
        }));
    }

    public virtual ValueTask<ApiResult<TEntity[]>> GetList(Guid[] ids)
    {
        return Get<TEntity[]>(BuildApi("GetMany", new
        {
            ids
        }));
    }

    public virtual ValueTask<ApiResult<TEntity[]>> GetList()
    {
        return Get<TEntity[]>(BuildApi("GetList"));
    }
}

public class RestHttpAccessBase<TEntity, TPostEntity> : RestHttpAccessBase<TEntity>
{
    public RestHttpAccessBase(HttpService httpService, string url) : base(httpService, url)
    {
    }


    public virtual ValueTask<ApiResult<TEntity>> Update(TPostEntity entity, Guid id)
    {
        return Post<TPostEntity, TEntity>(BuildApi("Update", new
        {
            id
        }), entity);
    }

    public virtual ValueTask<ApiResult<TEntity>> Create(TPostEntity entity)
    {
        return Post<TPostEntity, TEntity>(BuildApi("Create"), entity);
    }

    public virtual ValueTask<ApiResult> Delete(Guid id)
    {
        return Post(BuildApi("Delete", new { id }));
    }
}