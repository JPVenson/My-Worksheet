using MyWorksheet.Website.Server.Services.Mapping;
using MyWorksheet.Website.Server.Services.Text;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Text;
using Microsoft.AspNetCore.Mvc;

namespace MyWorksheet.Website.Server.Controllers.Api.StorageApi;

[Route("api/TextApi")]
public class TextApiControllerBase : ApiControllerBase
{
    private readonly ITextService _textService;
    private readonly IMapperService _mapper;

    public TextApiControllerBase(ITextService textService, IMapperService mapper)
    {
        _textService = textService;
        _mapper = mapper;
    }

    [HttpGet]
    [Route("GetForPage")]
    public IActionResult GetGroup(string groupName, string locale)
    {
        if (groupName == null)
        {
            return Data(new object[0]);
        }
        return Data(_mapper.ViewModelMapper.Map<TextResourceViewModel[]>(_textService.GetByGroupName(groupName.ToUpper(), locale)));
    }

    [HttpGet]
    [Route("GetCacheStatus")]
    public IActionResult GetCacheStatus()
    {
        return Data(_textService.GetCacheStatus());
    }

    [HttpGet]
    [Route("GetAll")]
    public IActionResult GetCache(string culture)
    {
        return Data(_mapper.ViewModelMapper.Map<TextResourceViewModel[]>(_textService.GetCache(culture)));
    }
}