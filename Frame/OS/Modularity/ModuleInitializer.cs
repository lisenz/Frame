using System;
using System.Globalization;
using Microsoft.Practices.ServiceLocation;
using Frame.OS.Modularity.Exceptions;

namespace Frame.OS.Modularity
{
    /// <summary>
    /// 模块初始化构造器。该类用于获取继承实现了IModule接口的模块对象并调用其初始化方法，
    /// 完成模块的一些初始化工作。
    /// </summary>
    public class ModuleInitializer : IModuleInitializer
    {
        /// <summary>
        /// 表示一个IOC的本地服务定位器。
        /// </summary>
        private readonly IServiceLocator serviceLocator;

        public ModuleInitializer(IServiceLocator serviceLocator)
        {
            if (serviceLocator == null)
            {
                throw new ArgumentNullException("serviceLocator");
            }
            this.serviceLocator = serviceLocator;
        }

        #region 实现IModuleInitializer接口

        /// <summary>
        /// 根据传入的模块元数据对象，创建获取指定模块，并完成初始化工作。
        /// </summary>
        /// <param name="moduleInfo">创建并执行初始化工作的模块的元数据对象。</param>
        public void Initialize(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null) throw new ArgumentNullException("moduleInfo");

            IModule moduleInstance = null;
            try
            {
                moduleInstance = this.CreateModule(moduleInfo);
                moduleInstance.Initialize(); // 这里调用IModule接口的初始化方法，完成该模块的初始化工作。
            }
            catch (Exception ex)
            {
                this.HandleModuleInitializationError(
                    moduleInfo,
                    moduleInstance != null ? moduleInstance.GetType().Assembly.FullName : null,
                    ex);
            }
        }

        #endregion

        /// <summary>
        /// 创建指定模块。该模块必须继承实现IModule接口。
        /// </summary>
        /// <param name="moduleInfo">创建的模块元数据对象。</param>
        /// <returns>返回创建的模块。</returns>
        protected virtual IModule CreateModule(ModuleInfo moduleInfo)
        {
            if (moduleInfo == null) throw new ArgumentNullException("moduleInfo");
            return this.CreateModule(moduleInfo.ModuleType);
        }

        /// <summary>
        /// 根据System.Type的程序集限定名(其中包括从中加载 System.Type 的程序集的名称)，创建指定类型的模块，
        /// 该模块必须继承实现IModule接口。
        /// </summary>
        /// <param name="typeName">继承了IModule接口的模块的数据类型完全限定名。</param>
        /// <returns>返回创建的模块。</returns>
        protected virtual IModule CreateModule(string typeName)
        {
            Type moduleType = Type.GetType(typeName);
            if (moduleType == null)
            {
                throw new ModuleInitializeException(string.Format(CultureInfo.CurrentCulture,
                    "无法检索模块类型{0}所加载的程序集."
                        + "你可以需要指定一个更加完全的限定类型名称.",
                typeName));
            }

            return (IModule)this.serviceLocator.GetInstance(moduleType);
        }

        public virtual void HandleModuleInitializationError(ModuleInfo moduleInfo, string assemblyName, Exception exception)
        {
            if (moduleInfo == null) throw new ArgumentNullException("moduleInfo");
            if (exception == null) throw new ArgumentNullException("exception");

            Exception moduleException;

            if (exception is ModuleInitializeException)
            {
                moduleException = exception;
            }
            else
            {
                if (!string.IsNullOrEmpty(assemblyName))
                {
                    moduleException = new ModuleInitializeException(moduleInfo.ModuleName, assemblyName, exception.Message, exception);
                }
                else
                {
                    moduleException = new ModuleInitializeException(moduleInfo.ModuleName, exception.Message, exception);
                }
            }


            throw moduleException;
        }

    }
}
