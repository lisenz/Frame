using System;
using System.Web;

namespace Frame.Core.Session
{
    public class SessionProvider : ISessionProvider
    {
        private static readonly string UserSessionKeyFormat = (typeof(SessionProvider).FullName + "${0}");

        protected virtual void CheckSessionValid()
        {
            if (null == HttpContext.Current.Session)
            {
                throw new InvalidOperationException("Asp.Net的Session对象为空,请确认.aspx页面的'EnableSessionState'属性为true。");
            }
        }

        protected virtual void ClearSessionState()
        {
            this.CheckSessionValid();
            string name = HttpContext.Current.User.Identity.Name;
            string keyFormat = string.Format(UserSessionKeyFormat, name);

            ISessionState state = HttpContext.Current.Session[keyFormat] as ISessionState;
            if (null != state)
            {
                HttpContext.Current.Session.Remove(keyFormat);
                state.Dispose();
            }
        }

        protected virtual ISessionState GetSessionState()
        {
            this.CheckSessionValid();
            string name = HttpContext.Current.User.Identity.Name;
            string keyFormat = string.Format(UserSessionKeyFormat, name);

            ISessionState state = HttpContext.Current.Session[keyFormat] as ISessionState;
            if (null == state)
            {
                lock (this)
                {
                    state = HttpContext.Current.Session[keyFormat] as ISessionState;
                    if (null == state)
                    {
                        state = new SessionState();
                        HttpContext.Current.Session[keyFormat] = state;
                    }
                }
            }

            return state;
        }

        protected ISessionState SessionState
        {
            get
            {
                return this.GetSessionState();
            }
        }

        public bool Remove(string name)
        {
            return this.SessionState.Remove(name);
        }

        public bool IsValid
        {
            get
            {
                return ((null != HttpContext.Current) && (null != HttpContext.Current.Session));
            }
        }

        public object this[string name]
        {
            get
            {
                return this.SessionState[name];
            }
            set
            {
                this.SessionState[name] = value;
            }
        }

        public void Clear()
        {
            this.ClearSessionState();
        }
    }
}
