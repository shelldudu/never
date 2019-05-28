using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Never.Reflection
{
    /// <summary>
    /// IEmit操作跟踪者
    /// </summary>
    /// <typeparam name="DelegateType">委托类型</typeparam>
    public sealed class TrackEmitBuilder<DelegateType>
    {
        #region prop

        /// <summary>
        /// Emit操作对象
        /// </summary>
        private readonly EasyEmitBuilder<DelegateType> emit = null;

        /// <summary>
        /// 日志字符串
        /// </summary>
        private readonly StringBuilder sb = null;

        /// <summary>
        /// 缓冲操作
        /// </summary>
        private readonly IList<KeyValuePair<OpCodeTrace, Action<EasyEmitBuilder<DelegateType>>>> buffers = null;

        /// <summary>
        /// 是否已经打印日志了
        /// </summary>
        private bool islogged = false;

        #endregion prop

        #region ctor

        private TrackEmitBuilder(EasyEmitBuilder<DelegateType> emit)
        {
            this.emit = emit;
            this.buffers = new List<KeyValuePair<OpCodeTrace, Action<EasyEmitBuilder<DelegateType>>>>(1000);
            this.sb = new StringBuilder(1000);
        }

        #endregion ctor

        #region itracker

        /// <summary>
        /// Emit操作对象
        /// </summary>
        /// <param name="trace"></param>
        /// <param name="buffer"></param>
        private void Append(OpCodeTrace trace, Action<EasyEmitBuilder<DelegateType>> buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer is null");

            this.buffers.Add(new KeyValuePair<OpCodeTrace, Action<EasyEmitBuilder<DelegateType>>>(trace, buffer));
        }

        /// <summary>
        /// 约定方法的调用
        /// </summary>
        public CallingConventions CallingConventions
        {
            get
            {
                return this.emit.CallingConventions;
            }
        }

        /// <summary>
        /// 返回的类型
        /// </summary>
        public Type ReturnType
        {
            get
            {
                return this.emit.ReturnType;
            }
        }

        /// <summary>
        /// 方法参数的类型
        /// </summary>
        public Type[] ParameterTypes
        {
            get
            {
                return this.emit.ParameterTypes;
            }
        }

        /// <summary>
        /// 声明一个新临时变量
        /// </summary>
        /// <param name="localType"></param>
        /// <returns></returns>
        public ILocal DeclareLocal(Type localType)
        {
            return this.emit.DeclareLocal(localType);
        }

        /// <summary>
        /// 声明一个新标签
        /// </summary>
        /// <returns></returns>
        public ILabel DefineLabel()
        {
            return this.emit.DefineLabel();
        }

        #endregion itracker

        #region method

        /// <summary>
        /// 创建一个新的动态方法
        /// </summary>
        /// <returns></returns>
        public static TrackEmitBuilder<DelegateType> NewDynamicMethod()
        {
            return NewDynamicMethod(string.Empty);
        }

        /// <summary>
        /// 创建一个新的动态方法
        /// </summary>
        /// <param name="name">方法名字</param>
        /// <returns></returns>
        public static TrackEmitBuilder<DelegateType> NewDynamicMethod(string name)
        {
            return new TrackEmitBuilder<DelegateType>(EasyEmitBuilder<DelegateType>.NewDynamicMethod(name));
        }

        /// <summary>
        /// 创建一个委托
        /// </summary>
        /// <returns></returns>
        public DelegateType CreateDelegate()
        {
            if (!islogged)
                this.BuildForLog();

            foreach (var buffer in this.buffers)
            {
                buffer.Value(this.emit);
            }

            return this.emit.CreateDelegate();
        }

        /// <summary>
        /// 开始写日志
        /// </summary>
        private void BuildForLog()
        {
            this.sb.Length = 0;
            this.sb.Capacity = 16;

            for (var i = 0; i < this.buffers.Count; i++)
            {
                var x = this.buffers[i];
                this.AppendMethodName(i, x.Key);
                this.AppendOpCode(i, x.Key);
                this.sb.AppendLine();
            }

            this.islogged = true;
        }

        private void AppendMethodName(int i, OpCodeTrace trace)
        {
            this.sb.AppendFormat("{0}", trace.MethodName);
            int length = "UnsignedCompareGreaterThan".Length + 3;
            int mlen = trace.MethodName.Length;
            for (var j = 0; j < length - mlen; j++)
                this.sb.Append(" ");
        }

        private void AppendOpCode(int i, OpCodeTrace trace)
        {
            this.sb.AppendFormat("OpCode:{0}", !trace.OpCode.HasValue ? "------" : trace.OpCode.Value.ToString());
        }

        #endregion method

        #region tostring

        /// <summary>
        ///  emit opcode tracker
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!islogged)
                this.BuildForLog();

            return this.sb.ToString();
        }

        #endregion tostring

        #region emit opcodes

        /// <summary>
        /// 返回指向当前方法的参数列表的非托管指针
        /// </summary>
        /// <returns></returns>
        public void ArgumentList()
        {
            this.Append(new OpCodeTrace() { MethodName = "ArgumentList" }, emit => { emit.ArgumentList(); });
            return;
        }

        /// <summary>
        /// 将空引用（O 类型）推送到计算堆栈上
        /// </summary>
        /// <returns></returns>
        public void LoadNull()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldnull, MethodName = "LoadNull" }, emit => { emit.LoadNull(); });
            return;
        }

        /// <summary>
        /// 碰撞了断点
        /// </summary>
        /// <returns></returns>
        public void Break()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Break, MethodName = "Break" }, emit => { emit.Break(); });
            return;
        }

        /// <summary>
        /// 创建新对象
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public void NewObject(ConstructorInfo constructor)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Newobj, MethodName = "NewObject" }, emit => { emit.NewObject(constructor); });
            return;
        }

        /// <summary>
        /// 创建新对象
        /// </summary>
        /// <param name="parameterTypes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public void NewObject(Type type, IList<Type> parameterTypes)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Newobj, MethodName = "NewObject" }, emit => { emit.NewObject(type, parameterTypes); });
            return;
        }

        /// <summary>
        /// 创建新对象
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public void NewObject(ConstructorBuilder constructor)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Newobj, MethodName = "NewObject" }, emit => { emit.NewObject(constructor); });
            return;
        }

        /// <summary>
        /// 将对新的从零开始的一维数组（其元素属于特定类型）的对象引用推送到计算堆栈上
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public void NewArray(Type elementType)
        {
            if (elementType == null)
                throw new ArgumentNullException("elementType");

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Newarr, MethodName = "NewArray" }, emit => { emit.NewArray(elementType); });
            return;
        }

        /// <summary>
        /// 将对特定类型实例的类型化引用推送到计算堆栈上
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public void MakeReferenceAny(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("elementType");

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Mkrefany, MethodName = "MakeReferenceAny" }, emit => { emit.MakeReferenceAny(type); });
            return;
        }

        /// <summary>
        /// 修补操作码
        /// </summary>
        /// <returns></returns>
        public void Nop()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Nop, MethodName = "Nop" }, emit => { emit.Nop(); });
            return;
        }

        /// <summary>
        /// 两个数按位与
        /// </summary>
        /// <returns></returns>
        public void And()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Add, MethodName = "And" }, emit => { emit.And(); });
            return;
        }

        /// <summary>
        /// 按位求补
        /// </summary>
        /// <returns></returns>
        public void Not()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Not, MethodName = "Not" }, emit => { emit.Not(); });
            return;
        }

        /// <summary>
        /// 两个数按位求补
        /// </summary>
        /// <returns></returns>
        public void Or()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Or, MethodName = "Or" }, emit => { emit.Or(); });
            return;
        }

        /// <summary>
        /// 两个数异或
        /// </summary>
        /// <returns></returns>
        public void Xor()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Xor, MethodName = "Xor" }, emit => { emit.Xor(); });
            return;
        }

        /// <summary>
        /// 左移
        /// </summary>
        /// <returns></returns>
        public void ShiftLeft()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Shl, MethodName = "ShiftLeft" }, emit => { emit.ShiftLeft(); });
            return;
        }

        /// <summary>
        /// 右移
        /// </summary>
        /// <returns></returns>
        public void ShiftRight()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Shr, MethodName = "ShiftRight" }, emit => { emit.ShiftRight(); });
            return;
        }

        /// <summary>
        /// 【无符号】右移
        /// </summary>
        /// <returns></returns>
        public void UnsignedShiftRight()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Shr_Un, MethodName = "UnsignedShiftRight" }, emit => { emit.UnsignedShiftRight(); });
            return;
        }

        /// <summary>
        /// 移除当前堆栈的值
        /// </summary>
        /// <returns></returns>
        public void Pop()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Pop, MethodName = "Pop" }, emit => { emit.Pop(); });
            return;
        }

        /// <summary>
        /// 执行返回
        /// </summary>
        /// <returns></returns>
        public void Return()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ret, MethodName = "Return" }, emit => { emit.Return(); });
            return;
        }

        /// <summary>
        /// 将引用对象转换为指定的类
        /// </summary>
        /// <param name="referenceType"></param>
        /// <returns></returns>
        public void CastClass(Type referenceType)
        {
            if (TypeHelper.IsValueType(referenceType))
                throw new ArgumentException("Can only cast to ReferenceTypes, found " + referenceType);

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Castclass, MethodName = "CastClass" }, emit => { emit.CastClass(referenceType); });
            return;
        }

        /// <summary>
        /// 引起不是有限数的异常
        /// </summary>
        /// <returns></returns>
        public void CheckFinite()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ckfinite, MethodName = "CheckFinite" }, emit => { emit.CheckFinite(); });
            return;
        }

        /// <summary>
        /// 将提供的值类型的大小（以字节为单位）推送到计算堆栈上
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public void SizeOf(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            if (!TypeHelper.IsValueType(valueType))
                throw new ArgumentException("valueType must be a ValueType");

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Sizeof, MethodName = "SizeOf" }, emit => { emit.SizeOf(valueType); });
            return;
        }

        /// <summary>
        /// 检索嵌入在类型化引用内的地址（and 类型）
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public void ReferenceAnyValue(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Refanyval, MethodName = "ReferenceAnyValue" }, emit => { emit.ReferenceAnyValue(valueType); });
            return;
        }

        /// <summary>
        /// 检索嵌入在类型化引用内的类型标记
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public void ReferenceAnyType(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Refanytype, MethodName = "ReferenceAnyType" }, emit => { emit.ReferenceAnyType(valueType); });

            return;
        }

        /// <summary>
        /// 退出当前方法并跳至指定方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public void Jump(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Jmp, MethodName = "Jump" }, emit => { emit.Jump(method); });
            return;
        }

        /// <summary>
        /// 测试对象引用（O 类型）是否为特定类的实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public void IsInstance(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Isinst, MethodName = "IsInstance" }, emit => { emit.IsInstance(type); });
            return;
        }

        /// <summary>
        /// 将位于指定地址的值类型的每个字段初始化为空引用或适当的基元类型的 0
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public void InitializeObject(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("type");

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Initobj, MethodName = "InitializeObject" }, emit => { emit.InitializeObject(valueType); });
            return;
        }

        /// <summary>
        /// 将位于特定地址的内存的指定块初始化为给定大小和初始值
        /// </summary>
        /// <returns></returns>
        public void InitializeBlock()
        {
            this.InitializeBlock(false);
            return;
        }

        /// <summary>
        /// 将位于特定地址的内存的指定块初始化为给定大小和初始值
        /// </summary>
        /// <param name="isVolatile"></param>
        /// <returns></returns>
        public void InitializeBlock(bool isVolatile)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Initblk, MethodName = "InitializeBlock" }, emit => { emit.InitializeBlock(isVolatile); });
            return;
        }

        /// <summary>
        /// 复制计算堆栈上当前最顶端的值，然后将副本推送到计算堆栈上
        /// </summary>
        /// <returns></returns>
        public void Duplicate()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Dup, MethodName = "Duplicate" }, emit => { emit.Duplicate(); });
            return;
        }

        /// <summary>
        /// 将位于对象（and、* 或 native int 类型）地址的值类型复制到目标对象（and、* 或 native int 类型）的地址
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public void CopyObject(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            if (!TypeHelper.IsValueType(valueType))
                throw new ArgumentException("CopyObject expects a ValueType; found " + valueType);

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Cpobj, MethodName = "CopyObject" }, emit => { emit.CopyObject(valueType); });
            return;
        }

        /// <summary>
        /// 将指定数目的字节从源地址复制到目标地址
        /// </summary>
        /// <returns></returns>
        public void CopyBlock()
        {
            this.CopyBlock(false);
            return;
        }

        /// <summary>
        /// 将指定数目的字节从源地址复制到目标地址
        /// </summary>
        /// <param name="isVolatile"></param>
        /// <returns></returns>
        public void CopyBlock(bool isVolatile)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Cpblk, MethodName = "CopyBlock" }, emit => { emit.CopyBlock(isVolatile); });
            return;
        }

        /// <summary>
        ///将指向实现与指定对象关联的特定虚方法的本机代码的非托管指针（native int 类型）推送到计算堆栈上
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public void Leave(ILabel label)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Leave, MethodName = "Leave" }, emit => { emit.Leave(label); });
            return;
        }

        /// <summary>
        /// 两个数相加
        /// </summary>
        /// <returns></returns>
        public void Add()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Add, MethodName = "Add" }, emit => { emit.Add(); });
            return;
        }

        /// <summary>
        /// 两个数相加并执行溢出检查
        /// </summary>
        /// <returns></returns>
        public void AddOverflow()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Add_Ovf, MethodName = "AddOverflow" }, emit => { emit.AddOverflow(); });
            return;
        }

        /// <summary>
        /// 【无符号】两个数相加并执行溢出检查
        /// </summary>
        /// <returns></returns>
        public void UnsignedAddOverflow()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Add_Ovf_Un, MethodName = "UnsignedAddOverflow" }, emit => { emit.UnsignedAddOverflow(); });
            return;
        }

        /// <summary>
        /// 两个数相除
        /// </summary>
        /// <returns></returns>
        public void Divide()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Div, MethodName = "Divide" }, emit => { emit.Divide(); });
            return;
        }

        /// <summary>
        /// 【无符号】两个数相除
        /// </summary>
        /// <returns></returns>
        public void UnsignedDivide()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Div_Un, MethodName = "UnsignedDivide" }, emit => { emit.UnsignedDivide(); });
            return;
        }

        /// <summary>
        /// 两个数相乘
        /// </summary>
        /// <returns></returns>
        public void Multiply()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Mul, MethodName = "Multiply" }, emit => { emit.Multiply(); });
            return;
        }

        /// <summary>
        /// 两个数相乘并执行溢出检查
        /// </summary>
        /// <returns></returns>
        public void MultiplyOverflow()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Mul_Ovf, MethodName = "MultiplyOverflow" }, emit => { emit.MultiplyOverflow(); });
            return;
        }

        /// <summary>
        /// 【无符号】两个数相乘并执行溢出检查
        /// </summary>
        /// <returns></returns>
        public void UnsignedMultiplyOverflow()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Mul_Ovf_Un, MethodName = "UnsignedMultiplyOverflow" }, emit => { emit.UnsignedMultiplyOverflow(); });
            return;
        }

        /// <summary>
        /// 两个数相除
        /// </summary>
        /// <returns></returns>
        public void Remainder()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Rem, MethodName = "Remainder" }, emit => { emit.Remainder(); });
            return;
        }

        /// <summary>
        /// 两个数相除余数
        /// </summary>
        /// <returns></returns>
        public void UnsignedRemainder()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Rem_Un, MethodName = "UnsignedRemainder" }, emit => { emit.UnsignedRemainder(); });
            return;
        }

        /// <summary>
        /// 两个数相减
        /// </summary>
        /// <returns></returns>
        public void Subtract()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Sub, MethodName = "Subtract" }, emit => { emit.Subtract(); });
            return;
        }

        /// <summary>
        /// 两个数相减并执行溢出检查
        /// </summary>
        /// <returns></returns>
        public void SubtractOverflow()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Sub_Ovf, MethodName = "SubtractOverflow" }, emit => { emit.SubtractOverflow(); });
            return;
        }

        /// <summary>
        /// 【无符号】两个数相减并执行溢出检查
        /// </summary>
        /// <returns></returns>
        public void UnsignedSubtractOverflow()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Sub_Ovf_Un, MethodName = "UnsignedSubtractOverflow" }, emit => { emit.UnsignedSubtractOverflow(); });
            return;
        }

        /// <summary>
        /// 一个值求反
        /// </summary>
        /// <returns></returns>
        public void Negate()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Neg, MethodName = "Negate" }, emit => { emit.Negate(); });
            return;
        }

        /// <summary>
        /// 装箱
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public void Box(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            if (valueType == typeof(void))
                throw new ArgumentNullException("this type is void type");

            if (!TypeHelper.IsValueType(valueType))
                throw new ArgumentNullException("this type is not value type");

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Box, MethodName = "Box" }, emit => { emit.Box(valueType); });
            return;
        }

        /// <summary>
        /// 无条件地将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public void Branch(ILabel label)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Br, MethodName = "Branch" }, emit => { emit.Branch(label); });
            return;
        }

        /// <summary>
        /// 如果两个值相等，则将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public void BranchIfEqual(ILabel label)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Beq, MethodName = "BranchIfEqual" }, emit => { emit.BranchIfEqual(label); });
            return;
        }

        /// <summary>
        /// 如果两个值不相等，则将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public void UnsignedBranchIfNotEqual(ILabel label)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Bne_Un, MethodName = "UnsignedBranchIfNotEqual" }, emit => { emit.UnsignedBranchIfNotEqual(label); });
            return;
        }

        /// <summary>
        /// 如果 value 为 false、空引用（Visual Basic 中的 Nothing）或零，则将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public void BranchIfFalse(ILabel label)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Brfalse, MethodName = "BranchIfFalse" }, emit => { emit.BranchIfFalse(label); });
            return;
        }

        /// <summary>
        /// 如果 value 为 true、非空或非零，则将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public void BranchIfTrue(ILabel label)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Brtrue, MethodName = "BranchIfTrue" }, emit => { emit.BranchIfTrue(label); });
            return;
        }

        /// <summary>
        /// 如果第一个值大于第二个值，则将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public void BranchIfGreater(ILabel label)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Bgt, MethodName = "BranchIfGreater" }, emit => { emit.BranchIfGreater(label); });
            return;
        }

        /// <summary>
        /// 如果第一个值大于或等于第二个值，则将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public void BranchIfGreaterOrEqual(ILabel label)
        {
            this.Append(new OpCodeTrace() { MethodName = "BranchIfGreaterOrEqual" }, emit => { emit.BranchIfGreaterOrEqual(label); });
            return;
        }

        /// <summary>
        /// 如果第一个值小于第二个值，则将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public void BranchIfLess(ILabel label)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Blt, MethodName = "BranchIfLess" }, emit => { emit.BranchIfLess(label); });
            return;
        }

        /// <summary>
        /// 如果第一个值小于或等于第二个值，则将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public void BranchIfLessOrEqual(ILabel label)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ble, MethodName = "BranchIfLessOrEqual" }, emit => { emit.BranchIfLessOrEqual(label); });
            return;
        }

        /// <summary>
        /// 跳转表
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        public void Switch(IList<ILabel> labels)
        {
            if (labels == null || labels.Count == 0)
                return;

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Switch, MethodName = "Switch" }, emit => { emit.Switch(labels); });
            return;
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="cons"></param>
        /// <returns></returns>
        public void Call(ConstructorInfo cons)
        {
            if (cons == null)
                throw new ArgumentNullException("cons");

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Call, MethodName = "Call" }, emit => { emit.Call(cons); });
            return;
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public void Call(MethodInfo method)
        {
            this.Call(method, null);
            return;
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="arglist">EmitCall 方法可发出对 varargs 方法的调用，因为指定可变变量的参数类型的 Emit 方法没有任何重载。</param>
        /// <returns></returns>
        public void Call(MethodInfo method, Type[] arglist)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            this.Append(new OpCodeTrace() { OpCode = (arglist != null && arglist.Length > 0) ? OpCodes.Call : OpCodes.Callvirt, MethodName = "Call" }, emit => { emit.Call(method, arglist); });
            return;
        }

        /// <summary>
        /// 执行虚方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public void CallVirtual(MethodInfo method)
        {
            this.CallVirtual(method, null);
            return;
        }

        /// <summary>
        /// 执行虚方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="constrained">是否对方法调用约束类型</param>
        /// <returns></returns>
        public void CallVirtual(MethodInfo method, Type constrained)
        {
            this.CallVirtual(method, constrained, null);
            return;
        }

        /// <summary>
        /// 执行虚方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="constrained">是否对方法调用约束类型</param>
        /// <param name="arglist">EmitCall 方法可发出对 varargs 方法的调用，因为指定可变变量的参数类型的 Emit 方法没有任何重载。</param>
        /// <returns></returns>
        public void CallVirtual(MethodInfo method, Type constrained, Type[] arglist)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            if (method.IsStatic)
                throw new ArgumentException("Only non-static methods can be called using CallVirtual");

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Callvirt, MethodName = "CallVirtual" }, emit => { emit.CallVirtual(method, constrained, arglist); });
            return;
        }

        /// <summary>
        /// 比较两个值。如果这两个值相等，则将整数值 1 (int32) 推送到计算堆栈上；否则，将 0 (int32) 推送到计算堆栈上。
        /// </summary>
        /// <returns></returns>
        public void CompareEqual()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ceq, MethodName = "CompareEqual" }, emit => { emit.CompareEqual(); });
            return;
        }

        /// <summary>
        /// 比较两个值。如果第一个值大于第二个值，则将整数值 1 (int32) 推送到计算堆栈上；反之，将 0 (int32) 推送到计算堆栈上。
        /// </summary>
        /// <returns></returns>
        public void CompareGreaterThan()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Cgt, MethodName = "CompareGreaterThan" }, emit => { emit.CompareGreaterThan(); });
            return;
        }

        /// <summary>
        /// 【无符号】比较两个值。如果第一个值大于第二个值，则将整数值 1 (int32) 推送到计算堆栈上；反之，将 0 (int32) 推送到计算堆栈上。
        /// </summary>
        /// <returns></returns>
        public void UnsignedCompareGreaterThan()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Cgt_Un, MethodName = "UnsignedCompareGreaterThan" }, emit => { emit.UnsignedCompareGreaterThan(); });
            return;
        }

        /// <summary>
        /// 比较两个值。如果第一个值小于第二个值，则将整数值 1 (int32) 推送到计算堆栈上；反之，将 0 (int32) 推送到计算堆栈上。
        /// </summary>
        /// <returns></returns>
        public void CompareLessThan()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Clt, MethodName = "CompareLessThan" }, emit => { emit.CompareLessThan(); });
            return;
        }

        /// <summary>
        /// 【无符号】比较两个值。如果第一个值小于第二个值，则将整数值 1 (int32) 推送到计算堆栈上；反之，将 0 (int32) 推送到计算堆栈上。
        /// </summary>
        /// <returns></returns>
        public void UnsignedCompareLessThan()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Clt_Un, MethodName = "UnsignedCompareLessThan" }, emit => { emit.UnsignedCompareLessThan(); });
            return;
        }

        /// <summary>
        /// 再次引出异常
        /// </summary>
        /// <returns></returns>
        public void ReturnThrow()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Rethrow, MethodName = "ReturnThrow" }, emit => { emit.ReturnThrow(); });
            return;
        }

        /// <summary>
        /// 引发出常
        /// </summary>
        /// <returns></returns>
        public void Throw()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Throw, MethodName = "Throw" }, emit => { emit.Throw(); });
            return;
        }

        /// <summary>
        /// 加载参数
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public void LoadArgument(ushort index)
        {
            if (this.ParameterTypes == null || this.ParameterTypes.Length == 0)
                return;

            if (index >= this.ParameterTypes.Length)
                throw new ArgumentException("index must be between 0 and " + (this.ParameterTypes.Length - 1) + ", inclusive");

            switch (index)
            {
                case 0:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldarg_0, MethodName = "LoadArgument" }, emit => { emit.LoadArgument(index); });
                        return;
                    }
                case 1:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldarg_1, MethodName = "LoadArgument" }, emit => { emit.LoadArgument(index); });
                        return;
                    }
                case 2:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldarg_2, MethodName = "LoadArgument" }, emit => { emit.LoadArgument(index); });
                        return;
                    }
                case 3:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldarg_3, MethodName = "LoadArgument" }, emit => { emit.LoadArgument(index); });
                        return;
                    }
            }

            if (index >= byte.MinValue && index <= byte.MaxValue)
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldarg_S, MethodName = "LoadArgument" }, emit => { emit.LoadArgument(index); });
                return;
            }

            this.Append(new OpCodeTrace() { MethodName = "LoadArgument" }, emit => { emit.LoadArgument(index); });
            return;
        }

        /// <summary>
        /// 加载参数地址
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public void LoadArgumentAddress(ushort index)
        {
            if (this.ParameterTypes == null || this.ParameterTypes.Length == 0)
                return;

            if (index >= this.ParameterTypes.Length)
                throw new ArgumentException("index must be between 0 and " + (this.ParameterTypes.Length - 1) + ", inclusive");

            if (index >= byte.MinValue && index <= byte.MaxValue)
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldarga_S, MethodName = "LoadArgumentAddress" }, emit => { emit.LoadArgumentAddress(index); });
                return;
            }

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldarga, MethodName = "LoadArgumentAddress" }, emit => { emit.LoadArgumentAddress(index); });
            return;
        }

        /// <summary>
        /// 加载uint
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public void LoadConstant(uint i)
        {
            switch (i)
            {
                case uint.MaxValue:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_M1, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 0:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_0, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 1:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_1, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 2:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_2, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 3:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_3, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 4:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_4, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 5:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_5, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 6:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_6, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 7:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_7, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 8:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_8, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
            }

            if (i <= sbyte.MaxValue)
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_S, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
            }

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
            return;
        }

        /// <summary>
        /// 加载int
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public void LoadConstant(int i)
        {
            switch (i)
            {
                case -1:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_M1, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 0:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_0, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 1:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_1, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 2:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_2, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 3:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_3, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 4:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_4, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 5:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_5, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 6:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_6, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 7:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_7, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
                case 8:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_8, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                        return;
                    }
            }

            if (i >= sbyte.MinValue && i <= sbyte.MaxValue)
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_S, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
                return;
            }

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(i); });
            return;
        }

        /// <summary>
        /// 加载bool
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public void LoadConstant(bool b)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I4_0, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(b); });
            return;
        }

        /// <summary>
        /// 加载ulong
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public void LoadConstant(ulong l)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I8, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(l); });
            return;
        }

        /// <summary>
        /// 加载long
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public void LoadConstant(long l)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_I8, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(l); });
            return;
        }

        /// <summary>
        /// 加载浮点数
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public void LoadConstant(float f)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_R4, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(f); });
            return;
        }

        /// <summary>
        /// 加载double数
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public void LoadConstant(double d)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldc_R8, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(d); });
            return;
        }

        /// <summary>
        /// 加载字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public void LoadConstant(string str)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldstr, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(str); });
            return;
        }

        /// <summary>
        /// 将元数据标记转换为其运行时表示形式，并将其推送到计算堆栈
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public void LoadConstant(FieldInfo field)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldtoken, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(field); });
            return;
        }

        /// <summary>
        /// 将元数据标记转换为其运行时表示形式，并将其推送到计算堆栈
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public void LoadConstant(MethodInfo method)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldtoken, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(method); });
            return;
        }

        /// <summary>
        /// 将元数据标记转换为其运行时表示形式，并将其推送到计算堆栈
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public void LoadConstant(Type type)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldtoken, MethodName = "LoadConstant" }, emit => { emit.LoadConstant(type); });
            return;
        }

        /// <summary>
        /// 按照指令中指定的类型，将指定数组索引中的元素加载到计算堆栈的顶部。
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public void LoadElement(Type elementType)
        {
            if (elementType == null)
                throw new ArgumentNullException("elementType");

            if (elementType.IsPointer)
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldelem_I, MethodName = "LoadElement" }, emit => { emit.LoadElement(elementType); });
                return;
            }

            if (!TypeHelper.IsValueType(elementType))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldelem_Ref, MethodName = "LoadElement" }, emit => { emit.LoadElement(elementType); });
                return;
            }

            if (elementType == typeof(sbyte))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldelem_I1, MethodName = "LoadElement" }, emit => { emit.LoadElement(elementType); });
                return;
            }

            if (elementType == typeof(byte))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldelem_U1, MethodName = "LoadElement" }, emit => { emit.LoadElement(elementType); });
                return;
            }

            if (elementType == typeof(short))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldelem_I2, MethodName = "LoadElement" }, emit => { emit.LoadElement(elementType); });
                return;
            }

            if (elementType == typeof(ushort))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldelem_U2, MethodName = "LoadElement" }, emit => { emit.LoadElement(elementType); });
                return;
            }

            if (elementType == typeof(int))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldelem_I4, MethodName = "LoadElement" }, emit => { emit.LoadElement(elementType); });
                return;
            }

            if (elementType == typeof(uint))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldelem_U4, MethodName = "LoadElement" }, emit => { emit.LoadElement(elementType); });
                return;
            }

            if (elementType == typeof(long) || elementType == typeof(ulong))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldelem_I8, MethodName = "CastClass" }, emit => { emit.LoadElement(elementType); });
                return;
            }

            if (elementType == typeof(float))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldelem_R4, MethodName = "LoadElement" }, emit => { emit.LoadElement(elementType); });
                return;
            }

            if (elementType == typeof(double))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldelem_R8, MethodName = "LoadElement" }, emit => { emit.LoadElement(elementType); });
                return;
            }

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldelem, MethodName = "LoadElement" }, emit => { emit.LoadElement(elementType); });

            return;
        }

        /// <summary>
        /// 将位于指定数组索引处的包含对象引用的元素作为 O 类型（对象引用）加载到计算堆栈的顶部。
        /// </summary>
        /// <returns></returns>
        public void LoadElementReference()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldelem_Ref, MethodName = "LoadElementReference" }, emit => { emit.LoadElementReference(); });
            return;
        }

        /// <summary>
        /// 将位于指定数组索引的数组元素的地址作为 &amp; 类型（托管指针）加载到计算堆栈的顶部。
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public void LoadElementAddress(Type elementType)
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldelema, MethodName = "LoadElementAddress" }, emit => { emit.LoadElementAddress(elementType); });
            return;
        }

        /// <summary>
        /// 查找对象中其引用当前位于计算堆栈的字段的值。
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public void LoadField(FieldInfo field)
        {
            this.LoadField(field, false);
            return;
        }

        /// <summary>
        /// 查找对象中其引用当前位于计算堆栈的字段的值。
        /// </summary>
        /// <param name="field"></param>
        /// <param name="isVolatile"></param>
        /// <returns></returns>
        public void LoadField(FieldInfo field, bool isVolatile)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            if (isVolatile)
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Volatile, MethodName = "LoadField" }, emit => { });

            if (!field.IsStatic)
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldfld, MethodName = "LoadField" }, emit => { emit.LoadField(field); });
            else
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldsfld, MethodName = "LoadField" }, emit => { emit.LoadField(field); });

            return;
        }

        /// <summary>
        /// 查找对象中其引用当前位于计算堆栈的字段的地址。
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public void LoadFieldAddress(FieldInfo field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            if (!field.IsStatic)
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldflda, MethodName = "LoadFieldAddress" }, emit => { emit.LoadFieldAddress(field); });
            else
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldsflda, MethodName = "LoadFieldAddress" }, emit => { emit.LoadFieldAddress(field); });

            return;
        }

        /// <summary>
        /// 加载临时变量
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        public void LoadLocal(ILocal local)
        {
            switch (local.Index)
            {
                case 0:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldloc_0, MethodName = "LoadLocal" }, emit => { emit.LoadLocal(local); });
                        return;
                    }
                case 1:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldloc_1, MethodName = "LoadLocal" }, emit => { emit.LoadLocal(local); });
                        return;
                    }
                case 2:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldloc_2, MethodName = "LoadLocal" }, emit => { emit.LoadLocal(local); });
                        return;
                    }
                case 3:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldloc_3, MethodName = "LoadLocal" }, emit => { emit.LoadLocal(local); });
                        return;
                    }
            }
            if (local.Index >= sbyte.MinValue && local.Index <= sbyte.MaxValue)
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldloc_S, MethodName = "LoadLocal" }, emit => { emit.LoadLocal(local); });
                return;
            }

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldloc, MethodName = "LoadLocal" }, emit => { emit.LoadLocal(local); });
            return;
        }

        /// <summary>
        /// 加载临时变量地址
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        public void LoadLocalAddress(ILocal local)
        {
            if (local.Index >= byte.MinValue && local.Index <= byte.MaxValue)
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldloca_S, MethodName = "LoadLocalAddress" }, emit => { emit.LoadLocalAddress(local); });
                return;
            }

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldloca, MethodName = "LoadLocalAddress" }, emit => { emit.LoadLocalAddress(local); });
            return;
        }

        /// <summary>
        ///  将指向实现特定方法的本机代码的非托管指针（native int 类型）推送到计算堆栈上。
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public void LoadFunctionPointer(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldftn, MethodName = "LoadFunctionPointer" }, emit => { emit.LoadFunctionPointer(method); });
            return;
        }

        /// <summary>
        ///  将 native int等 类型的值作为 native int等 间接加载到计算堆栈上
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public void LoadIndirect(Type type)
        {
            this.LoadIndirect(type);
            return;
        }

        /// <summary>
        ///  将 native int等 类型的值作为 native int等 间接加载到计算堆栈上
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isVolatile"></param>
        /// <returns></returns>
        public void LoadIndirect(Type type, bool isVolatile)
        {
            if (type == null)
                throw new ArgumentNullException("method");

            if (isVolatile)
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Volatile, MethodName = "LoadIndirect" }, emit => { });

            if (type.IsPointer)
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldind_I, MethodName = "LoadIndirect" }, emit => { emit.LoadIndirect(type, isVolatile); });
                return;
            }

            if (!TypeHelper.IsValueType(type))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldind_Ref, MethodName = "LoadIndirect" }, emit => { emit.LoadIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(sbyte))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldind_I1, MethodName = "LoadIndirect" }, emit => { emit.LoadIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(bool))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldind_I1, MethodName = "LoadIndirect" }, emit => { emit.LoadIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(byte))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldind_U1, MethodName = "LoadIndirect" }, emit => { emit.LoadIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(short))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldind_I2, MethodName = "LoadIndirect" }, emit => { emit.LoadIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(ushort))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldind_U2, MethodName = "LoadIndirect" }, emit => { emit.LoadIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(char))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldind_U2, MethodName = "LoadIndirect" }, emit => { emit.LoadIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(int))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldind_I4, MethodName = "LoadIndirect" }, emit => { emit.LoadIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(uint))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldind_U4, MethodName = "LoadIndirect" }, emit => { emit.LoadIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(long) || type == typeof(ulong))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldind_I8, MethodName = "LoadIndirect" }, emit => { emit.LoadIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(float))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldind_R4, MethodName = "LoadIndirect" }, emit => { emit.LoadIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(double))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldind_R8, MethodName = "LoadIndirect" }, emit => { emit.LoadIndirect(type, isVolatile); });
                return;
            }

            return;
        }

        /// <summary>
        /// 将从零开始的、一维数组的元素的数目推送到计算堆栈上
        /// </summary>
        /// <returns></returns>
        public void LoadLength()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldlen, MethodName = "LoadLength" }, emit => { emit.LoadLength(); });
            return;
        }

        /// <summary>
        /// 将地址指向的值类型对象复制到计算堆栈的顶部。
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public void LoadObject(Type valueType)
        {
            this.LoadObject(valueType, false);
            return;
        }

        /// <summary>
        /// 将地址指向的值类型对象复制到计算堆栈的顶部。
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="isVolatile"></param>
        /// <returns></returns>
        public void LoadObject(Type valueType, bool isVolatile)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            if (!TypeHelper.IsValueType(valueType))
                throw new ArgumentException("valueType must be a ValueType");

            if (isVolatile)
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Volatile, MethodName = "LoadObject" }, emit => { });

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldobj, MethodName = "LoadObject" }, emit => { emit.LoadObject(valueType, isVolatile); });
            return;
        }

        /// <summary>
        /// 将指向实现与指定对象关联的特定虚方法的本机代码的非托管指针（native int 类型）推送到计算堆栈上。
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public void LoadVirtualFunctionPointer(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Ldvirtftn, MethodName = "LoadVirtualFunctionPointer" }, emit => { emit.LoadVirtualFunctionPointer(method); });
            return;
        }

        /// <summary>
        /// 从本地动态内存池分配特定数目的字节并将第一个分配的字节的地址（瞬态指针，* 类型）推送到计算堆栈上
        /// </summary>
        /// <returns></returns>
        public void LocalAllocate()
        {
            this.Append(new OpCodeTrace() { OpCode = OpCodes.Localloc, MethodName = "LocalAllocate" }, emit => { emit.LocalAllocate(); });
            return;
        }

        /// <summary>
        /// 将临时变量存储在参数中
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public void StoreArgument(ushort index)
        {
            if (this.ParameterTypes == null || this.ParameterTypes.Length == 0)
                return;

            if (index >= this.ParameterTypes.Length)
                throw new ArgumentException("index must be between 0 and " + (this.ParameterTypes.Length - 1) + ", inclusive");

            if (index >= byte.MinValue && index <= byte.MaxValue)
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Starg_S, MethodName = "StoreArgument" }, emit => { emit.StoreArgument(index); });
                return;
            }

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Starg, MethodName = "StoreArgument" }, emit => { emit.StoreArgument(index); });
            return;
        }

        /// <summary>
        /// 将堆栈的值替换给定索引处的数组元素
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public void StoreElement(Type elementType)
        {
            if (elementType == null)
                throw new ArgumentNullException("elementType");

            if (TypeHelper.IsPointer(elementType))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stelem_I, MethodName = "StoreElement" }, emit => { emit.StoreElement(elementType); });
                return;
            }

            if (!TypeHelper.IsValueType(elementType))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stelem_Ref, MethodName = "StoreElement" }, emit => { emit.StoreElement(elementType); });
                return;
            }

            if (elementType == typeof(sbyte) || elementType == typeof(byte))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stelem_I1, MethodName = "StoreElement" }, emit => { emit.StoreElement(elementType); });
                return;
            }

            if (elementType == typeof(short) || elementType == typeof(ushort))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stelem_I2, MethodName = "StoreElement" }, emit => { emit.StoreElement(elementType); });
                return;
            }

            if (elementType == typeof(int) || elementType == typeof(uint))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stelem_I4, MethodName = "StoreElement" }, emit => { emit.StoreElement(elementType); });
                return;
            }

            if (elementType == typeof(long) || elementType == typeof(ulong))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stelem_I8, MethodName = "StoreElement" }, emit => { emit.StoreElement(elementType); });
                return;
            }

            if (elementType == typeof(float))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stelem_R4, MethodName = "StoreElement" }, emit => { emit.StoreElement(elementType); });
                return;
            }

            if (elementType == typeof(double))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stelem_R8, MethodName = "StoreElement" }, emit => { emit.StoreElement(elementType); });
                return;
            }

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Stelem, MethodName = "StoreElement" }, emit => { emit.StoreElement(elementType); });
            return;
        }

        /// <summary>
        /// 将堆栈的值替换静态字段
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public void StoreField(FieldInfo field)
        {
            this.StoreField(field, false);
            return;
        }

        /// <summary>
        /// 将堆栈的值替换静态字段
        /// </summary>
        /// <param name="field"></param>
        /// <param name="isVolatile"></param>
        /// <returns></returns>
        public void StoreField(FieldInfo field, bool isVolatile)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            if (isVolatile)
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Volatile, MethodName = "StoreField" }, emit => { });

            if (field.IsStatic)
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stsfld, MethodName = "StoreField" }, emit => { emit.StoreField(field, isVolatile); });
            else
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stfld, MethodName = "StoreField" }, emit => { emit.StoreField(field, isVolatile); });

            return;
        }

        /// <summary>
        /// 存储在局部变量中
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        public void StoreLocal(ILocal local)
        {
            switch (local.Index)
            {
                case 0:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Stloc_0, MethodName = "StoreLocal" }, emit => { emit.StoreLocal(local); });
                        return;
                    }
                case 1:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Stloc_1, MethodName = "StoreLocal" }, emit => { emit.StoreLocal(local); });
                        return;
                    }
                case 2:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Stloc_2, MethodName = "StoreLocal" }, emit => { emit.StoreLocal(local); });
                        return;
                    }
                case 3:
                    {
                        this.Append(new OpCodeTrace() { OpCode = OpCodes.Stloc_3, MethodName = "StoreLocal" }, emit => { emit.StoreLocal(local); });
                        return;
                    }
            }

            if (local.Index >= byte.MinValue && local.Index <= byte.MaxValue)
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stloc_S, MethodName = "StoreLocal" }, emit => { emit.StoreLocal(local); });
                return;
            }

            return;
        }

        /// <summary>
        /// 复制到内存中
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public void StoreObject(Type valueType)
        {
            this.StoreObject(valueType, false);
            return;
        }

        /// <summary>
        /// 复制到内存中
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="isVolatile"></param>
        /// <returns></returns>
        public void StoreObject(Type valueType, bool isVolatile)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            if (isVolatile)
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Volatile, MethodName = "StoreObject" }, emit => { });

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Stobj, MethodName = "StoreObject" }, emit => { emit.StoreObject(valueType, isVolatile); });
            return;
        }

        /// <summary>
        /// 存储在局部变量中
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public void StoreIndirect(Type type)
        {
            this.StoreIndirect(type);
            return;
        }

        /// <summary>
        /// 存储在局部变量中
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isVolatile"></param>
        /// <returns></returns>
        public void StoreIndirect(Type type, bool isVolatile)
        {
            if (type == null)
                throw new ArgumentNullException("valueType");

            if (isVolatile)
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Volatile, MethodName = "StoreIndirect" }, emit => { });

            if (TypeHelper.IsPointer(type))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stind_I, MethodName = "StoreIndirect" }, emit => { emit.StoreIndirect(type, isVolatile); });
                return;
            }

            if (!TypeHelper.IsValueType(type))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stind_Ref, MethodName = "StoreIndirect" }, emit => { emit.StoreIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(sbyte) || type == typeof(byte))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stind_I1, MethodName = "StoreIndirect" }, emit => { emit.StoreIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(short) || type == typeof(ushort))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stind_I2, MethodName = "StoreIndirect" }, emit => { emit.StoreIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(int) || type == typeof(uint))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stind_I4, MethodName = "StoreIndirect" }, emit => { emit.StoreIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(long) || type == typeof(ulong))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stind_I8, MethodName = "StoreIndirect" }, emit => { emit.StoreIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(float))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stind_R4, MethodName = "StoreIndirect" }, emit => { emit.StoreIndirect(type, isVolatile); });
                return;
            }

            if (type == typeof(double))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Stind_R8, MethodName = "StoreIndirect" }, emit => { emit.StoreIndirect(type, isVolatile); });
                return;
            }

            return;
        }

        /// <summary>
        /// 拆箱
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public void Unbox(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            if (valueType.IsByRef || TypeHelper.IsPointer(valueType))
                throw new ArgumentException("UnboxAny cannot operate on pointers, found " + valueType);

            if (valueType == typeof(void))
                throw new ArgumentException("Void cannot be boxed, and thus cannot be unboxed");

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Unbox, MethodName = "Unbox" }, emit => { emit.Unbox(valueType); });
            return;
        }

        /// <summary>
        /// 拆箱
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public void UnboxAny(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            if (valueType.IsByRef || TypeHelper.IsPointer(valueType))
                throw new ArgumentException("UnboxAny cannot operate on pointers, found " + valueType);

            if (valueType == typeof(void))
                throw new ArgumentException("Void cannot be boxed, and thus cannot be unboxed");

            this.Append(new OpCodeTrace() { OpCode = OpCodes.Unbox_Any, MethodName = "UnboxAny" }, emit => { emit.Unbox(valueType); });
            return;
        }

        /// <summary>
        /// 用给定标签标记 Microsoft 中间语言 (MSIL) 流的当前位置。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public void MarkLabel(ILabel label)
        {
            this.Append(new OpCodeTrace() { MethodName = "MarkLabel" }, emit => { emit.MarkLabel(label); });
            return;
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="primitiveType"></param>
        /// <returns></returns>
        public void Convert(Type primitiveType)
        {
            if (primitiveType == typeof(byte))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_U1, MethodName = "Convert" }, emit => { emit.Convert(primitiveType); });
                return;
            }

            if (primitiveType == (typeof(sbyte)))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_I1, MethodName = "Convert" }, emit => { emit.Convert(primitiveType); });
                return;
            }

            if (primitiveType == typeof(bool))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_I1, MethodName = "Convert" }, emit => { emit.Convert(primitiveType); });
                return;
            }

            if (primitiveType == typeof(short))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_I2, MethodName = "Convert" }, emit => { emit.Convert(primitiveType); });
                return;
            }

            if (primitiveType == typeof(ushort))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_U2, MethodName = "Convert" }, emit => { emit.Convert(primitiveType); });
                return;
            }

            if (primitiveType == typeof(int))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_I4, MethodName = "Convert" }, emit => { emit.Convert(primitiveType); });
                return;
            }

            if (primitiveType == typeof(uint))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_U4, MethodName = "Convert" }, emit => { emit.Convert(primitiveType); });
                return;
            }

            if (primitiveType == typeof(long))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_I8, MethodName = "Convert" }, emit => { emit.Convert(primitiveType); });
                return;
            }

            if (primitiveType == typeof(ulong))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_U8, MethodName = "Convert" }, emit => { emit.Convert(primitiveType); });
                return;
            }

            if (primitiveType == typeof(float))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_R4, MethodName = "Convert" }, emit => { emit.Convert(primitiveType); });
                return;
            }

            if (primitiveType == typeof(double))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_R8, MethodName = "Convert" }, emit => { emit.Convert(primitiveType); });
                return;
            }

            if (primitiveType == typeof(IntPtr))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_I, MethodName = "Convert" }, emit => { emit.Convert(primitiveType); });
                return;
            }

            if (primitiveType == typeof(UIntPtr))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_U, MethodName = "Convert" }, emit => { emit.Convert(primitiveType); });
                return;
            }

            return;
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="primitiveType"></param>
        /// <returns></returns>
        public void ConvertOverflow(Type primitiveType)
        {
            if (primitiveType == typeof(byte))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_U1, MethodName = "ConvertOverflow" }, emit => { emit.ConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == (typeof(sbyte)))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_I1, MethodName = "ConvertOverflow" }, emit => { emit.ConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(bool))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_I1, MethodName = "ConvertOverflow" }, emit => { emit.ConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(short))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_I2, MethodName = "ConvertOverflow" }, emit => { emit.ConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(ushort))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_U2, MethodName = "ConvertOverflow" }, emit => { emit.ConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(int))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_I4, MethodName = "ConvertOverflow" }, emit => { emit.ConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(uint))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_U4, MethodName = "ConvertOverflow" }, emit => { emit.ConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(long))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_I8, MethodName = "ConvertOverflow" }, emit => { emit.ConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(ulong))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_U8, MethodName = "ConvertOverflow" }, emit => { emit.ConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(IntPtr))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_I, MethodName = "ConvertOverflow" }, emit => { emit.ConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(UIntPtr))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_U, MethodName = "ConvertOverflow" }, emit => { emit.ConvertOverflow(primitiveType); });
                return;
            }
            return;
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="primitiveType"></param>
        /// <returns></returns>
        public void UnsignedConvertOverflow(Type primitiveType)
        {
            if (primitiveType == typeof(byte))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_U1_Un, MethodName = "UnsignedConvertOverflow" }, emit => { emit.UnsignedConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == (typeof(sbyte)))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_I1_Un, MethodName = "UnsignedConvertOverflow" }, emit => { emit.UnsignedConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(bool))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_I1_Un, MethodName = "UnsignedConvertOverflow" }, emit => { emit.UnsignedConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(short))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_I2_Un, MethodName = "UnsignedConvertOverflow" }, emit => { emit.UnsignedConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(ushort))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_U2_Un, MethodName = "UnsignedConvertOverflow" }, emit => { emit.UnsignedConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(int))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_I4_Un, MethodName = "UnsignedConvertOverflow" }, emit => { emit.UnsignedConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(uint))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_U4_Un, MethodName = "UnsignedConvertOverflow" }, emit => { emit.UnsignedConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(long))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_I8_Un, MethodName = "UnsignedConvertOverflow" }, emit => { emit.UnsignedConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(ulong))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_U8_Un, MethodName = "UnsignedConvertOverflow" }, emit => { emit.UnsignedConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(IntPtr))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_I_Un, MethodName = "UnsignedConvertOverflow" }, emit => { emit.UnsignedConvertOverflow(primitiveType); });
                return;
            }

            if (primitiveType == typeof(UIntPtr))
            {
                this.Append(new OpCodeTrace() { OpCode = OpCodes.Conv_Ovf_U_Un, MethodName = "UnsignedConvertOverflow" }, emit => { emit.UnsignedConvertOverflow(primitiveType); });
                return;
            }

            return;
        }

        #endregion emit opcodes
    }
}