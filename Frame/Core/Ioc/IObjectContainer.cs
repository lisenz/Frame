using System;
using System.Collections.Generic;
using Frame.Core.Collection;

namespace Frame.Core.Ioc
{
    public interface IObjectContainer : IDisposable
    {
        IEnumerable<TType> GetAllObjects<TType>();
        IEnumerable<object> GetAllObjects(Type type);
        IEnumerable<TType> GetNamedObjects<TType>();
        IEnumerable<object> GetNamedObjects(Type type);

        NameObjectCollection<TType> GetNamedObjectCollection<TType>();
        NameObjectCollection<object> GetNamedObjectCollection(Type type);

        TType GetObject<TType>();
        object GetObject(string name);
        TType GetObject<TType>(string name);
        object GetObject(Type type);
        object GetObject(Type type, string name);

        void Register<TType>(TType instance);
        void Register<TType>(string name, TType instance);
        void Register(Type type, object instance);
        void Register(Type type, string name, object instance);

        bool TryGetObject<TType>(out TType obj);
        bool TryGetObject(string name, out object obj);
        bool TryGetObject<TType>(string name, out TType obj);
        bool TryGetObject(Type type, out object obj);
        bool TryGetObject(Type type, string name, out object obj);
    }
}
