using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWorksheet.Website.Client.Services;

public interface ILazyLoadedService
{
    event EventHandler DataLoaded;
}