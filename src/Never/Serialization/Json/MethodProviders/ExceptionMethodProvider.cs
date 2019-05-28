using Never.Reflection;
using Never.Serialization.Json.Deserialize;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Never.Serialization.Json.MethodProviders
{
    /// <summary>
    /// 异常与流内容操作的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExceptionMethodProvider<T> : ConvertMethodProvider<T>, IConvertMethodProvider<T> where T : Exception
    {
        #region singleton

        /// <summary>
        /// The singleton
        /// </summary>
        private static ExceptionMethodProvider<T> singleton = new ExceptionMethodProvider<T>();

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        /// <value>The singleton.</value>
        public static ExceptionMethodProvider<T> Default
        {
            get
            {
                return singleton;
            }
        }

        #endregion singleton

        #region ctor

        /// <summary>
        /// Initializes static members of the <see cref="EnumMethodProvider{T}"/> class.
        /// </summary>
        static ExceptionMethodProvider()
        {
        }

        #endregion ctor

        #region IMethodProvider

        /// <summary>
        /// 从对象中写入流中
        /// </summary>
        /// <param name="writer">写入流</param>
        /// <param name="setting">配置项</param>
        /// <param name="source">源数据</param>
        public override void Write(ISerializerWriter writer, JsonSerializeSetting setting, T source)
        {
            ExceptionSerialierBuilder<T>.Register(setting).Invoke(writer, setting, source, 0);
        }

        /// <summary>
        /// 在流中读取字节后转换为对象
        /// </summary>
        /// <param name="setting">配置项</param>
        /// <param name="reader">字节流内容读取器</param>
        /// <param name="name">名字</param>
        /// <returns>返回目标对象</returns>
        public T Parse(IDeserializerReader reader, JsonDeserializeSetting setting, string name)
        {
            var node = reader.Read(name);
            if (node == null)
            {
                if (reader.Count == 0)
                    return default(T);

                return ExceptionDeseralizerBuilder<T>.Register(setting).Invoke(reader, setting, 0);
            }

            if (node.NodeType != ContentNodeType.Object)
                return default(T);

            return ExceptionDeseralizerBuilder<T>.Register(setting).Invoke(reader.Parse(node), setting, 0);
        }

        /// <summary>
        /// 在流中读取字节后转换为对象
        /// </summary>
        /// <param name="setting">配置项</param>
        /// <param name="reader">字符读取器</param>
        /// <param name="node">节点流内容</param>
        /// <param name="checkNullValue">是否检查空值</param>
        /// <returns></returns>
        public override T Parse(IDeserializerReader reader, JsonDeserializeSetting setting, IContentNode node, bool checkNullValue)
        {
            return default(T);
        }

        #endregion IMethodProvider

        #region builder

        /// <summary>
        /// 异常型处理
        /// </summary>
        private sealed class ExceptionSerialierBuilder<TException> : Serialize.SerialierBuilder<TException>, ISerialierBuilder<TException> where TException : Exception
        {
            #region field

            /// <summary>
            ///
            /// </summary>
            private readonly static object locker = new object();

            /// <summary>
            ///
            /// </summary>
            private static Action<ISerializerWriter, JsonSerializeSetting, TException, byte> action = null;

            #endregion field

            #region ctor

            /// <summary>
            /// Initializes a new instance of the <see cref="ExceptionSerialierBuilder{TException}"/> class.
            /// </summary>
            public ExceptionSerialierBuilder()
                : base(new List<string>() { "TargetSite" }, StringComparison.OrdinalIgnoreCase)
            {
            }

            #endregion ctor

            #region static build

            /// <summary>
            /// 进行构建
            /// </summary>
            /// <returns></returns>
            public static Action<ISerializerWriter, JsonSerializeSetting, TException, byte> Register(JsonSerializeSetting setting)
            {
                if (action != null)
                    return action;

                lock (locker)
                {
                    if (action != null)
                        return action;

                    var customSerialierBuilder = CustomSerializationProvider.QueryCustomeSerilizerbuilder<TException>();
                    if (customSerialierBuilder != null)
                        return action = customSerialierBuilder.Build(setting);

                    return action = new ExceptionSerialierBuilder<TException>().Build(setting);
                }
            }

            #endregion static build

            #region ISerialierBuilder

            /// <summary>
            /// 进行构建
            /// </summary>
            /// <returns></returns>
            public Action<ISerializerWriter, JsonSerializeSetting, TException, byte> Build(JsonSerializeSetting setting)
            {
                var emit = EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, TException, byte>>.NewDynamicMethod();
                this.Build(emit, setting);
                emit.Return();
                return emit.CreateDelegate();
            }

            #endregion ISerialierBuilder

            #region build

            /// <summary>
            /// 构建异常模块
            /// </summary>
            /// <param name="emit">emit构建</param>
            /// <param name="setting">配置</param>
            /// <param name="instanceLocal">当前对象变量</param>
            /// <param name="sourceType">节点成员</param>
            /// <param name="member">成员</param>
            /// <param name="memberType">成员类型</param>
            /// <param name="attributes">特性</param>
            /// <returns></returns>
            protected override bool BuildForExceptionModule(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, TException, byte>> emit, JsonSerializeSetting setting, ILocal instanceLocal, Type sourceType, MemberInfo member, Type memberType, Attribute[] attributes)
            {
                if (member.Name != "InnerException" || !TypeHelper.IsAssignableFrom(memberType, typeof(Exception)))
                    return false;

                var nullValue = emit.DefineLabel();
                var relLabel = emit.DefineLabel();
                if (instanceLocal == null)
                {
                    if (sourceType.IsValueType)
                        emit.LoadArgumentAddress(2);
                    else
                        emit.LoadArgument(2);
                }
                else
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);                             // @object
                }
                if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                    emit.Call(((PropertyInfo)member).GetGetMethod());
                else
                    emit.LoadField(((FieldInfo)member));

                emit.LoadNull();
                emit.CompareGreaterThan();
                emit.BranchIfFalse(nullValue);

                this.WriteMemberName(emit, setting, member, attributes);
                this.WriteColon(emit, setting);
                this.WriteObjectFrontSigil(emit, setting);

                emit.LoadArgument(0);
                emit.LoadArgument(1);
                if (instanceLocal == null)
                {
                    if (sourceType.IsValueType)
                        emit.LoadArgumentAddress(2);
                    else
                        emit.LoadArgument(2);
                }
                else
                {
                    if (sourceType.IsValueType)
                        emit.LoadLocalAddress(instanceLocal);
                    else
                        emit.LoadLocal(instanceLocal);                             // @object
                }
                if (member.MemberType == MemberTypes.Property)             // @object.get_item()
                    emit.Call(((PropertyInfo)member).GetGetMethod(true));
                else
                    emit.LoadField(((FieldInfo)member));

                emit.Call(Serialize.SerialierBuilderHelper.GetExceptionParseMethod(memberType));
                this.WriteObjectLastSigil(emit, setting);
                emit.Branch(relLabel);

                emit.MarkLabel(nullValue);
                this.WriteMemberName(emit, setting, member, attributes);
                this.WriteColon(emit, setting);
                if (setting.WriteNullWhenObjectIsNull)
                    this.WriteNull(emit, setting);
                else
                    this.WriteObjectSigil(emit, setting);
                emit.Branch(relLabel);

                emit.MarkLabel(relLabel);
                emit.Nop();
                return true;
            }

            /// <summary>
            /// 构建异常模块
            /// </summary>
            /// <param name="emit">emit构建</param>
            /// <param name="setting">配置</param>
            /// <param name="sourceType">成员类型</param>
            /// <returns></returns>
            protected override bool BuildForExceptionModule(EasyEmitBuilder<Action<ISerializerWriter, JsonSerializeSetting, TException, byte>> emit, JsonSerializeSetting setting, Type sourceType)
            {
                return base.BuildForExceptionModule(emit, setting, sourceType);
            }

            #endregion build
        }

        /// <summary>
        /// 异常型处理
        /// </summary>
        private sealed class ExceptionDeseralizerBuilder<TException> : Deserialize.DeseralizerBuilder<TException>, IDeserialierBuilder<TException> where TException : Exception
        {
            #region field

            /// <summary>
            /// 委托
            /// </summary>
            private static Func<IDeserializerReader, JsonDeserializeSetting, int, T> function = null;

            /// <summary>
            ///
            /// </summary>
            private readonly static object locker = new object();

            #endregion field

            #region static build

            /// <summary>
            /// 创建委托
            /// </summary>
            /// <returns></returns>
            public static Func<IDeserializerReader, JsonDeserializeSetting, int, T> Register(JsonDeserializeSetting setting)
            {
                if (function != null)
                    return function;

                lock (locker)
                {
                    if (function != null)
                        return function;

                    var customSerialierBuilder = CustomSerializationProvider.QueryCustomeDeserilizerbuilder<T>();
                    if (customSerialierBuilder != null)
                        return function = customSerialierBuilder.Build(setting);

                    return function = new ExceptionDeseralizerBuilder<T>().Build(setting);
                }
            }

            #endregion static build

            #region ctor

            /// <summary>
            /// Initializes a new instance of the <see cref="ExceptionSerialierBuilder{TException}"/> class.
            /// </summary>
            public ExceptionDeseralizerBuilder()
                : base(new List<string>() { "InnerException", "TargetSite", "Data" }, StringComparison.OrdinalIgnoreCase)
            {
            }

            #endregion ctor

            #region build

            /// <summary>
            /// 进行构建
            /// </summary>
            public Func<IDeserializerReader, JsonDeserializeSetting, int, TException> Build(JsonDeserializeSetting setting)
            {
                var emit = EasyEmitBuilder<Func<IDeserializerReader, JsonDeserializeSetting, int, TException>>.NewDynamicMethod();
                this.Build(emit);
                emit.Return();
                return emit.CreateDelegate();
            }

            #endregion build
        }

        #endregion builder
    }
}