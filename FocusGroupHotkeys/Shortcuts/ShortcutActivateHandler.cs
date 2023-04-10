using System.Threading.Tasks;
using FocusGroupHotkeys.Core.Shortcuts.Managing;

namespace FocusGroupHotkeys.Shortcuts {
    public delegate Task<bool> ShortcutActivateHandler(ShortcutProcessor processor, GroupedShortcut shortcut);
}