using System;
using MyWorksheet.Website.Server.Models;

namespace MyWorksheet.Website.Server.Services.Workflow;

public class GenericWorksheetStatusType : IWorksheetStatusType
{
    public GenericWorksheetStatusType(WorksheetStatusLookup worksheetStatusLookup)
    {
        WorksheetStatusLookup = worksheetStatusLookup;
        DisplayKey = WorksheetStatusLookup.DisplayKey;
    }

    public WorksheetStatusLookup WorksheetStatusLookup { get; private set; }

    public int CompareTo(object obj)
    {
        if (obj is IWorksheetStatusType statusType)
        {
            obj = statusType.ConvertToGuid();
        }

        return WorksheetStatusLookup.WorksheetStatusLookupId.CompareTo(obj);
    }

    public bool Equals(Guid other)
    {
        return WorksheetStatusLookup.WorksheetStatusLookupId == other;
    }

    public Guid ConvertToGuid()
    {
        return WorksheetStatusLookup.WorksheetStatusLookupId;
    }

    public string DisplayKey { get; }
}

public class WorksheetStatusType : IWorksheetStatusType
{
    private WorksheetStatusType(Guid guidValue, string display)
    {
        GuidValue = guidValue;
        DisplayKey = display;
    }

    public Guid GuidValue { get; private set; }
    public string DisplayKey { get; private set; }

    public static IWorksheetStatusType Invalid { get; } = new WorksheetStatusType(new Guid("00000000-0000-0000-0000-000000000001"), "Workflow.Manual/StatusType.Invalid");
    public static IWorksheetStatusType Created { get; } = new WorksheetStatusType(new Guid("00000000-0000-0000-0000-000000000002"), "Workflow.Manual/StatusType.Created");
    public static IWorksheetStatusType Submitted { get; } = new WorksheetStatusType(new Guid("00000000-0000-0000-0000-000000000003"), "Workflow.Manual/StatusType.Submitted");
    public static IWorksheetStatusType AwaitingResponse { get; } = new WorksheetStatusType(new Guid("00000000-0000-0000-0000-000000000004"), "Workflow.Manual/StatusType.AwaitingResponse");
    public static IWorksheetStatusType Confirmed { get; } = new WorksheetStatusType(new Guid("00000000-0000-0000-0000-000000000006"), "Workflow.Manual/StatusType.Confirmed");
    public static IWorksheetStatusType AwaitingPayment { get; } = new WorksheetStatusType(new Guid("00000000-0000-0000-0000-000000000007"), "Workflow.Manual/StatusType.AwaitingPayment");
    public static IWorksheetStatusType Rejected { get; } = new WorksheetStatusType(new Guid("00000000-0000-0000-0000-000000000008"), "Workflow.Manual/StatusType.Rejected");
    public static IWorksheetStatusType Payed { get; } = new WorksheetStatusType(new Guid("00000000-0000-0000-0000-000000000009"), "Workflow.Manual/StatusType.Payed");

    public int CompareTo(object obj)
    {
        if (obj is IWorksheetStatusType statusType)
        {
            obj = statusType.ConvertToGuid();
        }

        return GuidValue.CompareTo(obj);
    }

    public Guid ConvertToGuid()
    {
        return GuidValue;
    }

    public bool Equals(Guid other)
    {
        return GuidValue.Equals(other);
    }
}

public interface IWorksheetStatusType : IComparable, IEquatable<Guid>
{
    Guid ConvertToGuid();
    string DisplayKey { get; }
}
