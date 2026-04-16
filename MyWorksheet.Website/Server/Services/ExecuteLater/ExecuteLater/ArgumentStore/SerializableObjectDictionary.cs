using System.Runtime.Serialization;

namespace MyWorksheet.Website.Server.Services.ExecuteLater.ExecuteLater.ArgumentStore
{
	public class SerializableObjectDictionary : SerializableObjectDictionary<object, object>
	{
		internal static DataContractSerializer _objectSerilizer = new DataContractSerializer(typeof(SerializableObjectDictionary));

		public SerializableObjectDictionary()
		{
		}
	}
}