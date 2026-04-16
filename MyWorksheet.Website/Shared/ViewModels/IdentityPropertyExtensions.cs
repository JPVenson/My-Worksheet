using System;
using System.Linq;
using System.Reflection;

namespace MyWorksheet.Website.Shared.ViewModels
{
    public static class IdentityPropertyExtensions
    {
        public static Guid GetId(this object value)
        {
            if (value is ViewModelBase vmb)
            {
                return vmb.GetModelIdentifier().GetValueOrDefault();
            }

            var propertyInfos = value.GetType().GetProperties();
            var idProp = propertyInfos.FirstOrDefault(e => e.GetCustomAttribute<IdentityPropertyAttribute>() != null);
            if (idProp == null)
            {
                idProp = propertyInfos.First(e => e.PropertyType == typeof(Guid) && e.Name.EndsWith("Id"));
            }

            return (Guid)idProp.GetValue(value);
        }
    }
}