using System.Windows;
using System.Windows.Controls;

namespace FocusGroupHotkeys.AdvancedContext {
    public static class ContextMenuHelper {
        public static readonly DependencyProperty ContextProviderProperty = DependencyProperty.RegisterAttached(
            "ContextProvider", typeof(IContextProvider), typeof(ContextMenuHelper), new PropertyMetadata(null, OnContextProviderPropertyChanged));

        public static void SetContextProvider(DependencyObject element, IContextProvider value) {
            element.SetValue(ContextProviderProperty, value);
        }

        public static IContextProvider GetContextProvider(DependencyObject element) {
            return (IContextProvider) element.GetValue(ContextProviderProperty);
        }

        private static void OnContextProviderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue != e.OldValue) {
                ContextMenuService.RemoveContextMenuOpeningHandler(d, Handler);
                if (e.NewValue != null) {
                    ContextMenuService.AddContextMenuOpeningHandler(d, Handler);
                }
            }
        }

        private static void Handler(object sender, ContextMenuEventArgs e) {
            if (sender is DependencyObject targetElement && GetContextProvider(targetElement) is IContextProvider provider) {
                if (!(ContextMenuService.GetContextMenu(targetElement) is ContextMenu menu)) {
                    ContextMenuService.SetContextMenu(targetElement, menu = new ContextMenu());
                }

                menu.Items.Clear();
                foreach (ContextEntry entry in provider.GetContextEntries()) {
                    MenuItem item = new MenuItem();
                    item.Command =
                }
            }
        }
    }
}