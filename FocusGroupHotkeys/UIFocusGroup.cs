using System;
using System.Diagnostics;
using System.Windows;

namespace FocusGroupHotkeys {
    public class UIFocusGroup {
        public static readonly DependencyProperty FocusGroupPathProperty =
            DependencyProperty.RegisterAttached(
                "FocusGroupPath",
                typeof(string),
                typeof(UIFocusGroup),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty IsGlobalShortcutFocusTargetProperty =
            DependencyProperty.RegisterAttached(
                "IsGlobalShortcutFocusTarget",
                typeof(bool),
                typeof(AppShortcutManager),
                new PropertyMetadata(false, AppShortcutManager.OnIsGlobalShortcutFocusTargetChanged));

        public static readonly DependencyProperty UsageIDProperty =
            DependencyProperty.RegisterAttached(
                "UsageID",
                typeof(string),
                typeof(UIFocusGroup),
                new PropertyMetadata(AppShortcutManager.DEFAULT_USAGE_ID));

        public static readonly DependencyProperty HasGroupFocusProperty =
            DependencyProperty.RegisterAttached(
                "HasGroupFocus",
                typeof(bool),
                typeof(UIFocusGroup),
                new PropertyMetadata(false));


        public delegate void FocusGroupPathChangedEventHandler(string oldPath, string newPath);
        public static event FocusGroupPathChangedEventHandler OnFocusedGroupPathChanged;

        public static WeakReference<DependencyObject> CurrentlyFocusedObject { get; } = new WeakReference<DependencyObject>(null);

        /// <summary>
        /// The currently focused group
        /// </summary>
        public static string FocusedGroupPath { get; private set; }

        /// <summary>
        /// Sets the focus group path for the specific element
        /// </summary>
        public static void SetFocusGroupPath(DependencyObject element, string value) {
            element.SetValue(FocusGroupPathProperty, value);
        }

        /// <summary>
        /// Gets the focus group path for the specific element
        /// </summary>
        public static string GetFocusGroupPath(DependencyObject element) {
            return (string) element.GetValue(FocusGroupPathProperty);
        }

        public static void SetIsGlobalShortcutFocusTarget(UIElement element, bool value) {
            element.SetValue(UIFocusGroup.IsGlobalShortcutFocusTargetProperty, value);
        }

        public static bool GetIsGlobalShortcutFocusTarget(UIElement element) {
            return (bool) element.GetValue(UIFocusGroup.IsGlobalShortcutFocusTargetProperty);
        }

        public static void SetUsageID(DependencyObject element, string value) {
            element.SetValue(UsageIDProperty, value);
        }

        public static string GetUsageID(DependencyObject element) {
            return (string) element.GetValue(UsageIDProperty);
        }

        public static void SetHasGroupFocus(DependencyObject element, bool value) {
            element.SetValue(HasGroupFocusProperty, value);
        }

        public static bool GetHasGroupFocus(DependencyObject element) {
            return (bool) element.GetValue(HasGroupFocusProperty);
        }

        public static void RaiseFocusGroupPathChanged(string oldGroup, string newGroup) {
            OnFocusedGroupPathChanged?.Invoke(oldGroup, newGroup);
        }

        public static string ProcessFocusGroupChange(DependencyObject obj) {
            string oldPath = FocusedGroupPath;
            string newPath = GetFocusGroupPath(obj);
            if (oldPath == newPath) {
                return oldPath;
            }

            FocusedGroupPath = newPath;
            RaiseFocusGroupPathChanged(oldPath, newPath);
            UpdateVisualFocusGroup(obj);
            return newPath;
        }

        /// <summary>
        /// Looks through the given dependency object's parent chain for an element that has the <see cref="FocusGroupPathProperty"/> explicitly
        /// set, assuming that means it is a primary focus group, and then sets the <see cref="HasGroupFocusProperty"/> to true for
        /// that element, and false for the last element that was focused
        /// </summary>
        /// <param name="eventObject"></param>
        public static void UpdateVisualFocusGroup(DependencyObject eventObject) {
            if (CurrentlyFocusedObject.TryGetTarget(out DependencyObject lastFocused)) {
                CurrentlyFocusedObject.SetTarget(null);
                SetHasGroupFocus(lastFocused, false);
            }

            DependencyObject root = VisualTreeUtils.FindInheritedPropertyDefinition(FocusGroupPathProperty, eventObject); // = target;
            // do {
            //     root = VisualTreeUtils.FindInheritedPropertyDefinition(FocusGroupPathProperty, root);
            // } while (root != null && !GetHasAdvancedFocusVisual(root) && (root = VisualTreeHelper.GetParent(root)) != null);

            if (root != null) {
                CurrentlyFocusedObject.SetTarget(root);
                SetHasGroupFocus(root, true);
                if (root is UIElement element && element.Focusable && !element.IsFocused) {
                    element.Focus();
                }
            }
            else {
                // ???
                Debug.WriteLine("Failed to find root control that owns the FocusGroupPathProperty of " + GetFocusGroupPath(eventObject));
            }
        }
    }
}