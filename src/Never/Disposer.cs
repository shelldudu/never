using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never
{
    /// <summary>
    /// 垃圾处理者
    /// </summary>
    public sealed class Disposer : IDisposable
    {
        #region field

        /// <summary>
        /// 释放对象集合
        /// </summary>
        private readonly List<IDisposable> collection = null;

        /// <summary>
        /// 是否已经释放了
        /// </summary>
        private bool isDisposed = false;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="disposables"></param>
        public Disposer(IEnumerable<IDisposable> disposables)
        {
            this.collection = new List<IDisposable>(disposables);
        }

        /// <summary>
        ///
        /// </summary>
        public Disposer()
        {
            this.collection = new List<IDisposable>();
        }

        /// <summary>
        ///
        /// </summary>
        public Disposer(int capacity)
        {
            this.collection = new List<IDisposable>(capacity);
        }

        #endregion ctor

        #region add

        /// <summary>
        /// 加入集合
        /// </summary>
        /// <param name="disposer"></param>
        /// <returns></returns>
        public Disposer Push(object disposer)
        {
            return this.Push(disposer as IDisposable);
        }

        /// <summary>
        /// 加入集合
        /// </summary>
        /// <param name="disposer"></param>
        /// <returns></returns>
        public Disposer Push(IDisposable disposer)
        {
            if (disposer != null)
                collection.Add(disposer as IDisposable);

            return this;
        }

        #endregion add

        #region IDisposable

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (isDisposed)
                return;

            foreach (var d in this.collection)
                d.Dispose();

            this.collection.Clear();
            isDisposed = true;
        }

        #endregion IDisposable
    }
}