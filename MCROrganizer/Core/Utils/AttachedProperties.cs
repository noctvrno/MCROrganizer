using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MCROrganizer.Core.Utils
{
    #region TextBox Properties
    public class TextBoxProperties
    {
        public static DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(TextBoxProperties), new UIPropertyMetadata(false, OnIsFocusedChanged));

        public static bool GetIsFocused(DependencyObject dependencyObject)
        {
            return (bool)dependencyObject.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject dependencyObject, bool value)
        {
            dependencyObject.SetValue(IsFocusedProperty, value);
        }

        public static void OnIsFocusedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            TextBox textBox = dependencyObject as TextBox;
            if ((Boolean)dependencyPropertyChangedEventArgs.NewValue && !(Boolean)dependencyPropertyChangedEventArgs.OldValue && !textBox.IsFocused)
            {
                textBox.Focusable = true;
                textBox.Focus();
                Keyboard.Focus(textBox);
                textBox.SelectAll();
            }
        }
    }
    #endregion
}
