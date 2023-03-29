using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FocusGroupHotkeys.Core.Inputs;
using FocusGroupHotkeys.Core.Shortcuts.Usage;

namespace FocusGroupHotkeys.Core.Shortcuts {
    /// <summary>
    /// Represents a keyboard-based shortcut. This consists of 1 or more key strokes required to activate it
    /// <para>
    /// When the shortcut only consists of 1 key stroke, the shortcut may be activated immediately.
    /// </para>
    /// </summary>
    public class KeyboardShortcut : IKeyboardShortcut {
        private readonly List<KeyStroke> secondKeyStrokes;

        /// <summary>
        /// The primary key stroke required for this shortcut
        /// </summary>
        public KeyStroke PrimaryKeyStroke { get; }

        /// <summary>
        /// Other key strokes required to be pressed for this keyboard shortcut
        /// </summary>
        public IEnumerable<KeyStroke> SecondKeyStrokes => this.secondKeyStrokes;

        public IInputStroke PrimaryStroke => this.PrimaryKeyStroke;

        public IEnumerable<IInputStroke> SecondaryStrokes {
            get => this.secondKeyStrokes.Cast<IInputStroke>();
        }

        public bool IsKeyboard => true;

        public bool IsMouse => false;

        public bool HasSecondaryStrokes => this.secondKeyStrokes.Count > 0;

        public KeyboardShortcut(KeyStroke primaryKeyStroke) {
            this.PrimaryKeyStroke = primaryKeyStroke;
            this.secondKeyStrokes = new List<KeyStroke>();
        }

        public KeyboardShortcut(KeyStroke primaryKeyStroke, params KeyStroke[] secondKeyStrokes) {
            this.PrimaryKeyStroke = primaryKeyStroke;
            this.secondKeyStrokes = new List<KeyStroke>(secondKeyStrokes);
        }

        public KeyboardShortcut(KeyStroke primaryKeyStroke, IEnumerable<KeyStroke> secondKeyStrokes) {
            this.PrimaryKeyStroke = primaryKeyStroke;
            this.secondKeyStrokes = new List<KeyStroke>(secondKeyStrokes);
        }

        public KeyboardShortcut(KeyStroke primaryKeyStroke, List<KeyStroke> secondKeyStrokes) {
            this.PrimaryKeyStroke = primaryKeyStroke;
            this.secondKeyStrokes = secondKeyStrokes;
        }

        public KeyboardShortcut(IEnumerable<KeyStroke> strokes) {
            using (IEnumerator<KeyStroke> x = strokes.GetEnumerator()) {
                if (!x.MoveNext()) {
                    throw new Exception("IEnumerable did not contain 1 or more elements");
                }

                this.PrimaryKeyStroke = x.Current;
                this.secondKeyStrokes = new List<KeyStroke>();
                while (x.MoveNext()) {
                    this.secondKeyStrokes.Add(x.Current);
                }
            }
        }

        public IKeyboardShortcutUsage CreateKeyUsage() {
            return new KeyboardShortcutUsage(this);
        }

        public IShortcutUsage CreateUsage() {
            return this.CreateKeyUsage();
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.PrimaryKeyStroke.ToString());
            foreach (KeyStroke keyStroke in this.secondKeyStrokes) {
                sb.Append(", ").Append(keyStroke.ToString());
            }

            return sb.ToString();
        }

        public override bool Equals(object obj) {
            if (obj is KeyboardShortcut shortcut) {
                if (this.PrimaryKeyStroke.Equals(shortcut.PrimaryKeyStroke)) {
                    int lenA = this.secondKeyStrokes.Count;
                    int lenB = shortcut.secondKeyStrokes.Count;
                    if (lenA == 0 && lenB == 0) {
                        return true;
                    }
                    else if (lenA != lenB) {
                        return false;
                    }

                    for (int i = 0; i < lenA; i++) {
                        if (!this.secondKeyStrokes[i].Equals(shortcut.secondKeyStrokes[i])) {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode() {
            int code = this.PrimaryKeyStroke.GetHashCode();
            foreach (KeyStroke stroke in this.secondKeyStrokes)
                code += stroke.GetHashCode();
            return code;
        }
    }
}