﻿using System;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace Frame.OS.WPF.Commands
{
    public class ButtonBaseClickCommandBehavior : CommandBehaviorBase<ButtonBase>
    {
        public ButtonBaseClickCommandBehavior(ButtonBase clickableObject)
            : base(clickableObject)
        {
            if (clickableObject == null) throw new System.ArgumentNullException("clickableObject");
            clickableObject.Click += OnClick;
        }

        private void OnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            ExecuteCommand();
        }
    }
}
