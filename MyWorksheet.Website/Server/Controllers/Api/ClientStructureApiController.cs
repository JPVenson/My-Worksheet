// using System.Linq;
// using MyWorksheet.Entities.Poco;
// using MyWorksheet.Webpage.Helper.Roles;
// using MyWorksheet.Website.Server.Models;
// using MyWorksheet.Website.Server.Services.Cms;
// using MyWorksheet.Website.Server.Services.Mapping;
// using MyWorksheet.Website.Server.Util.Auth;
// using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.ClientStructure;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;

// namespace MyWorksheet.Website.Server.Controllers.Api;

// [Route("ClientStructureApi")]
// public class ClientStructureApiControllerBase : ApiControllerBase
// {
// 	private readonly IMapperService _mapper;
// 	private readonly IDbContextFactory<MyworksheetContext> _dbContextFactory;
// 	private readonly ModuleManager _moduleManager;

// 	public ClientStructureApiControllerBase(IMapperService mapper, IDbContextFactory<MyworksheetContext> dbContextFactory, ModuleManager moduleManager)
// 	{
// 		_mapper = mapper;
// 		_dbContextFactory = dbContextFactory;
// 		_moduleManager = moduleManager;
// 	}

// 	[HttpGet]
// 	public IActionResult GetRequestedClientStructure()
// 	{
// 		if (User?.Identity == null || !User.Identity.IsAuthenticated)
// 		{
// 			return Data(_mapper.ViewModelMapper.Map<ClientStructureGet>(_moduleManager.GetForRoles(Roles.Visitor.Id).Select(f => f.Model)));
// 		}

//         using var db = _dbContextFactory.CreateDbContext();
// 		return Data(_mapper.ViewModelMapper.Map<ClientStructureGet>(_moduleManager.GetForRoles(
// 				db.UserRoleMaps.Where(f => f.IdUser == User.GetUserId()).Select(f => f.IdRole).ToArray())
// 			.Select(f => f.Model)));
// 	}

// 	//public IActionResult PostClientStructure()
// }