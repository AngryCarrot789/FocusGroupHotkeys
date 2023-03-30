using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using FocusGroupHotkeys.Core.AdvancedContextService;

namespace FocusGroupHotkeys.AdvancedContextService {
    public static class ContextMenuHelper {
        public static readonly DependencyProperty ContextProviderProperty =
            DependencyProperty.RegisterAttached(
                "ContextProvider",
                typeof(IContextProvider),
                typeof(ContextMenuHelper),
                new PropertyMetadata(null, OnContextProviderPropertyChanged));

        public static readonly DependencyProperty InsertionIndexProperty =
            DependencyProperty.RegisterAttached(
                "InsertionIndex",
                typeof(int),
                typeof(ContextMenuHelper),
                new PropertyMetadata(-1));

        public static readonly DependencyPropertyKey LastAppendEndIndexPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "LastAppendEndIndex",
                typeof(int),
                typeof(ContextMenuHelper),
                new PropertyMetadata(-1));

        private static void SetLastAppendEndIndex(DependencyObject element, int value) {
            element.SetValue(LastAppendEndIndexPropertyKey, value);
        }

        private static int GetLastAppendEndIndex(DependencyObject element) {
            return (int) element.GetValue(LastAppendEndIndexPropertyKey.DependencyProperty);
        }

        public static void SetContextProvider(DependencyObject element, IContextProvider value) {
            element.SetValue(ContextProviderProperty, value);
        }

        public static IContextProvider GetContextProvider(DependencyObject element) {
            return (IContextProvider) element.GetValue(ContextProviderProperty);
        }

        public static void SetInsertionIndex(DependencyObject element, int value) {
            element.SetValue(InsertionIndexProperty, value);
        }

        public static int GetInsertionIndex(DependencyObject element) {
            return (int) element.GetValue(InsertionIndexProperty);
        }

        private static void OnContextProviderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue != e.OldValue) {
                ContextMenuService.RemoveContextMenuOpeningHandler(d, OnContextMenuOpening);
                ContextMenuService.RemoveContextMenuClosingHandler(d, OnContextMenuClosing);
                if (e.NewValue != null) {
                    GetOrCreateContextMenu(d);
                    ContextMenuService.AddContextMenuOpeningHandler(d, OnContextMenuOpening);
                    ContextMenuService.AddContextMenuClosingHandler(d, OnContextMenuClosing);
                }
            }
        }

        public static void OnContextMenuOpening(object sender, ContextMenuEventArgs e) {
            if (sender is DependencyObject targetElement && GetContextProvider(targetElement) is IContextProvider provider) {
                AdvancedContextMenu menu = GetOrCreateContextMenu(targetElement);
                int count = menu.Items.Count;
                int index = GetInsertionIndex(targetElement);
                if (index < 0) {
                    menu.Items.Clear();
                    foreach (IBaseContextEntry entry in provider.RootContextEntries) {
                        menu.Items.Add(entry);
                    }
                }
                else if (index == menu.Items.Count) {
                    foreach (IBaseContextEntry entry in provider.RootContextEntries) {
                        menu.Items.Add(entry);
                    }

                    SetLastAppendEndIndex(targetElement, menu.Items.Count);
                }
                else {
                    List<object> items = new List<object>();
                    foreach (object item in menu.Items) {
                        items.Add(item);
                    }

                    menu.Items.Clear();
                    List<IBaseContextEntry> entries = provider.RootContextEntries.ToList();
                    items.InsertRange(index = index > count ? count : index, entries);
                    foreach (object item in items) {
                        menu.Items.Add(item);
                    }

                    SetLastAppendEndIndex(targetElement, index + entries.Count);
                }

                if (menu.Items.Count < 1) {
                    e.Handled = true;
                }
            }
        }

        public static void OnContextMenuClosing(object sender, ContextMenuEventArgs e) {
            if (sender is DependencyObject targetElement && ContextMenuService.GetContextMenu(targetElement) is ContextMenu menu) {
                Application.Current.Dispatcher.Invoke(() => {
                    // Reduce memory usage :D Hopefully...

                    // 0
                    // 1
                    // 2
                    // 3
                    // 4 !
                    // 5 !

                    int index = GetInsertionIndex(targetElement); // 4
                    if (index < 0) {
                        menu.Items.Clear();
                    }
                    else {
                        int lastEndIndex = GetLastAppendEndIndex(targetElement);
                        if (lastEndIndex != -1) {
                            try {
                                for (int i = lastEndIndex - 1; i >= index; i--) {
                                    menu.Items.RemoveAt(i);
                                }
                            }
                            finally {
                                SetLastAppendEndIndex(targetElement, -1);
                            }
                        }
                    }
                }, DispatcherPriority.DataBind);
            }
        }

        private static AdvancedContextMenu GetOrCreateContextMenu(DependencyObject targetElement) {
            ContextMenu menu = ContextMenuService.GetContextMenu(targetElement);
            if (!(menu is AdvancedContextMenu advancedMenu)) {
                ContextMenuService.SetContextMenu(targetElement, advancedMenu = new AdvancedContextMenu());
            }

            return advancedMenu;
        }
    }
}