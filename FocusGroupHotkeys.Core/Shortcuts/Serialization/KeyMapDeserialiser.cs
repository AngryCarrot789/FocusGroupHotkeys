using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using FocusGroupHotkeys.Core.Inputs;
using FocusGroupHotkeys.Core.Shortcuts.Managing;

namespace FocusGroupHotkeys.Core.Shortcuts.Serialization {
    public abstract class KeyMapDeserialiser {
        private static readonly XmlSerializer Serializer;

        static KeyMapDeserialiser() {
            Serializer = new XmlSerializer(typeof(KeyMap));
        }

        public KeyMapDeserialiser() {

        }

        protected abstract Keystroke SerialiseKeystroke(in KeyStroke stroke);

        protected abstract Mousestroke SerialiseMousestroke(in MouseStroke stroke);

        protected abstract KeyStroke DeserialiseKeystroke(Keystroke stroke);

        protected abstract MouseStroke DeserialiseMousestroke(Mousestroke stroke);

        protected virtual void SerialiseInputStrokeToShortcut(Shortcut shortcut, IInputStroke stroke) {
            if (stroke is MouseStroke secondMouseStroke) {
                shortcut.Strokes.Add(this.SerialiseMousestroke(secondMouseStroke));
            }
            else if (stroke is KeyStroke secondKeyStroke) {
                shortcut.Strokes.Add(this.SerialiseKeystroke(secondKeyStroke));
            }
            else {
                throw new Exception("Unknown input stroke type: " + stroke?.GetType());
            }
        }

        public virtual void Serialise(Stream output, ShortcutGroup @group) {
            KeyMap map = new KeyMap();
            this.SerialiseGroup(map, @group);
            Serializer.Serialize(new XmlTextWriter(output, null) {
                Formatting = Formatting.Indented,
                Indentation = 4,
                Settings = {
                    CloseOutput = false
                }
            }, map);
        }

        /// <summary>
        /// Deserializes the given input stream into the root <see cref="ShortcutGroup"/>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual ShortcutGroup Deserialise(Stream input) {
            KeyMap result = (KeyMap) Serializer.Deserialize(input);
            if (result == null) {
                throw new Exception("Failed to decompile key map; null returned");
            }

            ShortcutGroup tempParent = new ShortcutGroup(null, false, false);
            ShortcutGroup group = this.DeserialiseGroup(result, tempParent);
            group.Parent = null;
            return group;
        }

        protected virtual IEnumerable<IInputStroke> DeserialiseStrokes(List<object> strokes) {
            foreach (object stroke in strokes) {
                if (stroke is Keystroke ks) {
                    yield return this.DeserialiseKeystroke(ks);
                }
                else if (stroke is Mousestroke ms) {
                    yield return this.DeserialiseMousestroke(ms);
                }
                else {
                    throw new Exception("Unknown input stroke type: " + stroke?.GetType());
                }
            }
        }

        protected virtual ShortcutGroup DeserialiseGroup(Group keyGroup, ShortcutGroup parent) {
            // if (string.IsNullOrWhiteSpace(group.Name) && group.IsGlobal != "true") {
            //     throw new Exception("Non-global group has a null or empty name");
            // }

            ShortcutGroup focusGroup = parent.CreateGroupByName(keyGroup.Name, keyGroup.IsGlobalBool, keyGroup.InheritFromParent == "true");
            if (keyGroup.Shortcuts != null && keyGroup.Shortcuts.Count > 0) {
                foreach (Shortcut cut in keyGroup.Shortcuts) {
                    bool hasKey = false;
                    bool hasMouse = false;
                    if (cut.Strokes != null && cut.Strokes.Count(x => x is Keystroke) > 0) {
                        hasKey = true;
                    }

                    if (cut.Strokes != null && cut.Strokes.Count(x => x is Mousestroke) > 0) {
                        hasMouse = true;
                    }

                    IShortcut shortcut;
                    if (hasKey && hasMouse) {
                        shortcut = new MouseKeyboardShortcut(this.DeserialiseStrokes(cut.Strokes));
                    }
                    else if (hasKey) {
                        List<KeyStroke> strokes = cut.Strokes.OfType<Keystroke>().Select(this.DeserialiseKeystroke).ToList();
                        shortcut = new KeyboardShortcut(strokes);
                    }
                    else if (hasMouse) {
                        List<MouseStroke> strokes = cut.Strokes.OfType<Mousestroke>().Select(this.DeserialiseMousestroke).ToList();
                        shortcut = new MouseShortcut(strokes);
                    }
                    else {
                        continue;
                    }

                    ManagedShortcut managed = focusGroup.AddShortcut(cut.Name, shortcut);
                    managed.ActionID = cut.ActionID;
                    managed.Description = cut.Description;
                }
            }

            if (keyGroup.InnerGroups != null && keyGroup.InnerGroups.Count > 0) {
                foreach (Group innerGroup in keyGroup.InnerGroups) {
                    this.DeserialiseGroup(innerGroup, focusGroup);
                }
            }

            return focusGroup;
        }

        protected virtual void SerialiseGroup(Group group, ShortcutGroup focusGroup) {
            group.Name = focusGroup.FocusGroupName;
            group.IsGlobal = SerialiseObject(focusGroup.IsGlobal);
            group.InheritFromParent = SerialiseObject(focusGroup.InheritFromParent);
            group.InnerGroups = new List<Group>();
            group.Shortcuts = new List<Shortcut>();
            foreach (ManagedShortcut shortcut in focusGroup.Shortcuts) {
                Shortcut cut = new Shortcut {
                    Name = shortcut.Name,
                    Description = shortcut.Description,
                    ActionID = shortcut.ActionID
                };

                cut.Strokes = new List<object>();
                this.SerialiseInputStrokeToShortcut(cut, shortcut.Shortcut.PrimaryStroke);
                foreach (IInputStroke stroke in shortcut.Shortcut.SecondaryStrokes) {
                    this.SerialiseInputStrokeToShortcut(cut, stroke);
                }

                group.Shortcuts.Add(cut);
            }

            foreach (ShortcutGroup innerGroup in focusGroup.Groups) {
                Group inner = new Group();
                group.InnerGroups.Add(inner);
                this.SerialiseGroup(inner, innerGroup);
            }
        }

        private static string SerialiseObject(bool value) {
            return value ? "true" : "false";
        }
    }
}