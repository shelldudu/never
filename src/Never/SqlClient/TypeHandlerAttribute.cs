using System;
using System.Reflection;
using Never.Reflection;

namespace Never.SqlClient
{
    /// <summary>
    /// 类型处理者
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class TypeHandlerAttribute : Attribute
    {
        /// <summary>
        /// 类型处理者
        /// </summary>
        public Type TypeHandler { get; set; }

        /// <summary>
        /// 转换参数的回调
        /// </summary>
        private readonly Func<TypeHandlerAttribute, object, object> toParameter = null;

        /// <summary>
        /// 转换对象的回调
        /// </summary>
        private readonly Func<TypeHandlerAttribute, System.Data.IDataRecord, string, object> toValue = null;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="typeHandler"></param>
        public TypeHandlerAttribute(Type typeHandler)
        {
            if (!typeHandler.IsAssignableFromType(typeof(ITypeHandler)))
                throw new ArgumentException("ITypeHandler is not AssignableFrom typehanlder");

            this.TypeHandler = typeHandler;

            //toVlaue
            if (this.TypeHandler.IsAssignableFromType(typeof(IReadingFromDataRecordToValueTypeHandler<>)))
            {
                var types = this.TypeHandler.MatchTargetType(typeof(IReadingFromDataRecordToValueTypeHandler<>));
                if (types.IsNullOrEmpty())
                    return;

                foreach (var type in types)
                {
                    if (type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IReadingFromDataRecordToValueTypeHandler<>))
                    {
                        var genraiceArgs = type.GetGenericArguments();
                        if (genraiceArgs != null && genraiceArgs.Length > 0)
                        {
                            var emit = EasyEmitBuilder<Func<TypeHandlerAttribute, System.Data.IDataRecord, string, object>>.NewDynamicMethod();
                            emit.LoadArgument(0);
                            emit.LoadArgument(1);
                            emit.LoadArgument(2);
                            emit.Call(typeof(TypeHandlerAttribute).GetMethod("OnDataReadingUsingEmit", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(genraiceArgs));
                            emit.Return();
                            toValue = emit.CreateDelegate();
                            break;
                        }
                    }
                }
            }

            //toParameter
            if (this.TypeHandler.IsAssignableFromType(typeof(ICastingValueToParameterTypeHandler<>)))
            {
                var types = this.TypeHandler.MatchTargetType(typeof(ICastingValueToParameterTypeHandler<>));
                if (types.IsNullOrEmpty())
                    return;

                foreach (var type in types)
                {
                    if (type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICastingValueToParameterTypeHandler<>))
                    {
                        var genraiceArgs = type.GetGenericArguments();
                        if (genraiceArgs != null && genraiceArgs.Length > 0)
                        {
                            var emit = EasyEmitBuilder<Func<TypeHandlerAttribute, object, object>>.NewDynamicMethod();
                            emit.LoadArgument(0);
                            emit.LoadArgument(1);
                            emit.Call(typeof(TypeHandlerAttribute).GetMethod("OnInitingParameterUsingEmit", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(genraiceArgs));
                            emit.Return();
                            toParameter = emit.CreateDelegate();
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 在读取的时候
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRecord"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public virtual T OnDataReading<T>(System.Data.IDataRecord dataRecord, string columnName)
        {
            var handler = Activator.CreateInstance(this.TypeHandler) as IReadingFromDataRecordToValueTypeHandler<T>;
            if (handler == null)
                return default(T);

            var ordinal = dataRecord.GetOrdinal(columnName);
            return handler.ToValue(dataRecord, ordinal, columnName);
        }

        /// <summary>
        /// 在准备参数的时候
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual T OnInitingParameter<T>(object value)
        {
            var handler = Activator.CreateInstance(this.TypeHandler) as ICastingValueToParameterTypeHandler<T>;
            if (handler == null)
                return default(T);

            return handler.ToParameter(value);
        }

        /// <summary>
        /// 返回回调
        /// </summary>
        /// <returns></returns>
        public Func<TypeHandlerAttribute, System.Data.IDataRecord, string, object> GetOnDataReadingCallBack()
        {
            return toValue;
        }

        /// <summary>
        /// 返回回调
        /// </summary>
        /// <returns></returns>
        public Func<TypeHandlerAttribute, object, object> GetOnInitingParameterCallBack()
        {
            return toParameter;
        }

        /// <summary>
        /// emit调用的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attribute"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static T OnInitingParameterUsingEmit<T>(TypeHandlerAttribute attribute, object value)
        {
            return attribute.OnInitingParameter<T>(value);
        }

        /// <summary>
        /// emit调用的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attribute"></param>
        /// <param name="dataRecord"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private static T OnDataReadingUsingEmit<T>(TypeHandlerAttribute attribute, System.Data.IDataRecord dataRecord, string columnName)
        {
            return attribute.OnDataReading<T>(dataRecord, columnName);
        }
    }
}