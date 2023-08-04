using System.Windows.Input;

namespace FocusGroupHotkeys.Core.Shortcuts {
    public interface IShortcutToCommand {
        ICommand GetCommandForShortcut(string shortcutId);
    }
}