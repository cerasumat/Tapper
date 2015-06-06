using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tapper.Test
{
    internal static class TypeUtil
    {
        internal static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        internal static bool IsSimple(this Type type)
        {
            return type.IsPrimitive || type == typeof(string) || type == typeof(DateTime);
        }

        /// <summary>
        /// 判断是否为可空类型。如果是，转换为基本类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static Type UnderlyingType(this Type type)
        {
            return type.IsNullable() ? Nullable.GetUnderlyingType(type) : type;
        }
    }
}
