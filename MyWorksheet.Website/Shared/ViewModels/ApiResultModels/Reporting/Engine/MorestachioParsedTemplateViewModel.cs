using System.Collections.Generic;
using Morestachio.Document.Contracts;
using Morestachio.Parsing.ParserErrors;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Engine
{
    public class MorestachioParsedTemplateViewModel : ViewModelBase
    {
        private IDocumentItem _document;

        private IEnumerable<IMorestachioError> _errors;

        public IDocumentItem Document
        {
            get { return _document; }
            set { SetProperty(ref _document, value); }
        }

        public IEnumerable<IMorestachioError> Errors
        {
            get { return _errors; }
            set { SetProperty(ref _errors, value); }
        }
    }
}