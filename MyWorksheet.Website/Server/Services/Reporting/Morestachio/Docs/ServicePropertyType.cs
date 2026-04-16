using System;
using System.Collections.Generic;

namespace Morestachio.Runner.MDoc;

public class ServicePropertyType : IEquatable<ServicePropertyType>
{
    public ServicePropertyType()
    {
        Properties = [];
        Formatter = new FormatterData();
    }

    public FormatterData Formatter { get; set; }
    public Type Type { get; set; }
    public IList<ServiceProperty> Properties { get; set; }

    public bool IsFrameworkType { get; set; }
    public string Description { get; set; }

    public bool Equals(ServicePropertyType other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Equals(Type, other.Type);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((ServicePropertyType)obj);
    }

    public override int GetHashCode()
    {
        return (Type != null ? Type.GetHashCode() : 0);
    }
}
