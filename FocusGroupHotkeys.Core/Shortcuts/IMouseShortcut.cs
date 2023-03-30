using System.Collections.Generic;
using FocusGroupHotkeys.Core.Inputs;
using FocusGroupHotkeys.Core.Shortcuts.Usage;

namespace FocusGroupHotkeys.Core.Shortcuts.ViewModels {
    public interface IMouseShortcut : IShortcut {
        /// <summary>
        /// Other key strokes required to be pressed for this mouse shortcut
        /// </summary>
        IEnumerable<MouseStroke> SecondMouseStrokes { get; }

        /// <summary>
        /// This can be used in order to track the usage of <see cref="SecondKeyStrokes"/>. If
        /// the list is empty, then the return value of this function is effectively pointless
        /// </summary>
        /// <returns></returns>
        IMouseShortcutUsage CreateMouseUsage();
    }
}