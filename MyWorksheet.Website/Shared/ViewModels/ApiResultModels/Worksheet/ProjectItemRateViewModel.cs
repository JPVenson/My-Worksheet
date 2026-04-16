using System;
using System.ComponentModel.DataAnnotations;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet
{
    [ObjectTracking("ProjectItemRate")]
    public class ProjectItemRateViewModel : ViewModelBase
    {
        private Guid _idProject;
        private Guid _idProjectChargeRate;
        private string _name;
        private string _currencyType;
        private ProjectChargeRateModel _projectChargeRate;
        private Guid _projectItemRateId;
        private double _rate;
        private double _taxRate;

        public bool Hidden { get => field; set => SetProperty(ref field, value); }

        public Guid ProjectItemRateId
        {
            get { return _projectItemRateId; }
            set { SetProperty(ref _projectItemRateId, value); }
        }

        [Required]
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        [Required]
        public string CurrencyType
        {
            get { return _currencyType; }
            set { SetProperty(ref _currencyType, value); }
        }

        [Required]
        [Range(1, double.MaxValue)]
        public double Rate
        {
            get { return _rate; }
            set { SetProperty(ref _rate, value); }
        }

        [Required]
        [Range(1, double.MaxValue)]
        public double TaxRate
        {
            get { return _taxRate; }
            set { SetProperty(ref _taxRate, value); }
        }

        public Guid IdProject
        {
            get { return _idProject; }
            set { SetProperty(ref _idProject, value); }
        }

        [Required]
        public Guid IdProjectChargeRate
        {
            get { return _idProjectChargeRate; }
            set { SetProperty(ref _idProjectChargeRate, value); }
        }

        public ProjectChargeRateModel ProjectChargeRate
        {
            get { return _projectChargeRate; }
            set { SetProperty(ref _projectChargeRate, value); }
        }

        public override Guid? GetModelIdentifier()
        {
            return ProjectItemRateId;
        }
    }
}