using System.Runtime.Serialization;

namespace MyWorksheet.Website.Server.Services.PriorityQueue.ExecuteLater.ArgumentStore;

public class SerializableObjectDictionary : SerializableObjectDictionary<object, object>
{
    internal static DataContractSerializer _objectSerilizer = new DataContractSerializer(typeof(SerializableObjectDictionary));

    public SerializableObjectDictionary()
    {
    }
}