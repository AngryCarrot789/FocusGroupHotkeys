using System.Collections.Generic;
using FocusGroupHotkeys.Core.Inputs;

namespace FocusGroupHotkeys.Core.Shortcuts.Usage {
    public class KeyboardShortcutUsage : IKeyboardShortcutUsage {
        private LinkedListNode<KeyStroke> currentStroke;

        public IKeyboardShortcut KeyboardShortcut { get; }

        public LinkedList<KeyStroke> Strokes { get; }

        public KeyStroke CurrentKeyStroke => this.currentStroke?.Value ?? default;

        public IShortcut Shortcut {
            get => this.KeyboardShortcut;
        }

        public bool IsCompleted => this.currentStroke == null;

        public IInputStroke PreviousStroke { get; private set; }

        public IInputStroke CurrentStroke => this.currentStroke?.Value;

        public IEnumerable<IInputStroke> RemainingStrokes {
            get {
                LinkedListNode<KeyStroke> stroke = this.currentStroke;
                while (stroke != null) {
                    yield return stroke.Value;
                    stroke = stroke.Next;
                }
            }
        }

        public bool IsCurrentStrokeMouseBased => false;

        public bool IsCurrentStrokeKeyBased => true;

        public KeyboardShortcutUsage(IKeyboardShortcut shortcut) {
            this.KeyboardShortcut = shortcut;
            this.Strokes = new LinkedList<KeyStroke>(shortcut.SecondKeyStrokes);
            this.currentStroke = this.Strokes.First;
            this.PreviousStroke = this.Shortcut.PrimaryStroke;
        }

        public bool OnKeyStroke(in KeyStroke stroke) {
            if (this.currentStroke == null) {
                return true;
            }

            if (this.currentStroke.Value.Equals(stroke)) {
                this.PreviousStroke = stroke;
                this.currentStroke = this.currentStroke.Next;
                return true;
            }
            else {
                return false;
            }
        }

        public bool OnInputStroke(IInputStroke stroke) {
            return stroke is KeyStroke keyStroke && this.OnKeyStroke(keyStroke);
        }
    }
}