using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace wpfKeyBoard.Model
{
    /// <summary>
    /// Represents a key on the virtual keybaord
    /// </summary>
    public class VKey : INotifyPropertyChanged
    {
        private bool _pressed,_enabled,_locked;

        public String Value { get; set; }

        public KeyType KeyType { get; set; }

        /// <summary>
        /// Specifies the possible keycode
        /// </summary>
        public Key KeyCode { get; set; }

        
        public String SwitcherooCode { get; set; }

        public bool IsPressed
        {
            get { return _pressed; }
            set
            {
                if (_pressed != value)
                {
                    _pressed = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLocked
        {
            get { return _locked; }
            set
            {
                if (_locked != value)
                {
                    _locked = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsEnabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool UsesRenderer { get; set; }


        public int Row { get; set; }
        public int Column { get; set; }

        public int RowSpan { get; set; }

        public int ColumnSpan { get; set; }


        public int Rotate { get; set; }
        public string PageAction { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;


        public override bool Equals(object obj)
        {
            
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var vkey = obj as VKey;
            return vkey.Row == Row && vkey.Column == Column;
        }

        
        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash*7) + Row.GetHashCode();
            hash = (hash*7) + Column.GetHashCode();
            hash = (hash*7) + Value.GetHashCode();
            return hash;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum KeyType
    {
        Text,
        Switcheroo,
        LanguageKey,
        Virtual,
        Page,
        Null
    }

    public class KeyboardUtilities
    {
        public static Key ParseKeyCode(string keycode)
        {
            Key key;
            return Enum.TryParse(keycode, true, out key) ? key : key;
        }

        public static KeyType ParseKeyType(string keytype)
        {
            KeyType key;
            return Enum.TryParse(keytype, true, out key) ? key : KeyType.Text;
        }
    }
}