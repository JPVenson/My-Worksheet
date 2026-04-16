using System.Linq;
using MyWorksheet.Website.Client.Services.Http.Base;
using MyWorksheet.Website.Client.Services.Repository;
using MyWorksheet.Website.Client.Util.Promise;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using ServiceLocator.Attributes;

namespace MyWorksheet.Website.Client.Services.Organisation;

[SingletonService()]
public class OrganizationService
{
    private readonly HttpService _httpService;

    public OrganizationService(HttpService httpService, ICacheRepository<OrganizationSelectionViewModel> organizations)
    {
        _httpService = httpService;
        Organizations = organizations;
        Roles = new FutureList<OrganisationRoleViewModel>(() => _httpService.OrganizationApiAccess.GetRoles().AsTask());
        Roles.WhenLoaded(() =>
        {
            AdministratorRole = Roles.FirstOrDefault(e => e.Name == "Administrator");
            CreatorRole = Roles.FirstOrDefault(e => e.Name == "Creator");
        });
    }

    public OrganisationRoleViewModel AdministratorRole { get; private set; }
    public OrganisationRoleViewModel CreatorRole { get; private set; }
    public ICacheRepository<OrganizationSelectionViewModel> Organizations { get; set; }
    public IFutureList<OrganisationRoleViewModel> Roles { get; set; }
}