using System;
using System.Windows.Input;
using System.Collections.Generic;

namespace Frame.OS.WPF.Commands
{
    public abstract class DelegateCommandBase : ICommand, IActiveAware
    {
        private readonly Action<object> _ExecuteMethod;
        private readonly Func<object, bool> _CanExecuteMethod;
        private bool _IsActive;
        private List<WeakReference> _CanExecuteChangedHandlers;

        protected DelegateCommandBase(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException("executeMethod", "无论是executeMethod或者canExecuteMethod委托对象都不能为null.");
            
            this._ExecuteMethod = executeMethod;
            this._CanExecuteMethod = canExecuteMethod;
        }

        #region 实现ICommand接口

        protected bool CanExecute(object parameter)
        {
            return _CanExecuteMethod == null || _CanExecuteMethod(parameter);
        }
        
        public event EventHandler CanExecuteChanged
        {
            add
            {
                WeakEventHandlerManager.AddWeakReferenceHandler(ref _CanExecuteChangedHandlers, value, 2);
            }
            remove
            {
                WeakEventHandlerManager.RemoveWeakReferenceHandler(_CanExecuteChangedHandlers, value);
            }
        }

        protected void Execute(object parameter)
        {
            this._ExecuteMethod(parameter);  
        }

        void ICommand.Execute(object parameter)
        {
            Execute(parameter);
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        #endregion

        #region 实现IActiveAware接口

        public bool IsActive
        {
            get { return _IsActive; }
            set
            {
                if (_IsActive != value)
                {
                    _IsActive = value;
                    OnIsActiveChanged();
                }
            }
        }

        public virtual event EventHandler IsActiveChanged;

        #endregion

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }

        protected virtual void OnCanExecuteChanged()
        {
            WeakEventHandlerManager.CallWeakReferenceHandlers(this, _CanExecuteChangedHandlers);
        }

        protected virtual void OnIsActiveChanged()
        {
            EventHandler isActiveChangedHandler = IsActiveChanged;
            if (isActiveChangedHandler != null) isActiveChangedHandler(this, EventArgs.Empty);
        }

    }
}
