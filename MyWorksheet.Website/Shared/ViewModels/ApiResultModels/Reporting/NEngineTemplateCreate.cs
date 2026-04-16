using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting
{
    [ObjectTracking("NEngineTemplate")]
    public class NEngineTemplateCreate : ViewModelBase
    {
        private string _comment;
        private string _fileExtention;
        private Guid? _idCreator;
        private string _name;
        private string _template;
        private string _usedDataSource;
        private string _usedFormattingEngine;
        private string _fileNameTemplate;

        public string FileNameTemplate
        {
            get { return _fileNameTemplate; }
            set { SetProperty(ref _fileNameTemplate, value); }
        }

        public string UsedFormattingEngine
        {
            get { return _usedFormattingEngine; }
            set { SetProperty(ref _usedFormattingEngine, value); }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string UsedDataSource
        {
            get { return _usedDataSource; }
            set { SetProperty(ref _usedDataSource, value); }
        }

        public string FileExtention
        {
            get { return _fileExtention; }
            set { SetProperty(ref _fileExtention, value); }
        }

        public string Comment
        {
            get { return _comment; }
            set { SetProperty(ref _comment, value); }
        }

        public Guid? IdCreator
        {
            get { return _idCreator; }
            set { SetProperty(ref _idCreator, value); }
        }

        public string Template
        {
            get { return _template; }
            set { SetProperty(ref _template, value); }
        }
    }
}