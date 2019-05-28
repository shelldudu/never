using System;

namespace Never.Aop
{
    /// <summary>
    /// 默认运行方式，不同环境应该有不同的运行方式，所有标准应该为：提供的值与当前相同（提供值为空则默认为相同）则工作，否则应视为不工作
    /// </summary>
    public class DefaultRuntimeMode : IRuntimeMode, IEquatable<DefaultRuntimeMode>
    {
        /// <summary>
        /// 运行方式
        /// </summary>
        public string RuntimeMode { get; set; }

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(DefaultRuntimeMode other)
        {
            return other != null && ObjectExtension.IsEquals(this.RuntimeMode, other.RuntimeMode);
        }
    }
}