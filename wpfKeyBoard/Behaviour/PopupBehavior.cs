using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace wpfKeyBoard.Behaviour
{
    public class PopupBehavior : Behavior<UIElement>
    {
        private Popup _popup;
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(UIElement.MouseDownEvent, new RoutedEventHandler(AssociatedObjectMouseDown), true);
            AssociatedObject.AddHandler(UIElement.MouseUpEvent, new RoutedEventHandler(AssociatedObjectMouseUp), true);
        }

        private Popup GetPopupComponent(FrameworkElement element)
        {
            if (_popup != null)
            {
                _popup.PlacementTarget = element;
                return _popup;
            }

            _popup = element.FindResource("popup") as Popup;
            _popup.PlacementTarget = element;
            _popup.Placement = PlacementMode.Custom;
            _popup.CustomPopupPlacementCallback = (size, targetSize, offset) =>
            {
                var midpoint = new Point(targetSize.Width / 2, targetSize.Height / 2);
                var placement1 =
                    new CustomPopupPlacement(new Point(0 + (midpoint.X - 24), (0 - size.Height) - 5),
                        PopupPrimaryAxis.Horizontal);
                return new[] { placement1, placement1 };
            };
            return _popup;
        }
        void AssociatedObjectMouseDown(object sender, System.Windows.RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;

            var popup = GetPopupComponent(element);
            if(popup != null)
                popup.IsOpen = true;
        }

        void AssociatedObjectMouseUp(object sender, System.Windows.RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            var popup = GetPopupComponent(element);
            if (popup != null)
            {
                Task.Factory.StartNew(() => Thread.Sleep(200)).ContinueWith((dummy) =>
                {
                    popup.Dispatcher.InvokeAsync(() => popup.IsOpen = false);
                });

            }
                

        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(UIElement.MouseDownEvent, new RoutedEventHandler(AssociatedObjectMouseDown));
            AssociatedObject.RemoveHandler(UIElement.MouseDownEvent, new RoutedEventHandler(AssociatedObjectMouseUp));
        }
    }
}
