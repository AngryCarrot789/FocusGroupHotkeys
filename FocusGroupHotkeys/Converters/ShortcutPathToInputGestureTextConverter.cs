using System;
using System.Globalization;
using System.Windows.Data;
using FocusGroupHotkeys.Core;
using FocusGroupHotkeys.Core.Shortcuts.Managing;

namespace FocusGroupHotkeys.Converters {
    public class ShortcutPathRepresentationConverter : IValueConverter {
        public string NoSuchShortcutFormat { get; set; } = "<{0}>";

        public string ShortcutFormat { get; set; } = null;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value is string path && !string.IsNullOrWhiteSpace(path)) {
                ManagedShortcut shortcut = IoC.ShortcutManager.FindShortcutByPath(path);
                if (shortcut == null) {
                    return this.NoSuchShortcutFormat != null ? string.Format(this.NoSuchShortcutFormat, path) : path;
                }
                else {
                    string str = shortcut.ToString();
                }
            }
            else {
                throw new Exception("Invalid shortcut path: " + value);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}