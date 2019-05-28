namespace Never.Collections
{
    /// <summary>
    /// 单向链表
    /// </summary>
    /// <typeparam name="T">操作的数据</typeparam>
    public sealed class SingleTrackNode<T>
    {
        #region

        /// <summary>
        /// 当前节点对象
        /// </summary>
        public T Current;

        /// <summary>
        /// 下一节点对象
        /// </summary>
        public SingleTrackNode<T> NextNode;

        /// <summary>
        /// 当前链接点
        /// </summary>
        private SingleTrackNode<T> current = null;

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleTrackNode{T}"/> class.
        /// </summary>
        /// <param name="current">The current.</param>
        public SingleTrackNode(T current)
            : this(current, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleTrackNode{T}"/> class.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="next">The next.</param>
        public SingleTrackNode(T current, SingleTrackNode<T> next)
        {
            this.Current = current;
            this.NextNode = next;
        }

        #endregion

        #region move

        /// <summary>
        /// 移动到下一节点
        /// </summary>
        /// <param name="item">操作的数据</param>
        /// <returns></returns>
        public SingleTrackNode<T> Next(T item)
        {
            if (this.NextNode == null)
            {
                this.NextNode = new SingleTrackNode<T>(item);
                this.current = this.NextNode;
                return this;
            }

            this.current.NextNode = new SingleTrackNode<T>(item);
            if (this.current.NextNode != null)
                this.current = this.current.NextNode;

            return this;
        }

        /// <summary>
        /// 形成一个环形
        /// </summary>
        /// <param name="item">操作的数据</param>
        public void Recyle(T item)
        {
            this.Recyle(new SingleTrackNode<T>(item));
        }

        /// <summary>
        /// 变成环形
        /// </summary>
        /// <param name="last">最后一个</param>
        public void Recyle(SingleTrackNode<T> last)
        {
            if (last == null)
                return;

            this.current.NextNode = last;
            if (last.NextNode != null)
                this.current = last.NextNode;

            last.NextNode = this;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            this.NextNode = null;
            this.current = null;
        }

        #endregion
    }
}