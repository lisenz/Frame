using Frame.Core.Threading;
using System;
using System.Collections.Generic;

namespace Frame.Test.Test
{
    /// <summary>
    ///这是 ThreadContextTest 的测试类，旨在
    ///包含所有 ThreadContextTest 单元测试
    ///</summary>
    public class ThreadContextTest
    {
        /// <summary>
        ///ThreadContext`1 构造函数 的测试
        ///</summary>
        public void ThreadContextConstructorTestHelper()
        {
            
            ThreadContext<string> target1 = new ThreadContext<string>();
            target1.ContextValue = "One";
            ThreadContext<string> target2 = new ThreadContext<string>();
            target2.ContextValue = "Two";

            int i = 1;
            Func<string> valueFactory = () => { return (i++).ToString(); }; // TODO: 初始化为适当的值
            ThreadContext<string> target3 = new ThreadContext<string>(valueFactory);

            string str1 = target1.ContextValue;
            string str2 = target2.ContextValue;
            string str3 = target3.ContextValue;
            str3 = target3.ContextValue; //1
            target3.ContextValue = null;
            str3 = target3.ContextValue; //2
        }

        /// <summary>
        ///Dispose 的测试
        ///</summary>
        public void DisposeTestHelper<T>()
        {
            ThreadContext<T> target = new ThreadContext<T>(); // TODO: 初始化为适当的值
            target.Dispose();
        }
    }
}
