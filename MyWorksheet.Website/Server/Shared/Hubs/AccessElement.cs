//using System;
//using System.Reflection;
//using System.Threading;

//namespace MyWorksheet.Hubs
//{
//	public abstract class HubAccess
//	{
//		protected HubAccess()
//		{
//		}

//		public virtual Type HubType => Type.GetType(GetType().Name.Replace("Info", ""));

//		public IHubContext HubContext { get; set; }
//	}

//	internal static class HubTypeExtensions
//	{
//		internal static string GetHubName(this Type type)
//		{
//			if (!typeof(IHub).IsAssignableFrom(type))
//				return (string)null;
//			return type.GetHubAttributeName() ?? HubTypeExtensions.GetHubTypeName(type);
//		}

//		internal static string GetHubAttributeName(this Type type)
//		{
//			if (!typeof(IHub).IsAssignableFrom(type))
//				return (string)null;
//			return ReflectionHelper.GetAttributeValue<HubNameAttribute, string>((ICustomAttributeProvider)type, (Func<HubNameAttribute, string>)(attr => attr.HubName));
//		}

//		private static string GetHubTypeName(Type type)
//		{
//			int length = type.Name.LastIndexOf('`');
//			if (length == -1)
//				return type.Name;
//			return type.Name.Substring(0, length);
//		}
//	}

//	public static class AccessElement<T> where T: HubAccess, new()
//	{
//		private static readonly Lazy<T> _instance = new Lazy<T>(() =>
//		{
//			var foo = new T();
//			var hubName = foo.HubType.GetHubName();
//			foo.HubContext = GlobalHost.ConnectionManager.GetHubContext(hubName);
//			return foo;
//		}, LazyThreadSafetyMode.ExecutionAndPublication);


//		public static T Instance => _instance.Value;
//	}
//}