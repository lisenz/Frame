using System;

namespace Frame.Core.Session
{
    public interface ISessionState : IDisposable
    {
        bool Remove(string name);
        object this[string name] { get; set; }
    }
}
