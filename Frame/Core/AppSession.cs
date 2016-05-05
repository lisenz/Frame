using System;
using System.Web;
using System.Collections.Generic;
//-------------
using Frame.Core.Session;

namespace Frame.Core
{
    [Serializable]
    public sealed class AppSession
    {
        private static ISessionProvider _Provider;

        static AppSession()
        {
            Initialize();
        }

        private static void CheckIsWebEnvironment()
        {
            if (null == HttpContext.Current)
            {
                throw new InvalidOperationException(string.Format("{0}只支持在Web环境中使用。",typeof(AppSession).FullName));
            }
        }

        private static void Initialize()
        {
            CheckIsWebEnvironment();
            if (!App.ObjectContainer.TryGetObject<ISessionProvider>(out _Provider))
            {
                _Provider = new SessionProvider();
            }
        }

        public static bool Remove(string name)
        {
            return _Provider.Remove(name);
        }

        public static void Set(string name, object value)
        {
            _Provider[name] = value;
        }

        public static object Get(string name)
        {
            return _Provider[name];
        }

        public static T Get<T>(string name)
        {
            return (T)Get(name);
        }

        public static void Clear()
        {
            _Provider.Clear();
        }

        public static bool IsValid
        {
            get
            {
                return _Provider.IsValid;
            }
        }
    }
}
