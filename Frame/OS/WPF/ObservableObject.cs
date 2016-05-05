using System.Windows;
using System.ComponentModel;

namespace Frame.OS.WPF
{
    public class ObservableObject<T> : FrameworkElement, INotifyPropertyChanged
    {
        public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register("Value", typeof(T), typeof(ObservableObject<T>), new PropertyMetadata(ValueChangedCallback));

        public event PropertyChangedEventHandler PropertyChanged;

        public T Value
        {
            get { return (T)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        private static void ValueChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ObservableObject<T> thisInstance = ((ObservableObject<T>)d);
            PropertyChangedEventHandler eventHandler = thisInstance.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(thisInstance, new PropertyChangedEventArgs("Value"));
            }
        }
    }
}
