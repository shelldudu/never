namespace Never.Collections
{
    /// <summary>
    /// 简单的双向链表
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public sealed class DoubleTrackNode<T>
    {
        #region

        /// <summary>
        /// 当前节点对象
        /// </summary>
        public T Current;

        /// <summary>
        /// 下一节点对象
        /// </summary>
        public DoubleTrackNode<T> NextNode;

        /// <summary>
        /// 前一节点对象
        /// </summary>
        public DoubleTrackNode<T> PreNode;

        #endregion

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleTrackNode{T}"/> class.
        /// </summary>
        /// <param name="item">节点对象</param>
        public DoubleTrackNode(T item)
            : this(item, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleTrackNode{T}"/> class.
        /// </summary>
        /// <param name="item">节点对象</param>
        /// <param name="prev">前一节</param>
        /// <param name="next">下一节</param>
        public DoubleTrackNode(T item, DoubleTrackNode<T> prev, DoubleTrackNode<T> next)
        {
            this.Current = item;
            this.PreNode = prev;
            this.NextNode = next;
        }

        #endregion
    }
}