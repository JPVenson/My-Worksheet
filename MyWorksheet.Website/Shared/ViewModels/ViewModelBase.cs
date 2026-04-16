using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using MyWorksheet.Website.Shared.ViewModels.ApiResultModels.Worksheet;

namespace MyWorksheet.Website.Shared.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanging, INotifyPropertyChanged, IEntityObject
    {
        /// <summary>
        ///     Allows to raise the AcceptPending Change for the Memento Pattern
        /// </summary>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        protected virtual bool SetProperty<TArgument>(ref TArgument member, TArgument value,
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(member, value))
            {
                return false;
            }

            SendPropertyChanging(propertyName);
            member = value;
            SendPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        ///     Allows to raise the AcceptPending Change for the Memento Pattern
        /// </summary>
        /// <param name="member"></param>
        /// <param name="value"></param>
        /// <param name="property"></param>
        protected virtual void SetProperty<TProperty>(ref TProperty member, TProperty value, Expression<Func<TProperty>> property)
        {
            SetProperty(ref member, value, GetPropertyName(property));
        }

        /// <summary>
        ///     Raises this ViewModels PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Name of the property that has a new value</param>
        protected virtual void SendPropertyChanged()
        {
            SendPropertyChanged("*");
        }

        /// <summary>
        ///     Raises this ViewModels PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Name of the property that has a new value</param>
        protected virtual void SendPropertyChanged([CallerMemberName] string propertyName = null)
        {
            SendPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Raises this ViewModels PropertyChanged event
        /// </summary>
        /// <param name="e">Arguments detailing the change</param>
        protected virtual void SendPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        ///     Raises the PropertyChanged Event
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        protected virtual void SendPropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            SendPropertyChanged(GetPropertyName(property));
        }

        /// <summary>
        ///     Raises this ViewModels PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Name of the property that has a new value</param>
        protected virtual void SendPropertyChanging([CallerMemberName] string propertyName = null)
        {
            SendPropertyChanging(new PropertyChangingEventArgs(propertyName));
        }

        /// <summary>
        ///     Raises this ViewModels PropertyChanging event
        /// </summary>
        /// <param name="e">Arguments detailing the change</param>
        protected virtual void SendPropertyChanging(PropertyChangingEventArgs e)
        {
            var handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        ///     Raises this ViewModels PropertyChanging event
        /// </summary>
        /// <param name="property">Arguments detailing the change</param>
        protected virtual void SendPropertyChanging<TProperty>(Expression<Func<TProperty>> property)
        {
            SendPropertyChanging(GetPropertyName(property));
        }

        /// <summary>
        ///     Helper for getting the Lambda Property from the expression
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static string GetPropertyName<TProperty>(Expression<Func<TProperty>> property)
        {
            return GetProperty(property).Name;
        }

        /// <summary>
        ///     Helper for getting the property info of an expression
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty<TProperty>(Expression<Func<TProperty>> property)
        {
            var lambda = (LambdaExpression)property;

            MemberExpression memberExpression;
            var body = lambda.Body as UnaryExpression;

            if (body != null)
            {
                var unaryExpression = body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            return memberExpression.Member as PropertyInfo;
        }

        /// <summary>
        ///     Helper for getting the property info of an expression
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty<TProperty, TObject>(Expression<Func<TObject, TProperty>> property)
        {
            var lambda = (LambdaExpression)property;

            MemberExpression memberExpression;
            var body = lambda.Body as UnaryExpression;

            if (body != null)
            {
                var unaryExpression = body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            return memberExpression.Member as PropertyInfo;
        }


        /// <inheritdoc />
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        public virtual event PropertyChangingEventHandler PropertyChanging;

        public virtual int State()
        {
            return HashCodeCompute.ComputeHash(this);
        }

        public virtual ViewModelState SnapshotState()
        {
            return HashCodeCompute.ComputeState(this);
        }

        public virtual Guid? GetModelIdentifier()
        {
            return null;
        }

        public virtual ViewModelBase Copy()
        {
            return base.MemberwiseClone() as ViewModelBase;
        }
    }

    public readonly struct StateValue
    {
        public StateValue(int hashcode, object valRef)
        {
            Hashcode = hashcode;
            ValRef = valRef;
        }

        public int Hashcode { get; }
        public object ValRef { get; }
    }

    public class ViewModelState : IEquatable<ViewModelState>
    {
        public ViewModelState(IDictionary<string, StateValue> states)
        {
            States = states;
            ModelState = states.Values.Any() ? states.Values.Select(e => e.Hashcode).Aggregate((e, f) => e ^ f) : 0;
        }

        public IDictionary<string, StateValue> States { get; }
        public int ModelState { get; }

        public class StateDif
        {
            public StateDif(string property, object oldValue, object currentValue)
            {
                Property = property;
                OldValue = oldValue;
                CurrentValue = currentValue;
            }

            public string Property { get; set; }
            public object OldValue { get; set; }
            public object CurrentValue { get; set; }

            public override string ToString()
            {
                return $"{Property} was '{OldValue}' but is '{CurrentValue}'";
            }
        }

        public IEnumerable<StateDif> Diff(ViewModelState modelState)
        {
            if (ModelState == modelState.ModelState)
            {
                yield break;
            }

            foreach (var state in States.Join(modelState.States, f => f.Key, f => f.Key, (oldValue, newValue) => (oldValue, newValue)))
            {
                if (!Equals(state.oldValue.Value, state.newValue.Value))
                {
                    yield return new StateDif(state.oldValue.Key, state.oldValue.Value.ValRef,
                        state.newValue.Value.ValRef);
                }
            }
        }

        public bool Equals(ViewModelState other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ModelState == other.ModelState;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((ViewModelState)obj);
        }

        public override int GetHashCode()
        {
            return ModelState;
        }
    }

    public static class CopyViewModelBase
    {
        public static T CopyClone<T>(this T vm) where T : ViewModelBase
        {
            return vm.Copy() as T;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class IgnoreStateAttribute : Attribute
    {
    }

    public static class HashCodeCompute
    {
        static HashCodeCompute()
        {
            Cache = new Dictionary<Type, Func<object, int>>();
            StateFuncCache = new Dictionary<Type, Func<object, ViewModelState>>();
        }
        static IDictionary<Type, Func<object, int>> Cache { get; set; }
        static IDictionary<Type, Func<object, ViewModelState>> StateFuncCache { get; set; }

        public static int ComputeHash(object obj)
        {
            var type = obj.GetType();
            if (!Cache.TryGetValue(type, out var fac))
            {
                fac = GetHashCodeFac(type);
                Cache[type] = fac;
            }

            return fac(obj);
        }

        public static ViewModelState ComputeState(object obj)
        {
            var type = obj.GetType();
            if (!StateFuncCache.TryGetValue(type, out var fac))
            {
                StateFuncCache[type] = fac = GetStateFac(type);
            }

            return fac(obj);
        }

        private static List<Type> _primitiveTypes = new List<Type>()
        {
            typeof(Boolean),
            typeof(Guid),
            typeof(Byte),
            typeof(SByte),
            typeof(Int16),
            typeof(UInt16),
            typeof(Int32),
            typeof(UInt32),
            typeof(Int64),
            typeof(UInt64),
            typeof(IntPtr),
            typeof(UIntPtr),
            typeof(Char),
            typeof(Double),
            typeof(Single),
            typeof(String),
            typeof(Decimal),
        };

        private static bool IsFrameworkType(Type type)
        {
            var nullable = Nullable.GetUnderlyingType(type);
            if (nullable != null)
            {
                type = nullable;
            }

            return _primitiveTypes.Contains(type);
        }

        private static Func<object, int> GetHashCodeFac(Type type)
        {
            var hashCode = type.Name.GetHashCode();
            var nullValue = hashCode ^ 1337;
            var queue = new List<Func<object, int>>();
            foreach (var propertyInfo in type.GetProperties()
                .Where(e => e.GetCustomAttribute<IgnoreStateAttribute>() == null)
                .Where(e => IsFrameworkType(e.PropertyType)))
            {
                queue.Add(f => propertyInfo.GetValue(f)?.GetHashCode() ?? nullValue);
            }

            return o =>
            {
                var code = hashCode;
                foreach (var func in queue)
                {
                    code ^= func(o);
                }

                return code;
            };
        }

        private static Func<object, ViewModelState> GetStateFac(Type type)
        {
            var hashCode = type.Name.GetHashCode();
            var nullValue = hashCode ^ 1337;
            var queue = new Dictionary<PropertyInfo, Func<object, StateValue>>();
            foreach (var propertyInfo in type.GetProperties()
                .Where(e => e.GetCustomAttribute<IgnoreStateAttribute>() == null)
                .Where(e => IsFrameworkType(e.PropertyType)))
            {
                queue.Add(propertyInfo, f =>
                {
                    var value = propertyInfo.GetValue(f) ?? nullValue;
                    return new StateValue(value.GetHashCode(), value);
                });
            }

            return o =>
            {
                return new ViewModelState(queue.ToDictionary(e => e.Key.Name, e => e.Value(o)));
            };
        }
    }
}
