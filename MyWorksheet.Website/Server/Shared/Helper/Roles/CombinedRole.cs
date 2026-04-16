using System.Collections.Generic;
using System.Linq;

namespace MyWorksheet.Webpage.Helper.Roles;

public class CombinedRole
{
    public CombinedRole()
    {

    }

    public List<RoleViewModel> Roles { get; set; }

    public CombinedRole And(RoleViewModel other)
    {
        Roles.Add(other);
        return this;
    }

    public override string ToString()
    {
        return Roles.Select(f => f.Name).Aggregate((e, f) => e + "," + f);
    }
}