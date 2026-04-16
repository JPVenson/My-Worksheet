using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.ChangeTracking;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Services.Repository;

public interface ICacheRepository<TEntity> where TEntity : IEntityObject
{
    PartialExternalList<TEntity> Cache { get; set; }
}