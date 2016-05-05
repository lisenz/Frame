using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frame.Core;
using Frame.Core.Ioc;
using Frame.Core.Collection;
using Frame.Test.Lib;

//------必须引用
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity.Configuration;

namespace Frame.Test.Test
{
    /// <summary>
    ///这是 UnityObjectContainerTest 的测试类，旨在
    ///包含所有 UnityObjectContainerTest 单元测试
    ///</summary>
    public class UnityObjectContainerTest
    {
        /// <summary>
        /// Container的测试
        ///</summary>
        public void GetContainerTest()
        {
            // 获取该属性的过程中，包括了创建App对象、AppConfig对象和UnityObjectContainer对象，
            // 其中AppConfig对象和UnityObjectContainer对象作为App对象的属性存在。
            // 这里测试返回一个UnityObjectContainer对象。
            App app = App.Current;
            IObjectContainer container = app.GetObjectContainer();
        }

        /// <summary>
        /// 通过配置文件方式进行注册对象及获取
        /// </summary>
        public void RegisterByConfigTest()
        {
            // 这里使用测试类库 <Frame.Test.Lib> 进行演示。
            // TODO: <Frame.Test.Lib> 需要引用。
            App app = App.Current;
            IObjectContainer container = app.GetObjectContainer();

            IContanerTest one = container.GetObject<ContainerOneTest>();
            IContanerTest two = container.GetObject<ContainerTwoTest>("DefaultTest");
            IContanerTest author = container.GetObject<IContanerTest>("AuthorTest");

            string oneResult = one.GetValue();
            string twoResult = two.GetValue();
            string authorResult = author.GetValue();

            IEnumerable<IContanerTest> list = container.GetAllObjects<IContanerTest>();
            int count = list.Count();
        }

        /// <summary>
        /// 通过代码方式进行注册对象及获取
        /// </summary>
        public void RegisterByCodeTest()
        {
            // 这里使用测试类库 <Frame.Test.Lib> 进行演示。
            // TODO: <Frame.Test.Lib> 需要引用。
            App app = App.Current;
            IObjectContainer container = app.GetObjectContainer();


            // 这里注册的是两个类型的对象，分别是ContainerOneTest和ContainerTwoTest，同时为它们指定在容器中的名字。
            container.Register<IContanerTest>("One", (new ContainerOneTest()));
            container.Register<IContanerTest>("Two", (new ContainerTwoTest()));
            // TODO:如果以上注册对象时，没有指定名字，那么当调用GetAllObjects<>()进行数量统计时，那些没有指定名字的将会获取不到。
            //      此时multiCount应该为2。
            int multiCount = container.GetAllObjects<IContanerTest>().Count();

            // 获取容器中注册对象时有指定名字的对象集合,这里可以以键值对的形式来使用对象
            NameObjectCollection<IContanerTest> namedCollection = container.GetNamedObjectCollection<IContanerTest>();
            IContanerTest one = namedCollection["One"];
            IContanerTest two = namedCollection["Two"];

            // 这里注册的对象没有在容器中指定名字，因此只能通过调用GetObject()或GetObject<IContanerTest>()来获取
            container.Register<IContanerTest>((new ContainerOneTest()));
            IContanerTest one1 = (IContanerTest)container.GetObject(typeof(IContanerTest));
            IContanerTest one2 = container.GetObject<IContanerTest>();
            // TODO:这里的对象one1和one2指向的是同一个对象
            bool result = one1.Equals(one1);

            // 获取容器中注册对象时有指定名字的对象集合
            IEnumerable<IContanerTest> nameds = container.GetNamedObjects<IContanerTest>();            

            // 获取容器中所有对象的集合，包括注册时没有指定名字的对象
            IEnumerable<IContanerTest> allObjs = container.GetAllObjects<IContanerTest>();
        }
    }
}
