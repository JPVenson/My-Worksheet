using System;

namespace MyWorksheet.Webpage.Helper.Roles;

public class RoleViewModel
{
    public RoleViewModel(Guid id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public CombinedRole And(RoleViewModel other)
    {
        return new CombinedRole() { Roles = { this, other } };
    }

    public string Name { get; private set; }
    public string Description { get; }
    public Guid Id { get; private set; }

    public override string ToString()
    {
        return Name;
    }
}
