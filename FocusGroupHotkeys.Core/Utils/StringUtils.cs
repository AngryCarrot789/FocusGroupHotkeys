using System.Text;

namespace FocusGroupHotkeys.Core.Utils {
    public static class StringUtils {
        public static string JSubstring(this string @this, int startIndex, int endIndex) {
            return @this.Substring(startIndex, endIndex - startIndex);
        }

        public static bool IsEmpty(this string @this) {
            return string.IsNullOrEmpty(@this);
        }

        public static string Join(string a, string b, char join) {
            return new StringBuilder(32).Append(a).Append(join).Append(b).ToString();
        }

        public static string Join(string a, string b, string c, char join) {
            return new StringBuilder(32).Append(a).Append(join).Append(b).Append(join).Append(c).ToString();
        }
    }
}