using System;
using System.Reflection;

namespace SunLine.EasyMoq.Core
{
	public static class HelperType
    {
        public static object GetDefaultValue(Type t)
        {
            if (t.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(t);
            }
        
            return null;
        }
    }
}