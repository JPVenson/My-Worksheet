using System;
using System.Collections.Generic;
using MyWorksheet.Public.Models.ObjectSchema;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Accounting.Organisation;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Address;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Payment;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Reporting.Reports;

public class ProjectSpecReportModel : ViewModelBase
{
    private string _additonalInfos;

    private AccountApiGet _creator;

    private AddressModel _creatorAddress;

    private Aggregations _infos;

    private string _invoiceNo;

    private OrganizationReportingViewModel _owner;

    private AddressModel _ownerAddress;

    private PaymentInfoModel _paymentInfoGroup;

    private List<PaymentInfoContentModel> _paymentInfos;

    private GetProjectModel _project;

    private List<WorksheetItemModel> _worksheetItems;

    private List<WorksheetModel> _worksheets;

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

    public List<WorksheetModel> Worksheets
    {
        get { return _worksheets; }
        set { SetProperty(ref _worksheets, value); }
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

    [JsonComment("Reporting/ProjectSpec.Comment.CommonInfos")]
    public Aggregations Infos
    {
        get { return _infos; }
        set { SetProperty(ref _infos, value); }
    }

    public string AdditonalInfos
    {
        get { return _additonalInfos; }
        set { SetProperty(ref _additonalInfos, value); }
    }

    public string InvoiceNo
    {
        get { return _invoiceNo; }
        set { SetProperty(ref _invoiceNo, value); }
    }

    public class Aggregations : ViewModelBase
    {
        private decimal _calculatedGross;

        private decimal _calculatedNet;

        private decimal _calculatedTax;

        private DateTime _dateOfCreation;

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

        public DateTime DateOfCreation
        {
            get { return _dateOfCreation; }
            set { SetProperty(ref _dateOfCreation, value); }
        }

        [JsonComment("Reporting/ProjectSpec.Comment.Aggregations.CalculatedNet")]
        public decimal CalculatedNet
        {
            get { return _calculatedNet; }
            set { SetProperty(ref _calculatedNet, value); }
        }
    }
}