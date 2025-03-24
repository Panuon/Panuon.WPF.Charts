using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Panuon.WPF.Charts
{
    internal class PropertyAccessor
    {
        // 缓存属性的Getter委托
        private static readonly Dictionary<string, Delegate> PropertyGetters = new Dictionary<string, Delegate>();

        // 缓存属性的Setter委托
        private static readonly Dictionary<string, Delegate> PropertySetters = new Dictionary<string, Delegate>();

        // 用于线程同步的锁对象
        private static readonly object SyncLock = new object();

        /// <summary>
        /// 获取指定类型的指定属性的值
        /// </summary>
        /// <typeparam name="TObject">对象类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="obj">要获取属性值的对象</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>属性值</returns>
        public static TProperty GetValue<TObject, TProperty>(TObject obj, string propertyName)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException(nameof(propertyName));

            // 创建缓存键
            string key = $"{typeof(TObject).FullName}.{propertyName}.Getter";

            // 尝试从缓存获取
            if (!PropertyGetters.TryGetValue(key, out Delegate getter))
            {
                lock (SyncLock)
                {
                    if (!PropertyGetters.TryGetValue(key, out getter))
                    {
                        // 获取属性信息
                        PropertyInfo propertyInfo = typeof(TObject).GetProperty(propertyName);
                        if (propertyInfo == null)
                        {
                            throw new ArgumentException($"Property '{propertyName}' does not exist in type '{typeof(TObject).FullName}'");
                        }

                        // 创建参数表达式
                        ParameterExpression parameter = Expression.Parameter(typeof(TObject), "obj");

                        // 创建属性表达式
                        Expression property = Expression.Property(parameter, propertyInfo);

                        // 创建lambda表达式
                        var lambda = Expression.Lambda<Func<TObject, TProperty>>(property, parameter);

                        // 编译表达式
                        getter = lambda.Compile();

                        // 添加到缓存
                        PropertyGetters[key] = getter;
                    }
                }
            }

            // 调用getter委托
            return ((Func<TObject, TProperty>)getter)(obj);
        }

        /// <summary>
        /// 设置指定类型的指定属性的值
        /// </summary>
        /// <typeparam name="TObject">对象类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="obj">要设置属性值的对象</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">要设置的值</param>
        public static void SetValue<TObject, TProperty>(TObject obj, string propertyName, TProperty value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException(nameof(propertyName));

            // 创建缓存键
            string key = $"{typeof(TObject).FullName}.{propertyName}.Setter";

            // 尝试从缓存获取
            if (!PropertySetters.TryGetValue(key, out Delegate setter))
            {
                lock (SyncLock)
                {
                    if (!PropertySetters.TryGetValue(key, out setter))
                    {
                        // 获取属性信息
                        PropertyInfo propertyInfo = typeof(TObject).GetProperty(propertyName);
                        if (propertyInfo == null)
                        {
                            throw new ArgumentException($"Property '{propertyName}' does not exist in type '{typeof(TObject).FullName}'");
                        }

                        // 创建参数表达式
                        ParameterExpression objParameter = Expression.Parameter(typeof(TObject), "obj");
                        ParameterExpression valueParameter = Expression.Parameter(typeof(TProperty), "value");

                        // 创建属性表达式
                        MemberExpression property = Expression.Property(objParameter, propertyInfo);

                        // 创建赋值表达式
                        BinaryExpression assign = Expression.Assign(property, valueParameter);

                        // 创建lambda表达式
                        var lambda = Expression.Lambda<Action<TObject, TProperty>>(assign, objParameter, valueParameter);

                        // 编译表达式
                        setter = lambda.Compile();

                        // 添加到缓存
                        PropertySetters[key] = setter;
                    }
                }
            }

            // 调用setter委托
            ((Action<TObject, TProperty>)setter)(obj, value);
        }

        /// <summary>
        /// 获取指定对象的指定属性的值（非泛型版本）
        /// </summary>
        /// <param name="obj">要获取属性值的对象</param>
        /// <param name="propertyName">属性名称</param>
        /// <returns>属性值</returns>
        public static object GetValue(object obj, string propertyName)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException(nameof(propertyName));

            Type objectType = obj.GetType();

            // 创建缓存键
            string key = $"{objectType.FullName}.{propertyName}.ObjectGetter";

            // 尝试从缓存获取
            if (!PropertyGetters.TryGetValue(key, out Delegate getter))
            {
                lock (SyncLock)
                {
                    if (!PropertyGetters.TryGetValue(key, out getter))
                    {
                        // 获取属性信息
                        PropertyInfo propertyInfo = objectType.GetProperty(propertyName);
                        if (propertyInfo == null)
                        {
                            throw new ArgumentException($"Property '{propertyName}' does not exist in type '{objectType.FullName}'");
                        }

                        // 创建参数表达式
                        ParameterExpression parameter = Expression.Parameter(typeof(object), "obj");

                        // 创建类型转换表达式
                        UnaryExpression convertedParameter = Expression.Convert(parameter, objectType);

                        // 创建属性表达式
                        Expression property = Expression.Property(convertedParameter, propertyInfo);

                        // 如果属性是值类型，需要装箱
                        Expression boxedProperty = propertyInfo.PropertyType.IsValueType
                            ? Expression.Convert(property, typeof(object))
                            : property;

                        // 创建lambda表达式
                        var lambda = Expression.Lambda<Func<object, object>>(boxedProperty, parameter);

                        // 编译表达式
                        getter = lambda.Compile();

                        // 添加到缓存
                        PropertyGetters[key] = getter;
                    }
                }
            }

            // 调用getter委托
            return ((Func<object, object>)getter)(obj);
        }

        /// <summary>
        /// 设置指定对象的指定属性的值（非泛型版本）
        /// </summary>
        /// <param name="obj">要设置属性值的对象</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="value">要设置的值</param>
        public static void SetValue(object obj, string propertyName, object value)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            if (string.IsNullOrEmpty(propertyName)) throw new ArgumentNullException(nameof(propertyName));

            Type objectType = obj.GetType();

            // 创建缓存键
            string key = $"{objectType.FullName}.{propertyName}.ObjectSetter";

            // 尝试从缓存获取
            if (!PropertySetters.TryGetValue(key, out Delegate setter))
            {
                lock (SyncLock)
                {
                    if (!PropertySetters.TryGetValue(key, out setter))
                    {
                        // 获取属性信息
                        PropertyInfo propertyInfo = objectType.GetProperty(propertyName);
                        if (propertyInfo == null)
                        {
                            throw new ArgumentException($"Property '{propertyName}' does not exist in type '{objectType.FullName}'");
                        }

                        // 创建参数表达式
                        ParameterExpression objParameter = Expression.Parameter(typeof(object), "obj");
                        ParameterExpression valueParameter = Expression.Parameter(typeof(object), "value");

                        // 创建类型转换表达式
                        UnaryExpression convertedObj = Expression.Convert(objParameter, objectType);
                        UnaryExpression convertedValue = Expression.Convert(valueParameter, propertyInfo.PropertyType);

                        // 创建属性表达式
                        MemberExpression property = Expression.Property(convertedObj, propertyInfo);

                        // 创建赋值表达式
                        BinaryExpression assign = Expression.Assign(property, convertedValue);

                        // 创建lambda表达式
                        var lambda = Expression.Lambda<Action<object, object>>(assign, objParameter, valueParameter);

                        // 编译表达式
                        setter = lambda.Compile();

                        // 添加到缓存
                        PropertySetters[key] = setter;
                    }
                }
            }

            // 调用setter委托
            ((Action<object, object>)setter)(obj, value);
        }
    }
}
