using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
//--------------------------------
using Frame.Core.Collection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;

namespace Frame.Core.Ioc
{
    internal class UnityObjectContainer : IObjectContainer, IDisposable
    {
        private IUnityContainer _container = null;
        private bool _disposed;
        private readonly object _disposeSyncRoot = new object();
        private const string UNITY_DIRECTORY_NAME = "Unity";

        public IUnityContainer UnityContainer
        {
            get
            {
                return this._container;
            }
        }

        private UnityObjectContainer()
        {
        }

        internal static UnityObjectContainer Create(App application)
        {
            return new UnityObjectContainer { _container = InitUnityContainer(application) };
        }

        private static IUnityContainer InitUnityContainer(App application)
        {
            IUnityContainer container = new Microsoft.Practices.Unity.UnityContainer();
            DirectoryInfo[] directories = application.GetConfiguration().ConfigDirectory.GetDirectories(UNITY_DIRECTORY_NAME);
            if (directories.Length == 1)
            {
                // 当应用程序根目录下的<Config>文件夹或者<App_Config>文件夹中存在<Unity>文件夹时，
                // 获取该文件夹中的所有后缀为.config的文件出来进行解析。
                // TODO:该文件夹中的文件信息为进行依赖注入的对象配置信息。
                DirectoryInfo dir = directories[0];
                IEnumerable<FileInfo> source = LoadConfigurations(dir);
                UnityConfigurationSection section = new UnityConfigurationSection();
                foreach (FileInfo info3 in source)
                {
                    try
                    {
                        ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap
                        {
                            ExeConfigFilename = info3.FullName
                        };
                        UnityConfigurationSection section2 = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None).GetSection("unity") as UnityConfigurationSection;
                        if (null != section2)
                        {
                            if (section2.Containers.Count > 1)
                            {
                                throw new Exception(string.Format("在文件{0}中不支持多<container>标签.", info3.FullName));
                            }
                            if (section2.Containers.Count != 0)
                            {
                                if (!string.IsNullOrEmpty(section2.Containers[0].Name))
                                {
                                    section2.Containers[0].Name = null;
                                }
                                foreach (AssemblyElement element in section2.Assemblies)
                                {
                                    section.Assemblies.Add(element);
                                }
                                section2.Assemblies.Clear();
                                foreach (AssemblyElement element in section.Assemblies)
                                {
                                    section2.Assemblies.Add(element);
                                }
                                section2.Configure(container);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        string message = string.Format("加载Unity Config '{0}' 出错 : {1}", info3.Name, exception.Message);
                        throw new ObjectContainerException(message, exception);
                    }
                }
                return container;
            }
            return container;
        }

        /// <summary>
        /// 获取当前Unity文件夹及其子目录中的所有后缀为config的文件。
        /// </summary>
        /// <param name="dir">Unity文件夹对象。</param>
        /// <returns>返回当前Unity文件夹及其子目录中的所有后缀为config的文件集合。</returns>
        private static IEnumerable<FileInfo> LoadConfigurations(DirectoryInfo dir)
        {
            return dir.GetFiles("*.config", SearchOption.AllDirectories);
        }

        public IEnumerable<TType> GetAllObjects<TType>()
        {
            TType local;
            IEnumerable<TType> namedObjects = this.GetNamedObjects<TType>();
            if (!this.TryGetObject<TType>(out local))
                return namedObjects;

            return namedObjects.Concat<TType>(new TType[] { local });            
        }

        public IEnumerable<object> GetAllObjects(Type type)
        {
            object obj;
            IEnumerable<object> namedObjects = this.GetNamedObjects(type);
            if (!this.TryGetObject(out obj))
                return namedObjects;

            return namedObjects.Concat<object>(new object[] { obj });
        }

        public IEnumerable<TType> GetNamedObjects<TType>()
        {
            return this._container.ResolveAll<TType>(new ResolverOverride[0]);
        }

        public IEnumerable<object> GetNamedObjects(Type type)
        {
            return this._container.ResolveAll(type, new ResolverOverride[0]);
        }

        public NameObjectCollection<TType> GetNamedObjectCollection<TType>()
        {
            Type t = typeof(TType);
            NameObjectCollection<TType> objects = new NameObjectCollection<TType>();
            foreach (ContainerRegistration registration in this._container.Registrations)
            {
                if (!(string.IsNullOrEmpty(registration.Name) || !registration.RegisteredType.Equals(t)))
                {
                    objects.Add(registration.Name, (TType)this._container.Resolve(t, registration.Name, new ResolverOverride[0]));
                }
            }
            return objects;
        }

        public NameObjectCollection<object> GetNamedObjectCollection(Type type)
        {
            NameObjectCollection<object> objects = new NameObjectCollection<object>();
            foreach (ContainerRegistration registration in this._container.Registrations)
            {
                if (!(string.IsNullOrEmpty(registration.Name) || !registration.RegisteredType.Equals(type)))
                {
                    objects.Add(registration.Name, this._container.Resolve(type, registration.Name, new ResolverOverride[0]));
                }
            }
            return objects;
        }

        public TType GetObject<TType>()
        {
            TType local;
            if (!this.TryGetObject<TType>(out local))
                throw new ObjectNotFoundException(string.Format("没有类型为'{0}'的对象被注册.", typeof(TType).FullName));
            return local;
        }

        public object GetObject(string name)
        {
            object obj;
            if (!this.TryGetObject(name, out obj))
                throw new ObjectNotFoundException(string.Format("没有名称为'{0}'的对象被注册.", name));
            return obj;
        }

        public TType GetObject<TType>(string name)
        {
            TType local;
            if (!this.TryGetObject<TType>(name, out local))
                throw new ObjectNotFoundException(string.Format("没有类型为'{0}'且名称为'{1}'的对象被注册.", 
                    typeof(TType).FullName, name));
            return local;
        }

        public object GetObject(Type type)
        {
            object obj;
            if (!this.TryGetObject(type, out obj))
                throw new ObjectNotFoundException(string.Format("没有类型为'{0}'的对象被注册.", type.FullName));
            return obj;
        }

        public object GetObject(Type type, string name)
        {
            object obj;
            if (!this.TryGetObject(type,name, out obj))
                throw new ObjectNotFoundException(string.Format("没有类型为'{0}'且名称为'{1}'的对象被注册.",
                    type.FullName, name));
            return obj;
        }

        public void Register<TType>(TType instance)
        {
            this.Register(typeof(TType), instance);
        }

        public void Register<TType>(string name, TType instance)
        {
            this.Register(typeof(TType), name, instance);
        }

        public void Register(Type type, object instance)
        {
            this._container.RegisterInstance(type, instance);
            
        }

        public void Register(Type type, string name, object instance)
        {
            this._container.RegisterInstance(type, name, instance);
        }
        
        public bool TryGetObject<TType>(out TType obj)
        {
            if (!this._container.IsRegistered<TType>())
            {
                obj = default(TType);
                return false;
            }

            obj = this._container.Resolve<TType>(new ResolverOverride[0]);
            return true;
        }

        public bool TryGetObject(string name, out object obj)
        {
            IList<Type> list = (from reg in this._container.Registrations 
                                where reg.Name.Equals(name) 
                                select reg.RegisteredType).ToList<Type>();
            if (list.Count == 0)
            {
                obj = null;
                return false;
            }
            if (list.Count != 1)
                throw new InvalidOperationException(string.Format("发现有多个同命名为'{0}'的对象.", name));
            return this.TryGetObject(list[0], name, out obj);

        }

        public bool TryGetObject<TType>(string name, out TType obj)
        {
            if (!this._container.IsRegistered(typeof(TType), name))
            {
                obj = default(TType);
                return false;
            }
            obj = this._container.Resolve<TType>(name, new ResolverOverride[0]);
            return true;
        }

        public bool TryGetObject(Type type, out object obj)
        {
            if (!this._container.IsRegistered(type))
            {
                obj = null;
                return false;
            }
            obj = this._container.Resolve(type, new ResolverOverride[0]);
            return true;
        }

        public bool TryGetObject(Type type, string name, out object obj)
        {
            if (!this._container.IsRegistered(type, name))
            {
                obj = null;
                return false;
            }
            obj = this._container.Resolve(type, name, new ResolverOverride[0]);
            return true;
        }

        public void Dispose()
        {
            if ((null != this._container) && !this._disposed)
            {
                lock (this._disposeSyncRoot)
                {
                    this._container.Dispose();
                    this._disposed = true;
                }
            }
        }

        ~UnityObjectContainer()
        {
            this.Dispose();
        }

    }
}
