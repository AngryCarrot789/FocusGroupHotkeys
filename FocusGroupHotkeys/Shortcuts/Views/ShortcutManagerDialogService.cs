using FocusGroupHotkeys.Core.Shortcuts.Dialogs;
using FocusGroupHotkeys.Core.Shortcuts.ViewModels;

namespace FocusGroupHotkeys.Shortcuts.Views {
    public class ShortcutManagerDialogService : IShortcutManagerDialogService {
        private ShortcutEditorWindow window;

        public bool IsOpen => this.window != null;

        public void ShowEditorDialog() {
            if (this.window != null) {
                return;
            }

            this.window = new ShortcutEditorWindow();
            ShortcutManagerViewModel manager = new ShortcutManagerViewModel();
            manager.LoadFromRoot(WPFShortcutManager.Instance.Root);
            this.window.DataContext = manager;
            this.window.Closed += (sender, args) => {
                this.window = null;
            };

            this.window.Show();
        }
    }
}