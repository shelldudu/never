using System;
using System.Runtime.Serialization;

namespace Never.Web.Sessions
{
    /// <summary>
    /// 存储Session对象模型
    /// </summary>
    [Serializable]
    [DataContract]
    public class SessionStateList
    {
        #region property

        /// <summary>
        /// 还有多少时间过期(以分钟为单位）
        /// </summary>
        [DataMember(Name = "ExpireDate", Order = 1)]
        public DateTime ExpireDate { get; set; }

        /// <summary>
        /// session列表
        /// </summary>

        [DataMember(Name = "SessionModels", Order = 2)]
        public SessionState[] SessionModels { get; set; }

        #endregion property

        #region ctor

        /// <summary>
        ///
        /// </summary>
        public SessionStateList()
        {
            this.ExpireDate = DateTime.Now;
            this.SessionModels = new SessionState[0];
        }

        #endregion ctor
    }
}