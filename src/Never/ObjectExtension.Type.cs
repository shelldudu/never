using Never.Exceptions;
using Never.Reflection;
using Never.Reflection.TypeConverters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Never
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static partial class ObjectExtension
    {
        #region attributes

        /// <summary>
        /// 获取在所有Attributes中某一Attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public static T GetAttribute<T>(this IEnumerable<Attribute> attributes) where T : Attribute
        {
            if (attributes == null)
                return default(T);

            foreach (var attr in attributes)
            {
                var attribute = attr as T;
                if (attribute != null)
                    return attribute;
            }

            return default(T);
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public static T[] GetAttributes<T>(this IEnumerable<Attribute> attributes) where T : Attribute
        {
            if (attributes == null)
                return new T[] { };

            var list = new List<T>();
            foreach (var attr in attributes)
            {
                var attribute = attr as T;
                if (attribute != null)
                    list.Add(attribute);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo">The member information.</param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            return GetAttribute<T>(GetAttributes<T>(memberInfo, inherit));
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldInfo">The field information.</param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this FieldInfo fieldInfo, bool inherit = false) where T : Attribute
        {
            return GetAttribute<T>(GetAttributes<T>(fieldInfo, inherit));
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo">The member information.</param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T[] GetAttributes<T>(this MemberInfo memberInfo, bool inherit = false) where T : Attribute
        {
            if (memberInfo == null)
                return new T[] { };

            var attrs = memberInfo.GetCustomAttributes(typeof(T), inherit) as IEnumerable<Attribute>;
            return GetAttributes<T>(attrs);
        }

        #endregion attributes

        #region to

        /// <summary>
        /// 将某一对象转换为T对象
        /// </summary>
        /// <typeparam name="T">目标对象的类型</typeparam>
        /// <param name="value">要转换的源对象</param>
        /// <returns></returns>
        public static T To<T>(this object value)
        {
            return To<T>(value, default(T));
        }

        /// <summary>
        /// 将某一对象转换为T对象
        /// </summary>
        /// <typeparam name="T">目标对象的类型</typeparam>
        /// <param name="value">要转换的源对象</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static T To<T>(this object value, T defaultValue)
        {
            return (T)To(value, typeof(T), defaultValue);
        }

        /// <summary>
        /// 将某一对象转换为T对象
        /// </summary>
        /// <param name="value">要转换的源对象</param>
        /// <param name="destinationType">目标对象的类型</param>
        /// <returns></returns>
        public static object To(this object value, Type destinationType)
        {
            return To(value, destinationType, value);
        }

        /// <summary>
        /// 将某一对象转换为T对象
        /// </summary>
        /// <param name="value">要转换的源对象</param>
        /// <param name="destinationType">目标对象的类型</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static object To(this object value, Type destinationType, object defaultValue)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture, defaultValue);
        }

        /// <summary>
        /// 将某一对象转换为T对象
        /// </summary>
        /// <param name="value">要转换的源对象</param>
        /// <param name="destinationType">目标对象的类型</param>
        /// <param name="culture">转换的文化区域信息</param>
        /// <returns></returns>
        public static object To(this object value, Type destinationType, CultureInfo culture)
        {
            return To(value, destinationType, culture, value);
        }

        /// <summary>
        /// 将某一对象转换为T对象
        /// </summary>
        /// <param name="value">要转换的源对象</param>
        /// <param name="destinationType">目标对象的类型</param>
        /// <param name="culture">转换的文化区域信息</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static object To(this object value, Type destinationType, CultureInfo culture, object defaultValue)
        {
            if (value == null)
                return defaultValue;

            var sourceType = value.GetType();
            if (sourceType == destinationType)
                return value;

            if (destinationType.IsEnum)
                return Enum.Parse(destinationType, value.ToString());

            var destinationConverter = GetConverter(destinationType);
            if (destinationConverter != null && destinationConverter.CanConvertFrom(sourceType))
                return destinationConverter.ConvertFrom(null, culture, value);

            var sourceConverter = GetConverter(sourceType);
            if (sourceConverter != null && sourceConverter.CanConvertTo(destinationType))
                return sourceConverter.ConvertTo(null, culture, value, destinationType);

            if (!destinationType.IsAssignableFrom(sourceType))
                return Convert.ChangeType(value, destinationType, culture);

            return defaultValue;
        }

        #endregion to

        #region generic type

        /// <summary>
        /// 获取类型转换器
        /// </summary>
        /// <param name="seqType">源类型</param>
        /// <returns></returns>
        public static TypeConverter GetConverter(this Type seqType)
        {
            if (!arrayGenericType.IsAssignableFrom(seqType))
                return TypeDescriptor.GetConverter(seqType);

            if (intGenericType.IsAssignableFrom(seqType))
                return new ArrayStringTypeConverter<int>();

            if (decimalGenericType.IsAssignableFrom(seqType))
                return new ArrayStringTypeConverter<decimal>();

            if (shortGenericType.IsAssignableFrom(seqType))
                return new ArrayStringTypeConverter<short>();

            if (byteGenericType.IsAssignableFrom(seqType))
                return new ArrayStringTypeConverter<byte>();

            if (longGenericType.IsAssignableFrom(seqType))
                return new ArrayStringTypeConverter<long>();

            if (floatGenericType.IsAssignableFrom(seqType))
                return new ArrayStringTypeConverter<float>();

            if (doubleGenericType.IsAssignableFrom(seqType))
                return new ArrayStringTypeConverter<double>();

            if (boolGenericType.IsAssignableFrom(seqType))
                return new ArrayStringTypeConverter<bool>();

            if (dateTimeGenericType.IsAssignableFrom(seqType))
                return new ArrayStringTypeConverter<DateTime>();

            if (charGenericType.IsAssignableFrom(seqType))
                return new ArrayStringTypeConverter<char>();

            return new ArrayStringTypeConverter<string>();
        }

        #endregion generic type

        #region findType

        /// <summary>
        /// 确定implementationType类型的实例是否可以分配给serviceType类型的实例
        /// </summary>
        /// <param name="serviceType">接口类型</param>
        /// <param name="implementationType">实例类型</param>
        /// <returns></returns>
        public static bool IsAssignableToType(this Type serviceType, Type implementationType)
        {
            if (implementationType == null || serviceType == null)
                return false;

            if (serviceType.IsAssignableFrom(implementationType))
                return true;

            var interfaceTypes = implementationType.GetInterfaces();
            if (interfaceTypes == null)
                return false;

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == serviceType)
                    return true;
            }

            if (implementationType.IsGenericType && implementationType.GetGenericTypeDefinition() == serviceType)
                return true;

            Type baseType = implementationType.BaseType;
            if (baseType == null)
                return false;

            return IsAssignableToType(serviceType, baseType);
        }

        /// <summary>
        /// 确定serviceType类型的实例是否可以分配给baseType类型的实例
        /// </summary>
        /// <param name="baseType">基类型</param>
        /// <param name="type">实例类型</param>
        /// <returns></returns>
        public static bool IsAssignableFromType(this Type type, Type baseType)
        {
            if (type == null || baseType == null)
                return false;

            return IsAssignableToType(baseType, type);
        }

        /// <summary>
        /// 匹配目标类型
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static IEnumerable<Type> MatchTargetType(this Type serviceType, Type target)
        {
            if (serviceType == null || target == null || serviceType == typeof(object))
                yield break;

            if (serviceType == target)
                yield return serviceType;

            var interfaces = serviceType.GetInterfaces();
            if (!target.IsGenericType)
            {
                foreach (var @interface in interfaces)
                {
                    if (@interface == target)
                        yield return @interface;

                    foreach (var s in MatchTargetType(@interface, target))
                        yield return s;
                }
            }
            else
            {
                foreach (var @interface in interfaces)
                {
                    if (@interface == target)
                        yield return @interface;

                    if (@interface.IsGenericType)
                    {
                        var g = @interface.GetGenericTypeDefinition();
                        if (g == target)
                            yield return @interface;
                    }

                    foreach (var s in MatchTargetType(@interface, target))
                        yield return s;
                }
            }
        }

        #endregion findType

        #region setprop

        /// <summary>
        /// 通过反射设置对象的某个属性为指定的值
        /// </summary>
        /// <param name="instance">指定对象</param>
        /// <param name="propertyName">指定对象的属性名字</param>
        /// <param name="value">新的指定值</param>
        public static void SetProperty(this object instance, string propertyName, object value)
        {
            if (instance == null)
                return;

            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName为空");

            var instanceType = instance.GetType();
            var properyInfo = instanceType.GetProperty(propertyName);
            if (properyInfo == null)
                throw new ArgumentException(string.Format("在对象{0}中没有找到{1}的属性", instanceType, propertyName));

            if (!properyInfo.CanWrite)
                throw new InvalidCastException(string.Format("对象中{0}找到{1}属性不可写", instanceType, propertyName));

            if (value != null && !value.GetType().IsAssignableFrom(properyInfo.PropertyType))
                value = To(value, properyInfo.PropertyType);

            properyInfo.SetValue(instance, value, null);
        }

        /// <summary>
        /// 通过反射设置对象的某个属性为指定的值
        /// </summary>
        /// <typeparam name="T">指定对象的类型</typeparam>
        /// <typeparam name="TProperty">某个属性的具体值类型</typeparam>
        /// <param name="instance">指定对象</param>
        /// <param name="propertySelector">指定对象的属性名字</param>
        /// <param name="value">新的指定值</param>
        public static void SetProperty<T, TProperty>(this T instance, Expression<Func<T, TProperty>> propertySelector, TProperty @value)
        {
            if (propertySelector == null)
            {
                throw new ParameterNullException("propertySelector", "要保存配置对象的属性propertySelector不可为空");
            }

            var member = propertySelector.Body as MemberExpression;
            if (member == null)
            {
                throw new ParameterNullException("propertySelector", string.Format("要保存配置对象的属性{0}必须是一个表达式", propertySelector));
            }

            var propertyInfo = member.Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ParameterNullException("propertySelector", string.Format("要保存配置对象的属性{0}必须是属于对象某一个属性", propertySelector));
            }

            if (!propertyInfo.CanWrite)
                throw new InvalidCastException(string.Format("对象中{0}找到{1}属性不可写", instance, propertyInfo.Name));

            var param_obj = Expression.Parameter(typeof(T), "obj");
            var param_val = Expression.Parameter(typeof(TProperty), "val");
            /*转换参数为真实类型*/
            var body_obj = Expression.Convert(param_obj, typeof(T));
            var body_val = Expression.Convert(param_val, typeof(TProperty));

            var setMethod = propertyInfo.GetSetMethod();
            if (setMethod == null)
            {
                setMethod = propertyInfo.GetAccessors(true).FirstOrDefault(o => o.Name == string.Concat("set_", propertyInfo.Name));
            }

            var body = Expression.Call(body_obj, setMethod, body_val);
            var method = Expression.Lambda<Action<T, TProperty>>(body, param_obj, param_val).Compile();

            method(instance, value);
        }

        /// <summary>
        /// 实现属性set方法
        /// </summary>
        /// <param name="prop">属性</param>
        /// <returns></returns>
        public static Action<object, object> BuildSetMethod(this PropertyInfo prop)
        {
            if (prop == null)
                return null;

            if (!prop.CanWrite)
                return null;

            var emit = EasyEmitBuilder<Action<object, object>>.NewDynamicMethod();
            var setMethod = prop.GetSetMethod(true);

            /*静态方法可以不用加载this*/
            if (!setMethod.IsStatic)
                emit.LoadArgument(0);

            emit.LoadArgument(1);

            if (prop.PropertyType.IsValueType)
                emit.UnboxAny(prop.PropertyType);
            else
                emit.CastClass(prop.PropertyType);

            if (setMethod.IsVirtual)
                emit.CallVirtual(setMethod);
            else
                emit.Call(setMethod);

            emit.Return();
            return emit.CreateDelegate();
        }

        /// <summary>
        /// 实现属性set方法
        /// </summary>
        /// <param name="field">字段</param>
        /// <returns></returns>
        public static Action<object, object> BuildSetMethod(this FieldInfo field)
        {
            if (field == null)
                return null;

            if (!field.IsInitOnly)
                return null;

            var emit = EasyEmitBuilder<Action<object, object>>.NewDynamicMethod();
            /*静态方法可以不用加载this*/
            if (!field.IsStatic)
                emit.LoadArgument(0);

            emit.LoadArgument(1);
            if (field.FieldType.IsValueType)
                emit.UnboxAny(field.FieldType);
            else
                emit.CastClass(field.FieldType);
            emit.StoreField(field);
            emit.Return();
            return emit.CreateDelegate();
        }

        #endregion setprop

        #region emun

        /// <summary>
        /// 成员中是否包含了Enum对象
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static bool MemberContainEnum(this Type type)
        {
            if (type == null)
                return false;

            if (type.IsEnum)
                return true;

            var properties = type.GetProperties();
            if (properties != null && properties.Length > 0)
            {
                for (var i = 0; i < properties.Length; i++)
                {
                    var prop = properties[i];
                    if (prop.PropertyType.IsEnum)
                        return true;

                    if (prop.PropertyType.IsClass && prop.PropertyType != stringType)
                    {
                        if (MemberContainEnum(prop.PropertyType))
                            return true;
                    }
                }
            }

            var fields = type.GetFields();
            if (fields != null && fields.Length > 0)
            {
                for (var i = 0; i < fields.Length; i++)
                {
                    var field = fields[i];
                    if (field.FieldType.IsEnum)
                        return true;

                    if (field.FieldType.IsClass && field.FieldType != stringType)
                    {
                        if (MemberContainEnum(field.FieldType))
                            return true;
                    }
                }
            }

            return false;
        }

        #endregion emun

        #region method

        /// <summary>
        /// 快速实现方法
        /// </summary>
        /// <param name="method">方法</param>
        /// <returns></returns>
        public static Func<object, object[], object> GetMethodInvoker(this MethodInfo method)
        {
            var emit = EasyEmitBuilder<Func<object, object[], object>>.NewDynamicMethod();
            var parameters = method.GetParameters();
            var parameterTypes = new List<Type>(parameters.Length);
            foreach (var parameter in parameters)
            {
                parameterTypes.Add(parameter.ParameterType.IsByRef ? parameter.ParameterType.GetElementType() : parameter.ParameterType);
            }

            var locals = new List<ILocal>(emit.ParameterTypes.Length);
            for (var i = 0; i < parameterTypes.Count; i++)
            {
                var local = emit.DeclareLocal(parameterTypes[i]);
                emit.LoadArgument(1);
                emit.LoadConstant(i);
                emit.LoadElementReference();
                if (parameterTypes[i].IsValueType)
                    emit.UnboxAny(parameterTypes[i]);
                else
                    emit.CastClass(parameterTypes[i]);

                emit.StoreLocal(local);
                locals.Add(local);
            }

            if (!method.IsStatic)
                emit.LoadArgument(0);

            for (var i = 0; i < parameterTypes.Count; i++)
            {
                if (parameterTypes[i].IsByRef)
                    emit.LoadLocalAddress(locals[i]);
                else
                    emit.LoadLocal(locals[i]);
            }

            if (method.IsVirtual)
                emit.CallVirtual(method);
            else
                emit.Call(method);

            if (method.ReturnType == typeof(void))
                emit.LoadNull();
            else if (method.ReturnType.IsValueType)
                emit.Box(method.ReturnType);

            emit.Return();
            return emit.CreateDelegate();
        }

        #endregion method
    }
}