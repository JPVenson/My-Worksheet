using System;
using System.Reflection;

namespace MyWorksheet.Website.Shared.ViewModels
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ObjectTrackingAttribute : Attribute
    {
        public ObjectTrackingAttribute(string trackingKey)
        {
            TrackingKey = trackingKey;
        }

        public string TrackingKey { get; set; }
    }

    public static class TrackingExtensions
    {
        public static string GetTrackingName(this Type type)
        {
            var objectTrackingAttribute = type.GetCustomAttribute<ObjectTrackingAttribute>(true);
            if (objectTrackingAttribute == null)
            {
                Console.WriteLine("No Tracking defined for: " + type);
            }
            return objectTrackingAttribute?.TrackingKey;
        }
    }
}
