using System;
using System.Collections.Generic;

namespace MyWorksheet.Webpage.Helper.Roles;

public class WorksheetStatusManager
{
    public static WorksheetStatusLookupModel Invalid { get; private set; } = new WorksheetStatusLookupModel(new Guid("00000000-0000-0000-0003-000000000000"), "INV");
    public static WorksheetStatusLookupModel Created { get; private set; } = new WorksheetStatusLookupModel(new Guid("00000000-0000-0000-0003-000000000002"), "CRE");
    public static WorksheetStatusLookupModel Submitted { get; private set; } = new WorksheetStatusLookupModel(new Guid("00000000-0000-0000-0003-000000000003"), "SUB");
    public static WorksheetStatusLookupModel AwaitingResponse { get; private set; } = new WorksheetStatusLookupModel(new Guid("00000000-0000-0000-0003-000000000004"), "AWR");
    public static WorksheetStatusLookupModel Confirmed { get; private set; } = new WorksheetStatusLookupModel(new Guid("00000000-0000-0000-0003-000000000006"), "CONF");
    public static WorksheetStatusLookupModel AwaitingPayment { get; private set; } = new WorksheetStatusLookupModel(new Guid("00000000-0000-0000-0003-000000000007"), "AWP");
    public static WorksheetStatusLookupModel Rejected { get; private set; } = new WorksheetStatusLookupModel(new Guid("00000000-0000-0000-0003-000000000008"), "REJT");
    public static WorksheetStatusLookupModel Payed { get; private set; } = new WorksheetStatusLookupModel(new Guid("00000000-0000-0000-0003-000000000009"), "PAY");

    public static IEnumerable<WorksheetStatusLookupModel> Yield()
    {
        yield return Invalid;
        yield return Created;
        yield return Submitted;
        yield return AwaitingResponse;
        yield return Confirmed;
        yield return AwaitingPayment;
        yield return Rejected;
        yield return Payed;
    }
}