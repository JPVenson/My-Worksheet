using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Katana.CommonTasks.Extentions;

namespace Katana.CommonTasks.Models;

public static class QuestionableBooleanExentations
{
    public static string Reason(this QuestionableBoolean value)
    {
        return (value as QuestionableBoolean?)?.Reason;
    }
}

[Serializable]
public struct QuestionableBoolean :
    IEquatable<QuestionableBoolean>, IEquatable<bool>,
    IComparable, IComparable<bool>,
    IConvertible,
    ISerializable, IXmlSerializable
{
    public QuestionableBoolean(bool value, string reason = FallbackReason)
    {
        this._value = value;
        this.Reason = reason ?? FallbackReason;
    }

    public const string FallbackReason = "Unknown Reason";
    public static readonly QuestionableBoolean Default = default(QuestionableBoolean);
    public static readonly QuestionableBoolean True = true.Because("");
    public static readonly QuestionableBoolean False = false.Because("");
    private bool _value;

    private QuestionableBoolean(SerializationInfo info, StreamingContext context)
    {
        _value = info.GetBoolean("");
        Reason = info.GetString("Reason");
    }

    public string Reason { get; private set; }

    [Pure]
    public bool Equals(QuestionableBoolean other)
    {
        return _value == other._value;
    }

    [Pure]
    public bool Equals(bool other)
    {
        return _value.Equals(other);
    }

    [Pure]
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        if (obj is QuestionableBoolean)
        {
            return Equals((QuestionableBoolean)obj);
        }
        if (obj is bool)
        {
            return Equals((bool)obj);
        }
        return false;
    }

    [Pure]
    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    [Pure]
    public int CompareTo(object obj)
    {
        return _value.CompareTo(obj);
    }

    [Pure]
    public int CompareTo(bool other)
    {
        return _value.CompareTo(other);
    }

    [Pure]
    public override string ToString()
    {
        return _value.ToString();
    }

    [Pure]
    public TypeCode GetTypeCode()
    {
        return _value.GetTypeCode();
    }

    [Pure]
    public bool ToBoolean(IFormatProvider provider)
    {
        return ((IConvertible)_value).ToBoolean(provider);
    }

    [Pure]
    public char ToChar(IFormatProvider provider)
    {
        return ((IConvertible)_value).ToChar(provider);
    }

    [Pure]
    public sbyte ToSByte(IFormatProvider provider)
    {
        return ((IConvertible)_value).ToSByte(provider);
    }

    [Pure]
    public byte ToByte(IFormatProvider provider)
    {
        return ((IConvertible)_value).ToByte(provider);
    }

    [Pure]
    public short ToInt16(IFormatProvider provider)
    {
        return ((IConvertible)_value).ToInt16(provider);
    }

    [Pure]
    public ushort ToUInt16(IFormatProvider provider)
    {
        return ((IConvertible)_value).ToUInt16(provider);
    }

    [Pure]
    public int ToInt32(IFormatProvider provider)
    {
        return ((IConvertible)_value).ToInt32(provider);
    }

    [Pure]
    public uint ToUInt32(IFormatProvider provider)
    {
        return ((IConvertible)_value).ToUInt32(provider);
    }

    [Pure]
    public long ToInt64(IFormatProvider provider)
    {
        return ((IConvertible)_value).ToInt64(provider);
    }

    [Pure]
    public ulong ToUInt64(IFormatProvider provider)
    {
        return ((IConvertible)_value).ToUInt64(provider);
    }

    [Pure]
    public float ToSingle(IFormatProvider provider)
    {
        return ((IConvertible)_value).ToSingle(provider);
    }

    [Pure]
    public double ToDouble(IFormatProvider provider)
    {
        return ((IConvertible)_value).ToDouble(provider);
    }

    [Pure]
    public decimal ToDecimal(IFormatProvider provider)
    {
        return ((IConvertible)_value).ToDecimal(provider);
    }

    [Pure]
    public DateTime ToDateTime(IFormatProvider provider)
    {
        return ((IConvertible)_value).ToDateTime(provider);
    }

    [Pure]
    public String ToString(IFormatProvider provider)
    {
        return _value.ToString(provider);
    }

    [Pure]
    public object ToType(Type conversionType, IFormatProvider provider)
    {
        return ((IConvertible)_value).ToType(conversionType, provider);
    }

    [Pure]
    public static bool operator ==(bool a, QuestionableBoolean b)
    {
        return b._value == a;
    }

    [Pure]
    public static bool operator !=(bool a, QuestionableBoolean b)
    {
        return b._value != a;
    }

    [Pure]
    public static bool operator ==(QuestionableBoolean a, bool b)
    {
        return a._value == b;
    }

    [Pure]
    public static bool operator !=(QuestionableBoolean a, bool b)
    {
        return a._value != b;
    }

    [Pure]
    public static bool operator ==(QuestionableBoolean a, QuestionableBoolean b)
    {
        return a._value == b._value;
    }

    [Pure]
    public static bool operator !=(QuestionableBoolean a, QuestionableBoolean b)
    {
        return a._value != b._value;
    }

    [Pure]
    public static implicit operator bool(QuestionableBoolean m)
    {
        return m._value;
    }

    [Pure]
    public static implicit operator QuestionableBoolean(bool m)
    {
        return new QuestionableBoolean(m);
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("", this._value);
        info.AddValue("Reason", Reason);
    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        reader.ReadStartElement("qBoolean");
        Reason = reader.GetAttribute(nameof(Reason));
        if (string.IsNullOrWhiteSpace(Reason))
        {
            Reason = Default.Reason;
        }
        _value = reader.ReadElementContentAsBoolean();
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteStartElement("qBoolean");
        writer.WriteStartAttribute(nameof(Reason));
        if (Reason != FallbackReason && Reason != null)
        {
            writer.WriteString(Reason == FallbackReason ? "" : Reason);
            writer.WriteEndAttribute();
        }
        writer.WriteString(_value.ToString());
        writer.WriteEndElement();
    }

    public void Read(BinaryReader r)
    {
        var reasonLength = r.ReadInt32();
        Reason = r.ReadString();
        _value = r.ReadBoolean();
    }

    public void Write(BinaryWriter w)
    {
        w.Write(string.IsNullOrWhiteSpace(Reason) || Reason == FallbackReason ? "" : Reason);
        w.Write(_value);
    }
}