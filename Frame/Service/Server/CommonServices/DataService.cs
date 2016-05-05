using System;
using System.Threading;
using System.Collections.Generic;
using Frame.Service.Server.SqlGe;
using Frame.Service.Server.Attributes;

namespace Frame.Service.Server.CommonServices
{
    /// <summary>
    /// 通用数据服务，在Url中传入CommandName指定一个需要执行的命令，执行后返回数据。
    /// 若要调用此服务，需在Config文件中的[system.web]的[httpHandlers]配置节中加入
    /// [add verb="*" path="*.cs" validate="false" type="Frame.Service.Server.Core, Frame.Service.Server, Version=4.0.0.0, Culture=neutral, PublicKeyToken=04db02423b9ebbb2"/]；
    /// [add verb="*" path="*.cs" validate="false" type="Frame.Service.Server.Core, Frame.Service.Server.CommonServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=04db02423b9ebbb2"/]
    /// 进行注册。
    /// </summary>
    [Service("DataService")]
    public class DataService
    {
        /// <summary>
        /// 是否缓存的默认标识。
        /// </summary>
        public const bool DefaultCacheable = true;

        /// <summary>
        /// 缓存标识。
        /// </summary>
        private bool _cacheable = DefaultCacheable;

        /// <summary>
        /// 服务执行对象的缓存列表。
        /// </summary>
        private IDictionary<string, IServiceCommand> _commands = new Dictionary<string, IServiceCommand>();

        /// <summary>
        /// 对服务执行对象读写操作时的锁状态对象。
        /// </summary>
        private readonly ReaderWriterLockSlim _commandsLock = new ReaderWriterLockSlim();

        /// <summary>
        /// 执行命令，并返回结果数据。
        /// </summary>
        /// <param name="context">服务上下文对象。</param>
        /// <returns>结果数据。</returns>
        [ServiceMethod]
        public object Execute(IServiceContext context)
        {
            return DoExecute(context);
        }

        /// <summary>
        /// 执行CommandName参数指定的命令，并返回数据。
        /// </summary>
        /// <param name="context">服务上下文对象。</param>
        /// <returns>结果数据。</returns>
        protected virtual object DoExecute(IServiceContext context)
        {
            string commandName = null;
            object nameObject;

            if (context.Params.TryGetValue("CommandName", out nameObject))
            {
                commandName = nameObject as string;
            }

            if (string.IsNullOrEmpty(commandName))
            {
                throw ServiceException.BadRequest(string.Format("'CommandName' 请求不到。"));
            }

            IServiceCommand command = null;

            if (Cacheable)
            {
                command = GetCachedCommand(commandName);
            }

            if (null == command)
            {
                IServiceCommandResolver resolver = new SqlCommandResolver();
                command = resolver.Resolve(commandName, context);
                if (Cacheable && null != command)
                {
                    if (command != null)
                        SetCachedCommand(commandName, command);
                }
            }

            if (null == command)
            {
                throw ServiceException.NotFound(string.Format("无法检索到指定名称的Command '{0}'", commandName));
            }

            return command.Execute(context);
        }

        /// <summary>
        /// 获取或设置一个值，该值指示缓存列表中是否已设置服务执行对象。
        /// </summary>
        public virtual bool Cacheable
        {
            get { return _cacheable; }
            set { _cacheable = value; }
        }

        /// <summary>
        /// 将一个服务执行对象放入缓存列表。
        /// </summary>
        /// <param name="name">服务执行对象在缓存列表中的键。</param>
        /// <param name="command">服务执行对象。</param>
        protected virtual void SetCachedCommand(string name, IServiceCommand command)
        {
            _commandsLock.EnterWriteLock();
            try
            {
                _commands[name] = command;
            }
            finally
            {
                _commandsLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 获取与指定键相关联的服务执行对象。
        /// </summary>
        /// <param name="name">服务执行对象的键。</param>
        /// <returns>若在缓存列表中找到指定的键，则返回对应的服务指定对象；否则，返回null。</returns>
        protected virtual IServiceCommand GetCachedCommand(string name)
        {
            _commandsLock.EnterReadLock();
            try
            {
                IServiceCommand command;
                return _commands.TryGetValue(name, out command) ? command : null;
            }
            finally
            {
                _commandsLock.ExitReadLock();
            }
        }
    }
}
