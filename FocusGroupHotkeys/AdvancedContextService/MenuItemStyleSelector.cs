using System.Windows;
using System.Windows.Controls;
using FocusGroupHotkeys.Core.AdvancedContextService;

namespace FocusGroupHotkeys.AdvancedContextService {
    public class MenuItemStyleSelector : StyleSelector {
        public Style NonCheckableMenuItemStyle { get; set; }
        public Style CheckableMenuItemStyle { get; set; }

        public Style SeparatorStyle { get; set; }

        public MenuItemStyleSelector() {

        }

        public override Style SelectStyle(object item, DependencyObject container) {
            if (container is MenuItem) {
                return item is ContextEntryCheckable ? this.CheckableMenuItemStyle : this.NonCheckableMenuItemStyle;
            }
            else if (container is Separator) {
                return this.SeparatorStyle;
            }
            else {
                return base.SelectStyle(item, container);
            }
        }
    }
}