using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Panuon.WPF.Charts
{
    internal class PropertyAccessor
    {
        private static readonly Dictionary<string, Delegate> PropertyGetters = new Dictionary<string, Delegate>();
        private static readonly Dictionary<string, Delegate> PropertySetters = new Dictionary<string, Delegate>();
        private static readonly object SyncLock = new object();

        public static object GetValue(object obj, string propertyName)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException(nameof(propertyName));

            if (obj is DataRow dataRow)
            {
                return dataRow.IsNull(propertyName) ? null : dataRow[propertyName];
            }

            Type objectType = obj.GetType();
            string key = $"{objectType.FullName}.{propertyName}.ObjectGetter";

            if (!PropertyGetters.TryGetValue(key, out Delegate getter))
            {
                lock (SyncLock)
                {
                    if (!PropertyGetters.TryGetValue(key, out getter))
                    {
                        PropertyInfo propertyInfo = objectType.GetProperty(propertyName);
                        if (propertyInfo == null)
                        {
                            throw new ArgumentException($"Property '{propertyName}' does not exist in type '{objectType.FullName}'");
                        }

                        ParameterExpression parameter = Expression.Parameter(typeof(object), "obj");
                        UnaryExpression convertedParameter = Expression.Convert(parameter, objectType);
                        Expression property = Expression.Property(convertedParameter, propertyInfo);
                        Expression boxedProperty = propertyInfo.PropertyType.IsValueType
                            ? Expression.Convert(property, typeof(object))
                            : property;
                        var lambda = Expression.Lambda<Func<object, object>>(boxedProperty, parameter);
                        getter = lambda.Compile();
                        PropertyGetters[key] = getter;
                    }
                }
            }

            return ((Func<object, object>)getter)(obj);
        }
    }
}