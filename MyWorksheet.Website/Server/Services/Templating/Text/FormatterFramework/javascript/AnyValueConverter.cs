using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ChakraCore.NET;
using ChakraCore.NET.API;

namespace MyWorksheet.ReportService.Services.Templating.Text.FormatterFramework.javascript;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class JsCallingConventionAttribute : Attribute
{
    public BindingFlags MethodsBindingFlags { get; set; } = BindingFlags.Public | BindingFlags.Instance;
}

/// <summary>
///		Marks this function to be executed from JS code
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class JsCallableAttribute : Attribute
{

}

/// <summary>
///		Is able to convert any object to its corresponding Javascript object
/// </summary>
/// <seealso cref="ChakraCore.NET.ServiceBase" />
/// <seealso cref="ChakraCore.NET.IJSValueConverterService" />
public class AnyValueConverter : ServiceBase, IJSValueConverterService
{
    private readonly JsValueConverterService _ownService;

    public AnyValueConverter()
    {
        _ownService = new JsValueConverterService(this);
    }

    public bool CanConvert<T>()
    {
        return CanConvert(typeof(T));
    }

    public bool CanConvert(Type t)
    {
        return _ownService.CanConvert(t);
    }

    public void RegisterConverter<T>(toJSValueDelegate<T> toJsValue, fromJSValueDelegate<T> fromJsValue,
        bool throewIfExists = true)
    {
        _ownService.RegisterConverter(toJsValue, fromJsValue, throewIfExists);
    }

    public JavaScriptValue ToJSValue<T>(T value)
    {
        return ToUnknownValue(value);
    }

    public T FromJSValue<T>(JavaScriptValue value)
    {
        if (_ownService.CanConvert<T>())
        {
            return _ownService.FromJSValue<T>(value);
        }

        return default(T);
    }

    /// <summary>
    ///		Can convert any object to Javascript
    /// </summary>
    public JavaScriptValue ToUnknownValue(object value)
    {
        if (value == null)
        {
            return JavaScriptValue.Null;
        }
        //check if there is any registration for this object
        if (_ownService.CanConvert(value.GetType()))
        {
            return _ownService.ToJsUnknownValue(value);
        }
        //otherwise convert it to a new Javascript object
        var jsObject = JavaScriptValue.CreateObject();
        var valueService = CurrentNode.GetService<IJSValueService>();
        var type = value.GetType();
        //this is an array so get each item and set it on the object
        if (type.IsArray ||
            typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string))
        {
            var listValue = value as IEnumerable;
            var array = CurrentNode.GetService<IJSValueService>().CreateArray(Convert.ToUInt32(listValue
                .OfType<object>()
                .Count()));
            var num = 0;
            foreach (var obj in listValue)
            {
                array.SetIndexedProperty(ToJSValue(num++), ToJSValue(obj));
            }

            return array;
        }
        //get all properties and convert them into the JSObject
        foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var csValue = propertyInfo.GetValue(value);

            if (csValue == null)
            {
                valueService.WriteProperty(jsObject, propertyInfo.Name, JavaScriptValue.Null);
                continue;
            }

            valueService.WriteProperty(jsObject, propertyInfo.Name, csValue);
        }

        //this object supports calling methods
        var methods = type.GetMethods()
            .Where(e => e.GetCustomAttribute<JsCallableAttribute>() != null);

        foreach (var methodInfo in methods)
        {
            //create an untyped delegate for this method
            var delToMethod = CreateDelegate(methodInfo, value);
            //if not known add an generic converter that translates the function
            _ownService.RegisterConverter(delToMethod.GetType(), CreateGenericMethodConverter(methodInfo), (node, scriptValue) => { return null; }, false);

            valueService.WriteProperty(jsObject, JavaScriptPropertyId.FromString(methodInfo.Name), delToMethod);
        }

        return jsObject;
    }

    private toJSValueDelegate<object> CreateGenericMethodConverter(MethodInfo methodInfo)
    {
        return (node, o) =>
        {
            return JavaScriptValue.CreateFunction(
                (callee, isConstructCall, arguments, argumentsCount, callback) =>
                {
                    var csArgumentList = methodInfo.GetParameters();
                    if (csArgumentList.Length > arguments.Length - 1) //check that all required arguments are presend
                    {
                        return JavaScriptValue.Invalid;
                    }
                    try
                    {
                        var csArgs = arguments.Skip(1).Select((item, index) => //skip the first argument as its the self reference
                                FromJsUnknownValue(csArgumentList[index].ParameterType, item))
                            .ToArray();
                        return ToUnknownValue((o as Delegate)?.DynamicInvoke(csArgs));
                    }
                    catch (Exception)
                    {
                        return JavaScriptValue.Invalid;
                    }
                });
        };
    }

    private static Delegate CreateDelegate(MethodInfo methodInfo, object target)
    {
        Func<Type[], Type> getType;
        var isAction = methodInfo.ReturnType.Equals(typeof(void));
        var types = methodInfo.GetParameters().Select(p => p.ParameterType);

        if (isAction)
        {
            getType = Expression.GetActionType;
        }
        else
        {
            getType = Expression.GetFuncType;
            types = types.Concat(new[] { methodInfo.ReturnType });
        }

        if (methodInfo.IsStatic)
        {
            return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
        }

        return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
    }
    /// <summary>
    ///		Creates an Cs object from an Javascript one
    /// </summary>
    public object FromJsUnknownValue(Type type, JavaScriptValue value)
    {
        if (_ownService.CanConvert(type))
        {
            return _ownService.FromJSUnknownValue(type, value);
        }

        if (type.IsArray)
        {
            var elementType = type.GetElementType();
            var capacity = FromJSValue<int>(value.GetProperty(JavaScriptPropertyId.FromString("length")));
            var objList = new ArrayList();
            if (objList == null)
            {
                throw new InvalidOperationException("Could not construct type");
            }

            for (int index = 0; index < capacity; ++index)
            {
                objList.Add(FromJsUnknownValue(elementType, value.GetIndexedProperty(ToJSValue<int>(index))));
            }

            return objList.ToArray(elementType);
        }

        //try to construct it from its ... its ... javascript value type prop

        switch (value.ValueType)
        {
            case JavaScriptValueType.Null:
            case JavaScriptValueType.Undefined:
                return null;
            case JavaScriptValueType.Number:
                return value.ToInt32();
            case JavaScriptValueType.String:
                return value.ToString();
            case JavaScriptValueType.Boolean:
                return value.ToBoolean();
            case JavaScriptValueType.Object:
                break;
            case JavaScriptValueType.Function:
                break;
            case JavaScriptValueType.Error:
                break;
            case JavaScriptValueType.Array:
                break;
            case JavaScriptValueType.Symbol:
                break;
            case JavaScriptValueType.ArrayBuffer:
                break;
            case JavaScriptValueType.TypedArray:
                break;
            case JavaScriptValueType.DataView:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        //TODO reverse engineer the type 
        return _ownService.FromJSUnknownValue(typeof(string), value.ConvertToString());
    }

    //this is the dump from the old JsValueConverter for Primitive types and also for still having a sort of collection for adding types manual
    public class JsValueConverterService : IJSValueConverterService
    {
        private readonly SortedDictionary<Type, Tuple<object, object>> _converters =
            new SortedDictionary<Type, Tuple<object, object>>(TypeComparer.Instance);

        private readonly IJSValueConverterService _parentNode;

        public JsValueConverterService(IJSValueConverterService parentNode)
        {
            _parentNode = parentNode;
            InitDefault();
        }

        public JavaScriptValue ToJSValue<T>(T value)
        {
            return ToJsUnknownValue(value);
        }

        public bool CanConvert<T>()
        {
            return _converters.ContainsKey(typeof(T));
        }

        public bool CanConvert(Type t)
        {
            return _converters.ContainsKey(t);
        }

        public void RegisterConverter<T>(toJSValueDelegate<T> toJsValue, fromJSValueDelegate<T> fromJsValue,
            bool throwIfExists = true)
        {
            if (CanConvert<T>())
            {
                if (throwIfExists)
                {
                    throw new ArgumentException($"type {typeof(T).FullName} already registered");
                }
            }
            else
            {
                _converters.Add(typeof(T), new Tuple<object, object>(toJsValue, fromJsValue));
            }
        }

        public T FromJSValue<T>(JavaScriptValue value)
        {
            if (!CanConvert<T>())
            {
                throw new NotImplementedException(
                    $"type {typeof(T).FullName} not registered for convertion");
            }

            var fromJsValueDelegate = _converters[typeof(T)].Item2 as fromJSValueDelegate<T>;
            if (fromJsValueDelegate == null)
            {
                throw new NotImplementedException(
                    $"type {typeof(T).FullName} does not support convert from JSValue");
            }

            return fromJsValueDelegate(CurrentNode, value);
        }

        public IServiceNode CurrentNode
        {
            get { return _parentNode.CurrentNode; }
            set { }
        }

        public void RegisterConverter(Type type, toJSValueDelegate<object> toJsValue,
            fromJSValueDelegate<object> fromJsValue, bool throewIfExists = true)
        {
            if (CanConvert(type))
            {
                if (throewIfExists)
                {
                    throw new ArgumentException($"type {type.FullName} already registered");
                }
            }
            else
            {
                _converters.Add(type, new Tuple<object, object>(toJsValue, fromJsValue));
            }
        }

        public object FromJSUnknownValue(Type toType, JavaScriptValue value)
        {
            if (!CanConvert(toType))
            {
                throw new NotImplementedException(
                    $"type {toType.FullName} not registered for convertion");
            }

            var fromJsValueDelegate = _converters[toType].Item2 as Delegate;
            if (fromJsValueDelegate == null)
            {
                throw new NotImplementedException(
                    $"type {toType.FullName} does not support convert from JSValue");
            }

            return fromJsValueDelegate.DynamicInvoke(CurrentNode, value);
        }

        public JavaScriptValue ToJsUnknownValue(object value)
        {
            if (!CanConvert(value.GetType()))
            {
                throw new NotImplementedException($"type {value.GetType().FullName} not registered for convertion");
            }

            var toJsValueDelegate = _converters[value.GetType()].Item1 as Delegate;
            if (toJsValueDelegate == null)
            {
                throw new NotImplementedException(
                    $"type {value.GetType().FullName} does not support convert to JSValue");
            }

            return (JavaScriptValue)toJsValueDelegate.DynamicInvoke(CurrentNode, value);
        }

        private void InitDefault()
        {
            RegisterConverter((node, value) => JavaScriptValue.FromString(value),
                (node, value) => value.ConvertToString().ToString());
            this.RegisterArrayConverter<string>();
            RegisterConverter((node, value) => JavaScriptValue.FromString(value.ToString()),
                (node, value) => DateTime.Parse(value.ConvertToString().ToString()));
            this.RegisterArrayConverter<DateTime>();
            RegisterConverter((node, value) => JavaScriptValue.FromString(value.ToString()),
                (node, value) => DateTimeOffset.Parse(value.ConvertToString().ToString()));
            this.RegisterArrayConverter<DateTimeOffset>();
            RegisterConverter((node, value) => JavaScriptValue.FromInt32(value),
                (node, value) => value.ConvertToNumber().ToInt32());
            this.RegisterArrayConverter<int>();
            RegisterConverter((node, value) => JavaScriptValue.FromDouble(value),
                (node, value) => value.ConvertToNumber().ToDouble());
            this.RegisterArrayConverter<double>();
            RegisterConverter((node, value) => JavaScriptValue.FromBoolean(value),
                (node, value) => value.ConvertToBoolean().ToBoolean());
            this.RegisterArrayConverter<bool>();
            RegisterConverter((node, value) => JavaScriptValue.FromDouble(value),
                (node, value) => Convert.ToSingle(value.ConvertToNumber().ToDouble()));
            this.RegisterArrayConverter<float>();
            RegisterConverter((node, value) => JavaScriptValue.FromDouble(Convert.ToDouble(value)),
                (node, value) => BitConverter.GetBytes(value.ToInt32())[0]);
            this.RegisterArrayConverter<byte>();
            RegisterConverter((node, value) => JavaScriptValue.FromDouble(Convert.ToDouble(value)),
                (node, value) => Convert.ToDecimal(value.ConvertToNumber().ToDouble()));
            this.RegisterArrayConverter<decimal>();
            RegisterConverter((node, value) => value.ReferenceValue, (node, value) => new JSValue(node, value));
            this.RegisterMethodConverter();
            this.RegisterFunctionConverter<Func<string, string>>();
            RegisterConverter((node, value) => JavaScriptValue.FromString(value.ToString()),
                (node, value) => Guid.Parse(value.ConvertToString().ToString()));
            RegisterConverter((node, value) => value, (node, value) => value);
            this.RegisterArrayConverter<JavaScriptValue>();
        }
    }
}