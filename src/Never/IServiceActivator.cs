using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never
{
    /// <summary>
    /// 对象创建者
    /// </summary>
    public interface IServiceActivator
    {
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        ActivatorService<TService> CreateService<TService>();

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        ActivatorService<object> CreateService(Type serviceType);
    }

    /// <summary>
    /// 对象创建结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ActivatorService<T>
    {
        /// <summary>
        ///
        /// </summary>
        public T Value { get; }

        /// <summary>
        ///
        /// </summary>
        public Disposer Disposer { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="disposer"></param>
        public ActivatorService(T value, Disposer disposer)
        {
            this.Value = value;
            this.Disposer = disposer;
        }
    }
}