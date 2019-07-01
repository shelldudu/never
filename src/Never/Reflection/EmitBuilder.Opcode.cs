using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Never.Reflection
{
    /// <summary>
    /// 定义Emit操作接口
    /// </summary>
    public abstract partial class EmitBuilder : IEmitBuilder
    {
        #region opcodes

        /// <summary>
        /// 返回指向当前方法的参数列表的非托管指针
        /// </summary>
        /// <returns></returns>
        public virtual void ArgumentList()
        {
            this.il.Emit(OpCodes.Arglist);
            return;
        }

        /// <summary>
        /// 将空引用（O 类型）推送到计算堆栈上
        /// </summary>
        /// <returns></returns>
        public virtual void LoadNull()
        {
            this.il.Emit(OpCodes.Ldnull);
            return;
        }

        /// <summary>
        /// 碰撞了断点
        /// </summary>
        /// <returns></returns>
        public virtual void Break()
        {
            this.il.Emit(OpCodes.Break);
            return;
        }

        /// <summary>
        /// 创建新对象
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public virtual void NewObject(ConstructorInfo constructor)
        {
            this.il.Emit(OpCodes.Newobj, constructor);
            return;
        }

        /// <summary>
        /// 创建新对象
        /// </summary>
        /// <param name="parameterTypes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual void NewObject(Type type, IList<Type> parameterTypes)
        {
            if (this.TryNewObject(type, parameterTypes))
                return;

            throw new ArgumentNullException(string.Format("在当前类型{0}找不到构造函数", type.FullName));
        }

        /// <summary>
        /// 创建新对象
        /// </summary>
        /// <param name="parameterTypes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool TryNewObject(Type type, IList<Type> parameterTypes)
        {
            if (parameterTypes == null)
                parameterTypes = new Type[0];

            var allctors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var ctor in allctors)
            {
                var paremeters = ctor.GetParameters();
                if (paremeters.Length != parameterTypes.Count)
                    continue;

                bool allMatch = true;
                for (var i = 0; i < paremeters.Length; i++)
                {
                    if (!allMatch)
                        break;
                    if (paremeters[i].ParameterType != parameterTypes[i])
                        allMatch = false;
                }

                if (allMatch)
                {
                    this.il.Emit(OpCodes.Newobj, ctor);
                    return true;
                }
            };

            return false;
        }
        /// <summary>
        /// 创建新对象
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public virtual void NewObject(ConstructorBuilder constructor)
        {
            this.il.Emit(OpCodes.Newobj, constructor);
            return;
        }

        /// <summary>
        /// 将对新的从零开始的一维数组（其元素属于特定类型）的对象引用推送到计算堆栈上
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public virtual void NewArray(Type elementType)
        {
            if (elementType == null)
                throw new ArgumentNullException("elementType");

            this.il.Emit(OpCodes.Newarr, elementType);
            return;
        }

        /// <summary>
        /// 将对特定类型实例的类型化引用推送到计算堆栈上
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual void MakeReferenceAny(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("elementType");

            this.il.Emit(OpCodes.Mkrefany, type);
            return;
        }

        /// <summary>
        /// 修补操作码
        /// </summary>
        /// <returns></returns>
        public virtual void Nop()
        {
            this.il.Emit(OpCodes.Nop);
            return;
        }

        /// <summary>
        /// 两个数按位与
        /// </summary>
        /// <returns></returns>
        public virtual void And()
        {
            this.il.Emit(OpCodes.And);
            return;
        }

        /// <summary>
        /// 按位求补
        /// </summary>
        /// <returns></returns>
        public virtual void Not()
        {
            this.il.Emit(OpCodes.Not);
            return;
        }

        /// <summary>
        /// 两个数按位求补
        /// </summary>
        /// <returns></returns>
        public virtual void Or()
        {
            this.il.Emit(OpCodes.Or);
            return;
        }

        /// <summary>
        /// 两个数异或
        /// </summary>
        /// <returns></returns>
        public virtual void Xor()
        {
            this.il.Emit(OpCodes.Xor);
            return;
        }

        /// <summary>
        /// 左移
        /// </summary>
        /// <returns></returns>
        public virtual void ShiftLeft()
        {
            this.il.Emit(OpCodes.Shl);
            return;
        }

        /// <summary>
        /// 右移
        /// </summary>
        /// <returns></returns>
        public virtual void ShiftRight()
        {
            this.il.Emit(OpCodes.Shr);
            return;
        }

        /// <summary>
        /// 【无符号】右移
        /// </summary>
        /// <returns></returns>
        public virtual void UnsignedShiftRight()
        {
            this.il.Emit(OpCodes.Shr_Un);
            return;
        }

        /// <summary>
        /// 移除当前堆栈的值
        /// </summary>
        /// <returns></returns>
        public virtual void Pop()
        {
            this.il.Emit(OpCodes.Pop);
            return;
        }

        /// <summary>
        /// 执行返回
        /// </summary>
        /// <returns></returns>
        public virtual void Return()
        {
            this.il.Emit(OpCodes.Ret);
            return;
        }

        /// <summary>
        /// 将引用对象转换为指定的类
        /// </summary>
        /// <param name="referenceType"></param>
        /// <returns></returns>
        public virtual void CastClass(Type referenceType)
        {
            if (TypeHelper.IsValueType(referenceType))
                throw new ArgumentException("Can only cast to ReferenceTypes, found " + referenceType);

            this.il.Emit(OpCodes.Castclass, referenceType);
            return;
        }

        /// <summary>
        /// 引起不是有限数的异常
        /// </summary>
        /// <returns></returns>
        public virtual void CheckFinite()
        {
            this.il.Emit(OpCodes.Ckfinite);
            return;
        }

        /// <summary>
        /// 将提供的值类型的大小（以字节为单位）推送到计算堆栈上
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public virtual void SizeOf(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            if (!TypeHelper.IsValueType(valueType))
                throw new ArgumentException("valueType must be a ValueType");

            this.il.Emit(OpCodes.Sizeof, valueType);
            return;
        }

        /// <summary>
        /// 检索嵌入在类型化引用内的地址（and 类型）
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public virtual void ReferenceAnyValue(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            this.il.Emit(OpCodes.Refanyval, valueType);
            return;
        }

        /// <summary>
        /// 检索嵌入在类型化引用内的类型标记
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public virtual void ReferenceAnyType(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            this.il.Emit(OpCodes.Refanytype, valueType);
        }

        /// <summary>
        /// 退出当前方法并跳至指定方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public virtual void Jump(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            this.il.Emit(OpCodes.Jmp, method);
            return;
        }

        /// <summary>
        /// 测试对象引用（O 类型）是否为特定类的实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual void IsInstance(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            this.il.Emit(OpCodes.Isinst, type);
            return;
        }

        /// <summary>
        /// 将位于指定地址的值类型的每个字段初始化为空引用或适当的基元类型的 0
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public virtual void InitializeObject(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("type");

            this.il.Emit(OpCodes.Initobj, valueType);
            return;
        }

        /// <summary>
        /// 将位于特定地址的内存的指定块初始化为给定大小和初始值
        /// </summary>
        /// <returns></returns>
        public virtual void InitializeBlock()
        {
            this.InitializeBlock(false);
        }

        /// <summary>
        /// 将位于特定地址的内存的指定块初始化为给定大小和初始值
        /// </summary>
        /// <param name="isVolatile"></param>
        /// <returns></returns>
        public virtual void InitializeBlock(bool isVolatile)
        {
            if (isVolatile)
                this.il.Emit(OpCodes.Volatile);

            this.il.Emit(OpCodes.Initblk);
            return;
        }

        /// <summary>
        /// 复制计算堆栈上当前最顶端的值，然后将副本推送到计算堆栈上
        /// </summary>
        /// <returns></returns>
        public virtual void Duplicate()
        {
            this.il.Emit(OpCodes.Dup);
            return;
        }

        /// <summary>
        /// 将位于对象（and、* 或 native int 类型）地址的值类型复制到目标对象（and、* 或 native int 类型）的地址
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public virtual void CopyObject(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            if (!TypeHelper.IsValueType(valueType))
                throw new ArgumentException("CopyObject expects a ValueType; found " + valueType);

            this.il.Emit(OpCodes.Cpobj, valueType);
            return;
        }

        /// <summary>
        /// 将指定数目的字节从源地址复制到目标地址
        /// </summary>
        /// <returns></returns>
        public virtual void CopyBlock()
        {
            this.CopyBlock(false);
        }

        /// <summary>
        /// 将指定数目的字节从源地址复制到目标地址
        /// </summary>
        /// <param name="isVolatile"></param>
        /// <returns></returns>
        public virtual void CopyBlock(bool isVolatile)
        {
            if (isVolatile)
                this.il.Emit(OpCodes.Volatile);

            this.il.Emit(OpCodes.Cpblk);
            return;
        }

        /// <summary>
        ///将指向实现与指定对象关联的特定虚方法的本机代码的非托管指针（native int 类型）推送到计算堆栈上
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public virtual void Leave(ILabel label)
        {
            if (label.Owner != this)
                throw new ArgumentException(string.Concat(label, " is not owned by this iemit, so it can been used"));

            this.il.Emit(OpCodes.Leave, label.Label);
            return;
        }

        /// <summary>
        /// 用给定标签标记 Microsoft 中间语言 (MSIL) 流的当前位置。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public virtual void MarkLabel(ILabel label)
        {
            if (label.Owner != this)
                throw new ArgumentException(string.Concat(label, " is not owned by this iemit, so it can been used"));
            this.il.MarkLabel(label.Label);
            return;
        }

        /// <summary>
        /// 两个数相加
        /// </summary>
        /// <returns></returns>
        public virtual void Add()
        {
            this.il.Emit(OpCodes.Add);
            return;
        }

        /// <summary>
        /// 两个数相加并执行溢出检查
        /// </summary>
        /// <returns></returns>
        public virtual void AddOverflow()
        {
            this.il.Emit(OpCodes.Add_Ovf);
            return;
        }

        /// <summary>
        /// 【无符号】两个数相加并执行溢出检查
        /// </summary>
        /// <returns></returns>
        public virtual void UnsignedAddOverflow()
        {
            this.il.Emit(OpCodes.Add_Ovf_Un);
            return;
        }

        /// <summary>
        /// 两个数相除
        /// </summary>
        /// <returns></returns>
        public virtual void Divide()
        {
            this.il.Emit(OpCodes.Div);
            return;
        }

        /// <summary>
        /// 【无符号】两个数相除
        /// </summary>
        /// <returns></returns>
        public virtual void UnsignedDivide()
        {
            this.il.Emit(OpCodes.Div_Un);
            return;
        }

        /// <summary>
        /// 两个数相乘
        /// </summary>
        /// <returns></returns>
        public virtual void Multiply()
        {
            this.il.Emit(OpCodes.Mul);
            return;
        }

        /// <summary>
        /// 两个数相乘并执行溢出检查
        /// </summary>
        /// <returns></returns>
        public virtual void MultiplyOverflow()
        {
            this.il.Emit(OpCodes.Mul_Ovf);
            return;
        }

        /// <summary>
        /// 【无符号】两个数相乘并执行溢出检查
        /// </summary>
        /// <returns></returns>
        public virtual void UnsignedMultiplyOverflow()
        {
            this.il.Emit(OpCodes.Mul_Ovf_Un);
            return;
        }

        /// <summary>
        /// 两个数相除
        /// </summary>
        /// <returns></returns>
        public virtual void Remainder()
        {
            this.il.Emit(OpCodes.Rem);
            return;
        }

        /// <summary>
        /// 两个数相除余数
        /// </summary>
        /// <returns></returns>
        public virtual void UnsignedRemainder()
        {
            this.il.Emit(OpCodes.Rem_Un);
            return;
        }

        /// <summary>
        /// 两个数相减
        /// </summary>
        /// <returns></returns>
        public virtual void Subtract()
        {
            this.il.Emit(OpCodes.Sub);
            return;
        }

        /// <summary>
        /// 两个数相减并执行溢出检查
        /// </summary>
        /// <returns></returns>
        public virtual void SubtractOverflow()
        {
            this.il.Emit(OpCodes.Sub_Ovf);
            return;
        }

        /// <summary>
        /// 【无符号】两个数相减并执行溢出检查
        /// </summary>
        /// <returns></returns>
        public virtual void UnsignedSubtractOverflow()
        {
            this.il.Emit(OpCodes.Sub_Ovf_Un);
            return;
        }

        /// <summary>
        /// 一个值求反
        /// </summary>
        /// <returns></returns>
        public virtual void Negate()
        {
            this.il.Emit(OpCodes.Neg);
            return;
        }

        /// <summary>
        /// 装箱
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public virtual void Box(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            if (valueType == typeof(void))
                throw new ArgumentNullException("this type is void type");

            if (!TypeHelper.IsValueType(valueType))
                throw new ArgumentNullException("this type is not value type");

            this.il.Emit(OpCodes.Box, valueType);
            return;
        }

        /// <summary>
        /// 无条件地将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public virtual void Branch(ILabel label)
        {
            if (label.Owner != this)
                throw new ArgumentException(string.Concat(label, " is not owned by this iemit, so it can been used"));

            this.il.Emit(OpCodes.Br, label.Label);
            return;
        }

        /// <summary>
        /// 如果两个值相等，则将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public virtual void BranchIfEqual(ILabel label)
        {
            if (label.Owner != this)
                throw new ArgumentException(string.Concat(label, " is not owned by this iemit, so it can been used"));

            this.il.Emit(OpCodes.Beq, label.Label);
            return;
        }

        /// <summary>
        /// 如果两个值不相等，则将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public virtual void UnsignedBranchIfNotEqual(ILabel label)
        {
            if (label.Owner != this)
                throw new ArgumentException(string.Concat(label, " is not owned by this iemit, so it can been used"));

            this.il.Emit(OpCodes.Bne_Un, label.Label);
            return;
        }

        /// <summary>
        /// 如果 value 为 false、空引用（Visual Basic 中的 Nothing）或零，则将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public virtual void BranchIfFalse(ILabel label)
        {
            if (label.Owner != this)
                throw new ArgumentException(string.Concat(label, " is not owned by this iemit, so it can been used"));

            this.il.Emit(OpCodes.Brfalse, label.Label);
            return;
        }

        /// <summary>
        /// 如果 value 为 true、非空或非零，则将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public virtual void BranchIfTrue(ILabel label)
        {
            if (label.Owner != this)
                throw new ArgumentException(string.Concat(label, " is not owned by this iemit, so it can been used"));

            this.il.Emit(OpCodes.Brtrue, label.Label);
            return;
        }

        /// <summary>
        /// 如果第一个值大于第二个值，则将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public virtual void BranchIfGreater(ILabel label)
        {
            if (label.Owner != this)
                throw new ArgumentException(string.Concat(label, " is not owned by this iemit, so it can been used"));

            this.il.Emit(OpCodes.Bgt, label.Label);
            return;
        }

        /// <summary>
        /// 如果第一个值大于或等于第二个值，则将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public virtual void BranchIfGreaterOrEqual(ILabel label)
        {
            if (label.Owner != this)
                throw new ArgumentException(string.Concat(label, " is not owned by this iemit, so it can been used"));

            this.il.Emit(OpCodes.Bge, label.Label);
            return;
        }

        /// <summary>
        /// 如果第一个值小于第二个值，则将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public virtual void BranchIfLess(ILabel label)
        {
            if (label.Owner != this)
                throw new ArgumentException(string.Concat(label, " is not owned by this iemit, so it can been used"));

            this.il.Emit(OpCodes.Blt, label.Label);
            return;
        }

        /// <summary>
        /// 如果第一个值小于或等于第二个值，则将控制转移到目标指令。
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public virtual void BranchIfLessOrEqual(ILabel label)
        {
            if (label.Owner != this)
                throw new ArgumentException(string.Concat(label, " is not owned by this iemit, so it can been used"));

            this.il.Emit(OpCodes.Ble, label.Label);
            return;
        }

        /// <summary>
        /// 跳转表
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        public virtual void Switch(IList<ILabel> labels)
        {
            if (labels == null)
                return;

            var list = new List<Label>(labels.Count);
            foreach (var label in labels)
            {
                if (label.Owner != this)
                    throw new ArgumentException(string.Concat(label, " is not owned by this iemit, so it can been used"));
                list.Add(label.Label);
            }

            this.il.Emit(OpCodes.Switch, list.ToArray());
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="cons"></param>
        /// <returns></returns>
        public virtual void Call(ConstructorInfo cons)
        {
            if (cons == null)
                throw new ArgumentNullException("cons");

            this.il.Emit(OpCodes.Call, cons);
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public virtual void Call(MethodInfo method)
        {
            this.Call(method, null);
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="arglist">EmitCall 方法可发出对 varargs 方法的调用，因为指定可变变量的参数类型的 Emit 方法没有任何重载。</param>
        /// <returns></returns>
        public virtual void Call(MethodInfo method, Type[] arglist)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            if (arglist == null || arglist.Length == 0)
            {
                this.il.Emit(OpCodes.Call, method);
                return;
            }

            var parameters = method.GetParameters();
            var types = new List<Type>(parameters.Length);
            foreach (var i in parameters)
                types.Add(i.ParameterType);

            this.il.EmitCall(OpCodes.Call, method, types.ToArray());
            return;
        }

        /// <summary>
        /// 执行虚方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public virtual void CallVirtual(MethodInfo method)
        {
            this.CallVirtual(method, null, null);
        }

        /// <summary>
        /// 执行虚方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="constrained">是否对方法调用约束类型</param>
        /// <returns></returns>
        public virtual void CallVirtual(MethodInfo method, Type constrained)
        {
            this.CallVirtual(method, constrained, null);
        }

        /// <summary>
        /// 执行虚方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="constrained">是否对方法调用约束类型</param>
        /// <param name="arglist">EmitCall 方法可发出对 varargs 方法的调用，因为指定可变变量的参数类型的 Emit 方法没有任何重载。</param>
        /// <returns></returns>
        public virtual void CallVirtual(MethodInfo method, Type constrained, Type[] arglist)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            if (method.IsStatic)
                throw new ArgumentException("Only non-static methods can be called using CallVirtual");

            if (constrained != null)
                this.il.Emit(OpCodes.Constrained, constrained);

            if (arglist == null)
            {
                this.il.Emit(OpCodes.Callvirt, method);
                return;
            }

            var parameters = method.GetParameters();
            var types = new List<Type>(parameters.Length);
            foreach (var i in parameters)
                types.Add(i.ParameterType);

            this.il.EmitCall(OpCodes.Callvirt, method, types.ToArray());
            return;
        }

        /// <summary>
        /// 比较两个值。如果这两个值相等，则将整数值 1 (int32) 推送到计算堆栈上；否则，将 0 (int32) 推送到计算堆栈上。
        /// 如果与null比较，堆栈为null，则返回1，堆栈不为null，则返回0
        /// </summary>
        /// <returns></returns>
        public virtual void CompareEqual()
        {
            this.il.Emit(OpCodes.Ceq);
            return;
        }

        /// <summary>
        /// 比较两个值。如果第一个值大于第二个值，则将整数值 1 (int32) 推送到计算堆栈上；反之，将 0 (int32) 推送到计算堆栈上。
        /// 如果与null比较，则返回0
        /// </summary>
        /// <returns></returns>
        public virtual void CompareGreaterThan()
        {
            this.il.Emit(OpCodes.Cgt);
            return;
        }

        /// <summary>
        /// 【无符号】比较两个值。如果第一个值大于第二个值，则将整数值 1 (int32) 推送到计算堆栈上；反之，将 0 (int32) 推送到计算堆栈上。
        /// 如果与null比较，则返回0
        /// </summary>
        /// <returns></returns>
        public virtual void UnsignedCompareGreaterThan()
        {
            this.il.Emit(OpCodes.Cgt_Un);
            return;
        }

        /// <summary>
        /// 比较两个值。如果第一个值小于第二个值，则将整数值 1 (int32) 推送到计算堆栈上；反之，将 0 (int32) 推送到计算堆栈上。
        /// </summary>
        /// <returns></returns>
        public virtual void CompareLessThan()
        {
            this.il.Emit(OpCodes.Clt);
            return;
        }

        /// <summary>
        /// 【无符号】比较两个值。如果第一个值小于第二个值，则将整数值 1 (int32) 推送到计算堆栈上；反之，将 0 (int32) 推送到计算堆栈上。
        /// </summary>
        /// <returns></returns>
        public virtual void UnsignedCompareLessThan()
        {
            this.il.Emit(OpCodes.Clt_Un);
            return;
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="primitiveType"></param>
        /// <returns></returns>
        public virtual void Convert(Type primitiveType)
        {
            if (primitiveType == null)
                throw new ArgumentNullException("primitiveType can not be null");

            if (!TypeHelper.IsPrimitiveType(primitiveType))
                throw new ArgumentException("primitiveType is not primitive type");

            if (primitiveType == typeof(char))
                throw new ArgumentException("primitiveType can not be character type");

            if (primitiveType == typeof(byte))
            {
                this.il.Emit(OpCodes.Conv_U1);
                return;
            }

            if (primitiveType == (typeof(sbyte)))
            {
                this.il.Emit(OpCodes.Conv_I1);
                return;
            }

            if (primitiveType == typeof(bool))
            {
                this.il.Emit(OpCodes.Conv_I1);
                return;
            }

            if (primitiveType == typeof(short))
            {
                this.il.Emit(OpCodes.Conv_I2);
                return;
            }

            if (primitiveType == typeof(ushort))
            {
                this.il.Emit(OpCodes.Conv_U2);
                return;
            }

            if (primitiveType == typeof(int))
            {
                this.il.Emit(OpCodes.Conv_I4);
                return;
            }

            if (primitiveType == typeof(uint))
            {
                this.il.Emit(OpCodes.Conv_U4);
                return;
            }

            if (primitiveType == typeof(long))
            {
                this.il.Emit(OpCodes.Conv_I8);
                return;
            }

            if (primitiveType == typeof(ulong))
            {
                this.il.Emit(OpCodes.Conv_U8);
                return;
            }

            if (primitiveType == typeof(float))
            {
                this.il.Emit(OpCodes.Conv_R4);
                return;
            }

            if (primitiveType == typeof(double))
            {
                this.il.Emit(OpCodes.Conv_R8);
                return;
            }

            if (primitiveType == typeof(IntPtr))
            {
                this.il.Emit(OpCodes.Conv_I);
                return;
            }

            if (primitiveType == typeof(UIntPtr))
            {
                this.il.Emit(OpCodes.Conv_U);
                return;
            }

            return;
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="primitiveType"></param>
        /// <returns></returns>
        public virtual void ConvertOverflow(Type primitiveType)
        {
            if (primitiveType == null)
                throw new ArgumentNullException("primitiveType can not be null");

            if (!TypeHelper.IsPrimitiveType(primitiveType))
                throw new ArgumentException("primitiveType is not primitive type");

            if (primitiveType == typeof(char))
                throw new ArgumentException("primitiveType can not be character type");

            if (primitiveType == typeof(byte))
            {
                this.il.Emit(OpCodes.Conv_Ovf_U1);
                return;
            }

            if (primitiveType == (typeof(sbyte)))
            {
                this.il.Emit(OpCodes.Conv_Ovf_I1);
                return;
            }

            if (primitiveType == typeof(bool))
            {
                this.il.Emit(OpCodes.Conv_Ovf_I1);
                return;
            }

            if (primitiveType == typeof(short))
            {
                this.il.Emit(OpCodes.Conv_Ovf_I2);
                return;
            }

            if (primitiveType == typeof(ushort))
            {
                this.il.Emit(OpCodes.Conv_Ovf_U2);
                return;
            }

            if (primitiveType == typeof(int))
            {
                this.il.Emit(OpCodes.Conv_Ovf_I4);
                return;
            }

            if (primitiveType == typeof(uint))
            {
                this.il.Emit(OpCodes.Conv_Ovf_U4);
                return;
            }

            if (primitiveType == typeof(long))
            {
                this.il.Emit(OpCodes.Conv_Ovf_I8);
                return;
            }

            if (primitiveType == typeof(ulong))
            {
                this.il.Emit(OpCodes.Conv_Ovf_U8);
                return;
            }

            if (primitiveType == typeof(IntPtr))
            {
                this.il.Emit(OpCodes.Conv_Ovf_I);
                return;
            }

            if (primitiveType == typeof(UIntPtr))
            {
                this.il.Emit(OpCodes.Conv_Ovf_U);
                return;
            }

            return;
        }

        /// <summary>
        /// 类型转换
        /// </summary>
        /// <param name="primitiveType"></param>
        /// <returns></returns>
        public virtual void UnsignedConvertOverflow(Type primitiveType)
        {
            if (primitiveType == null)
                throw new ArgumentNullException("primitiveType can not be null");

            if (!TypeHelper.IsPrimitiveType(primitiveType))
                throw new ArgumentException("primitiveType is not primitive type");

            if (primitiveType == typeof(char))
                throw new ArgumentException("primitiveType can not be character type");

            if (primitiveType == typeof(byte))
            {
                this.il.Emit(OpCodes.Conv_Ovf_U1_Un);
                return;
            }

            if (primitiveType == (typeof(sbyte)))
            {
                this.il.Emit(OpCodes.Conv_Ovf_I1_Un);
                return;
            }

            if (primitiveType == typeof(bool))
            {
                this.il.Emit(OpCodes.Conv_Ovf_I1_Un);
                return;
            }

            if (primitiveType == typeof(short))
            {
                this.il.Emit(OpCodes.Conv_Ovf_I2_Un);
                return;
            }

            if (primitiveType == typeof(ushort))
            {
                this.il.Emit(OpCodes.Conv_Ovf_U2_Un);
                return;
            }

            if (primitiveType == typeof(int))
            {
                this.il.Emit(OpCodes.Conv_Ovf_I4_Un);
                return;
            }

            if (primitiveType == typeof(uint))
            {
                this.il.Emit(OpCodes.Conv_Ovf_U4_Un);
                return;
            }

            if (primitiveType == typeof(long))
            {
                this.il.Emit(OpCodes.Conv_Ovf_I8_Un);
                return;
            }

            if (primitiveType == typeof(ulong))
            {
                this.il.Emit(OpCodes.Conv_Ovf_U8_Un);
                return;
            }

            if (primitiveType == typeof(IntPtr))
            {
                this.il.Emit(OpCodes.Conv_Ovf_I_Un);
                return;
            }

            if (primitiveType == typeof(UIntPtr))
            {
                this.il.Emit(OpCodes.Conv_Ovf_U_Un);
                return;
            }
        }

        /// <summary>
        /// 再次引出异常
        /// </summary>
        /// <returns></returns>
        public virtual void ReturnThrow()
        {
            this.il.Emit(OpCodes.Rethrow);
            return;
        }

        /// <summary>
        /// 引发出常
        /// </summary>
        /// <returns></returns>
        public virtual void Throw()
        {
            this.il.Emit(OpCodes.Throw);
            return;
        }

        /// <summary>
        /// 加载参数
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual void LoadArgument(ushort index)
        {
            if (this.ParameterTypes == null || this.ParameterTypes.Length == 0)
                return;

            if (index >= this.ParameterTypes.Length)
                throw new ArgumentException("index must be between 0 and " + (this.ParameterTypes.Length - 1) + ", inclusive");

            switch (index)
            {
                case 0:
                    {
                        this.il.Emit(OpCodes.Ldarg_0);
                        return;
                    }
                case 1:
                    {
                        this.il.Emit(OpCodes.Ldarg_1);
                        return;
                    }
                case 2:
                    {
                        this.il.Emit(OpCodes.Ldarg_2);
                        return;
                    }
                case 3:
                    {
                        this.il.Emit(OpCodes.Ldarg_3);
                        return;
                    }
            }

            if (index >= byte.MinValue && index <= byte.MaxValue)
            {
                this.il.Emit(OpCodes.Ldarg_S, (byte)index);
                return;
            }

            this.il.Emit(OpCodes.Ldarg, (short)index);
            return;
        }

        /// <summary>
        /// 加载参数地址
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual void LoadArgumentAddress(ushort index)
        {
            if (this.ParameterTypes == null || this.ParameterTypes.Length == 0)
                return;

            if (index >= this.ParameterTypes.Length)
                throw new ArgumentException("index must be between 0 and " + (this.ParameterTypes.Length - 1) + ", inclusive");

            if (index >= byte.MinValue && index <= byte.MaxValue)
            {
                this.il.Emit(OpCodes.Ldarga_S, (byte)index);
                return;
            }

            this.il.Emit(OpCodes.Ldarga, (short)index);
            return;
        }

        /// <summary>
        /// 加载uint
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public virtual void LoadConstant(uint i)
        {
            switch (i)
            {
                case uint.MaxValue:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_M1);
                        return;
                    }
                case 0:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_0);
                        return;
                    }
                case 1:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_1);
                        return;
                    }
                case 2:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_2);
                        return;
                    }
                case 3:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_3);
                        return;
                    }
                case 4:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_4);
                        return;
                    }
                case 5:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_5);
                        return;
                    }
                case 6:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_6);
                        return;
                    }
                case 7:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_7);
                        return;
                    }
                case 8:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_8);
                        return;
                    }
            }

            if (i <= sbyte.MaxValue)
            {
                byte asByte;
                unchecked
                {
                    asByte = (byte)i;
                }
                this.il.Emit(OpCodes.Ldc_I4_S, (byte)i);
                return;
            }

            this.il.Emit(OpCodes.Ldc_I4, unchecked((int)i));
            return;
        }

        /// <summary>
        /// 加载int
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public virtual void LoadConstant(int i)
        {
            switch (i)
            {
                case -1:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_M1);
                        return;
                    }
                case 0:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_0);
                        return;
                    }
                case 1:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_1);
                        return;
                    }
                case 2:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_2);
                        return;
                    }
                case 3:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_3);
                        return;
                    }
                case 4:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_4);
                        return;
                    }
                case 5:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_5);
                        return;
                    }
                case 6:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_6);
                        return;
                    }
                case 7:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_7);
                        return;
                    }
                case 8:
                    {
                        this.il.Emit(OpCodes.Ldc_I4_8);
                        return;
                    }
            }

            if (i >= sbyte.MinValue && i <= sbyte.MaxValue)
            {
                this.il.Emit(OpCodes.Ldc_I4_S, (byte)i);
                return;
            }

            this.il.Emit(OpCodes.Ldc_I4, i);
            return;
        }

        /// <summary>
        /// 加载bool
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public virtual void LoadConstant(bool b)
        {
            this.LoadConstant(b ? 1 : 0);
        }

        /// <summary>
        /// 加载ulong
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public virtual void LoadConstant(ulong l)
        {
            this.il.Emit(OpCodes.Ldc_I8, unchecked((long)l));
        }

        /// <summary>
        /// 加载long
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public virtual void LoadConstant(long l)
        {
            this.il.Emit(OpCodes.Ldc_I8, l);
        }

        /// <summary>
        /// 加载浮点数
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public virtual void LoadConstant(float f)
        {
            this.il.Emit(OpCodes.Ldc_R4, f);
        }

        /// <summary>
        /// 加载double数
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public virtual void LoadConstant(double d)
        {
            this.il.Emit(OpCodes.Ldc_R8, d);
        }

        /// <summary>
        /// 加载字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public virtual void LoadConstant(string str)
        {
            this.il.Emit(OpCodes.Ldstr, str);
            return;
        }

        /// <summary>
        /// 将元数据标记转换为其运行时表示形式，并将其推送到计算堆栈
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public virtual void LoadConstant(FieldInfo field)
        {
            this.il.Emit(OpCodes.Ldtoken, field);
            return;
        }

        /// <summary>
        /// 将元数据标记转换为其运行时表示形式，并将其推送到计算堆栈
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public virtual void LoadConstant(MethodInfo method)
        {
            this.il.Emit(OpCodes.Ldtoken, method);
            return;
        }

        /// <summary>
        /// 将元数据标记转换为其运行时表示形式，并将其推送到计算堆栈
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual void LoadConstant(Type type)
        {
            this.il.Emit(OpCodes.Ldtoken, type);
            return;
        }

        /// <summary>
        /// 按照指令中指定的类型，将指定数组索引中的元素加载到计算堆栈的顶部。
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public virtual void LoadElement(Type elementType)
        {
            if (elementType == null)
                throw new ArgumentNullException("elementType");

            if (elementType.IsPointer)
            {
                this.il.Emit(OpCodes.Ldelem_I);
                return;
            }

            if (!TypeHelper.IsValueType(elementType))
            {
                this.il.Emit(OpCodes.Ldelem_Ref);
                return;
            }

            if (elementType == typeof(sbyte))
            {
                this.il.Emit(OpCodes.Ldelem_I1);
                return;
            }

            if (elementType == typeof(byte))
            {
                this.il.Emit(OpCodes.Ldelem_U1);
                return;
            }

            if (elementType == typeof(short))
            {
                this.il.Emit(OpCodes.Ldelem_I2);
                return;
            }

            if (elementType == typeof(ushort))
            {
                this.il.Emit(OpCodes.Ldelem_U2);
                return;
            }

            if (elementType == typeof(int))
            {
                this.il.Emit(OpCodes.Ldelem_I4);
                return;
            }

            if (elementType == typeof(uint))
            {
                this.il.Emit(OpCodes.Ldelem_U4);
                return;
            }

            if (elementType == typeof(long) || elementType == typeof(ulong))
            {
                this.il.Emit(OpCodes.Ldelem_I8);
                return;
            }

            if (elementType == typeof(float))
            {
                this.il.Emit(OpCodes.Ldelem_R4);
                return;
            }

            if (elementType == typeof(double))
            {
                this.il.Emit(OpCodes.Ldelem_R8);
                return;
            }

            this.il.Emit(OpCodes.Ldelem, elementType);
            return;
        }

        /// <summary>
        /// 将位于指定数组索引处的包含对象引用的元素作为 O 类型（对象引用）加载到计算堆栈的顶部。
        /// </summary>
        /// <returns></returns>
        public virtual void LoadElementReference()
        {
            this.il.Emit(OpCodes.Ldelem_Ref);
        }

        /// <summary>
        /// 将位于指定数组索引的数组元素的地址作为 &amp; 类型（托管指针）加载到计算堆栈的顶部。
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public virtual void LoadElementAddress(Type elementType)
        {
            this.il.Emit(OpCodes.Ldelema, elementType);
            return;
        }

        /// <summary>
        /// 查找对象中其引用当前位于计算堆栈的字段的值。
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public virtual void LoadField(FieldInfo field)
        {
            this.LoadField(field, false);
        }

        /// <summary>
        /// 查找对象中其引用当前位于计算堆栈的字段的值。
        /// </summary>
        /// <param name="field"></param>
        /// <param name="isVolatile"></param>
        /// <returns></returns>
        public virtual void LoadField(FieldInfo field, bool isVolatile)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            if (isVolatile)
                this.il.Emit(OpCodes.Volatile);

            if (!field.IsStatic)
                this.il.Emit(OpCodes.Ldfld, field);
            else
                this.il.Emit(OpCodes.Ldsfld, field);
            return;
        }

        /// <summary>
        /// 查找对象中其引用当前位于计算堆栈的字段的地址。
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public virtual void LoadFieldAddress(FieldInfo field)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            if (!field.IsStatic)
                this.il.Emit(OpCodes.Ldflda, field);
            else
                this.il.Emit(OpCodes.Ldsflda, field);
            return;
        }

        /// <summary>
        /// 加载临时变量
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        public virtual void LoadLocal(ILocal local)
        {
            if (local.Owner != this)
                throw new ArgumentException(string.Concat(local, " is not owned by this iemit, so it can been used"));

            switch (local.Index)
            {
                case 0:
                    {
                        this.il.Emit(OpCodes.Ldloc_0);
                        return;
                    }
                case 1:
                    {
                        this.il.Emit(OpCodes.Ldloc_1);
                        return;
                    }
                case 2:
                    {
                        this.il.Emit(OpCodes.Ldloc_2);
                        return;
                    }
                case 3:
                    {
                        this.il.Emit(OpCodes.Ldloc_3);
                        return;
                    }
            }

            if (local.Index >= sbyte.MinValue && local.Index <= sbyte.MaxValue)
            {
                this.il.Emit(OpCodes.Ldloc_S, (byte)local.Index);
                return;
            }

            this.il.Emit(OpCodes.Ldloc, local.LocalBuilder);
            return;
        }

        /// <summary>
        /// 加载临时变量地址
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        public virtual void LoadLocalAddress(ILocal local)
        {
            if (local.Owner != this)
                throw new ArgumentException(string.Concat(local, " is not owned by this iemit, so it can been used"));

            if (local.Index >= byte.MinValue && local.Index <= byte.MaxValue)
            {
                this.il.Emit(OpCodes.Ldloca_S, (byte)local.Index);
                return;
            }

            short asShort;
            unchecked
            {
                asShort = (short)local.Index;
            }

            this.il.Emit(OpCodes.Ldloca, asShort);
            return;
        }

        /// <summary>
        ///  将指向实现特定方法的本机代码的非托管指针（native int 类型）推送到计算堆栈上。
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public virtual void LoadFunctionPointer(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            this.il.Emit(OpCodes.Ldftn, method);
            return;
        }

        /// <summary>
        ///  将 native int等 类型的值作为 native int等 间接加载到计算堆栈上
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual void LoadIndirect(Type type)
        {
            this.LoadIndirect(type, false);
        }

        /// <summary>
        ///  将 native int等 类型的值作为 native int等 间接加载到计算堆栈上
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isVolatile"></param>
        /// <returns></returns>
        public virtual void LoadIndirect(Type type, bool isVolatile)
        {
            if (type == null)
                throw new ArgumentNullException("method");

            if (isVolatile)
                this.il.Emit(OpCodes.Volatile);

            if (type.IsPointer)
            {
                this.il.Emit(OpCodes.Ldind_I);
                return;
            }

            if (!TypeHelper.IsValueType(type))
            {
                this.il.Emit(OpCodes.Ldind_Ref);
                return;
            }

            if (type == typeof(sbyte))
            {
                this.il.Emit(OpCodes.Ldind_I1);
                return;
            }

            if (type == typeof(bool))
            {
                this.il.Emit(OpCodes.Ldind_I1);
                return;
            }

            if (type == typeof(byte))
            {
                this.il.Emit(OpCodes.Ldind_U1);
                return;
            }

            if (type == typeof(short))
            {
                this.il.Emit(OpCodes.Ldind_I2);
                return;
            }

            if (type == typeof(ushort))
            {
                this.il.Emit(OpCodes.Ldind_U2);
                return;
            }

            if (type == typeof(char))
            {
                this.il.Emit(OpCodes.Ldind_U2);
                return;
            }

            if (type == typeof(int))
            {
                this.il.Emit(OpCodes.Ldind_I4);
                return;
            }

            if (type == typeof(uint))
            {
                this.il.Emit(OpCodes.Ldind_U4);
                return;
            }

            if (type == typeof(long) || type == typeof(ulong))
            {
                this.il.Emit(OpCodes.Ldind_I8);
                return;
            }

            if (type == typeof(float))
            {
                this.il.Emit(OpCodes.Ldind_R4);
                return;
            }

            if (type == typeof(double))
            {
                this.il.Emit(OpCodes.Ldind_R8);
                return;
            }

            return;
        }

        /// <summary>
        /// 将从零开始的、一维数组的元素的数目推送到计算堆栈上
        /// </summary>
        /// <returns></returns>
        public virtual void LoadLength()
        {
            this.il.Emit(OpCodes.Ldlen);
            return;
        }

        /// <summary>
        /// 将地址指向的值类型对象复制到计算堆栈的顶部。
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public virtual void LoadObject(Type valueType)
        {
            this.LoadObject(valueType, false);
        }

        /// <summary>
        /// 将地址指向的值类型对象复制到计算堆栈的顶部。
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="isVolatile"></param>
        /// <returns></returns>
        public virtual void LoadObject(Type valueType, bool isVolatile)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            if (!TypeHelper.IsValueType(valueType))
                throw new ArgumentException("valueType must be a ValueType");

            if (isVolatile)
                this.il.Emit(OpCodes.Volatile);

            this.il.Emit(OpCodes.Ldobj, valueType);
            return;
        }

        /// <summary>
        /// 将指向实现与指定对象关联的特定虚方法的本机代码的非托管指针（native int 类型）推送到计算堆栈上。
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public virtual void LoadVirtualFunctionPointer(MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            this.il.Emit(OpCodes.Ldvirtftn, method);
            return;
        }

        /// <summary>
        /// 从本地动态内存池分配特定数目的字节并将第一个分配的字节的地址（瞬态指针，* 类型）推送到计算堆栈上
        /// </summary>
        /// <returns></returns>
        public virtual void LocalAllocate()
        {
            this.il.Emit(OpCodes.Localloc);
            return;
        }

        /// <summary>
        /// 将临时变量存储在参数中
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual void StoreArgument(ushort index)
        {
            if (this.ParameterTypes == null || this.ParameterTypes.Length == 0)
                return;

            if (index >= this.ParameterTypes.Length)
                throw new ArgumentException("index must be between 0 and " + (this.ParameterTypes.Length - 1) + ", inclusive");

            if (index >= byte.MinValue && index <= byte.MaxValue)
            {
                this.il.Emit(OpCodes.Starg_S, (byte)index);
                return;
            }

            short asShort;
            unchecked
            {
                asShort = (short)index;
            }

            this.il.Emit(OpCodes.Starg, asShort);
            return;
        }

        /// <summary>
        /// 将堆栈的值替换给定索引处的数组元素
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public virtual void StoreElement(Type elementType)
        {
            if (elementType == null)
                throw new ArgumentNullException("elementType");

            if (TypeHelper.IsPointer(elementType))
            {
                this.il.Emit(OpCodes.Stelem_I);
                return;
            }

            if (!TypeHelper.IsValueType(elementType))
            {
                this.il.Emit(OpCodes.Stelem_Ref);
                return;
            }

            if (elementType == typeof(sbyte) || elementType == typeof(byte))
            {
                this.il.Emit(OpCodes.Stelem_I1);
                return;
            }

            if (elementType == typeof(short) || elementType == typeof(ushort))
            {
                this.il.Emit(OpCodes.Stelem_I2);
                return;
            }

            if (elementType == typeof(int) || elementType == typeof(uint))
            {
                this.il.Emit(OpCodes.Stelem_I4);
                return;
            }

            if (elementType == typeof(long) || elementType == typeof(ulong))
            {
                this.il.Emit(OpCodes.Stelem_I8);
                return;
            }

            if (elementType == typeof(float))
            {
                this.il.Emit(OpCodes.Stelem_R4);
                return;
            }

            if (elementType == typeof(double))
            {
                this.il.Emit(OpCodes.Stelem_R8);
                return;
            }

            this.il.Emit(OpCodes.Stelem, elementType);
            return;
        }

        /// <summary>
        /// 将堆栈的值替换给定索引处的数组元素
        /// </summary>
        /// <returns></returns>
        public virtual void StoreElementReference()
        {
            this.il.Emit(OpCodes.Stelem_Ref);
            return;
        }

        /// <summary>
        /// 将堆栈的值替换静态字段
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public virtual void StoreField(FieldInfo field)
        {
            this.StoreField(field, false);
        }

        /// <summary>
        /// 将堆栈的值替换静态字段
        /// </summary>
        /// <param name="field"></param>
        /// <param name="isVolatile"></param>
        /// <returns></returns>
        public virtual void StoreField(FieldInfo field, bool isVolatile)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            if (isVolatile)
                this.il.Emit(OpCodes.Volatile);

            if (field.IsStatic)
                this.il.Emit(OpCodes.Stsfld, field);
            else
                this.il.Emit(OpCodes.Stfld, field);

            return;
        }

        /// <summary>
        /// 存储在局部变量中
        /// </summary>
        /// <param name="local"></param>
        /// <returns></returns>
        public virtual void StoreLocal(ILocal local)
        {
            if (local.Owner != this)
                throw new ArgumentException(string.Concat(local, " is not owned by this iemit, so it can been used"));

            switch (local.Index)
            {
                case 0:
                    {
                        this.il.Emit(OpCodes.Stloc_0);
                        return;
                    }
                case 1:
                    {
                        this.il.Emit(OpCodes.Stloc_1);
                        return;
                    }
                case 2:
                    {
                        this.il.Emit(OpCodes.Stloc_2);
                        return;
                    }
                case 3:
                    {
                        this.il.Emit(OpCodes.Stloc_3);
                        return;
                    }
            }

            if (local.Index >= byte.MinValue && local.Index <= byte.MaxValue)
            {
                this.il.Emit(OpCodes.Stloc_S, (byte)local.Index);
                return;
            }

            return;
        }

        /// <summary>
        /// 复制到内存中
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public virtual void StoreObject(Type valueType)
        {
            this.StoreObject(valueType);
        }

        /// <summary>
        /// 复制到内存中
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="isVolatile"></param>
        /// <returns></returns>
        public virtual void StoreObject(Type valueType, bool isVolatile)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            if (isVolatile)
            {
                this.il.Emit(OpCodes.Volatile);
            }

            this.il.Emit(OpCodes.Stobj, valueType);
            return;
        }

        /// <summary>
        /// 存储在局部变量中
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual void StoreIndirect(Type type)
        {
            this.StoreIndirect(type, false);
        }

        /// <summary>
        /// 存储在局部变量中
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isVolatile"></param>
        /// <returns></returns>
        public virtual void StoreIndirect(Type type, bool isVolatile)
        {
            if (type == null)
                throw new ArgumentNullException("valueType");

            if (isVolatile)
                this.il.Emit(OpCodes.Volatile);

            if (TypeHelper.IsPointer(type))
            {
                this.il.Emit(OpCodes.Stind_I);
                return;
            }

            if (!TypeHelper.IsValueType(type))
            {
                this.il.Emit(OpCodes.Stind_Ref);
                return;
            }

            if (type == typeof(sbyte) || type == typeof(byte))
            {
                this.il.Emit(OpCodes.Stind_I1);
                return;
            }

            if (type == typeof(short) || type == typeof(ushort))
            {
                this.il.Emit(OpCodes.Stind_I2);
                return;
            }

            if (type == typeof(int) || type == typeof(uint))
            {
                this.il.Emit(OpCodes.Stind_I4);
                return;
            }

            if (type == typeof(long) || type == typeof(ulong))
            {
                this.il.Emit(OpCodes.Stind_I8);
                return;
            }

            if (type == typeof(float))
            {
                this.il.Emit(OpCodes.Stind_R4);
                return;
            }

            if (type == typeof(double))
            {
                this.il.Emit(OpCodes.Stind_R8);
                return;
            }

            return;
        }

        /// <summary>
        /// 拆箱
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public virtual void Unbox(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            if (valueType.IsByRef || TypeHelper.IsPointer(valueType))
                throw new ArgumentException("UnboxAny cannot operate on pointers, found " + valueType);

            if (valueType == typeof(void))
                throw new ArgumentException("Void cannot be boxed, and thus cannot be unboxed");

            this.il.Emit(OpCodes.Unbox, valueType);
            return;
        }

        /// <summary>
        /// 拆箱
        /// </summary>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public virtual void UnboxAny(Type valueType)
        {
            if (valueType == null)
                throw new ArgumentNullException("valueType");

            if (valueType.IsByRef || TypeHelper.IsPointer(valueType))
                throw new ArgumentException("UnboxAny cannot operate on pointers, found " + valueType);

            if (valueType == typeof(void))
                throw new ArgumentException("Void cannot be boxed, and thus cannot be unboxed");

            this.il.Emit(OpCodes.Unbox_Any, valueType);
            return;
        }

        #endregion opcodes
    }
}