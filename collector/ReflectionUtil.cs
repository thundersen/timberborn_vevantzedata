using System.Reflection;

namespace VeVantZeData.Collector
{
    static class ReflectionUtil
    {
        internal static TF GetInstanceField<T, TF>(this T instance, string fieldName)
        {
            var bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            return (TF)(typeof(T).GetField(fieldName, bindFlags).GetValue(instance));
        }
    }
}