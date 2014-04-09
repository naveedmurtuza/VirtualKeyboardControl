using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Resources;
using System.Xml.Linq;
using Microsoft.Expression.Interactivity.Core;
using wpfKeyBoard.Model;
using Keyboard = wpfKeyBoard.Model.Keyboard;

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
    ///     "Add Reference"->"Projects"->[Select this project]
    ///     Step 2)
    ///     Go ahead and use your control in the XAML file.
    ///     <MyNamespace:CustomControl1 />
    /// </summary>
    [TemplatePart(Name = PART_KeyboardButtonPanel, Type = typeof (ContentControl))]
    [TemplatePart(Name = PART_LanguagePopup, Type = typeof (Popup))]
    public class VirtualKeyboardControl : Control
    {
        private const string PART_KeyboardButtonPanel = "PART_KeyboardButtonPanel";
        private const string PART_LanguagePopup = "PART_LanguagePopup";


        #region Depedency Properties

       public static readonly DependencyProperty VKeysProperty = DependencyProperty.Register(
            "VKeys", typeof (ObservableCollection<VKey>), typeof (VirtualKeyboardControl),
            new PropertyMetadata(default(ObservableCollection<VKey>)));

        public static readonly DependencyProperty TextFontFamilyUriProperty = DependencyProperty.Register(
            "TextFontFamilyUri", typeof (string), typeof (VirtualKeyboardControl), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.Register(
            "ColumnCount", typeof (int), typeof (VirtualKeyboardControl), new PropertyMetadata(default(int)));

        public static readonly DependencyProperty RowCountProperty = DependencyProperty.Register(
            "RowCount", typeof (int), typeof (VirtualKeyboardControl), new PropertyMetadata(default(int)));

        #endregion

        private readonly List<Keyboard> _keyboards = new List<Keyboard>();
        private ContentControl _buttonPanel;
        private int _currentPageIndex;
        private Popup _langPopup;
        private Switcheroo _selectedSwitcheroo;

        static VirtualKeyboardControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (VirtualKeyboardControl),
                new FrameworkPropertyMetadata(typeof (VirtualKeyboardControl)));
        }

        public VirtualKeyboardControl()
        {
            if (VKeys == null)
            {
                VKeys = new ObservableCollection<VKey>();
            }
            Languages = new List<Language>();
            LanguageSelectedCommand = new ActionCommand(OnLanguageChanged);
            KeyDoubleClickCommand = new ActionCommand(OnKeyDoubleClickCommand);
            KeyPressCommand = new ActionCommand(OnKeyPress);
            Load();
        }

        

        #region Events

        public static readonly RoutedEvent VirtualKeyPressedEvent = EventManager.RegisterRoutedEvent(
            "VirtualKeyPressed", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (VirtualKeyboardControl));

        public event RoutedEventHandler VirtualKeyPressed
        {
            add { AddHandler(VirtualKeyPressedEvent, value); }
            remove { RemoveHandler(VirtualKeyPressedEvent, value); }
        }

        #endregion //Events

        public int RowCount
        {
            get { return (int) GetValue(RowCountProperty); }
            set { SetValue(RowCountProperty, value); }
        }

        public int ColumnCount
        {
            get { return (int) GetValue(ColumnCountProperty); }
            set { SetValue(ColumnCountProperty, value); }
        }

        public string TextFontFamilyUri
        {
            get { return (string) GetValue(TextFontFamilyUriProperty); }
            set { SetValue(TextFontFamilyUriProperty, value); }
        }


        public ICommand LanguageSelectedCommand { get; private set; }

        public ICommand KeyDoubleClickCommand { get; private set; }
        public ICommand KeyPressCommand { get; private set; }
        public List<Language> Languages { get; private set; }

        public ObservableCollection<VKey> VKeys
        {
            get { return (ObservableCollection<VKey>) GetValue(VKeysProperty); }
            set { SetValue(VKeysProperty, value); }
        }

        public Keyboard SelectedKeyboard { get; set; }

        private void OnLanguageChanged(object o)
        {
            var language = (Language) o;
            _selectedSwitcheroo = null;
            ChangeKeyboard(language.ISO2Code);
            if (_langPopup.IsOpen)
                _langPopup.IsOpen = false;
        }


        private void ChangeKeyboard(string langCode)
        {
            SelectedKeyboard = _keyboards.FirstOrDefault(keyboard => keyboard.Language.ISO2Code == langCode);
            RowCount = SelectedKeyboard.RowCount;
            ColumnCount = SelectedKeyboard.ColumnCount;
            RefillKeys(SelectedKeyboard.DefaultSwitcheroo);
        }


        private void Load()
        {
            var uris = new List<string>();

            using (
                Stream stream =
                    Application.GetResourceStream(new Uri("pack://application:,,,/WpfKeyboard.Resources;Component/keyboards/Includes.xml")).Stream)
            {
                var reader = new StreamReader(stream);
                XDocument doc = XDocument.Load(reader);
                IEnumerable<XElement> includes = doc.Descendants("keyboard");
                uris.AddRange(includes.Select(element => element.Attribute("name").Value));
            }
            foreach (string uri in uris)
            {
                StreamResourceInfo sri = Application.GetResourceStream(new Uri("pack://application:,,,/WpfKeyboard.Resources;Component/keyboards/" + uri));
                if (sri != null)
                {
                    using (Stream s = sri.Stream)
                    {
                        _keyboards.Add(KeyboardParser.ParseKeyboardFile(new StreamReader(s), OnKeyPress, CanKeyPress));
                    }
                }
            }
            Languages = _keyboards.Select(keyboard => keyboard.Language).ToList();
            ChangeKeyboard(Languages.First(lang => lang.IsDefault).ISO2Code);
        }

        private void OnKeyDoubleClickCommand(object obj)
        {
            var param = (object[])obj;
            var sender = param[0] as ButtonBase;
            var vkey = param[1] as VKey;
            SetLockStatus(vkey, true);

        }

        private void SetLockStatus(VKey vkey,bool status)
        {
            Switcheroo switcheroo =
                SelectedKeyboard.Switcheroos.FirstOrDefault(x => x.Name == vkey.SwitcherooCode && x.IsVolatile);
            if (switcheroo != null)
            {
                foreach (var key in switcheroo.KeyCollection.Where(k => k.IsPressed))
                {
                    key.IsLocked = status;
                }
            }
        }
        private void OnKeyPress(object o)
        {
            var sender = o as ButtonBase;
            var key = sender.Tag as VKey;
            Key modifierStatus = default(Key);
            switch (key.KeyType)
            {
                case KeyType.LanguageKey:
                    ToggleLanguagePopup(sender);
                    break;
                case KeyType.Switcheroo:
                case KeyType.Page:
                    HandleSwitcherooOrPageActionKey(key);
                    break;
            }
            
            if (_selectedSwitcheroo != null &&_selectedSwitcheroo.Name != SelectedKeyboard.DefaultSwitcheroo)
            {
                modifierStatus = _selectedSwitcheroo.KeyCollection.First(k => k.IsPressed).KeyCode;
            }
            RaiseEvent(new VirtualKeyPressedEventArgs(VirtualKeyPressedEvent, key, modifierStatus));

            if (key.KeyType != KeyType.Text) return;

            if (_selectedSwitcheroo != null && _selectedSwitcheroo.IsVolatile)
            {
                VKey presedkey = _selectedSwitcheroo.KeyCollection.FirstOrDefault(k => k.IsPressed);
                if (presedkey != null && !presedkey.IsLocked)
                {
                    var switcherooCode = HandleSwitcherooKey(key);
                    RefillKeys(switcherooCode, key);
                }
            }
            
        }

        private void RefillKeys(string switcherooCode,VKey key = null)
        {
            Switcheroo switcheroo = SelectedKeyboard.Switcheroos.FirstOrDefault(x => x.Name == switcherooCode);
            TextFontFamilyUri = String.IsNullOrEmpty(switcheroo.FontFamilyUri) ? switcheroo.FontFamily : switcheroo.FontFamilyUri;
            VKeys.Clear();
            foreach (VKey k in switcheroo.KeyCollection)
            {
                VKeys.Add(k);
            }
            if (key == null) return;
            //this is to ensure that the pressed keys remain the same
            List<VKey> keys = switcheroo.KeyCollection.FindAll(vkey => key.Value == vkey.Value);
            foreach (VKey k in keys)
            {
                k.IsPressed = key.IsPressed;
            }
            //reset the current key state ,since it now belongs to previous key collection...
            if (!String.IsNullOrEmpty(key.SwitcherooCode))
                key.IsPressed = !key.IsPressed;
        }

        private void HandleSwitcherooOrPageActionKey(VKey key)
        {
            string switcherooCode =
             key.KeyType == KeyType.Switcheroo
                ? HandleSwitcherooKey(key)
                : HandlePageActionKey(key);
            //update the keys
            RefillKeys(switcherooCode,key);
        }

        private string HandlePageActionKey(VKey key)
        {
            string switcherooCode = key.SwitcherooCode;
            int maxPages = SelectedKeyboard.Switcheroos.First(x => x.Name == switcherooCode).PageCount;
            switch (key.PageAction.ToLower())
            {
                case "next":
                    _currentPageIndex++;
                    switcherooCode = switcherooCode + _currentPageIndex;
                    break;
                case "previous":
                    _currentPageIndex--;
                    switcherooCode = _currentPageIndex == 0 ? switcherooCode : switcherooCode + _currentPageIndex;
                    break;
            }
            Switcheroo switcheroo = SelectedKeyboard.Switcheroos.First(s => s.Name == switcherooCode);
            //update page action keys state
            List<VKey> pageKeys = switcheroo.KeyCollection.FindAll(vkey => null != vkey.PageAction);
            foreach (VKey item in pageKeys)
            {
                if (item.PageAction == "next")
                {
                    item.IsEnabled = _currentPageIndex == 0 & maxPages > 1;
                }
                else
                {
                    item.IsEnabled = _currentPageIndex != 0;
                }
            }
            return switcherooCode;
        }

        private string HandleSwitcherooKey(VKey key)
        {
            string switcherooCode = key.SwitcherooCode;
            //switch the layout ...
            switcherooCode = key.IsPressed ? switcherooCode : SelectedKeyboard.DefaultSwitcheroo;
            //unlock the key if it is locked
            SetLockStatus(key,false);
            if (_selectedSwitcheroo == null || _selectedSwitcheroo.Name != switcherooCode)
            {
                Switcheroo switcheroo = SelectedKeyboard.Switcheroos.First(s => s.Name == switcherooCode);
                //we switched the layout so reset the page index
                _currentPageIndex = 0;
                //enable the keys with page action next, and disable the previous
                List<VKey> pageKeys =
                    switcheroo.KeyCollection.FindAll(vkey => null != vkey.PageAction);
                foreach (VKey item in pageKeys)
                {
                    item.IsEnabled = item.PageAction == "next";
                }
                _selectedSwitcheroo = switcheroo;
            }
            
            //update the modifier key status
            return switcherooCode;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _buttonPanel = GetTemplateChild(PART_KeyboardButtonPanel) as ContentControl;
            _langPopup = GetTemplateChild(PART_LanguagePopup) as Popup;
            _langPopup.DataContext = this;
        }

        private void ToggleLanguagePopup(ButtonBase button)
        {
            _langPopup.PlacementTarget = button;
            _langPopup.Placement = PlacementMode.Custom;
            _langPopup.CustomPopupPlacementCallback = (size, targetSize, offset) =>
            {
                var placement1 =
                    new CustomPopupPlacement(new Point(0 - (size.Width - targetSize.Width), (0 - size.Height) - 5),
                        PopupPrimaryAxis.Horizontal);
                return new[] {placement1, placement1};
            };
            _langPopup.IsOpen = true;
        }

        private bool CanKeyPress(VKey key)
        {
            return true;
        }
    }

    public class VirtualKeyPressedEventArgs : RoutedEventArgs
    {
        private readonly Key _modifierKey;
        private readonly VKey _vkey;

        public VirtualKeyPressedEventArgs(RoutedEvent routedEvent, VKey vkey, Key modifierKey)
            : base(routedEvent)
        {
            _vkey = vkey;
            _modifierKey = modifierKey;
        }

        public Key ModifierKeyStatus
        {
            get { return _modifierKey; }
        }

        public VKey VKey
        {
            get { return _vkey; }
        }
    }
}