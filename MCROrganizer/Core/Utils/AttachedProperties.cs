using MCROrganizer.Core.CustomControls;
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

    #region MenuItem Properties
    public class MenuItemProperties
    {
        // RunStateProperty.
        public static DependencyProperty RunStateProperty = DependencyProperty.RegisterAttached("RunState", typeof(RunState), typeof(MenuItemProperties), new UIPropertyMetadata(RunState.Pending, OnRunStateChanged));

        public static RunState GetRunState(DependencyObject dependencyObject)
        {
            return (RunState)dependencyObject.GetValue(RunStateProperty);
        }

        public static void SetRunState(DependencyObject dependencyObject, RunState value)
        {
            dependencyObject.SetValue(RunStateProperty, value);
        }

        public static void OnRunStateChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is DesignRunMenuItem designRunMenuItem)
                designRunMenuItem.DesignRunMenuItemDataContext.RunState = (RunState)dependencyPropertyChangedEventArgs.NewValue;
        }
    }
    #endregion
}
