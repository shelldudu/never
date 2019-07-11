#if !NET461
#else

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Never.Web.Fakes
{
    /// <summary>
    ///
    /// </summary>
    public class FakeHttpSessionStateWrapper : HttpSessionStateBase
    {
        #region field

        private readonly SessionStateItemCollection session = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeHttpResponseWrapper"/> class.
        /// </summary>
        public FakeHttpSessionStateWrapper()
            : this(new SessionStateItemCollection())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeHttpSessionStateWrapper"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public FakeHttpSessionStateWrapper(SessionStateItemCollection session)
        {
            this.session = session ?? new SessionStateItemCollection();
        }

        #endregion ctor

        #region member

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        public override int Count
        {
            get { return session.Count; }
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>
        /// The keys.
        /// </value>
        public override NameObjectCollectionBase.KeysCollection Keys
        {
            get { return session.Keys; }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override object this[string name]
        {
            get { return session[name]; }
            set { session[name] = value; }
        }

        /// <summary>
        /// Existses the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return session[key] != null;
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override object this[int index]
        {
            get { return session[index]; }
            set { session[index] = value; }
        }

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public override void Add(string name, object value)
        {
            session[name] = value;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator GetEnumerator()
        {
            return session.GetEnumerator();
        }

        /// <summary>
        /// Removes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        public override void Remove(string name)
        {
            session.Remove(name);
        }

        #endregion member
    }
}

#endif