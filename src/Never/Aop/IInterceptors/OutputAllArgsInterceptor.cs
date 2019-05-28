using System;

namespace Never.Aop.IInterceptors
{
    /// <summary>
    /// 控制端输出的拦截器
    /// </summary>
    public sealed class OutputAllArgsInterceptor : StandardInterceptor, IInterceptor
    {
        /// <summary>
        /// 对方法进行调用后
        /// </summary>
        /// <param name="invocation"></param>
        public override void PostProceed(IInvocation invocation)
        {
            if (invocation == null)
                return;

#if DEBUG
            Console.WriteLine("after excuting info:");

            /*arguments*/
            Console.WriteLine("all method arguments are ");
            if (invocation.Arguments != null && invocation.Arguments.Length > 0)
            {
                for (var i = 0; i < invocation.Arguments.Length; i++)
                {
                    if (invocation.Arguments[i].Value == null)
                        Console.WriteLine(string.Format("{0}.key = {1},value = null ", (i + 1).ToString(), invocation.Arguments[i].Key));
                    else
                        Console.WriteLine(string.Format("{0}.key = {1},value = {2}", (i + 1).ToString(), invocation.Arguments[i].Key, invocation.Arguments[i].Value.ToString()));
                }
            }
            else
            {
                Console.WriteLine("empty");
            }
#endif
        }

        /// <summary>
        /// 在对方法进行调用前
        /// </summary>
        /// <param name="invocation"></param>
        public override void PreProceed(IInvocation invocation)
        {
            if (invocation == null)
                return;
#if DEBUG
            Console.WriteLine("before excuting info:");
            Console.WriteLine(string.Concat("proxy type is ", invocation.ProxyType.FullName));
            Console.WriteLine(string.Concat("excuting method name is ", invocation.Method.Name));

            /*interfaces*/
            Console.WriteLine("all proxy interfaces are ");
            if (invocation.Interfaces != null && invocation.Interfaces.Length > 0)
            {
                for (var i = 0; i < invocation.Interfaces.Length; i++)
                {
                    Console.WriteLine(string.Format("{0}.{1}", (i + 1).ToString(), invocation.Interfaces[i].FullName));
                }
            }
            else
            {
                Console.WriteLine("empty");
            }

            /*attributes*/
            Console.WriteLine("all proxy attributes are ");
            if (invocation.ProxyAttributes != null && invocation.ProxyAttributes.Length > 0)
            {
                for (var i = 0; i < invocation.ProxyAttributes.Length; i++)
                {
                    Console.WriteLine(string.Format("{0}.{1}", (i + 1).ToString(), invocation.ProxyAttributes[i].ToString()));
                }
            }
            else
            {
                Console.WriteLine("empty");
            }

            /*attributes*/
            Console.WriteLine("all method attributes are ");
            if (invocation.MethodAttributes != null && invocation.MethodAttributes.Length > 0)
            {
                for (var i = 0; i < invocation.MethodAttributes.Length; i++)
                {
                    Console.WriteLine(string.Format("{0}.{1}", (i + 1).ToString(), invocation.MethodAttributes[i].ToString()));
                }
            }
            else
            {
                Console.WriteLine("empty");
            }

            /*arguments*/
            Console.WriteLine("all method arguments are ");
            if (invocation.Arguments != null && invocation.Arguments.Length > 0)
            {
                for (var i = 0; i < invocation.Arguments.Length; i++)
                {
                    if (invocation.Arguments[i].Value == null)
                        Console.WriteLine(string.Format("{0}.key = {1},value = null ", (i + 1).ToString(), invocation.Arguments[i].Key));
                    else
                        Console.WriteLine(string.Format("{0}.key = {1},value = {2}", (i + 1).ToString(), invocation.Arguments[i].Key, invocation.Arguments[i].Value.ToString()));
                }
            }
            else
            {
                Console.WriteLine("empty");
            }
#endif
        }
    }
}