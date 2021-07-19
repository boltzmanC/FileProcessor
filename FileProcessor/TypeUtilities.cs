using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace FileProcessor
{
    public static class TypeUtilities
    {
        public static List<T> GetAllPublicConstantValues<T>(this Type type)
        {
            return type
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
            .Select(x => (T)x.GetRawConstantValue())
            .ToList();
        }

        public static List<T> GetAllPublicConstantNames<T>(this Type type)
        {
            return type
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
            .Select(x => (T)x.GetRawConstantValue())
            .ToList();
        }
    }
}
