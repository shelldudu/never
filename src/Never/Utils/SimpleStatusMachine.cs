using Never.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Never.Utils
{
    /// <summary>
    /// 状态机处理
    /// </summary>
    /// <typeparam name="T">操作数据类型</typeparam>
    public class SimpleStatusMachine<T> where T : struct, IConvertible
    {
        #region field

        /// <summary>
        /// 链表
        /// </summary>
        private readonly DoubleTrackNode<Helper> link = null;

        /// <summary>
        /// 当前节点
        /// </summary>
        private DoubleTrackNode<Helper> currentNode = null;

        #endregion field

        #region helper

        /// <summary>
        /// 辅助类
        /// </summary>
        private struct Helper : IEquatable<Helper>
        {
            /// <summary>
            /// 节点
            /// </summary>
            public T Node;

            /// <summary>
            /// 是否可以回退
            /// </summary>
            public bool CanReturnBack;

            /// <summary>
            /// Initializes a new instance of the <see cref="Helper"/> struct.
            /// </summary>
            /// <param name="node">The node.</param>
            /// <param name="canreturnBack">if set to <c>true</c> [canreturn back].</param>
            public Helper(T node, bool canreturnBack)
            {
                this.Node = node;
                this.CanReturnBack = canreturnBack;
            }

            /// <summary>
            /// Equalses the specified other.
            /// </summary>
            /// <param name="other">The other.</param>
            /// <returns></returns>
            public bool Equals(Helper other)
            {
                return this.Node.GetHashCode() == other.Node.GetHashCode();
            }
        }

        #endregion helper

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleStatusMachine{T}"/> class.
        /// </summary>
        /// <param name="root">第一状态.</param>
        public SimpleStatusMachine(T root)
        {
            this.link = new DoubleTrackNode<Helper>(new Helper(root, true));
            this.currentNode = this.link;
        }

        #endregion ctor

        #region utils

        /// <summary>
        /// 移动到下一节点
        /// </summary>
        /// <param name="next">下一节点</param>
        public SimpleStatusMachine<T> Next(T next)
        {
            return this.Next(next, true);
        }

        /// <summary>
        /// 移动到下一节点
        /// </summary>
        /// <param name="next">下一节点</param>
        /// <param name="canReturnBack">是否可以回退</param>
        /// <returns></returns>
        public SimpleStatusMachine<T> Next(T next, bool canReturnBack)
        {
            var newNode = new Helper(next, canReturnBack);

            if (!newNode.Equals(this.link.Current))
            {
                var pr = new DoubleTrackNode<Helper>(newNode);
                pr.PreNode = this.currentNode;
                this.currentNode.NextNode = pr;
                this.currentNode = pr;
            }

            return this;
        }

        #endregion utils

        #region 返回状态

        /// <summary>
        /// 返回当前状态前面与后面相近的状态
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        public IEnumerable<T> GetCanHandlerStatus(T node)
        {
            ICollection<T> result = new List<T>(2);
            var current = this.link;
            var newNode = new Helper(node, true);
            while (current != null)
            {
                if (current.Current.Equals(newNode))
                {
                    result.Add(newNode.Node);

                    if (current.PreNode != null && current.Current.CanReturnBack)
                        result.Add(current.PreNode.Current.Node);

                    if (current.NextNode != null)
                        result.Add(current.NextNode.Current.Node);

                    break;
                }

                current = current.NextNode;
            }

            return result;
        }

        /// <summary>
        /// 是否可以执行当前状态切换到下个状态
        /// </summary>
        /// <param name="current">当前状态</param>
        /// <param name="expect">期望下一状态</param>
        /// <returns></returns>
        public bool CanSwap(T current, T expect)
        {
            var currentValue = current.ToInt32(CultureInfo.InvariantCulture);
            var expectValue = expect.ToInt32(CultureInfo.InvariantCulture);

            if (currentValue == expectValue)
                return true;
            var list = this.GetCanHandlerStatus(current) as List<T>;
            if (list == null || list.Count == 0)
                return false;

            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].ToInt32(CultureInfo.InvariantCulture) == expectValue)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 执行当前状态切换到下个状态
        /// </summary>
        /// <param name="current">当前状态</param>
        /// <param name="callback">改变当前状态的回调</param>
        public void Move(T current, Func<T> callback)
        {
            Exception execException = null;
            bool success = true;
            T execResult = default(T);

            try
            {
                execResult = callback.Invoke();
                if (this.CanSwap(current, execResult))
                    return;
                else
                    success = false;
            }
            catch (Exception ex)
            {
                execException = ex;
            }

            if (execException != null)
                throw new Exception(string.Format("执行方法method{0}出错了", callback.Method.Name), execException);

            if (!success)
                throw new Exception(string.Format("将状态{0}切换到另一状态{1}失败了", current.ToString(), execResult.ToString()));
        }

        #endregion 返回状态
    }
}