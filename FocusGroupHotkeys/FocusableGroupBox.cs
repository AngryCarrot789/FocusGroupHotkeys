using System.Windows;

namespace FocusGroupHotkeys {
    public static class FocusableGroupBox {
        public static readonly DependencyProperty IsGroupBoxFocusedProperty =
            DependencyProperty.RegisterAttached(
                "IsGroupBoxFocused",
                typeof(bool),
                typeof(FocusableGroupBox),
                new PropertyMetadata(false));

        public static void SetIsGroupBoxFocused(DependencyObject element, bool value) {
            element.SetValue(IsGroupBoxFocusedProperty, value);
        }

        public static bool GetIsGroupBoxFocused(DependencyObject element) {
            return (bool) element.GetValue(IsGroupBoxFocusedProperty);
        }
    }
}