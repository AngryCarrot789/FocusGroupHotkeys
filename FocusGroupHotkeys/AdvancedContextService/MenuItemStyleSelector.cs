using System.Windows;
using System.Windows.Controls;
using FocusGroupHotkeys.Core.AdvancedContextService;

namespace FocusGroupHotkeys.AdvancedContextService {
    public class MenuItemTemplateSelector : StyleSelector {
        public Style MenuItemTemplate { get; set; }

        public Style SeparatorTemplate { get; set; }

        public MenuItemTemplateSelector() {

        }

        public override Style SelectStyle(object item, DependencyObject container) {
            if (item is ContextEntry) {
                return this.MenuItemTemplate;
            }
            else if (item is ContextEntrySeparator) {
                return this.SeparatorTemplate;
            }
            else {
                return base.SelectStyle(item, container);
            }
        }
    }
}