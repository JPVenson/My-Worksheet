using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using MyWorksheet.Website.Server.Services.Reporting.Text;
using XmlLikeSerilizer = System.Runtime.Serialization.DataContractSerializer;

namespace MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;

[XmlRoot("any-dictionary")]
public class SerializableObjectDictionary<TKey, TValue>
    : Dictionary<TKey, TValue>, IXmlSerializable
{
    public SerializableObjectDictionary()
    {
    }

    public SerializableObjectDictionary(IDictionary<TKey, TValue> arguments) : base(arguments)
    {
    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        bool wasEmpty = reader.IsEmptyElement;
        reader.Read();

        if (wasEmpty)
        {
            return;
        }

        try
        {

            var types = new Dictionary<int, KeyValuePair<Type, XmlLikeSerilizer>>();
            //read meta
            if (!reader.IsEmptyElement)
            {
                reader.ReadStartElement("value-types");
                while (reader.IsStartElement("type"))
                {
                    var index = reader.GetAttribute("index");
                    var type = reader.ReadElementString();
                    var parsedIndex = 0;
                    if (!int.TryParse(index, out parsedIndex))
                    {
                        throw new InvalidOperationException($"Could not parse the Attribute 'index' to 'int'. Value: '{index}'");
                    }

                    var parsedType = Type.GetType(type);
                    if (parsedType == null)
                    {
                        if (type == "MyWorksheet.Webpage.Services.Templating.Text.TextTemplateDataQuery")
                        {
                            parsedType = typeof(TextTemplateDataQuery);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Could not parse the Element 'type' to 'Type'. Value: '{type}'");
                        }
                    }

                    if (typeof(IDictionary).IsAssignableFrom(parsedType))
                    {
                        types.Add(parsedIndex, new KeyValuePair<Type, XmlLikeSerilizer>(parsedType, new XmlLikeSerilizer(typeof(SerializableObjectDictionary<,>).MakeGenericType(parsedType.GenericTypeArguments))));
                    }
                    else
                    {
                        types.Add(parsedIndex, new KeyValuePair<Type, XmlLikeSerilizer>(parsedType, new XmlLikeSerilizer(parsedType)));
                    }
                }

                reader.ReadEndElement();
            }
            else
            {
                reader.ReadStartElement("value-types");
            }

            while (reader.IsStartElement("item"))
            {
                reader.ReadStartElement("item");
                TKey key = default(TKey);
                TValue value = default(TValue);

                if (reader.HasAttributes)
                {
                    var typeIdKey = reader.GetAttribute("typeId");
                    if (!int.TryParse(typeIdKey, out var parsedTypeId))
                    {
                        throw new InvalidOperationException($"Could not parse the Attribute 'type-id' to 'int'. Value: '{parsedTypeId}'");
                    }

                    if (!types.ContainsKey(parsedTypeId))
                    {
                        throw new InvalidOperationException($"Invalid type-id on element '{parsedTypeId}'");
                    }

                    if (reader.IsEmptyElement)
                    {
                        reader.ReadStartElement("key");
                        key = default(TKey);
                    }
                    else
                    {
                        reader.ReadStartElement("key");
                        var type = types[parsedTypeId];
                        key = (TKey)type.Value.ReadObject(reader);
                    }
                }
                else
                {
                    reader.ReadStartElement("key");
                    key = (TKey)reader.ReadContentAs(typeof(TKey), reader as IXmlNamespaceResolver);
                }

                //END key
                reader.ReadEndElement();

                if (reader.HasAttributes)
                {
                    var typeIdKey = reader.GetAttribute("typeId");
                    if (!int.TryParse(typeIdKey, out var parsedTypeId))
                    {
                        throw new InvalidOperationException($"Could not parse the Attribute 'type-id' to 'int'. Value: '{parsedTypeId}'");
                    }

                    if (!types.ContainsKey(parsedTypeId))
                    {
                        throw new InvalidOperationException($"Invalid type-id on element '{parsedTypeId}'");
                    }

                    if (reader.IsEmptyElement)
                    {
                        reader.ReadStartElement("value");
                        value = default(TValue);
                    }
                    else
                    {
                        reader.ReadStartElement("value");
                        var type = types[parsedTypeId];
                        value = (TValue)type.Value.ReadObject(reader);
                    }
                }
                else
                {
                    reader.ReadStartElement("value");
                    value = (TValue)reader.ReadContentAs(typeof(TValue), reader as IXmlNamespaceResolver);
                }

                Add(key, value);
                reader.ReadEndElement();
                reader.ReadEndElement();
            }

            reader.ReadEndElement();
        }
        catch (Exception e)
        {
            throw;
        }
    }

    private Assembly MsCoreLib = typeof(string).Assembly;

    public void WriteXml(XmlWriter writer)
    {
        var typeLookup = Values.Where(e => e != null)
            .Select((item, valIndex) => new KeyValuePair<int, Type>(valIndex, item.GetType()))
            .GroupBy(e => e.Value)
            .Select(e => e.First());

        Dictionary<KeyValuePair<int, Type>, XmlLikeSerilizer> types;

        XmlLikeSerilizer GetSerilizer(Type fromType)
        {
            if (typeof(IDictionary).IsAssignableFrom(fromType))
            {
                return new XmlLikeSerilizer(typeof(SerializableObjectDictionary<,>).MakeGenericType(fromType.GenericTypeArguments));
            }
            return new XmlLikeSerilizer(fromType);
        }

        if (typeof(TKey) == typeof(object) || typeof(TValue) == typeof(object))
        {
            types = typeLookup
                .ToDictionary(e => e, e => GetSerilizer(e.Value));
        }
        else
        {
            types = typeLookup
                .Where(e => e.Value.Assembly != MsCoreLib || typeof(IDictionary).IsAssignableFrom(e.Value))
                .ToDictionary(e => e, e => GetSerilizer(e.Value));
        }



        //write lookup table for all types in the value part of the dictionary
        writer.WriteStartElement("value-types");
        foreach (var type in types)
        {
            writer.WriteStartElement("type");
            //give it an index
            writer.WriteAttributeString("index", type.Key.Key.ToString());
            writer.WriteValue(type.Key.Value.AssemblyQualifiedName);
            writer.WriteEndElement();
        }

        writer.WriteEndElement();

        foreach (var item in this)
        {
            writer.WriteStartElement("item");
            var subWriterKey = types.FirstOrDefault(e => e.Key.Value.ToString() == item.Key?.GetType().ToString());
            var subWriterValue = types.FirstOrDefault(e => e.Key.Value.ToString() == item.Value?.GetType().ToString());

            //write the Key
            writer.WriteStartElement("key");
            if (item.Key != null)
            {
                if (subWriterKey.Value == null)
                {
                    //its a Primitive type so just write the value
                    writer.WriteValue(item.Key);
                }
                else
                {
                    writer.WriteAttributeString("typeId", subWriterKey.Key.Key.ToString());
                    subWriterKey.Value.WriteObject(writer, item.Key);
                }
            }

            writer.WriteEndElement();

            writer.WriteStartElement("value");
            var value = item.Value;

            if (value != null)
            {
                if (subWriterValue.Value == null)
                {
                    //its a Primitive type so just write the value
                    writer.WriteValue(value);
                }
                else
                {
                    writer.WriteAttributeString("typeId", subWriterValue.Key.Key.ToString());

                    if (value is IDictionary dictionary)
                    {
                        var val = Activator.CreateInstance(typeof(SerializableObjectDictionary<,>).MakeGenericType(dictionary.GetType().GenericTypeArguments), dictionary);
                        subWriterValue.Value.WriteObject(writer, val);
                    }
                    else
                    {
                        subWriterValue.Value.WriteObject(writer, value);
                    }
                }
            }

            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }

    public TValue GetOrNull(TKey key)
    {
        if (ContainsKey(key))
        {
            return this[key];
        }

        return default(TValue);
    }
}