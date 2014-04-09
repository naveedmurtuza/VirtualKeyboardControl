using System.Windows;
using System.Windows.Controls;
using wpfKeyBoard.Model;

namespace wpfKeyBoard
{
    public class KeyTemplateSelector : DataTemplateSelector
    {
        

        public override DataTemplate SelectTemplate(object item,
          DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is VKey)
            {
                VKey vkey = item as VKey;
                switch (vkey.KeyType)
                {
                        case KeyType.Switcheroo:
                        return element.FindResource("toggleButtonTemplate") as DataTemplate;
                    case KeyType.Null:
                        return element.FindResource("nullButtonTemplate") as DataTemplate;
                    default:
                        return element.FindResource("buttonTemplate") as DataTemplate;
                }                   
            }

            return null;
        }
    }
}
