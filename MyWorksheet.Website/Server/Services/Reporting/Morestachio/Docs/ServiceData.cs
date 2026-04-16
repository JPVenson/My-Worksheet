using System.Collections.Generic;

namespace Morestachio.Runner.MDoc;

public class ServiceData
{
    public string ServiceName { get; set; }
    public ICollection<ServicePropertyType> Types { get; set; }
    public string Description { get; set; }
}
