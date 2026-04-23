using System;
using System.Threading.Tasks;
using MyWorksheet.Website.Server.Services;

namespace MyWorksheet.Website.Shared.Services;

public class RequireInit : IRequireInit
{
    public int Order { get; protected set; }

    public virtual void Init()
    {
    }

    public virtual void Init(IServiceProvider services)
    {
    }

    public virtual async ValueTask InitAsync()
    {

    }

    public virtual async ValueTask InitAsync(IServiceProvider services)
    {

    }
}