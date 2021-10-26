using System;

namespace Core.Utilities
{
    public class ReflectionUtilities
    {
        private ReflectionUtilities()
        {
        }

        // int - 0, objects - null
        public static object GetDefaultValue(Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }
    }
}