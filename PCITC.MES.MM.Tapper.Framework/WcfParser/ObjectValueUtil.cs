using System;
using System.Collections.Generic;
using System.Linq;

namespace PCITC.MES.MM.Tapper.Framework.WcfParser
{
    public  static class ObjectValueUtil
    {
        public static object CreateInstance(this Type type, string name, Dictionary<string, string> paramValues)
        {
            if (type.IsNullable())
            {
                return GetNullBaseValue(name, type, paramValues);
            }

            if (type.IsSimple())
            {
                return GetPrimitiveValue(name, type, paramValues);
            }

            if (type.IsArray)
            {
                return GetArrayValue(name, type, paramValues);
            }

            if (type.IsEnum)
            {
                return GetEnumValue(name, type, paramValues);
            }

            return GetObjectValue(name, type, paramValues);

        }

        private static object GetEnumValue(string name, Type type, Dictionary<string, string> paramValues)
        {
            var value = paramValues[name];
            return Enum.Parse(type, value);

        }

        private static object GetNullBaseValue(string name, Type type, Dictionary<string, string> paramValues)
        {
            if (!paramValues.ContainsKey(name))
            {
                return null;
            }

            var value = paramValues[name];
            if (string.IsNullOrEmpty(value) || value.ToUpper() == "NULL")
            {
                return null;
            }

            return Convert.ChangeType(value, type.UnderlyingType());
        }

        private static object GetPrimitiveValue(string name, Type type, Dictionary<string, string> paramValues)
        {
            if (!paramValues.ContainsKey(name))
            {
                return GetDefaultValue(type);
            }

            var value = paramValues[name];
            if (string.IsNullOrEmpty(value) || value.ToUpper() == "NULL")
            {
                return GetDefaultValue(type);
            }

            return Convert.ChangeType(value, type);
        }

        private static object GetArrayValue(string name, Type type, Dictionary<string, string> paramValues)
        {
            var elementType = type.GetElementType();

            var elements = new List<object>();

            if (paramValues.ContainsKey(name))
            {
                elements.Add(CreateInstance(elementType, name, paramValues));
            }

            return ToArray(elements, elementType);
        }

        // 不要使用elements.ToArray()
        // System.Array 与 object[] 不是相同的类型。
        // 而反射机制能识别前者，但不能识别后者
        private static object ToArray(List<object> elements, Type elementType)
        {
            var array = Array.CreateInstance(elementType, elements.Count);
            for (var i = 0; i < array.Length; i++)
            {
                array.SetValue(elements[i], i);
            }

            return array;
        }

        private static object GetObjectValue(string name, Type type, Dictionary<string, string> paramValues)
        {
            if (!ValueProvided(name, paramValues) || !HasDefaultConstructor(type))
            {
                return null;
            }

            var dynamicObj = new DynamicObject(type);
            dynamicObj.CallConstructor();

            foreach (var propInfo in type.GetProperties())
            {
                var propName = name + "." + propInfo.Name;
                var propValue = CreateInstance(propInfo.PropertyType, propName, paramValues);

                if (propValue != null)
                {
                    dynamicObj.SetProperty(propInfo.Name, propValue);
                }
            }

            return dynamicObj.ObjectInstance;
        }

        private static bool ValueProvided(string paramName, Dictionary<string, string> paramValues)
        {
            return paramValues.Keys.Any(key => key.StartsWith(paramName));
        }

        private static bool HasDefaultConstructor(Type type)
        {
            return type.GetConstructors().Any(constructor => constructor.GetParameters().Length == 0);
        }

        private static object GetDefaultValue(Type type)
        {
            return type == typeof(string) ? null : Convert.ChangeType(0, type);
        }
    }
}
