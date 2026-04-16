using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyWorksheet.Website.Server.Services.Blob.Thumbnail;

public class NestedTry
{
    private NestedTry _parent;
    private Func<Task<object>> _tryable;
    private List<NestedTry> _retrys;

    private NestedTry(Func<Task<object>> tryable, NestedTry parent) : this(tryable)
    {
        _parent = parent;
    }

    public NestedTry(Func<Task<object>> tryable)
    {
        _tryable = tryable;
        _retrys = [];
    }

    public NestedTry Otherwise(Func<Task<object>> tryable)
    {
        var thenable = new NestedTry(tryable, this);
        _retrys.Add(thenable);
        return thenable;
    }

    public NestedTry(Func<object> tryable)
    {
        _tryable = async () =>
        {
            await Task.CompletedTask;
            return tryable();
        };
        _retrys = [];
    }

    public NestedTry Otherwise(Func<object> tryable)
    {
        var thenable = new NestedTry(tryable);
        thenable._parent = this;
        _retrys.Add(thenable);
        return thenable;
    }

    public async Task<object> InvokeChainAsync()
    {
        var parent = this;
        while (parent._parent != null)
        {
            parent = parent._parent;
        }

        return await parent.InvokeChainInternalAsync();
    }

    private async Task<object> InvokeChainInternalAsync()
    {
        try
        {
            return await _tryable();
        }
        catch (Exception)
        {
            foreach (var nestedTry in _retrys)
            {
                var tried = await nestedTry.InvokeChainInternalAsync();
                if (tried != null)
                {
                    return tried;
                }
            }
        }
        return null;
    }
}