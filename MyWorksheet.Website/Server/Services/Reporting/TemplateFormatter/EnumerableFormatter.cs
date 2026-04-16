//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Dynamic;
//using System.Linq.Expressions;

//namespace MyWorksheet.Webpage.Services.Templating.Text
//{
//	/// <summary>
//	///		Used to create an Example for what the Formatter can be used for
//	/// </summary>
//	public static class EnumerableFormatter
//	{
//		static EnumerableFormatter()
//		{
//			Formatter = new Dictionary<string, Func<IEnumerable<object>, string, object>>();

//			Formatter.Add("first or default", (collection, arg) => collection.FirstOrDefault());
//			Formatter.Add("max", (collection, arg) => collection.Max());
//			Formatter.Add("min", (collection, arg) => collection.Min());
//			Formatter.Add("order desc", (collection, arg) => collection.OrderByDescending(e => e));
//			Formatter.Add("reverse", (collection, arg) => collection.Reverse());
//			Formatter.Add("order", (collection, arg) => collection.OrderBy(e => e));
//			Formatter.Add("contains ", (collection, arg) => collection.Any(e => e.Equals(arg)));
//			Formatter.Add("element at ", (collection, arg) => collection.ElementAt(int.Parse(arg)));

//			UntypedFormatter = new Dictionary<string, Func<IEnumerable, string, object>>();
//			UntypedFormatter.Add("order by ", (collection, arg) => collection.OrderBy(arg));
//			UntypedFormatter.Add("count", (collection, arg) => collection.Count());
//			UntypedFormatter.Add("distinct", (collection, arg) => collection.Distinct());
//			UntypedFormatter.Add("group by ", (collection, arg) => collection.GroupBy(arg, "it"));
//			UntypedFormatter.Add("select ", (collection, arg) => collection.Select(arg));
//			UntypedFormatter.Add("where ", (collection, arg) => collection.Where(arg));
//			UntypedFormatter.Add("any ", (collection, arg) => collection.Any());
//			UntypedFormatter.Add("take ", (collection, arg) => collection.Take(int.Parse(arg)));
//			UntypedFormatter.Add("aggregate", (collection, arg) =>
//			{
//				var colQuery = collection.AsQueryable();

//				if (typeof(int).IsAssignableFrom(colQuery.ElementType))
//				{
//					return colQuery.Cast<int>().Sum();
//				}
//				if (typeof(decimal).IsAssignableFrom(colQuery.ElementType))
//				{
//					return colQuery.Cast<decimal>().Sum();
//				}

//				return collection;
//			});
//		}

//		public static Func<object, object> PropExpression(string propName)
//		{
//			var parameterExpression = Expression.Parameter(typeof(object));
//			var propCall = Expression.Property(parameterExpression, propName);
//			return Expression.Lambda<Func<object, object>>(propCall, parameterExpression).Compile();
//		}

//		public static IDictionary<string, Func<IEnumerable<object>, string, object>> Formatter { get; set; }
//		public static IDictionary<string, Func<IEnumerable, string, object>> UntypedFormatter { get; set; }

//		public static object FormatArgument(IEnumerable sourceCollection, string arguments)
//		{
//			var untypedFormatter = UntypedFormatter.FirstOrDefault(e => arguments.StartsWith(e.Key));

//			if (untypedFormatter.Value != null)
//			{
//				return untypedFormatter.Value(sourceCollection, arguments.Remove(0, untypedFormatter.Key.Length));
//			}

//			var formatter = Formatter.FirstOrDefault(e => arguments.StartsWith(e.Key));

//			if (formatter.Value != null)
//			{
//				return formatter.Value(sourceCollection.Cast<object>(), arguments.Remove(0, formatter.Key.Length));
//			}
//			return sourceCollection;
//		}
//	}
//}