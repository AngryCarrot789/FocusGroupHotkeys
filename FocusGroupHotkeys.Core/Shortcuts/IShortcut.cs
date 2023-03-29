using System.Collections.Generic;
using FocusGroupHotkeys.Core.Inputs;
using FocusGroupHotkeys.Core.Shortcuts.Usage;

namespace FocusGroupHotkeys.Core.Shortcuts {
    /// <summary>
    /// The base class for all shortcuts
    /// </summary>
    public interface IShortcut {
        /// <summary>
        /// Whether this shortcut is a keyboard-based shortcut. When false, it may be something else (mouse, joystick, etc)
        /// </summary>
        bool IsKeyboard { get; }

        /// <summary>
        /// Whether this shortcut is a mouse-based shortcut. When false, it may be something else (keyboard, joystick, etc)
        /// </summary>
        bool IsMouse { get; }

        /// <summary>
        /// Whether this shortcut has secondary input strokes or not. When it does, it requires
        /// a "Usage" implementation, in order to track the progression of key strokes
        /// </summary>
        bool HasSecondaryStrokes { get; }

        /// <summary>
        /// This shortcut's primary input stroke for initial or full activation
        /// </summary>
        IInputStroke PrimaryStroke { get; }

        /// <summary>
        /// Optional secondary input strokes that are required for this shortcut to be fully activated
        /// </summary>
        IEnumerable<IInputStroke> SecondaryStrokes { get; }

        IShortcutUsage CreateUsage();
    }
}