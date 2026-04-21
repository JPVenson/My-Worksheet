using System;
using System.Threading.Tasks;

namespace MyWorksheet.Website.Server.Services;

public interface IRequireInit
{
    int Order { get; }

    void Init();
    ValueTask InitAsync();
    void Init(IServiceProvider services);
    ValueTask InitAsync(IServiceProvider services);
}
