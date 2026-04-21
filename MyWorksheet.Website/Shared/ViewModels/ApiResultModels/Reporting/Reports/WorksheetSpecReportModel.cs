using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet.Asserts;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;

public class WorksheetSpecReportModel : ViewModelBase
{
    private string _additonalInfos;

    private AssertAggregations _assertInfos;

    private List<WorksheetAssertGetViewModel> _asserts;

    private AccountApiGet _creator;

    private AddressModel _creatorAddress;

    private DateTime _dateOfCreation;

    private WorksheetAggregations _infos;

    private ProjectItemRateViewModel[] _itemRates;

    private OrganizationReportingViewModel _owner;

    private AddressModel _ownerAddress;

    private PaymentInfoModel _paymentInfoGroup;

    private List<PaymentInfoContentModel> _paymentInfos;

    private GetProjectModel _project;

    private WorksheetModel _worksheet;

    private List<WorksheetItemModel> _worksheetItems;

    private WorktimeAggregations _worktimeInfos;

    [JsonComment("Reporting/ProjectSpec.Comment.Owner")]
    public OrganizationReportingViewModel Owner
    {
        get { return _owner; }
        set { SetProperty(ref _owner, value); }
    }

    [JsonComment("Reporting/ProjectSpec.Comment.Creator")]
    public AccountApiGet Creator
    {
        get { return _creator; }
        set { SetProperty(ref _creator, value); }
    }

    public AddressModel OwnerAddress
    {
        get { return _ownerAddress; }
        set { SetProperty(ref _ownerAddress, value); }
    }

    public AddressModel CreatorAddress
    {
        get { return _creatorAddress; }
        set { SetProperty(ref _creatorAddress, value); }
    }

    public WorksheetModel Worksheet
    {
        get { return _worksheet; }
        set { SetProperty(ref _worksheet, value); }
    }

    public ProjectItemRateViewModel[] ItemRates
    {
        get { return _itemRates; }
        set { SetProperty(ref _itemRates, value); }
    }

    public GetProjectModel Project
    {
        get { return _project; }
        set { SetProperty(ref _project, value); }
    }

    public List<WorksheetItemModel> WorksheetItems
    {
        get { return _worksheetItems; }
        set { SetProperty(ref _worksheetItems, value); }
    }

    [IgnoreDataMember]
    [JsonComment("Reporting/ProjectSpec.WorksheetRates")]
    public KeyValuePair<ProjectItemRateViewModel, WorksheetItemModel[]>[] WorksheetsWithRates
    {
        get
        {
            return WorksheetItems.GroupBy(e => e.IdProjectItemRate)
                .Select(e => new KeyValuePair<ProjectItemRateViewModel, WorksheetItemModel[]>(
                    ItemRates.FirstOrDefault(g => g.ProjectItemRateId == e.Key),
                    e.ToArray()))
                .ToArray();
        }
    }

    public PaymentInfoModel PaymentInfoGroup
    {
        get { return _paymentInfoGroup; }
        set { SetProperty(ref _paymentInfoGroup, value); }
    }

    public List<PaymentInfoContentModel> PaymentInfos
    {
        get { return _paymentInfos; }
        set { SetProperty(ref _paymentInfos, value); }
    }

    public List<WorksheetAssertGetViewModel> Asserts
    {
        get { return _asserts; }
        set { SetProperty(ref _asserts, value); }
    }

    [JsonComment("Reporting/ProjectSpec.Comment.WorktimeInfos")]
    public WorktimeAggregations WorktimeInfos
    {
        get { return _worktimeInfos; }
        set { SetProperty(ref _worktimeInfos, value); }
    }

    [JsonComment("Reporting/ProjectSpec.Comment.AssertInfos")]
    public AssertAggregations AssertInfos
    {
        get { return _assertInfos; }
        set { SetProperty(ref _assertInfos, value); }
    }

    [JsonComment("Reporting/ProjectSpec.Comment.CommonInfos")]
    public WorksheetAggregations Infos
    {
        get { return _infos; }
        set { SetProperty(ref _infos, value); }
    }

    public string AdditonalInfos
    {
        get { return _additonalInfos; }
        set { SetProperty(ref _additonalInfos, value); }
    }

    public DateTime DateOfCreation
    {
        get { return _dateOfCreation; }
        set { SetProperty(ref _dateOfCreation, value); }
    }

    public class WorksheetAggregations : ViewModelBase
    {
        private decimal _calculatedGross;

        [JsonComment("Reporting/ProjectSpec.Comment.Aggregations.CalculatedNet")]
        public decimal CalculatedGross
        {
            get { return _calculatedGross; }
            set { SetProperty(ref _calculatedGross, value); }
        }
    }

    public class WorktimeAggregations : ViewModelBase
    {
        private decimal _calculatedGross;

        private decimal _calculatedNet;

        private decimal _calculatedTax;

        private decimal _workedMinutes;

        [JsonComment("Reporting/ProjectSpec.Comment.Aggregations.WorkedMinutes")]
        public decimal WorkedMinutes
        {
            get { return _workedMinutes; }
            set { SetProperty(ref _workedMinutes, value); }
        }

        [JsonComment("Reporting/ProjectSpec.Comment.Aggregations.CalculatedTax")]
        public decimal CalculatedTax
        {
            get { return _calculatedTax; }
            set { SetProperty(ref _calculatedTax, value); }
        }

        [JsonComment("Calculated Gross based on Net + Tax of all Incomes")]
        public decimal CalculatedGross
        {
            get { return _calculatedGross; }
            set { SetProperty(ref _calculatedGross, value); }
        }

        [JsonComment("Reporting/ProjectSpec.Comment.Aggregations.CalculatedNet")]
        public decimal CalculatedNet
        {
            get { return _calculatedNet; }
            set { SetProperty(ref _calculatedNet, value); }
        }
    }

    public class AssertAggregations : ViewModelBase
    {
        private decimal _totalTax;

        private decimal _totalValue;

        [JsonComment("Reporting/ProjectSpec.Comment.AssertInfos.TotalValue")]
        public decimal TotalValue
        {
            get { return _totalValue; }
            set { SetProperty(ref _totalValue, value); }
        }

        [JsonComment("Reporting/ProjectSpec.Comment.AssertInfos.TotalTax")]
        public decimal TotalTax
        {
            get { return _totalTax; }
            set { SetProperty(ref _totalTax, value); }
        }
    }
}