using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace wpfKeyBoard
{
    public class VirtualKeyboardHelper : DependencyObject
    {
        public static readonly DependencyProperty AttachVirtualKeyboardProperty = DependencyProperty.RegisterAttached("AttachVirtualKeyboard", typeof(bool), typeof(VirtualKeyboardHelper), new UIPropertyMetadata(false,OnIsMonitoringChanged));
        //public static readonly DependencyProperty IsMonitoringProperty = DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(VirtualKeyboardHelper), new UIPropertyMetadata(false, OnIsMonitoringChanged));
        
        public static void SetAttachVirtualKeyboard(DependencyObject obj, bool value)
        {
            obj.SetValue(AttachVirtualKeyboardProperty, value);
        }
        //public static void SetIsMonitoring(DependencyObject obj, bool value)
        //{
        //    obj.SetValue(IsMonitoringProperty, value);
        //}
        static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                var txtBox = d as TextBox;

                if ((bool)e.NewValue)
                {
                    txtBox.GotFocus += TextBoxGotFocus;
                }
                else
                {
                    txtBox.GotFocus -= TextBoxGotFocus;
                }
            }
            else if (d is PasswordBox)
            {
                var passBox = d as PasswordBox;

                if ((bool)e.NewValue)
                {
                    passBox.GotFocus += PasswordGotFocus;
                }
                else
                {
                    passBox.GotFocus -= PasswordGotFocus;
                }
            }
  
        }

        static void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (txtBox == null)
                return;
            
            VirtualKeyboardControl vkc = new VirtualKeyboardControl();
            vkc.Width = 500;
            vkc.Height = 200;
            Popup p = new Popup();
           
            StackPanel s = new StackPanel();
            s.Background = Brushes.Azure;
            s.Children.Add(vkc);
            p.Child = s;
            p.AllowsTransparency = true;
            p.PlacementTarget = txtBox;
            p.Placement = PlacementMode.Bottom;
            p.IsOpen = true;

        }

        static void PasswordGotFocus(object sender, RoutedEventArgs e)
        {
            var passBox = sender as PasswordBox;
            if (passBox == null)
                return;
            
        }
    }
}
