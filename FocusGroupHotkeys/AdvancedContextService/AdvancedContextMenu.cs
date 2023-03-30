using System;
using System.Windows;
using System.Windows.Controls;
using FocusGroupHotkeys.Core.AdvancedContextService;

namespace FocusGroupHotkeys.AdvancedContextService {
    public class AdvancedContextMenu : ContextMenu {
        private object currentItem;

        public AdvancedContextMenu() {

        }

        public static DependencyObject CreateChildMenuItem(object item) {
            if (item is ContextEntry) {
                return new AdvancedMenuItem();
            }
            else if (item is ContextEntrySeparator) {
                return new Separator();
            }
            else {
                throw new Exception("Unknown item type: " + item);
                // return new MenuItem();
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item) {
            if (item is MenuItem || item is Separator)
                return true;
            this.currentItem = item;
            return false;
        }

        protected override DependencyObject GetContainerForItemOverride() {
            object item = this.currentItem;
            this.currentItem = null;
            if (this.UsesItemContainerTemplate) {
                DataTemplate dataTemplate = this.ItemContainerTemplateSelector.SelectTemplate(item, this);
                if (dataTemplate != null) {
                    object obj = dataTemplate.LoadContent();
                    if (obj is MenuItem || obj is Separator) {
                        return (DependencyObject) obj;
                    }

                    throw new InvalidOperationException("Invalid data template object: " + obj);
                }
            }

            return CreateChildMenuItem(item);
        }
    }
}