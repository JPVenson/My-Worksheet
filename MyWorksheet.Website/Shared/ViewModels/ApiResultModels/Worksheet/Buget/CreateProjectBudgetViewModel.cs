using System;
using System.ComponentModel.DataAnnotations;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Buget
{
    public class CreateProjectBudgetViewModel : ViewModelBase
    {
        private bool _allowOverbooking;

        private DateTimeOffset? _deadline;

        private Guid? _idAppUser;

        private Guid _idProject;

        private decimal? _totalBudget;

        private int? _totalTimeBudget;

        private DateTimeOffset? _validFrom;

        public Guid? IdAppUser
        {
            get { return _idAppUser; }
            set { SetProperty(ref _idAppUser, value); }
        }

        public Guid IdProject
        {
            get { return _idProject; }
            set { SetProperty(ref _idProject, value); }
        }

        public DateTimeOffset? Deadline
        {
            get { return _deadline; }
            set { SetProperty(ref _deadline, value); }
        }

        // 1440 minutes = 24 hours
        [Range(1440, int.MaxValue, ErrorMessage = "Budget/TimeBudget.Min24h")]
        public int? TotalTimeBudget
        {
            get { return _totalTimeBudget; }
            set { SetProperty(ref _totalTimeBudget, value); }
        }

        public decimal? TotalBudget
        {
            get { return _totalBudget; }
            set { SetProperty(ref _totalBudget, value); }
        }

        public DateTimeOffset? ValidFrom
        {
            get { return _validFrom; }
            set { SetProperty(ref _validFrom, value); }
        }

        public bool AllowOverbooking
        {
            get { return _allowOverbooking; }
            set { SetProperty(ref _allowOverbooking, value); }
        }
    }
}