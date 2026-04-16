using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MyWorksheet.Website.Shared.ViewModels.Notifications
{
    public class StandardNotification<TData> : ViewModelBase
    {
        public TData Data
        {
            get { return _data; }
            set { SetProperty(ref _data, value); }
        }

        private TData _data;
        [JsonConverter(typeof(StringEnumConverter))]
        public ActionTypes Mode
        {
            get { return _mode; }
            set { SetProperty(ref _mode, value); }
        }

        private ActionTypes _mode;
    }
}
