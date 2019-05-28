using System;
using System.Runtime.Serialization;

namespace Never.Web.Sessions
{
    /// <summary>
    /// 存储Session对象模型
    /// </summary>
    [Serializable]
    [DataContract]
    public class SessionState
    {
        #region property

        /// <summary>
        /// SessionId
        /// </summary>
        [DataMember(Name = "SessionId", Order = 1)]
        public string SessionId { get; set; }

        /// <summary>
        /// 当前域
        /// </summary>
        [DataMember(Name = "ApplicationName", Order = 2)]
        public string ApplicationName { get; set; }

        /// <summary>
        /// 锁Id
        /// </summary>
        [DataMember(Name = "LockId", Order = 3)]
        public long LockId { get; set; }

        /// <summary>
        /// 是否锁住当前记录
        /// </summary>
        [DataMember(Name = "IsLocked", Order = 4)]
        public bool IsLocked { get; set; }

        /// <summary>
        /// 加锁时间
        /// </summary>
        [DataMember(Name = "LockDate", Order = 5)]
        public DateTime LockDate { get; set; }

        /// <summary>
        /// 过期时间(以分钟为单位）
        /// </summary>
        [DataMember(Name = "Timeout", Order = 6)]
        public int Timeout { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [DataMember(Name = "ExpireDate", Order = 7)]
        public DateTime ExpireDate { get; set; }

        /// <summary>
        /// session存储的内容
        /// </summary>
        [DataMember(Name = "SessionText", Order = 8)]
        public string SessionText { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember(Name = "CreateDate", Order = 9)]
        public DateTime CreateDate { get; set; }

        /// <summary>
        ///
        /// </summary>
        [DataMember(Name = "ActionFlag", Order = 10)]
        public SessionStateActions ActionFlag { get; set; }

        #endregion property

        #region ctor

        /// <summary>
        ///
        /// </summary>
        public SessionState()
        {
            this.CreateDate = DateTime.Now;
            this.LockDate = DateTime.Now;
            this.ExpireDate = DateTime.Now.AddMinutes(20);
            this.ActionFlag = SessionStateActions.None;
        }

        #endregion ctor
    }
}