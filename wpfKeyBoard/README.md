## VirtualKeyboardControl ##

**VirtualKeyboardControl** is a full-featured On-Screen Keyboard with customizable layouts,languages and themes. The layout and languages can be added using xml files. The theme for buttons provided is a verbatim copy from MahApps.Metro. 

This library also contains **VkbTextBox** control which is a Textbox control with the keyboard attached.
 
### Usage ###
Include ```Wpfkeyboard.dll``` ```Wpfkeyboard.Resources.dll```

In App.xaml

```
<Application x:Class="WpfApplication2.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             StartupUri="MainWindow.xaml">
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <ResourceDictionary Source="pack://application:,,,/WpfKeyboard.Resources;component/styles/Colors.xaml" />
                <ResourceDictionary Source="pack://application:,,,/WpfKeyboard.Resources;component/styles/Button.xaml" />
                <ResourceDictionary Source="pack://application:,,,/WpfKeyboard.Resources;component/styles/ToggleButton.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

In your MainWindow

add xml reference ```xmlns:wpfKeyBoard="clr-namespace:wpfKeyBoard;assembly=wpfKeyBoard"```

then
```<wpfKeyBoard:VirtualKeyboardControl x:Name="KeyboardControl" VirtualKeyPressed="KeyboardControl_OnVirtualKeyPressed" />```
or
```<wpfKeyBoard:VkbTextBox Width="400" VerticalContentAlignment="Center" EnterClosesKeyboard="True"   />```

### Existing Keyboard Layouts ###
Apart from the styles ```Wpfkeyboard.Resources.dll``` contains two keyboard layouts en-US,ar-SA, FontAwesome for rendering icons, 
### Creating your own Keyboard Layouts ###
Here is the short version of the existing keyboard file. The attributes used are explained in the below table.For Multi-page ```switcheroos``` just use the switcheroo name and the page index (eg. &lt;numSym1>{&lt;/numSym>)
<pre>
```
&lt;keyboard default="true" rows="4" columns="24" langcode="en" langName="English" langNative="English" description="English QWERTY Layout">
  &lt;switcheroos>
    &lt;normal default="true" fontFamily="Segoe UI"></normal>
    &lt;shift volatile="true" fontFamily="Segoe UI"></shift>
&lt;!--by specifying the fontfamilyUri, specified font is used when displaying this key collection -->
    &lt;smiley fontfamilyUri="/WpfKeyboard.Resources;Component/fonts/#Emoticons" ></smiley>
    &lt;numSym pages="2"/>
    &lt;ctrl volatile="true" />
  &lt;/switcheroos>
	&lt;row>
		&lt;key>
			&lt;normal>q&lt;/normal>
			&lt;shift>Q&lt;/shift>
			&lt;numSym keytype="virtual" keycode="Tab">&lt;/numSym>
			&lt;smiley usesRenderer="true" >;)&lt;/smiley>
      &lt;ctrl>q&lt;/ctrl>
		&lt;/key>
		....
		&lt;key>
			&lt;normal>t&lt;/normal>
			&lt;shift>T&lt;/shift>
			&lt;numSym>$&lt;/numSym>
			&lt;smiley usesRenderer="true" >;)&lt;/smiley>
      &lt;ctrl>new tab&lt;/ctrl>
		&lt;/key>
		....
	&lt;/row>
	....
&lt;/keyboard>
```
</pre>
<table><tbody><tr><th>attribute</th><th>usage</th><th>available in tag</th></tr><tr><td>rows</td><td>number of rows in keyboard</td><td>keyboard</td></tr><tr><td>columns</td><td>number of columns in keyboard</td><td>keyboard</td></tr><tr><td>default</td><td>the default lang/layout to show</td><td>keyboard</td></tr><tr><td>langcode</td><td>typically ISO2 language code</td><td>keyboard</td></tr><tr><td>langName</td><td>Name of the language</td><td>keyboard</td></tr><tr><td>langNative</td><td>Native Name of language</td><td>keyboard</td></tr><tr><td>description</td><td>keyboard layout description</td><td>keyboard</td></tr><tr><td>default</td><td>this is the layout that will appear</td><td>switcheroos&lt;switcheroo-name&gt;</td></tr><tr><td>fontfamilyUri</td><td>a wpf pack uri for this swictheroo</td><td>switcheroos&lt;switcheroo-name&gt;</td></tr><tr><td>fontFamily</td><td>font family name for this swictheroo</td><td>switcheroos&lt;switcheroo-name&gt;</td></tr><tr><td>volatile</td><td>key state is volatile, like shift</td><td>switcheroos&lt;switcheroo-name&gt;</td></tr><tr><td>pages</td><td>number of pages</td><td>switcheroos&lt;switcheroo-name&gt;</td></tr><tr><td>keytype</td><td>wpfKeyBoard.Model.KeyType</td><td>row/key/&lt;switcheroo-name&gt;</td></tr><tr><td>keycode</td><td>System.Input.Key value</td><td>row/key/&lt;switcheroo-name&gt;</td></tr><tr><td>colspan</td><td>x</td><td>row/key/&lt;switcheroo-name&gt;</td></tr><tr><td>rotate</td><td>rotates the text by given degress</td><td>row/key/&lt;switcheroo-name&gt;</td></tr><tr><td>switcherooCode</td><td>x</td><td>row/key/&lt;switcheroo-name&gt;</td></tr><tr><td>column</td><td>x</td><td>row/key/&lt;switcheroo-name&gt;</td></tr><tr><td>pageAction</td><td>action string to execute when nay page button is pressed</td><td>row/key/&lt;switcheroo-name&gt;</td></tr><tr><td>IsEnabled</td><td>x</td><td>row/key/&lt;switcheroo-name&gt;</td></tr><tr><td>value</td><td>use this value instead from the 'tag value'</td><td>row/key/&lt;switcheroo-name&gt;</td></tr><tr><td>usesRenderer</td><td>uses FontAwesome font to draw icon</td><td>row/key/&lt;switcheroo-name&gt;</td></tr></tbody></table>

###Boring ...###
Copyright (c) 2014, Naveed Quadri

Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee is hereby granted, provided that the above copyright notice and this permission notice appear in all copies.

THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.