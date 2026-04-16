using System;
namespace MyWorksheet.Webpage.Helper.Roles;

public class WorksheetStatusLookupModel
{
    public WorksheetStatusLookupModel(Guid id, string code)
    {
        Id = id;
        Code = code;
        Description = "";
    }

    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Description { get; set; }
}