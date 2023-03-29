using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FocusGroupHotkeys.Core.Inputs;
using FocusGroupHotkeys.Core.Shortcuts.Usage;
using FocusGroupHotkeys.Core.Shortcuts.ViewModels;

namespace FocusGroupHotkeys.Core.Shortcuts {
    public class MouseKeyboardShortcut : IMouseShortcut, IKeyboardShortcut {
        private readonly List<IInputStroke> secondStrokes;

        public MouseStroke PrimaryMouseStroke => this.PrimaryStroke is MouseStroke stroke ? stroke : throw new Exception("Primary stroke is not a mouse stroke");

        public KeyStroke PrimaryKeyStroke => this.PrimaryStroke is KeyStroke stroke ? stroke : throw new Exception("Primary stroke is not a key stroke");

        public IEnumerable<MouseStroke> SecondMouseStrokes => this.secondStrokes.OfType<MouseStroke>();

        public IEnumerable<KeyStroke> SecondKeyStrokes => this.secondStrokes.OfType<KeyStroke>();

        public IInputStroke PrimaryStroke { get; }

        public IEnumerable<IInputStroke> SecondaryStrokes {
            get => this.secondStrokes;
        }

        public bool IsKeyboard => true;

        public bool IsMouse => true;

        public bool HasSecondaryStrokes => this.secondStrokes.Count > 0;

        public MouseKeyboardShortcut(IInputStroke primaryMouseStroke) {
            this.PrimaryStroke = primaryMouseStroke;
            this.secondStrokes = new List<IInputStroke>();
        }

        public MouseKeyboardShortcut(IInputStroke primaryMouseStroke, params IInputStroke[] secondMouseStrokes) {
            this.PrimaryStroke = primaryMouseStroke;
            this.secondStrokes = new List<IInputStroke>(secondMouseStrokes);
        }

        public MouseKeyboardShortcut(IInputStroke primaryMouseStroke, IEnumerable<IInputStroke> secondMouseStrokes) {
            this.PrimaryStroke = primaryMouseStroke;
            this.secondStrokes = new List<IInputStroke>(secondMouseStrokes);
        }

        public MouseKeyboardShortcut(IInputStroke primaryMouseStroke, List<IInputStroke> secondStrokes) {
            this.PrimaryStroke = primaryMouseStroke;
            this.secondStrokes = secondStrokes;
        }

        public MouseKeyboardShortcut(IEnumerable<IInputStroke> strokes) {
            using (IEnumerator<IInputStroke> x = strokes.GetEnumerator()) {
                if (!x.MoveNext()) {
                    throw new Exception("IEnumerable did not contain 1 or more elements");
                }

                this.PrimaryStroke = x.Current;
                this.secondStrokes = new List<IInputStroke>();
                while (x.MoveNext()) {
                    this.secondStrokes.Add(x.Current);
                }
            }
        }

        public IMouseShortcutUsage CreateMouseUsage() {
            return (IMouseShortcutUsage) this.CreateUsage();
        }

        public IKeyboardShortcutUsage CreateKeyUsage() {
            return (IKeyboardShortcutUsage) this.CreateUsage();
        }

        public IShortcutUsage CreateUsage() {
            return new MouseKeyboardShortcutUsage(this);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.PrimaryStroke);
            foreach (IInputStroke stroke in this.secondStrokes) {
                sb.Append(", ").Append(stroke);
            }

            return sb.ToString();
        }

        public override bool Equals(object obj) {
            if (obj is MouseKeyboardShortcut shortcut) {
                if (this.PrimaryStroke.Equals(shortcut.PrimaryStroke)) {
                    int lenA = this.secondStrokes.Count;
                    int lenB = shortcut.secondStrokes.Count;
                    if (lenA == 0 && lenB == 0) {
                        return true;
                    }
                    else if (lenA != lenB) {
                        return false;
                    }

                    for (int i = 0; i < lenA; i++) {
                        if (!this.secondStrokes[i].Equals(shortcut.secondStrokes[i])) {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode() {
            int code = this.PrimaryStroke.GetHashCode();
            foreach (IInputStroke stroke in this.secondStrokes)
                code += stroke.GetHashCode();
            return code;
        }
    }
}