using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using FocusGroupHotkeys.Core.Actions;
using FocusGroupHotkeys.Core.Shortcuts.Inputs;
using FocusGroupHotkeys.Core.Shortcuts.Managing;

namespace FocusGroupHotkeys.Shortcuts.Converters {
    public class ActionIdToShortcutGestureConverter : IValueConverter {
        public static ActionIdToShortcutGestureConverter Instance { get; } = new ActionIdToShortcutGestureConverter();

        public string NoSuchActionText { get; set; } = null;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is string id) {
                return ActionIdToGesture(id, this.NoSuchActionText, out string gesture) ? gesture : DependencyProperty.UnsetValue;
            }

            throw new Exception("Value is not a shortcut string");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        public static bool ActionIdToGesture(string id, string fallback, out string gesture) {
            if (ActionManager.Instance.GetAction(id) == null) {
                return (gesture = fallback) != null;
            }

            List<GroupedShortcut> shortcut = WPFShortcutManager.Instance.GetShortcutsByAction(id);
            if (shortcut == null || shortcut.Count < 1) {
                return (gesture = fallback) != null;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(ToString(shortcut[0]));
            if (shortcut.Count > 1) {
                for (int i = 1, end = shortcut.Count - 1; i < end; i++) {
                    sb.Append(", ").Append(ToString(shortcut[i]));
                }

                sb.Append(" or ").Append(ToString(shortcut[shortcut.Count - 1]));
            }

            gesture = sb.ToString();
            return true;
        }

        private static string ToString(GroupedShortcut shortcut) {
            return string.Join(", ", shortcut.Shortcut.InputStrokes.Select(ToString));
        }

        private static string ToString(IInputStroke stroke) {
            if (stroke is MouseStroke ms) {
                return ms.ToString(false, false, false);
            }
            else if (stroke is KeyStroke ks) {
                return ks.ToString(true, false);
            }
            else {
                return stroke.ToString();
            }
        }
    }
}