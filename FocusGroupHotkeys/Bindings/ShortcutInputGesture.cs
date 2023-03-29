using System.Windows;
using System.Windows.Input;
using FocusGroupHotkeys.Core.Inputs;

namespace FocusGroupHotkeys.Bindings {
    public class ShortcutInputGesture : InputGesture {
        public static ShortcutInputGesture CurrentInputGesture { get; private set; }

        public ShortcutBinding ShortcutBinding { get; }

        public bool IsCompleted { get; set; }

        public ShortcutInputGesture(ShortcutBinding shortcutPath) {
            this.ShortcutBinding = shortcutPath;
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs) {
            try {
                CurrentInputGesture = this;
                string focusedGroup;
                if (!(targetElement is DependencyObject obj) || (focusedGroup = UIFocusGroup.GetFocusGroupPath(obj)) == null) {
                    return false;
                }

                AppShortcutManager manager = AppShortcutManager.Instance;
                if (this.ShortcutBinding != null && inputEventArgs is KeyEventArgs args) {
                    KeyStroke stroke = new KeyStroke((int) args.Key, (int) Keyboard.Modifiers, args.IsUp);
                    if (manager.OnKeyStroke(focusedGroup, stroke)) {
                        args.Handled = true;
                        if (this.IsCompleted) {
                            this.IsCompleted = false;
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                }

                return false;
            }
            finally {
                CurrentInputGesture = null;
            }
        }
    }
}