using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace Frame.OS.WPF.Commands
{
    public class CommandBehaviorBase<T> where T : UIElement
    {
        private ICommand _Command;
        private object _CommandParameter;
        private readonly WeakReference _TargetObject;
        private readonly EventHandler _CommandCanExecuteChangedHandler;

        public CommandBehaviorBase(T targetObject)
        {
            this._TargetObject = new WeakReference(targetObject);
            this._CommandCanExecuteChangedHandler = new EventHandler(this.CommandCanExecuteChanged);
        }

        public ICommand Command
        {
            get { return _Command; }
            set
            {
                if (this._Command != null)
                {
                    this._Command.CanExecuteChanged -= this._CommandCanExecuteChangedHandler;
                }

                this._Command = value;
                if (this._Command != null)
                {
                    this._Command.CanExecuteChanged += this._CommandCanExecuteChangedHandler;
                    UpdateEnabledState();
                }
            }
        }

        public object CommandParameter
        {
            get { return this._CommandParameter; }
            set
            {
                if (this._CommandParameter != value)
                {
                    this._CommandParameter = value;
                    this.UpdateEnabledState();
                }
            }
        }

        protected T TargetObject
        {
            get
            {
                return _TargetObject.Target as T;
            }
        }

        protected virtual void UpdateEnabledState()
        {
            if (TargetObject == null)
            {
                this.Command = null;
                this.CommandParameter = null;
            }
            else if (this.Command != null)
            {
                TargetObject.IsEnabled = this.Command.CanExecute(this.CommandParameter);
            }
        }

        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.UpdateEnabledState();
        }

        protected virtual void ExecuteCommand()
        {
            if (this.Command != null)
            {
                this.Command.Execute(this.CommandParameter);
            }
        }
    }
}
