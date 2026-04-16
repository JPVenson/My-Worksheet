using System;
using System.Collections.Generic;

namespace MyWorksheet.Website.Client.Util.Promise;

public interface IAsyncIEnumerable<out TValue> : IEnumerable<TValue>
{
    void WhenAvailable(Action action);
}