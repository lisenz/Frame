using System;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using Frame.Service.Server.Attributes;

namespace Frame.Service.Server.Core
{
    /// <summary>
    /// 通用服务实现，调用一个对象方法作为服务的实现
    /// </summary>
    public class Service : IService
    {
        /// <summary>
        /// 一个临时服务对象。
        /// </summary>
        private object _instance;

        /// <summary>
        /// 存储服务对象调用的服务方法缓存列表。
        /// </summary>
        private IDictionary<string, ServiceMethod> _methods = new Dictionary<string, ServiceMethod>();

        /// <summary>
        /// 获取或设置服务方法缓存列表时的锁对象。
        /// </summary>
        private readonly ReaderWriterLockSlim _methodsLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 创建临时服务对象时的锁对象。
        /// </summary>
        private readonly object _syncRoot = new object();

        /// <summary>
        /// 获取或设置服务的名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置服务的路由URL模式规则。
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 获取或设置服务的类型。
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 获取或设置服务的默认调用方法的名称。
        /// </summary>
        public string DefaultMethod { get; set; }

        /// <summary>
        /// 执行服务的调用并返回值。
        /// </summary>
        /// <param name="context">一次服务请求的上下文对象。</param>
        /// <returns>调用服务后返回的执行结果。</returns>
        public object Execute(IServiceContext context)
        {
            string methodName = string.IsNullOrEmpty(context.Method) ? DefaultMethod : context.Method;

            if (string.IsNullOrEmpty(methodName))
            {
                throw ServiceException.NotFound(string.Format("该服务请求或默认方法在服务‘{0}’中未找到。", Name));
            }

            ServiceMethod method = ResolveMethod(context, methodName);

            if (method.IsStatic)
            {
                return method.Invoke(context);
            }
            else
            {
                return method.Invoke(GetInstance(context), context);
            }
        }

        /// <summary>
        /// 获取解析与指定的方法名称相关联的服务方法。
        /// </summary>
        /// <param name="context">服务上下文对象，默认无效，当对该方法进行重写时以提供相关信息。</param>
        /// <param name="name">要获取解析的服务方法的名称。</param>
        /// <returns>获取的服务方法。</returns>
        protected virtual ServiceMethod ResolveMethod(IServiceContext context, string name)
        {
            _methodsLock.EnterUpgradeableReadLock();
            try
            {
                ServiceMethod method;
                if (_methods.TryGetValue(name, out method))
                {
                    return method;
                }
                else
                {
                    MethodInfo methodInfo = FindMethod(context, name);

                    if (null == methodInfo)
                    {
                        throw ServiceException.NotFound(
                            string.Format("在类型为{0}的服务中不存在方法{1}", Type.FullName, name));
                    }

                    CheckServiceMethod(methodInfo);
                    _methodsLock.EnterWriteLock();
                    try
                    {
                        if (_methods.TryGetValue(name, out method))
                        {
                            return method;
                        }

                        _methods[name] = (method = new ServiceMethod(methodInfo));
                        return method;
                    }
                    finally
                    {
                        _methodsLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _methodsLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// 检查验证服务方法。
        /// </summary>
        /// <param name="method">服务方法。</param>
        protected virtual void CheckServiceMethod(MethodInfo method)
        {
            if (!method.IsPublic)
            {
                throw ServiceException.NotFound(
                    string.Format("在类型为{0}的服务中，方法{1}必须是公共的。", Type.FullName, method.Name));
            }

            if (method.GetCustomAttributes(typeof(ServiceMethodAttribute), true).Length == 0)
            {
                throw ServiceException.NotFound(
                    string.Format("{0}在类型为{1}的服务中不是一个服务方法。", method.Name, Type.FullName));
            }
        }

        /// <summary>
        /// 从服务对象中搜索出指定名称的公共方法。
        /// </summary>
        /// <param name="context">服务上下文对象，默认不使用，当对该方法进行重写以提供相关信息。</param>
        /// <param name="name">公共方法的名称。</param>
        /// <returns>返回一个方法元数据对象。</returns>
        protected virtual MethodInfo FindMethod(IServiceContext context, string name)
        {
            return Type.GetMethod(name);
        }

        /// <summary>
        /// 创建服务上文对象中指定类型的服务对象。
        /// </summary>
        /// <param name="context">服务上文对象。</param>
        /// <returns>返回一个指定类型的服务对象。</returns>
        protected virtual object GetInstance(IServiceContext context)
        {
            if (null == _instance)
            {
                lock (_syncRoot)
                {
                    if (null == _instance)
                    {
                        try
                        {
                            _instance = Activator.CreateInstance(Type);
                        }
                        catch (Exception)
                        {
                            throw new ServiceException(string.Format("创建一个类型为{0}的服务对象出错。", Type.FullName));

                        }
                    }
                }
            }
            return _instance;
        }
    }
}
