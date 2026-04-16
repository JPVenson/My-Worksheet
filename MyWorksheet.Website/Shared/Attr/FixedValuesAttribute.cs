using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MyWorksheet.Public.Models.Attr
{
    public class FixedValuesAttribute : ValidationAttribute
    {
        private readonly object[] _anyValue;

        public FixedValuesAttribute(params object[] anyValue)
        {
            _anyValue = anyValue;
        }

        public override bool IsValid(object value)
        {
            if (_anyValue.Any(e => (e == null && value == null) || e != null && e.Equals(value)))
            {
                return true;
            }
            return false;
        }
    }
}