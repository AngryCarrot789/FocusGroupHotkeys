using System.Collections.Generic;
using FocusGroupHotkeys.Core.Inputs;
using FocusGroupHotkeys.Core.Shortcuts.Usage;

namespace FocusGroupHotkeys.Core.Shortcuts {
    public interface IKeyboardShortcut : IShortcut {
        /// <summary>
        /// Other key strokes required to be pressed for this keyboard shortcut
        /// </summary>
        IEnumerable<KeyStroke> SecondKeyStrokes { get; }

        /// <summary>
        /// This can be used in order to track the usage of <see cref="SecondKeyStrokes"/>. If
        /// the list is empty, then the return value of this function is effectively pointless
        /// </summary>
        /// <returns></returns>
        IKeyboardShortcutUsage CreateKeyUsage();
    }
}