using System;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;

namespace Frame.OS.WPF.Commands
{
    public class CompositeCommand : ICommand
    {
        private readonly List<ICommand> registeredCommands = new List<ICommand>();
        private readonly bool monitorCommandActivity;
        private readonly EventHandler onRegisteredCommandCanExecuteChangedHandler;
        private List<WeakReference> _canExecuteChangedHandlers;
        
        public CompositeCommand()
        {
            this.onRegisteredCommandCanExecuteChangedHandler = new EventHandler(this.OnRegisteredCommandCanExecuteChanged);
        }

        public CompositeCommand(bool monitorCommandActivity)
            : this()
        {
            this.monitorCommandActivity = monitorCommandActivity;
        }

        public virtual void RegisterCommand(ICommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (command == this)
            {
                throw new ArgumentException("不能注册组合命令本身.");
            }

            lock (this.registeredCommands)
            {
                if (this.registeredCommands.Contains(command))
                {
                    throw new InvalidOperationException("同一个组合命令不能重复注册.");
                }
                this.registeredCommands.Add(command);
            }

            command.CanExecuteChanged += this.onRegisteredCommandCanExecuteChangedHandler;
            this.OnCanExecuteChanged();

            if (this.monitorCommandActivity)
            {
                var activeAwareCommand = command as IActiveAware;
                if (activeAwareCommand != null)
                {
                    activeAwareCommand.IsActiveChanged += this.Command_IsActiveChanged;
                }
            }
        }

        public virtual void UnregisterCommand(ICommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            bool removed;
            lock (this.registeredCommands)
            {
                removed = this.registeredCommands.Remove(command);
            }

            if (removed)
            {
                command.CanExecuteChanged -= this.onRegisteredCommandCanExecuteChangedHandler;
                this.OnCanExecuteChanged();

                if (this.monitorCommandActivity)
                {
                    var activeAwareCommand = command as IActiveAware;
                    if (activeAwareCommand != null)
                    {
                        activeAwareCommand.IsActiveChanged -= this.Command_IsActiveChanged;
                    }
                }
            }
        }

        private void OnRegisteredCommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.OnCanExecuteChanged();
        }

        public virtual bool CanExecute(object parameter)
        {
            bool hasEnabledCommandsThatShouldBeExecuted = false;

            ICommand[] commandList;
            lock (this.registeredCommands)
            {
                commandList = this.registeredCommands.ToArray();
            }
            foreach (ICommand command in commandList)
            {
                if (this.ShouldExecute(command))
                {
                    if (!command.CanExecute(parameter))
                    {
                        return false;
                    }

                    hasEnabledCommandsThatShouldBeExecuted = true;
                }
            }

            return hasEnabledCommandsThatShouldBeExecuted;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                WeakEventHandlerManager.AddWeakReferenceHandler(ref _canExecuteChangedHandlers, value, 2);
            }
            remove
            {
                WeakEventHandlerManager.RemoveWeakReferenceHandler(_canExecuteChangedHandlers, value);
            }
        }

        public virtual void Execute(object parameter)
        {
            Queue<ICommand> commands;
            lock (this.registeredCommands)
            {
                commands = new Queue<ICommand>(this.registeredCommands.Where(this.ShouldExecute).ToList());
            }

            while (commands.Count > 0)
            {
                ICommand command = commands.Dequeue();
                command.Execute(parameter);
            }
        }

        protected virtual bool ShouldExecute(ICommand command)
        {
            var activeAwareCommand = command as IActiveAware;

            if (this.monitorCommandActivity && activeAwareCommand != null)
            {
                return activeAwareCommand.IsActive;
            }

            return true;
        }

        public IList<ICommand> RegisteredCommands
        {
            get
            {
                IList<ICommand> commandList;
                lock (this.registeredCommands)
                {
                    commandList = this.registeredCommands.ToList();
                }

                return commandList;
            }
        }

        protected virtual void OnCanExecuteChanged()
        {
            WeakEventHandlerManager.CallWeakReferenceHandlers(this, _canExecuteChangedHandlers);
        }

        private void Command_IsActiveChanged(object sender, EventArgs e)
        {
            this.OnCanExecuteChanged();
        }
    }
}
