using System.Collections.Generic;

namespace Never.Aop.DynamicProxy.Builders
{
    /// <summary>
    /// 构建T对象的虚拟实现
    /// </summary>
    public class IllusiveMockBuilder<Target> : IllusiveMockBuilder
    {
        #region ctor

        internal IllusiveMockBuilder(List<MockSetup> rules) : base(rules, typeof(Target))
        {
        }

        #endregion ctor
    }
}