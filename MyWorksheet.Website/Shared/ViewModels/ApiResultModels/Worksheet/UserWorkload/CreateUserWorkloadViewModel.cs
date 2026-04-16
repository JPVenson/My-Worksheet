using System;
namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.UserWorkload
{
    [ObjectTracking("UserWorkload")]
    public class CreateUserWorkloadViewModel : ViewModelBase
    {
        private int? _dayWorkTimeFriday;
        private int? _dayWorkTimeMonday;
        private int? _dayWorkTimeSaturday;
        private int? _dayWorkTimeSunday;
        private int? _dayWorkTimeThursday;
        private int? _dayWorkTimeTuesday;
        private int? _dayWorkTimeWednesday;

        private Guid? _idOrganisation;

        private Guid? _idProject;

        private int _monthWorktime;

        private int _weekWorktime;

        private int _workTimeMode;

        public int WorkTimeMode
        {
            get { return _workTimeMode; }
            set { SetProperty(ref _workTimeMode, value); }
        }

        public int? DayWorkTimeMonday
        {
            get { return _dayWorkTimeMonday; }
            set { SetProperty(ref _dayWorkTimeMonday, value); }
        }

        public int? DayWorkTimeTuesday
        {
            get { return _dayWorkTimeTuesday; }
            set { SetProperty(ref _dayWorkTimeTuesday, value); }
        }

        public int? DayWorkTimeWednesday
        {
            get { return _dayWorkTimeWednesday; }
            set { SetProperty(ref _dayWorkTimeWednesday, value); }
        }

        public int? DayWorkTimeThursday
        {
            get { return _dayWorkTimeThursday; }
            set { SetProperty(ref _dayWorkTimeThursday, value); }
        }

        public int? DayWorkTimeFriday
        {
            get { return _dayWorkTimeFriday; }
            set { SetProperty(ref _dayWorkTimeFriday, value); }
        }

        public int? DayWorkTimeSaturday
        {
            get { return _dayWorkTimeSaturday; }
            set { SetProperty(ref _dayWorkTimeSaturday, value); }
        }

        public int? DayWorkTimeSunday
        {
            get { return _dayWorkTimeSunday; }
            set { SetProperty(ref _dayWorkTimeSunday, value); }
        }

        public int WeekWorktime
        {
            get { return _weekWorktime; }
            set { SetProperty(ref _weekWorktime, value); }
        }

        public int MonthWorktime
        {
            get { return _monthWorktime; }
            set { SetProperty(ref _monthWorktime, value); }
        }

        public Guid? IdProject
        {
            get { return _idProject; }
            set { SetProperty(ref _idProject, value); }
        }

        public Guid? IdOrganisation
        {
            get { return _idOrganisation; }
            set { SetProperty(ref _idOrganisation, value); }
        }

        //public int? State()
        //{
        //	return HashCode.Combine(
        //		WorkTimeMode,
        //		HashCode.Combine(
        //			DayWorkTimeMonday,
        //			DayWorkTimeTuesday,
        //			DayWorkTimeWednesday,
        //			DayWorkTimeThursday,
        //			DayWorkTimeFriday,
        //			DayWorkTimeSaturday,
        //			DayWorkTimeSunday),
        //		WeekWorktime,
        //		MonthWorktime,
        //		IdProject,
        //		IdOrganisation);
        //}
    }
}