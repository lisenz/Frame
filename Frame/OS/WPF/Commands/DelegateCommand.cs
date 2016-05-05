using System;
using System.Windows.Input;

namespace Frame.OS.WPF.Commands
{
    public class DelegateCommand<T> : DelegateCommandBase
    {
        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, (o) => true)
        {
        }

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : base((o) => executeMethod((T)o), (o) => canExecuteMethod((T)o))
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException("executeMethod", "Neither the executeMethod nor the canExecuteMethod delegates can be null.");

            Type genericType = typeof(T);

            // DelegateCommand allows object or Nullable<>.  
            // note: Nullable<> is a struct so we cannot use a class constraint.
            if (genericType.IsValueType)
            {
                if ((!genericType.IsGenericType) || (!typeof(Nullable<>).IsAssignableFrom(genericType.GetGenericTypeDefinition())))
                {
                    throw new InvalidCastException("T for DelegateCommand<T> is not an object nor Nullable.");
                }
            }
        }

        public bool CanExecute(T parameter)
        {
            return base.CanExecute(parameter);
        }

        public void Execute(T parameter)
        {
            base.Execute(parameter);
        }
    }

    public class DelegateCommand : DelegateCommandBase
    {

        public DelegateCommand(Action executeMethod)
            : this(executeMethod, () => true)
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base((o) => executeMethod(), (o) => canExecuteMethod())
        {
            if (executeMethod == null || canExecuteMethod == null)
                throw new ArgumentNullException("executeMethod", "Neither the executeMethod nor the canExecuteMethod delegates can be null.");
        }

        public void Execute()
        {
            Execute(null);
        }

        public bool CanExecute()
        {
            return CanExecute(null);
        }
    }
}
