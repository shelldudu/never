using Never.Aop.DynamicProxy.Builders;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace Never.Aop.DynamicProxy
{
    /// <summary>
    /// 创建一个T对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Mock<T>
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="Mock{T}"/> class.
        /// </summary>
        public Mock()
        {
        }

        #endregion ctor

        #region field

        private readonly List<MockSetup> rules = new List<MockSetup>(1);

        #endregion field

        #region create illusive

        /// <summary>
        /// 执行对象某个不带返回值的方法
        /// </summary>
        /// <param name="expression"></param>
        public IMockSetup<T, TResult> Setup<TResult>(Expression<Func<T, TResult>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression is null");

            if (expression.Body is ParameterExpression)
                throw new ArgumentException("expression body is parameter");

            var memberExpress = expression.Body as MemberExpression;
            if (memberExpress == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                if (unaryExpression != null)
                    memberExpress = unaryExpression.Operand as MemberExpression;
            }

            if (memberExpress == null)
            {
                var methodExpress = expression.Body as MethodCallExpression;
                if (methodExpress == null)
                    throw new ArgumentException("expression cannot support");

                var methodSetup = new MockSetup<T, TResult>();
                methodSetup.Method = methodExpress.Method;

                this.rules.Add(methodSetup);
                return methodSetup;
            }

            var mockSetup = new MockSetup<T, TResult>();
            mockSetup.Field = memberExpress.Member as FieldInfo;
            mockSetup.Property = memberExpress.Member as PropertyInfo;
            mockSetup.Method = memberExpress.Member as MethodInfo;

            this.rules.Add(mockSetup);
            return mockSetup;
        }

        /// <summary>
        /// 执行对象某个不带返回值的方法
        /// </summary>
        /// <param name="expression"></param>
        public IMockSetup<T> Setup(Expression<Action<T>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression is null");

            if (expression.Body is ParameterExpression)
                throw new ArgumentException("expression body is parameter");

            var memberExpress = expression.Body as MemberExpression;
            if (memberExpress == null)
            {
                var unaryExpression = expression.Body as UnaryExpression;
                if (unaryExpression != null)
                    memberExpress = unaryExpression.Operand as MemberExpression;
            }

            if (memberExpress == null)
            {
                var methodExpress = expression.Body as MethodCallExpression;
                if (methodExpress == null)
                    throw new ArgumentException("expression cannot support");

                var methodSetup = new MockSetup<T, object>();
                methodSetup.Method = methodExpress.Method;

                this.rules.Add(methodSetup);
                return methodSetup;
            }

            var mockSetup = new MockSetup<T, object>();
            mockSetup.Field = memberExpress.Member as FieldInfo;
            mockSetup.Property = memberExpress.Member as PropertyInfo;
            mockSetup.Method = memberExpress.Member as MethodInfo;

            this.rules.Add(mockSetup);
            return mockSetup;
        }

        /// <summary>
        /// 用于创建一个虚假的对象，用于配合Setup等方法
        /// </summary>
        /// <returns></returns>
        public virtual T CreateIllusive()
        {
            var type = typeof(T);
            /*构建接口，较为容易*/
            if (type.IsVisible && type.IsInterface)
                type = new IllusiveMockBuilder<T>(this.rules).BuildInterface(type);
            else
                type = new IllusiveMockBuilder<T>(this.rules).BuildClass(type);

            return (T)Activator.CreateInstance(type);
        }

        #endregion create illusive

        #region create proxy

        /// <summary>
        /// 创建代理对象，T为接口是最佳条件，T若为class,只能重写virtual方法
        /// </summary>
        /// <param name="source">代理对象</param>
        /// <returns></returns>
        public virtual T CreateProxy(T source)
        {
            return this.CreateProxy(source, new InterceptCompileSetting(), new IInterceptor[0]);
        }

        /// <summary>
        /// 创建代理对象，T为接口是最佳条件，T若为class,只能重写virtual方法
        /// </summary>
        /// <param name="setting">配置</param>
        /// <param name="source">代理对象</param>
        /// <returns></returns>
        public virtual T CreateProxy(T source, InterceptCompileSetting setting)
        {
            return this.CreateProxy(source, setting, new IInterceptor[0]);
        }

        /// <summary>
        /// 创建代理对象，T为接口是最佳条件，T若为class,只能重写virtual方法
        /// </summary>
        /// <param name="source">代理对象</param>
        /// <param name="interceptors">所实现的拦截器接口</param>
        /// <returns></returns>
        public virtual T CreateProxy(T source, IInterceptor[] interceptors)
        {
            return this.CreateProxy(source, new InterceptCompileSetting(), interceptors);
        }

        /// <summary>
        /// 创建代理对象，T为接口是最佳条件，T若为class,只能重写virtual方法
        /// </summary>
        /// <param name="source">代理对象</param>
        /// <param name="setting">配置</param>
        /// <param name="interceptors">所实现的拦截器接口</param>
        /// <returns></returns>
        public virtual T CreateProxy(T source, InterceptCompileSetting setting, IInterceptor[] interceptors)
        {
            if (interceptors == null)
                interceptors = new IInterceptor[0];

            var types = new Type[interceptors.Length];
            for (var i = 0; i < interceptors.Length; i++)
                types[i] = interceptors[i].GetType();

            return ProxyMockBuilder<T>.Register(source, setting, types).Invoke(source, interceptors);
        }

        #endregion create proxy
    }
}