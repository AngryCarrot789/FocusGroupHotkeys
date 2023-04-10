using FocusGroupHotkeys.Core.Shortcuts.Dialogs;
using FocusGroupHotkeys.Core.Shortcuts.Inputs;

namespace FocusGroupHotkeys.Shortcuts.Dialogs {
    public class KeyboardDialogService : IKeyboardDialogService {
        public KeyStroke? ShowGetKeyStrokeDialog() {
            KeyStrokeInputWindow window = new KeyStrokeInputWindow();
            if (window.ShowDialog() != true || window.Stroke.Equals(default)) {
                return null;
            }
            else {
                return window.Stroke;
            }
        }
    }
}