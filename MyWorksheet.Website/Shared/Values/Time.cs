using System;

namespace MyWorksheet.Public.Models.Values;

public struct Time : IEquatable<decimal>
{
    public Time(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; private set; }

    public static implicit operator Time(decimal from)
    {
        return new Time(from);
    }

    public bool Equals(decimal other)
    {
        return Value.Equals(other);
    }
}
