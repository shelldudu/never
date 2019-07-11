#if !NET461
#else

using Never.Caching;
using Never.IoC;
using Never.Utils;
using Never.Web.Caching;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;

namespace Never.Web.Sessions
{
    /// <summary>
    ///
    /// </summary>
    public class SessionStateStoreProvider : System.Web.SessionState.SessionStateStoreProviderBase
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private readonly ICaching cache = null;

        /// <summary>
        ///
        /// </summary>
        private readonly SessionStateSection sessionCfg = null;

        /// <summary>
        ///
        /// </summary>
        private static readonly string applicationName = "State.Login";

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        public SessionStateStoreProvider() : this(new MemoryCache(), null)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="sessionCfg">etc:use (SessionStateSection)AppConfig.GetSection("system.web/sessionState")</param>
        public SessionStateStoreProvider(ICaching cache, SessionStateSection sessionCfg)
        {
            this.cache = cache;
            this.sessionCfg = sessionCfg ?? new SessionStateSection() { Timeout = TimeSpan.FromHours(2) };
        }

        #endregion ctor

        #region 初始化

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
        }

        #endregion 初始化

        #region SessionStateStoreProviderBase成员

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// <remarks>
        /// checked
        /// </remarks>
        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            return new SessionStateStoreData(new SessionStateItemCollection(), SessionStateUtility.GetSessionStaticObjects(context), timeout);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <param name="timeout"></param>
        /// <remarks>
        /// checked
        /// </remarks>
        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            var model = new SessionStateList()
            {
                SessionModels = new SessionState[1]
                {
                    new SessionState()
                    {
                        ApplicationName = applicationName,
                        ExpireDate = DateTime.Now.AddMinutes(timeout),
                        LockId = 0,
                        SessionId = id,
                        SessionText = "",
                        IsLocked = false,
                        Timeout = timeout,
                        ActionFlag = SessionStateActions.InitializeItem
                    }
                },
                ExpireDate = DateTime.Now.AddMinutes(timeout),
            };

            this.cache.Set(id, model, TimeSpan.FromMinutes(timeout));
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
        }

        /// <summary>
        /// 在请求结束的时候事件
        /// </summary>
        /// <param name="context"></param>
        public override void EndRequest(HttpContext context)
        {
        }

        /// <summary>
        /// 在请求开始时:SessionStateModule 对象调用GetItemExclusive方法,在这个期间有一个AcquireRequestState事件,
        /// 当EnableSessionState 属性设置为true时,也是默认情形,
        /// 如果EnableSessionState属性设置为ReadOnly,SessionStateModule对象调用的方法就改为GetItem方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <param name="locked"></param>
        /// <param name="lockAge"></param>
        /// <param name="lockId"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out System.Web.SessionState.SessionStateActions actions)
        {
            return this.GetSessionStoreItem(false, context, id, out locked, out lockAge, out lockId, out actions);
        }

        /// <summary>
        /// 在请求开始时:SessionStateModule 对象调用GetItemExclusive方法,在这个期间有一个AcquireRequestState事件,
        /// 当EnableSessionState 属性设置为true时,也是默认情形,
        /// 如果EnableSessionState属性设置为ReadOnly,SessionStateModule对象调用的方法就改为GetItem方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <param name="locked"></param>
        /// <param name="lockAge"></param>
        /// <param name="lockId"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out System.Web.SessionState.SessionStateActions actions)
        {
            return this.GetSessionStoreItem(true, context, id, out locked, out lockAge, out lockId, out actions);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lockRecord"></param>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <param name="locked"></param>
        /// <param name="lockAge"></param>
        /// <param name="lockId"></param>
        /// <param name="actionFlags"></param>
        /// <returns></returns>
        public SessionStateStoreData GetSessionStoreItem(bool lockRecord, HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out System.Web.SessionState.SessionStateActions actionFlags)
        {
            //init out
            SessionStateStoreData item = null;
            lockAge = TimeSpan.Zero;
            lockId = null;
            locked = false;
            actionFlags = 0;
            //要存储的内容
            string serializedItems = "";
            int timeout = 0;
            bool found = false;
            bool deleted = false;

            var model = this.cache.Get<SessionStateList>(id);
            if (lockRecord)
            {
                if (model == null || model.SessionModels == null || model.SessionModels.Count() == 0)
                    locked = true;
                else
                {
                    var lockedList = from n in model.SessionModels
                                     where !n.IsLocked && n.LockDate > DateTime.Now
                                     select n;

                    if (lockedList.Count() == 0)
                        locked = true;
                    else
                        locked = false;

                    foreach (var n in model.SessionModels)
                    {
                        if (!n.IsLocked && n.LockDate > DateTime.Now)
                        {
                            n.IsLocked = true;
                            n.LockDate = DateTime.Now;
                        }
                    }
                }
            }

            if (model != null && model.SessionModels != null && model.SessionModels.Count() > 0)
            {
                //删除数据
                var first = model.SessionModels.OrderByDescending(o => o.LockDate).FirstOrDefault();
                if (first != null)
                {
                    if (first.ExpireDate < DateTime.Now)
                    {
                        locked = false;
                        deleted = true;
                    }
                    else
                        found = true;

                    serializedItems = first.SessionText;
                    lockAge = DateTime.Now.Subtract(first.LockDate);
                    lockId = first.LockId;
                    actionFlags = (System.Web.SessionState.SessionStateActions)first.ActionFlag;
                    timeout = first.Timeout;
                }
            }
            if (!found)
                locked = false;

            if (deleted)
                model.SessionModels = new SessionState[0];

            if (found && !locked)
            {
                lockId = (long)lockId + 1;
                foreach (var session in model.SessionModels)
                {
                    session.LockId = (long)lockId;
                    session.ActionFlag = SessionStateActions.None;
                }
                if ((model.ExpireDate - DateTime.Now).Minutes <= 0)
                    model.ExpireDate = DateTime.Now.AddMinutes(sessionCfg.Timeout.TotalMinutes);

                this.cache.Set(id, model, model.ExpireDate - DateTime.Now);
            }

            //以分钟为单位
            if (actionFlags == (System.Web.SessionState.SessionStateActions)SessionStateActions.InitializeItem)
                item = this.CreateNewStoreData(context, (int)sessionCfg.Timeout.TotalMinutes);
            else
                item = new System.Func<SessionStateStoreData>(() =>
                {
                    using (var ms = new MemoryStream(Convert.FromBase64String(serializedItems)))
                    {
                        var sessionItems = new SessionStateItemCollection();
                        if (ms.Length > 0)
                        {
                            BinaryReader reader = new BinaryReader(ms);
                            sessionItems = SessionStateItemCollection.Deserialize(reader);
                        }
                        return new SessionStateStoreData(sessionItems, SessionStateUtility.GetSessionStaticObjects(context), timeout <= 0 ? (int)sessionCfg.Timeout.TotalMinutes : timeout);
                    }
                }).Invoke();

            return item;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <remarks>
        /// checked
        /// </remarks>
        public override void InitializeRequest(HttpContext context)
        {
            //不作任何动作
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <param name="lockId"></param>
        /// <remarks>
        /// checked
        /// </remarks>
        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            //存储Session对象模型
            var model = this.cache.Get<SessionStateList>(id);
            if (model == null || model.SessionModels == null || model.SessionModels.Count() == 0)
                return;

            var first = model.SessionModels.FirstOrDefault(o => o.LockId == (long)lockId);
            if (first != null)
            {
                first.IsLocked = false;
                first.ExpireDate = DateTime.Now.AddMinutes(sessionCfg.Timeout.TotalMinutes);
            }
            if ((model.ExpireDate - DateTime.Now).Minutes <= 0)
                model.ExpireDate = DateTime.Now.AddMinutes(sessionCfg.Timeout.TotalMinutes);

            try
            {
                this.cache.Set(id, model, model.ExpireDate - DateTime.Now);
            }
            catch
            {
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <param name="lockId"></param>
        /// <param name="item"></param>
        /// <remarks>
        /// checked
        /// </remarks>
        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            //存储Session对象模型
            var model = this.cache.Get<SessionStateList>(id);
            if (model == null || model.SessionModels == null || model.SessionModels.Count() == 0)
                return;

            //删除过期的对象
            model.SessionModels = (from n in model.SessionModels where n.LockId != (long)lockId || n.ExpireDate > DateTime.Now select n).ToArray();

            if ((model.ExpireDate - DateTime.Now).Minutes <= 0)
                model.ExpireDate = DateTime.Now.AddMinutes(sessionCfg.Timeout.TotalMinutes);

            this.cache.Set(id, model, model.ExpireDate - DateTime.Now);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <remarks>
        /// checked
        /// </remarks>
        public override void ResetItemTimeout(HttpContext context, string id)
        {
            var model = this.cache.Get<SessionStateList>(id);
            if (model == null || model.SessionModels == null || model.SessionModels.Count() == 0)
                return;

            //以分钟为单位
            foreach (var session in model.SessionModels)
                session.ExpireDate = DateTime.Now.AddMinutes(sessionCfg.Timeout.TotalMinutes);

            if ((model.ExpireDate - DateTime.Now).Minutes <= 0)
                model.ExpireDate = DateTime.Now.AddMinutes(sessionCfg.Timeout.TotalMinutes);

            this.cache.Set(id, model, TimeSpan.FromMinutes(sessionCfg.Timeout.TotalMinutes));
        }

        /// <summary>
        /// checked
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <param name="lockId"></param>
        /// <param name="newItem"></param>
        /// <remarks>
        /// checked
        /// </remarks>
        public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            string sessionText = new System.Func<string>(() =>
            {
                var coll = (SessionStateItemCollection)item.Items;
                using (var output = new MemoryStream())
                //using (var bin = new BinaryWriter(output))
                {
                    coll.Serialize(new BinaryWriter(output));
                    return Convert.ToBase64String(output.ToArray());
                }
            }).Invoke();

            //存储Session对象模型
            var model = this.cache.Get<SessionStateList>(id);
            if (model == null)
                model = new SessionStateList()
                {
                    ExpireDate = DateTime.Now.AddMinutes(item.Timeout)
                };

            if (newItem || model.SessionModels == null || model.SessionModels.Count() == 0)
            {
                if (model.SessionModels != null)
                {
                    //删除过期的对象
                    var list = (from n in model.SessionModels where n.ExpireDate > DateTime.Now select n).ToList();
                    model.SessionModels = list.ToArray();
                }
                if (model.SessionModels == null)
                    model.SessionModels = new SessionState[0];

                var second = new[]{new SessionState()
                {
                    SessionId = id,
                    ApplicationName = applicationName,
                    ExpireDate = DateTime.Now.AddMinutes(item.Timeout),
                    LockId = 0,
                    Timeout = item.Timeout,
                    IsLocked = false,
                    SessionText = sessionText
                }};
                model.SessionModels = model.SessionModels.Union(second, EqualityComparer<SessionState>.Default).ToArray();
            }
            else
            {
                if (model.SessionModels != null && lockId != null)
                {
                    var newLockId = (long)lockId;
                    foreach (var session in model.SessionModels)
                    {
                        if (session.LockId == newLockId)
                        {
                            session.SessionText = sessionText;
                            session.IsLocked = false;
                            session.ExpireDate = DateTime.Now.AddMinutes(item.Timeout);
                        }
                    }
                }
            }
            if ((model.ExpireDate - DateTime.Now).Minutes <= 0)
                model.ExpireDate = DateTime.Now.AddMinutes(sessionCfg.Timeout.TotalMinutes);

            this.cache.Set(id, model, model.ExpireDate - DateTime.Now);
        }

        /// <summary>
        /// 当丢失session的时候，回调
        /// </summary>
        /// <param name="expireCallback"></param>
        /// <returns></returns>
        /// <remarks>
        /// checked
        /// </remarks>
        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            return false;
        }

        #endregion SessionStateStoreProviderBase成员
    }
}

#endif