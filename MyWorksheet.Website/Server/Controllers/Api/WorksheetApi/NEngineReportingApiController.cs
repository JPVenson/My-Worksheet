//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Security.Principal;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Http;
//using AutoMapper;
//using PersonalWebpage.Entities.Manager;
//using PersonalWebpage.Entities.Poco;
//using PersonalWebpage.ViewModel.ApiResultModels.Worksheet;
//using Microsoft.Owin.Security.Provider;

//namespace PersonalWebpage.Controllers.Api.WorksheetApi
//{
//    [Route("api/NEngineReporting")]
//    [RevokableAuthorizeAttribute(Roles = Startup.AdminRoleName)]
//    public class NEngineReportingApiControllerBase : ApiControllerBase
//    {
//        private readonly IMapperService _mapper;
//        private readonly TemplateCreator _templateCreator;

//        public NEngineReportingApiControllerBase(IMapperService mapper, TemplateCreator templateCreator)
//        {
//            _mapper = mapper;
//            _templateCreator = templateCreator;
//        }

//        [HttpGet]
//        [Route("GetByKey")]
//        [AllowAnonymous]
//        public HttpResponseMessage GetByKey(string key, bool download)
//        {
//            var content = _templateCreator.CompiledTemplates.FirstOrDefault(f => f.Key == key).Value;

//            if (content == null)
//                return new HttpResponseMessage(HttpStatusCode.NotFound);

//            if (content.RunningState == TemplateRunningState.Scheduled)
//            {
//                return new HttpResponseMessage(HttpStatusCode.NoContent);
//            }

//            if (content.RunningState == TemplateRunningState.Running)
//                return new HttpResponseMessage(HttpStatusCode.NoContent);

//            HttpResponseMessage result = null;
//            result = Request.CreateResponse(HttpStatusCode.OK);
//            result.Content = new StringContent(content.Template);
//            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue(download ? "attachment" : "inline");
//            result.Content.Headers.ContentDisposition.FileName = content.Filename;
//            result.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(content.Filename));
//            result.Content.Headers.ContentDisposition.CreationDate = DateTimeOffset.UtcNow;
//            return result;
//        }

//        [HttpGet]
//        [Route("GetAllQueued")]
//        public IActionResult GetCache()
//        {
//            var templates = new HashSet<CachedTemplateElement>();
//            foreach (var source in _templateCreator.CompiledTemplates.Where(f => f.Value.ExecutingUser == User.Identity.Name))
//            {
//                var hasDoneTemplate = _templateCreator.ScheduledTemplates.FirstOrDefault(f => f.Key == source.Key);
//	            var template = _mapper.ViewModelMapper.Map<CachedTemplateElement>(source.Value);
//	            template.Key = source.Key;
//				if (hasDoneTemplate.Value == null)
//                {
//                    templates.Add(template);
//                }
//                else
//                {
//                    templates.Add(template);
//                }
//			}


//            return Data(templates);
//        }

//        [HttpGet]
//        [Route("GetAll")]
//        public IActionResult GetTemplates()
//        {
//            return Data(_mapper.ViewModelMapper.Map<NEngineApiElement[]>(new DbEntities().Select<NEngineTemplate>()));
//        }

//        [HttpGet]
//        [Route("Get")]
//        public IActionResult GetTemplates(Guid nEngineTemplateId)
//        {
//            return Data(_mapper.ViewModelMapper.Map<NEngineApiElement>(new DbEntities().Select<NEngineTemplate>(nEngineTemplateId)));
//        }

//        [HttpPost]
//        [Route("Create")]
//        public IActionResult CreateTemplate(NEngineApiElement template)
//        {
//            using (var db = new DbEntities())
//            {
//                template.NEngineTemplate_ID = 0;
//                var map = _mapper.ViewModelMapper.Map<NEngineTemplate>(template);
//                return Data(db.InsertWithSelect(map));
//            }
//        }

//        [HttpPost]
//        [Route("Alter")]
//        public IActionResult UpdateTemplate(NEngineApiElement template)
//        {
//            using (var db = new DbEntities())
//            {
//                var map = _mapper.ViewModelMapper.Map<NEngineTemplate>(template);
//                db.Update(map);
//                return Data(map);
//            }
//        }

//        [HttpPost]
//        [Route("ProcessTemplate")]
//        public IActionResult ProcessWorksheetTemplate(Guid worksheetId, Guid templateId, string fileName = null)
//        {
//            return Data(_templateCreator.Enqueue(templateId, worksheetId, fileName, User.Identity.Name));
//        }
//    }
//}