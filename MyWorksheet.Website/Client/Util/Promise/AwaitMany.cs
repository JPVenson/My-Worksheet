using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MyWorksheet.Website.Client.Util.Promise
{
    public class AwaitMany : IAsyncDisposable
    {
        public AwaitMany()
        {
            _awaitables = new List<ValueTask>();
        }

        private IList<ValueTask> _awaitables;

        public void Await(Task task)
        {
            _awaitables.Add(new ValueTask(task));
        }

        public void Await(ValueTask task)
        {
            _awaitables.Add(task);
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var valueTask in _awaitables.ToArray())
            {
                await valueTask;
            }
        }
    }
}
