using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWorksheet.Website.Client.Services.Http;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Shared.Services.Activation;
using MyWorksheet.Website.Shared.ViewModels;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload;
using Microsoft.AspNetCore.Components;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.UserWorkload;

[SingletonService()]
public class UserWorkloadService
{
    public UserWorkloadService(ICacheRepository<GetUserWorkloadViewModel> userWorkloadRepository)
    {
        UserWorkloadRepository = userWorkloadRepository;

        Modes = new IWorktimeMode[]
        {
            new DayWorktimeMode(),
            new WeekWorktimeMode()
        };
    }

    public IWorktimeMode[] Modes { get; set; }

    public ICacheRepository<GetUserWorkloadViewModel> UserWorkloadRepository { get; set; }

    public double GetWeekWorktime(GetUserWorkloadViewModel workload)
    {
        var mode = Modes.FirstOrDefault(e => e.Key == workload.WorkTimeMode);
        return mode.GetWeekWorktime(workload);
    }

    public async ValueTask<GetUserWorkloadViewModel> GetWorkloadForProjectOrDefault(Guid projectProjectId)
    {
        GetUserWorkloadViewModel workload;
        if (UserWorkloadRepository.Cache.FullyLoaded)
        {
            workload = UserWorkloadRepository.Cache.FirstOrDefault(e => e.IdProject == projectProjectId);
            if (workload == null)
            {
                workload = UserWorkloadRepository.Cache.FirstOrDefault(e =>
                    !e.IdProject.HasValue && !e.IdOrganisation.HasValue);
            }
        }
        else
        {
            Console.WriteLine("Cache is not loaded");
            workload = await UserWorkloadRepository.Cache
                .FindBy(e => e.IdProject == projectProjectId, e => (e as UserWorkloadApiAccess).GetWorkloadForProject(projectProjectId));
            if (workload == null)
            {
                workload = await UserWorkloadRepository.Cache
                    .FindBy(e => e?.IdProject == null && e?.IdOrganisation == null, f => (f as UserWorkloadApiAccess).GetDefaultWorkload());
            }
        }

        return workload.CopyClone();
    }
}