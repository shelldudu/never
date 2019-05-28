using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Never.Reflection
{
    /// <summary>
    /// 定义Emit操作接口
    /// </summary>
    public abstract partial class EmitBuilder : IEmitBuilder
    {
        #region field

        /// <summary>
        /// 方法调用协定
        /// </summary>
        private readonly CallingConventions callConvention;

        /// <summary>
        /// 方法的返回类型
        /// </summary>
        private readonly Type returnType = null;

        /// <summary>
        /// 方法的参数
        /// </summary>
        private readonly Type[] parameterTypes = null;

        /// <summary>
        /// IL指令
        /// </summary>
        private readonly ILGenerator il = null;

        /// <summary>
        /// 当前Local临时标量的下标
        /// </summary>
        private int localCount = 0;

        /// <summary>
        /// 内存模块
        /// </summary>
        private static readonly ModuleBuilder moduleBuilder = null;

        /// <summary>
        /// 动态程序集
        /// </summary>
        private static readonly AssemblyBuilder assemblyBuilder = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="EmitBuilder"/> class.
        /// </summary>
        static EmitBuilder()
        {
            var asemblyName = new AssemblyName("Never.DynamicAssembly");
            asemblyName.Version = new Version("1.0.0.0");
            asemblyName.CultureInfo = new System.Globalization.CultureInfo("zh-cn");

#if NET461
            assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(asemblyName, AssemblyBuilderAccess.Run);
#else
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(asemblyName, AssemblyBuilderAccess.Run);
#endif
            moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmitBuilder"/> class.
        /// </summary>
        /// <param name="il">il</param>
        /// <param name="callConvention"></param>
        /// <param name="returnType"></param>
        /// <param name="parameterTypes"></param>
        protected EmitBuilder(ILGenerator il, CallingConventions callConvention, Type returnType, Type[] parameterTypes)
        {
            this.il = il;
            this.callConvention = callConvention;
            this.returnType = returnType;
            this.parameterTypes = parameterTypes;
        }

        #endregion ctor

        #region prop

        /// <summary>
        /// 内存模块
        /// </summary>
        public static ModuleBuilder ModuleBuilder
        {
            get
            {
                return moduleBuilder;
            }
        }

        #endregion prop

        #region IEmit

        /// <summary>
        /// IL指令
        /// </summary>
        public ILGenerator IL
        {
            get
            {
                return this.il;
            }
        }

        /// <summary>
        /// 约定方法的调用
        /// </summary>
        public CallingConventions CallingConventions
        {
            get { return this.callConvention; }
        }

        /// <summary>
        /// 方法参数的类型
        /// </summary>
        public Type[] ParameterTypes
        {
            get { return this.parameterTypes; }
        }

        /// <summary>
        /// 返回的类型
        /// </summary>
        public Type ReturnType
        {
            get { return this.returnType; }
        }

        /// <summary>
        /// 声明一个新临时变量
        /// </summary>
        /// <param name="localType"></param>
        /// <returns></returns>
        public ILocal DeclareLocal(Type localType)
        {
            var local = new MyLocal();
            local.Index = localCount;
            local.LocalType = localType;
            local.LocalBuilder = this.il.DeclareLocal(localType);
            local.Owner = this;
            System.Threading.Interlocked.Increment(ref localCount);
            return local;
        }

        /// <summary>
        /// 声明一个新标签
        /// </summary>
        /// <returns></returns>
        public ILabel DefineLabel()
        {
            var label = new MyLabel();
            label.Label = this.il.DefineLabel();
            label.Owner = this;
            return label;
        }

        #endregion IEmit
    }
}