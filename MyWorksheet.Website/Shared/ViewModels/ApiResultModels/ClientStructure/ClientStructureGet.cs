using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.ClientStructure
{
    public class ClientStructureGet : ViewModelBase
    {
        private string _additonalInfos;

        private bool _canBeDirectlyNavigated;

        private Guid _clientStructureId;

        private string _controllerName;

        private string _displayRoute;

        private string _inActiveNotice;

        private bool _isActive;

        private bool _menuItemOnly;

        private Guid _orderId;

        private int? _parentRoute;

        private string _title;

        private string _urlRoute;

        public Guid ClientStructureId
        {
            get { return _clientStructureId; }
            set { SetProperty(ref _clientStructureId, value); }
        }

        public bool MenuItemOnly
        {
            get { return _menuItemOnly; }
            set { SetProperty(ref _menuItemOnly, value); }
        }

        public bool CanBeDirectlyNavigated
        {
            get { return _canBeDirectlyNavigated; }
            set { SetProperty(ref _canBeDirectlyNavigated, value); }
        }

        public int? ParentRoute
        {
            get { return _parentRoute; }
            set { SetProperty(ref _parentRoute, value); }
        }

        public string DisplayRoute
        {
            get { return _displayRoute; }
            set { SetProperty(ref _displayRoute, value); }
        }

        public string AdditonalInfos
        {
            get { return _additonalInfos; }
            set { SetProperty(ref _additonalInfos, value); }
        }

        public string ControllerName
        {
            get { return _controllerName; }
            set { SetProperty(ref _controllerName, value); }
        }

        public string UrlRoute
        {
            get { return _urlRoute; }
            set { SetProperty(ref _urlRoute, value); }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public Guid OrderId
        {
            get { return _orderId; }
            set { SetProperty(ref _orderId, value); }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }

        public string InActiveNotice
        {
            get { return _inActiveNotice; }
            set { SetProperty(ref _inActiveNotice, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return ClientStructureId;
        }
    }
}