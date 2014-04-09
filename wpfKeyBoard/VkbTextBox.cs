using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using wpfKeyBoard.Model;
using Keyboard = System.Windows.Input.Keyboard;

namespace wpfKeyBoard
{
    /// <summary>
    ///     Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///     Step 1a) Using this custom control in a XAML file that exists in the current project.
    ///     Add this XmlNamespace attribute to the root element of the markup file where it is
    ///     to be used:
    ///     xmlns:MyNamespace="clr-namespace:wpfKeyBoard"
    ///     Step 1b) Using this custom control in a XAML file that exists in a different project.
    ///     Add this XmlNamespace attribute to the root element of the markup file where it is
    ///     to be used:
    ///     xmlns:MyNamespace="clr-namespace:wpfKeyBoard;assembly=wpfKeyBoard"
    ///     You will also need to add a project reference from the project where the XAML file lives
    ///     to this project and Rebuild to avoid compilation errors:
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///     Step 2)
    ///     Go ahead and use your control in the XAML file.
    ///     <MyNamespace:VkbTextBox />
    /// </summary>
    [TemplatePart(Name = PART_KeyboardPopup, Type = typeof (Popup))]
    [TemplatePart(Name = PART_Keyboard, Type = typeof (VirtualKeyboardControl))]
    [TemplatePart(Name = PART_TextBox, Type = typeof (TextBox))]
    public class VkbTextBox : TextBox
    {
        private const string PART_KeyboardPopup = "PART_KeyboardPopup";
        private const string PART_Keyboard = "PART_Keyboard";
        private const string PART_TextBox = "PART_TextBox";

        public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(
            "DisplayText", typeof (string), typeof (VkbTextBox), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty EnterClosesKeyboardProperty = DependencyProperty.Register(
            "EnterClosesKeyboard", typeof (bool), typeof (VkbTextBox), new PropertyMetadata(false));

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            "IsOpen", typeof (bool), typeof (VkbTextBox), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IconTriggerProperty = DependencyProperty.Register(
            "IconTrigger", typeof (bool), typeof (VkbTextBox), new PropertyMetadata(default(bool)));

        private VirtualKeyboardControl _virtualKeyboardControl;
        private Popup _virtualKeyboardPopup;


        static VkbTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (VkbTextBox),
                new FrameworkPropertyMetadata(typeof (VkbTextBox)));
        }

        public VkbTextBox()
        {
            Keyboard.AddKeyDownHandler(this, OnKeyDown);
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideCapturedElement);
        }

        public string DisplayText
        {
            get { return (string) GetValue(DisplayTextProperty); }
            set { SetValue(DisplayTextProperty, value); }
        }

        public bool EnterClosesKeyboard
        {
            get { return (bool) GetValue(EnterClosesKeyboardProperty); }
            set { SetValue(EnterClosesKeyboardProperty, value); }
        }

        public bool IsOpen
        {
            get { return (bool) GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public bool IconTrigger
        {
            get { return (bool) GetValue(IconTriggerProperty); }
            set { SetValue(IconTriggerProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_virtualKeyboardPopup != null)
                _virtualKeyboardPopup.Opened -= OnVirtualKeyboardPopupOpened;

            _virtualKeyboardPopup = GetTemplateChild(PART_KeyboardPopup) as Popup;

            if (_virtualKeyboardPopup != null)
                _virtualKeyboardPopup.Opened += OnVirtualKeyboardPopupOpened;

            if (_virtualKeyboardControl != null)
                _virtualKeyboardControl.VirtualKeyPressed -= OnVirtualKeyPressed;

            _virtualKeyboardControl = GetTemplateChild(PART_Keyboard) as VirtualKeyboardControl;

            if (_virtualKeyboardControl != null)
                _virtualKeyboardControl.VirtualKeyPressed += OnVirtualKeyPressed;
        }

        #region Event Handlers

        private void OnVirtualKeyPressed(object sender, RoutedEventArgs routedEventArgs)
        {
            var args = (VirtualKeyPressedEventArgs) routedEventArgs;
            var modifier = args.ModifierKeyStatus;
            if (modifier == Key.LeftCtrl || modifier == Key.LeftCtrl)
                return;//we dont need to add anything to text if ctrl is pressed
            switch (args.VKey.KeyType)
            {
                case KeyType.Text:
                    
                    Text += args.VKey.Value;
                    break;
                case KeyType.Virtual:
                    switch (args.VKey.KeyCode)
                    {
                        case Key.Back:
                            if (Text.Length > 0)
                                Text = Text.Remove(Text.Length - 1);
                            break;
                        case Key.Enter:
                            if (EnterClosesKeyboard)
                            {
                                CloseKeyboard();
                            }
                            break;
                    }
                    break;
            }
        }


        private void OnVirtualKeyboardPopupOpened(object sender, EventArgs e)
        {
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!IsOpen)
            {
                if (IsKeyModifyingPopupState(e))
                {
                    IsOpen = true;
                    // Calculator will get focus in CalculatorPopup_Opened().
                    e.Handled = true;
                }
            }
            else
            {
                if (IsKeyModifyingPopupState(e))
                {
                    CloseKeyboard();
                    e.Handled = true;
                }
                else if (e.Key == Key.Escape)
                {
                    //if (EnterClosesKeyboard)
                    //    Value = _initialValue;
                    CloseKeyboard();
                    e.Handled = true;
                }
            }
        }

        private void OnMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
        {
            CloseKeyboard();
        }

        #endregion //Event Handlers

        #region Methods

        internal static bool IsKeyModifyingPopupState(KeyEventArgs e)
        {
            return ((((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt) &&
                     ((e.SystemKey == Key.Down) || (e.SystemKey == Key.Up)))
                    || (e.Key == Key.F4));
        }

        private void CloseKeyboard()
        {
            if (IsOpen)
                IsOpen = false;
            ReleaseMouseCapture();
        }

        #endregion //Methods
    }
}