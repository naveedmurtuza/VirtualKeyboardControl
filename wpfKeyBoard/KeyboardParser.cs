using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using wpfKeyBoard.Model;

namespace wpfKeyBoard
{
    internal class KeyboardParser
    {
        private static VKey BuildKeyFromElement(int rowIndex, int colIndex, XElement key, Action<object> keyPress,
            Func<VKey, bool> canPress)
        {
            var vkey = new VKey
            {
                UsesRenderer = key.Attribute("usesRenderer") != null && bool.Parse(key.Attribute("usesRenderer").Value),
                Row = rowIndex,
                Rotate = key.Attribute("rotate") == null ? 0 : int.Parse(key.Attribute("rotate").Value),
                RowSpan = key.Attribute("rowspan") == null ? 1 : int.Parse(key.Attribute("rowspan").Value),
                IsEnabled = key.Attribute("enabled") == null || bool.Parse(key.Attribute("enabled").Value),
                KeyType = 
                    KeyboardUtilities.ParseKeyType(key.Attribute("keytype") == null
                        ? "text"
                        : key.Attribute("keytype").Value),
                KeyCode =
                    KeyboardUtilities.ParseKeyCode(key.Attribute("keycode") == null
                        ? "text"
                        : key.Attribute("keycode").Value),
                PageAction = key.Attribute("pageAction") == null ? null : key.Attribute("pageAction").Value,
                SwitcherooCode = key.Attribute("switcherooCode") == null ? null : key.Attribute("switcherooCode").Value,
                Value = key.Attribute("value") != null ? key.Attribute("value").Value : key.Value,
                ColumnSpan = key.Attribute("colspan") == null ? 2 : int.Parse(key.Attribute("colspan").Value)*2,
                Column = colIndex,
            };

            if (vkey.KeyType == KeyType.Null)
                vkey.ColumnSpan = 1;
            return vkey;
        }

        public static Keyboard ParseKeyboardFile(TextReader reader, Action<object> keyPress, Func<VKey, bool> canPress)
        {
            XDocument doc = XDocument.Load(reader);
            XElement keyboardElement = doc.Element("keyboard");
            IEnumerable<XElement> switcherooElements = keyboardElement.Element("switcheroos").Elements();


            List<Switcheroo> switcheroos = switcherooElements.Select(switcherooElement => new Switcheroo
            {
                Name = switcherooElement.Name.LocalName,
                FontFamilyUri =
                    switcherooElement.Attribute("fontFamilyUri") == null
                        ? string.Empty
                        : switcherooElement.Attribute("fontFamilyUri").Value,
                FontFamily =
                    switcherooElement.Attribute("fontFamily") == null
                        ? string.Empty
                        : switcherooElement.Attribute("fontFamily").Value,
                PageCount =
                    switcherooElement.Attribute("pages") == null
                        ? 0
                        : int.Parse(switcherooElement.Attribute("pages").Value),
                IsVolatile =
                    switcherooElement.Attribute("volatile") != null &&
                    bool.Parse(switcherooElement.Attribute("volatile").Value),
                IsDefault =
                    switcherooElement.Attribute("default") != null &&
                    bool.Parse(switcherooElement.Attribute("default").Value)
            }).ToList();
            //look if there are sub pages in the switcheroo
            //sub pages are expected to be switcheroo/swictheroo1/switcheroo2
            var pagedSwitcheroos = new List<Switcheroo>();
            foreach (Switcheroo switcheroo in switcheroos.Where(switcheroo => switcheroo.HasPages))
            {
                for (int i = 1; i < switcheroo.PageCount; i++)
                {
                    pagedSwitcheroos.Add(new Switcheroo
                    {
                        Name = switcheroo.Name + Convert.ToString(i),
                        FontFamily = switcheroo.FontFamily,
                        FontFamilyUri = switcheroo.FontFamilyUri,
                        PageCount = switcheroo.PageCount,
                        Virtual = true
                    });
                }
            }
            switcheroos.AddRange(pagedSwitcheroos);

            string langCode = keyboardElement.Attribute("langcode").Value;
            string langName = keyboardElement.Attribute("langName").Value;
            string langNative = keyboardElement.Attribute("langNative").Value;
            string langDescription = keyboardElement.Attribute("description").Value;
            int colCount = int.Parse(keyboardElement.Attribute("columns").Value);
            int rowCount = int.Parse(keyboardElement.Attribute("rows").Value);
            bool isDefault = keyboardElement.Attribute("default") == null ||
                             bool.Parse(keyboardElement.Attribute("default").Value);


            IEnumerable<XElement> rows = doc.Descendants("row");
            int rowIndex = 0;
            foreach (XElement row in rows)
            {
                foreach (Switcheroo switcheroo in switcheroos)
                {
                    string xname = switcheroo.Name;
                    IEnumerable<XElement> elements = row.Descendants(xname);
                    int colIndex = 0;

                    foreach (XElement key in elements)
                    {
                        VKey vkey = BuildKeyFromElement(rowIndex, colIndex, key, keyPress, canPress);
                        switcheroo.KeyCollection.Add(vkey);
                        colIndex = (colIndex + (vkey.ColumnSpan));
                    }
                }

                rowIndex++;
            }
            //sub pages are not expected to contain all keys 
            //so make a union of main page and the subpage
            foreach (
                Switcheroo switcheroo in switcheroos.Where(switcheroo => switcheroo.HasPages && !switcheroo.Virtual))
            {
                for (int i = 1; i < switcheroo.PageCount; i++)
                {
                    string switcherooCode = switcheroo.Name + Convert.ToString(i);
                    Switcheroo other = switcheroos.FirstOrDefault(s => s.Name == switcherooCode);
                    other.KeyCollection = switcheroo.KeyCollection.Union(other.KeyCollection).ToList();
                }
            }
            //toggle the next/previous page buttons
            foreach (Switcheroo switcheroo in switcheroos)
            {
                foreach (VKey k in switcheroo.KeyCollection.Where(k => k.PageAction != null))
                {
                    k.IsEnabled = k.PageAction != "previous" && switcheroo.HasPages;
                    if (String.IsNullOrEmpty(k.SwitcherooCode))
                        k.SwitcherooCode = switcheroo.Name;
                }
            }
            return new Keyboard
            {
                RowCount = rowCount,
                ColumnCount = colCount,
                Language = new Language
                {
                    ISO2Code = langCode,
                    Name = langName,
                    NativeName = langNative,
                    Description = langDescription,
                    IsDefault = isDefault,
                },
                DefaultSwitcheroo = switcheroos.First(s => s.IsDefault).Name,
                
                //KeyCollection = keyCollection,
                Switcheroos = switcheroos
            };
        }
    }
}