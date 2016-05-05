using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frame.Core
{
    /// <summary>
    /// 单例实体供应器，用于生成指定类型的单例实体。
    /// </summary>
    /// <typeparam name="T">要生成的实体对象的类型。</typeparam>
    public class SingleProvider<T> where T : new()
    {
        SingleProvider() { }

        public static T Instance
        {
            get
            {
                return SingleCreator._Instance;
            }
        }

        class SingleCreator
        {
            static SingleCreator() { }
            internal static readonly T _Instance = new T();
        }
    }
}
