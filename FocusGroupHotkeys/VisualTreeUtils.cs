using System.Windows;
using System.Windows.Media;

namespace FocusGroupHotkeys {
    public static class VisualTreeUtils {
        /// <summary>
        /// Returns the control which has the given inherited property defined
        /// </summary>
        /// <param name="property"></param>
        /// <param name="startObject"></param>
        /// <returns></returns>
        public static DependencyObject FindInheritedPropertyDefinition(DependencyProperty property, DependencyObject startObject) {
            DependencyObject obj = startObject;
            while (obj != null && obj.ReadLocalValue(property) == DependencyProperty.UnsetValue)
                obj = VisualTreeHelper.GetParent(obj);
            return obj;
        }
    }
}