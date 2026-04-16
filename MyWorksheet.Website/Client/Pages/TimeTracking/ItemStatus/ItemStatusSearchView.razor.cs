using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Client.Pages.TimeTracking.ItemStatus;

public partial class ItemStatusSearchView
{
    public ICacheRepository<WorksheetStatusViewModel> WorksheetItemStatusRepository { get; set; }
}