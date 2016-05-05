using System;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using Frame.Service.Server.Attributes;

namespace Frame.Service.Server.Core
{
    /// <summary>
    /// 通用页面实现，调用一个对象方法作为页面的实现
    /// </summary>
    public class Page:IPage
    {
        /// <summary>
        /// 一个临时服务对象。
        /// </summary>
        private object _instance;

        /// <summary>
        /// 存储服务对象调用的页面方法缓存列表。
        /// </summary>
        private IDictionary<string, PageAction> _actions = new Dictionary<string, PageAction>();

        /// <summary>
        /// 获取或设置服务方法缓存列表时的锁对象。
        /// </summary>
        private readonly ReaderWriterLockSlim _actionsLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 创建临时服务对象时的锁对象。
        /// </summary>
        private readonly object _syncRoot = new object();

        /// <summary>
        /// 获取或设置页面的名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置页面的URL路径。
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 页面对应的View模版文件名称。
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// 获取或设置页面的类型。
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 获取或设置页面的默认调用方法的名称。
        /// </summary>
        public string DefaultAction { get; set; }

        /// <summary>
        /// 执行页面的调用并返回值。
        /// </summary>
        /// <param name="context">一次页面请求的上下文对象。</param>
        /// <returns>调用页面后返回的执行结果。</returns>
        public object Visit(IPageContext context)
        {
            string methodName = string.IsNullOrEmpty(context.Action) ? DefaultAction : context.Action;

            if (string.IsNullOrEmpty(methodName))
            {
                throw ServiceException.NotFound(string.Format("该页面请求或默认方法在页面‘{0}’中未找到。", Name));
            }

            PageAction action = ResolveAction(context, methodName);

            if (action.IsStatic)
            {
                return action.Invoke(context);
            }
            else
            {
                return action.Invoke(GetInstance(context), context);
            }
        }

        /// <summary>
        /// 获取解析与指定的方法名称相关联的服务方法。
        /// </summary>
        /// <param name="context">页面上下文对象，默认无效，当对该方法进行重写时以提供相关信息。</param>
        /// <param name="name">要获取解析的页面方法的名称。</param>
        /// <returns>获取的页面方法。</returns>
        protected virtual PageAction ResolveAction(IPageContext context, string name)
        {
            _actionsLock.EnterUpgradeableReadLock();
            try
            {
                PageAction action;
                if (_actions.TryGetValue(name, out action))
                {
                    return action;
                }
                else
                {
                    MethodInfo methodInfo = FindAction(context, name);

                    if (null == methodInfo)
                    {
                        throw ServiceException.NotFound(
                            string.Format("在类型为{0}的页面中不存在方法{1}", Type.FullName, name));
                    }

                    CheckPageAction(methodInfo);
                    _actionsLock.EnterWriteLock();
                    try
                    {
                        if (_actions.TryGetValue(name, out action))
                        {
                            return action;
                        }

                        _actions[name] = (action = new PageAction(methodInfo));
                        return action;
                    }
                    finally
                    {
                        _actionsLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _actionsLock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// 检查验证页面方法。
        /// </summary>
        /// <param name="method">页面方法。</param>
        protected virtual void CheckPageAction(MethodInfo action)
        {
            if (!action.IsPublic)
            {
                throw ServiceException.NotFound(
                    string.Format("在类型为{0}的页面中，方法{1}必须是公共的。", Type.FullName, action.Name));
            }

            if (action.GetCustomAttributes(typeof(ActionAttribute), true).Length == 0)
            {
                throw ServiceException.NotFound(
                    string.Format("{0}在类型为{1}的页面中不是一个页面方法。", action.Name, Type.FullName));
            }
        }

        /// <summary>
        /// 从页面对象中搜索出指定名称的公共方法。
        /// </summary>
        /// <param name="context">页面上下文对象，默认不使用，当对该方法进行重写以提供相关信息。</param>
        /// <param name="name">公共方法的名称。</param>
        /// <returns>返回一个方法元数据对象。</returns>
        protected virtual MethodInfo FindAction(IPageContext context, string name)
        {
            return Type.GetMethod(name);
        }

        /// <summary>
        /// 创建页面上文对象中指定类型的页面对象。
        /// </summary>
        /// <param name="context">页面上文对象。</param>
        /// <returns>返回一个指定类型的页面对象。</returns>
        protected virtual object GetInstance(IPageContext context)
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
                            throw new ServiceException(string.Format("创建一个类型为{0}的页面对象出错。", Type.FullName));

                        }
                    }
                }
            }
            return _instance;
        }
    }
}
