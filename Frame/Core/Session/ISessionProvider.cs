using System;

namespace Frame.Core.Session
{
    public interface ISessionProvider
    {
        bool Remove(string name);
        bool IsValid { get; }

        object this[string name] { get; set; }

        void Clear();
    }
}
