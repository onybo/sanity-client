using System;
using System.Reflection;

namespace Olav.Sanity.Client.Mutators
{
    public class PatchMutator
    {
        private object _obj;
        public object Patch
        {
            get => _obj;
            set
            {
                RequireId(value);
                _obj =value;
            }
        }

        private void RequireId(object type)
        {
            if (type is IHaveId)
                return;
            if (type.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance, null, typeof(string), new Type[0], null) == null)
            {
                throw new Exception("Type does not implemented required public string property named Id");
            }
        }
    }
}
