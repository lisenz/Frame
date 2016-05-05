using System;
using System.Windows;

namespace Frame.OS.WPF.Regions
{
    public class ItemMetadata : DependencyObject
    {
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(ItemMetadata), null);

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(ItemMetadata), new PropertyMetadata(DependencyPropertyChanged));


        public ItemMetadata(object item)
        {
            this.Item = item;
        }

        public object Item { get; private set; }

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public event EventHandler MetadataChanged;

        public void InvokeMetadataChanged()
        {
            EventHandler metadataChangedHandler = MetadataChanged;
            if (metadataChangedHandler != null) 
                metadataChangedHandler(this, EventArgs.Empty);
        }

        private static void DependencyPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            ItemMetadata itemMetadata = dependencyObject as ItemMetadata;
            if (itemMetadata != null)
            {
                itemMetadata.InvokeMetadataChanged();
            }
        }
    }
}
