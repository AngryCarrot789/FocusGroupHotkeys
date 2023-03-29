using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FocusGroupHotkeys.Core.Inputs;
using FocusGroupHotkeys.Core.Shortcuts.Usage;
using FocusGroupHotkeys.Core.Shortcuts.ViewModels;

namespace FocusGroupHotkeys.Core.Shortcuts {
    public class MouseShortcut : IMouseShortcut {
        private readonly List<MouseStroke> secondMouseStrokes;

        /// <summary>
        /// The primary mouse stroke required for this shortcut
        /// </summary>
        public MouseStroke PrimaryMouseStroke { get; }

        public IEnumerable<MouseStroke> SecondMouseStrokes => this.secondMouseStrokes;

        public IInputStroke PrimaryStroke => this.PrimaryMouseStroke;

        public IEnumerable<IInputStroke> SecondaryStrokes {
            get => this.SecondMouseStrokes.Cast<IInputStroke>();
        }

        public bool IsKeyboard => false;

        public bool IsMouse => true;

        public bool HasSecondaryStrokes => this.secondMouseStrokes.Count > 0;

        public MouseShortcut(MouseStroke primaryMouseStroke) {
            this.PrimaryMouseStroke = primaryMouseStroke;
            this.secondMouseStrokes = new List<MouseStroke>();
        }

        public MouseShortcut(MouseStroke primaryMouseStroke, params MouseStroke[] secondMouseStrokes) {
            this.PrimaryMouseStroke = primaryMouseStroke;
            this.secondMouseStrokes = new List<MouseStroke>(secondMouseStrokes);
        }

        public MouseShortcut(MouseStroke primaryMouseStroke, IEnumerable<MouseStroke> secondMouseStrokes) {
            this.PrimaryMouseStroke = primaryMouseStroke;
            this.secondMouseStrokes = new List<MouseStroke>(secondMouseStrokes);
        }

        public MouseShortcut(MouseStroke primaryMouseStroke, List<MouseStroke> secondMouseStrokes) {
            this.PrimaryMouseStroke = primaryMouseStroke;
            this.secondMouseStrokes = secondMouseStrokes;
        }

        public MouseShortcut(IEnumerable<MouseStroke> strokes) {
            using (IEnumerator<MouseStroke> x = strokes.GetEnumerator()) {
                if (!x.MoveNext()) {
                    throw new Exception("IEnumerable did not contain 1 or more elements");
                }

                this.PrimaryMouseStroke = x.Current;
                this.secondMouseStrokes = new List<MouseStroke>();
                while (x.MoveNext()) {
                    this.secondMouseStrokes.Add(x.Current);
                }
            }
        }

        public IMouseShortcutUsage CreateMouseUsage() {
            return new MouseShortcutUsage(this);
        }

        public IShortcutUsage CreateUsage() {
            return this.CreateMouseUsage();
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.PrimaryMouseStroke.ToString());
            foreach (MouseStroke keyStroke in this.secondMouseStrokes) {
                sb.Append(", ").Append(keyStroke.ToString());
            }

            return sb.ToString();
        }

        public override bool Equals(object obj) {
            if (obj is MouseShortcut shortcut) {
                if (this.PrimaryMouseStroke.Equals(shortcut.PrimaryMouseStroke)) {
                    int lenA = this.secondMouseStrokes.Count;
                    int lenB = shortcut.secondMouseStrokes.Count;
                    if (lenA == 0 && lenB == 0) {
                        return true;
                    }
                    else if (lenA != lenB) {
                        return false;
                    }

                    for (int i = 0; i < lenA; i++) {
                        if (!this.secondMouseStrokes[i].Equals(shortcut.secondMouseStrokes[i])) {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode() {
            int code = this.PrimaryMouseStroke.GetHashCode();
            foreach (MouseStroke stroke in this.secondMouseStrokes)
                code += stroke.GetHashCode();
            return code;
        }
    }
}