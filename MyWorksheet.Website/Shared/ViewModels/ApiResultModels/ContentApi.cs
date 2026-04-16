using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels
{
    public class ContentApi : ViewModelBase
    {
        private int _cMSContnetID;

        private string _content;

        private string _content_ID;

        [Required]
        public int CMSContnetID
        {
            get { return _cMSContnetID; }
            set { SetProperty(ref _cMSContnetID, value); }
        }

        [Required]
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        [Required]
        public string Content_ID
        {
            get { return _content_ID; }
            set { SetProperty(ref _content_ID, value); }
        }
    }

    public class AppLoggerLogViewModel
    {
        public string Message { get; set; }
        public string Category { get; set; }
        public string Level { get; set; }
        public string Key { get; set; }
        public IDictionary<string, string> OptionalData { get; set; }
        public DateTime DateCreated { get; set; }
    }
}