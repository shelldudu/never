using System.Collections.Generic;

namespace Never
{
    /// <summary>
    /// 树节点
    /// </summary>
    public interface ITreeNode
    {
        /// <summary>
        /// 节点
        /// </summary>
        int Id { get; }

        /// <summary>
        /// 父节点
        /// </summary>
        int ParentId { get; }

        /// <summary>
        /// 深度层次
        /// </summary>
        int Level { get; set; }
    }

    /// <summary>
    /// 树节点
    /// </summary>
    public interface ITreeNode<TNode> : ITreeNode where TNode : ITreeNode
    {
        /// <summary>
        /// 父节点
        /// </summary>
        TNode Parent { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        IEnumerable<TNode> Children { get; set; }
    }
}