using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using FocusGroupHotkeys.Core.Utils;

namespace FocusGroupHotkeys.Converters {
    public class KeyStrokeRepresentationConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values == null || values.Length != 3) {
                throw new Exception("This converter requires 3 elements; keycode, modifiers, isRelease");
            }

            if (!(values[0] is int keyCode)) throw new Exception("values[0] must be an int: keycode");
            if (!(values[1] is int modifiers)) throw new Exception("values[1] must be an int: modifiers");
            if (!(values[2] is bool isRelease)) throw new Exception("values[2] must be a bool: isRelease");

            StringBuilder sb = new StringBuilder();
            string mods = ModsToString((ModifierKeys) modifiers);
            if (mods.Length > 0) {
                sb.Append(mods).Append('+');
            }

            sb.Append((Key) keyCode).Append(isRelease ? " (R)" : " (P)");
            return sb.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }

        public static string ModsToString(ModifierKeys keys) {
            StringJoiner joiner = new StringJoiner(new StringBuilder(), "+");
            if ((keys & ModifierKeys.Control) != 0) joiner.Append("Ctrl");
            if ((keys & ModifierKeys.Alt) != 0)     joiner.Append("Alt");
            if ((keys & ModifierKeys.Shift) != 0)   joiner.Append("Shift");
            if ((keys & ModifierKeys.Windows) != 0) joiner.Append("Win");
            return joiner.ToString();
        }
    }
}