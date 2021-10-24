using System;

namespace Core.Utilities
{
    public class ReflectionUtilities
    {
        private ReflectionUtilities()
        {
        }

        public static object GetDefaultValue(Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }
    }
}