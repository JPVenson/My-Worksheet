using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Morestachio.Rendering;
using Morestachio.Runner.MDoc;

namespace Morestachio.Demo.Blazor.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[RevokableAuthorize]
    public class MorestachioApiController : ControllerBase
    {
        static MorestachioApiController()
        {
        }

        private readonly MorestachioDocumentationProvider _morestachioDocumentationProvider;
        private static IRenderer _formatterDisplayRenderer;

        public MorestachioApiController(MorestachioDocumentationProvider morestachioDocumentationProvider)
        {
            this._morestachioDocumentationProvider = morestachioDocumentationProvider;
        }

        /// <summary>
        ///		Gets information about an Build in formatter
        /// </summary>
        /// <param name="formatterName"></param>
        /// <returns></returns>
        [Produces("text/html")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        [Route("GetFormatterInfoFormatted")]
        public async Task<IActionResult> GetFormatterInfoFormatted(string formatterName)
        {
            var template = System.IO.File.ReadAllText("formatterInfo.html");
            var parser = ParserOptionsBuilder.New()
                                             .WithTemplate(template)
                                             .WithFormatters(typeof(Morestachio.Linq.DynamicLinq))
                                             .BuildAndParse();
            _formatterDisplayRenderer = parser.CreateCompiledRenderer();
            var formatter = _morestachioDocumentationProvider.GetMorestachioFormatterDocumentation(formatterName);
            if (formatter.Any())
            {
                return new OkObjectResult(await _formatterDisplayRenderer.RenderAndStringifyAsync(formatter))
                {
                    ContentTypes =
                    {
                        new Microsoft.Net.Http.Headers.MediaTypeHeaderValue("text/html"),
                    }
                };
            }
            return base.NoContent();
        }
    }
}