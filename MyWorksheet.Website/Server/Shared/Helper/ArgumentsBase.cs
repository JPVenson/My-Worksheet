using System;
using System.Collections.Generic;
using System.Linq;

namespace MyWorksheet.Helper;

public class ArgumentsBase
{
    private bool _valid = true;

    protected ArgumentsBase GetIfValid()
    {
        if (_valid)
        {
            return this;
        }

        return null;
    }

    private void SetPerReflection(object value, string name)
    {
        GetType().GetProperty(name).SetValue(this, value);
    }
    private Type GetPerReflection(string name)
    {
        return GetType().GetProperty(name).PropertyType;
    }

    protected void SetOrAbort<T>(IDictionary<string, object> arguments, string name)
    {
        if (!Set(arguments, (val) => SetPerReflection(val, name), name, typeof(T)))
        {
            _valid = false;
        }
    }

    protected bool TrySet<T>(IDictionary<string, object> arguments, string name)
    {
        return Set(arguments, (val) => SetPerReflection(val, name), name, typeof(T));
    }



    protected void SetOrAbort(IDictionary<string, object> arguments, string name)
    {
        if (!Set(arguments, (val) => SetPerReflection(val, name), name, GetPerReflection(name)))
        {
            _valid = false;
        }
    }

    protected bool TrySet(IDictionary<string, object> arguments, string name)
    {
        return Set(arguments, (val) => SetPerReflection(val, name), name, GetPerReflection(name));
    }

    protected void SetOrAbort<T>(IDictionary<string, object> arguments, Action<T> setter, string name)
    {
        if (!Set(arguments, setter, name))
        {
            _valid = false;
        }
    }

    protected bool TrySet<T>(IDictionary<string, object> arguments, Action<T> setter, string name)
    {
        return Set(arguments, setter, name);
    }

    protected bool Set<T>(IDictionary<string, object> arguments, Action<T> setter, string name)
    {
        return Set(arguments, (val) => setter((T)val), name, typeof(T));
    }

    protected bool Set(IDictionary<string, object> arguments, Action<object> setter, string name, Type toBe)
    {
        if (!_valid)
        {
            return false;
        }

        var nameParts = name.Split('.');
        if (nameParts.Length > 1)
        {
            foreach (var namePart in nameParts.Take(nameParts.Length - 1))
            {
                if (!arguments.ContainsKey(namePart))
                {
                    return false;
                }

                arguments = arguments[namePart] as IDictionary<string, object>;
                if (arguments == null)
                {
                    return false;
                }
            }

            name = nameParts.Last();
        }


        if (!arguments.ContainsKey(name))
        {
            return false;
        }

        var val = arguments[name];

        if (!(toBe.IsInstanceOfType(val)))
        {
            if (val?.GetType() == null)
            {
                return false;
            }

            if (toBe == typeof(Guid) && val is string strVal)
            {
                val = Guid.Parse(strVal);
            }
            else if (toBe.IsInstanceOfType(val))
            {
                val = Convert.ChangeType(val, toBe);
            }
            else
            {
                return false;
            }
        }

        setter(val);
        return true;
    }
}