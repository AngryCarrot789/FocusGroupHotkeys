using System;
using System.Collections.Generic;
using System.Linq;
using FocusGroupHotkeys.Core.Inputs;

namespace FocusGroupHotkeys.Core.Shortcuts.Managing {
    /// <summary>
    /// A collection of shortcuts
    /// </summary>
    public class ShortcutGroup {
        private readonly List<ShortcutGroup> groups;
        private readonly List<ManagedShortcut> shortcuts;

        public ShortcutGroup Parent { get; set; }

        public string FocusGroupPath { get; }

        public string FocusGroupName { get; }

        /// <summary>
        /// Whether this focus group runs globally (across the entire application). Global focus groups contain shortcuts that can be run, irregardless of what group is focused
        /// </summary>
        public bool IsGlobal { get; }

        /// <summary>
        /// Inherits shortcuts from the parent group
        /// </summary>
        public bool InheritFromParent { get; }

        /// <summary>
        /// All shortcuts in this focus group
        /// </summary>
        public IEnumerable<ManagedShortcut> Shortcuts => this.shortcuts;

        /// <summary>
        /// All child-groups in this focus group
        /// </summary>
        public IEnumerable<ShortcutGroup> Groups => this.groups;

        public ShortcutGroup(string focusGroupPath, bool isGlobal, bool inherit = false) {
            this.InheritFromParent = inherit;
            this.FocusGroupPath = focusGroupPath;
            if (string.IsNullOrWhiteSpace(focusGroupPath)) {
                this.FocusGroupName = null;
            }
            else {
                int split = focusGroupPath.LastIndexOf('/');
                this.FocusGroupName = split == -1 ? focusGroupPath : focusGroupPath.Substring(split + 1);
            }

            this.IsGlobal = isGlobal;
            this.groups = new List<ShortcutGroup>();
            this.shortcuts = new List<ManagedShortcut>();
        }

        public static ShortcutGroup CreateRoot(bool isGlobal = true, bool inherit = false) {
            return new ShortcutGroup(null, isGlobal, inherit);
        }

        private string GetNewItemName(string name) {
            return this.FocusGroupPath != null ? (this.FocusGroupPath + '/' + name) : name;
        }

        public ShortcutGroup CreateGroupByName(string name, bool isGlobal, bool inherit = false) {
            ShortcutGroup group = new ShortcutGroup(this.GetNewItemName(name), isGlobal, inherit);
            this.AddGroup(group);
            return group;
        }

        public ManagedShortcut AddShortcut(string name, IShortcut shortcut) {
            ManagedShortcut managed = new ManagedShortcut(this, name, shortcut);
            this.shortcuts.Add(managed);
            return managed;
        }

        public void AddGroup(ShortcutGroup group) {
            group.Parent = this;
            this.groups.Add(group);
        }

        public bool ContainsShortcutByName(string name) {
            return this.shortcuts.Any(x => x.Name == name);
        }

        public bool ContainsGroupByName(string name) {
            return this.groups.Any(x => x.FocusGroupName == name);
        }

        public List<ManagedShortcut> GetShortcutsWithPrimaryStroke(IInputStroke stroke, string focus) {
            List<ManagedShortcut> list = new List<ManagedShortcut>();
            this.CollectShortcutsWithPrimaryStroke(stroke, focus, list);
            return list;
        }

        public void CollectShortcutsWithPrimaryStroke(IInputStroke stroke, string focus, List<ManagedShortcut> list) {
            foreach (ShortcutGroup focusGroup in this.Groups) {
                focusGroup.CollectShortcutsWithPrimaryStroke(stroke, focus, list);
            }

            if (this.IsGlobal || this.IsValidSearchForGroup(focus)) {
                foreach (ManagedShortcut shortcut in this.shortcuts) {
                    if (shortcut.Shortcut.PrimaryStroke.Equals(stroke)) {
                        list.Add(shortcut);
                    }
                }
            }
        }

        private bool IsValidSearchForGroup(string focusedGroup) {
            return this.FocusGroupPath != null && focusedGroup != null && (this.InheritFromParent ? focusedGroup.StartsWith(this.FocusGroupPath) : focusedGroup.Equals(this.FocusGroupPath));
        }

        // public Shortcut RecursiveFindShortcutByPrimaryStroke<T>(in T stroke, string focusedGroup, out ShortcutFocusGroup group, bool inherit) where T : IInputStroke {
        //     group = null;
        //     if (!this.IsGlobal) {
        //         if (focusedGroup == null || this.FocusGroupPath == null || (inherit ? !focusedGroup.StartsWith(this.FocusGroupPath) : !focusedGroup.Equals(this.FocusGroupPath))) {
        //             return null;
        //         }
        //     }
        //     foreach (ShortcutFocusGroup focusGroup in this.Groups) {
        //         Shortcut found = focusGroup.RecursiveFindShortcutByPrimaryStroke(stroke, focusedGroup, out @group, focusGroup.InheritFromParent);
        //         if (found != null) {
        //             return found;
        //         }
        //     }
        //     if (this.primaryStrokeToShortcut.TryGetValue(stroke, out Shortcut sc) && sc is KeyboardShortcut shortcut) {
        //         @group = this;
        //         return shortcut;
        //     }
        //     return null;
        // }

        public ShortcutGroup GetGroupByName(string name) {
            return this.groups.FirstOrDefault(x => name.Equals(x.FocusGroupName));
        }

        /// <summary>
        /// Gets a group in the given path, and optionally creates groups if they are not present
        /// </summary>
        /// <param name="path"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public ShortcutGroup GetGroupByPath(string path, bool create, bool subGroupsRoot = false) {
            if (string.IsNullOrWhiteSpace(path)) {
                return this;
            }

            string[] split = path.Split('/');
            return this.GetGroupByPath(split, create, subGroupsRoot);
        }

        public ShortcutGroup GetGroupByPath(string[] path, bool create, bool subGroupsRoot = false) {
            return this.GetGroupByPath(path, 0, path.Length, create, subGroupsRoot);
        }

        public ShortcutGroup GetGroupByPath(string[] path, int startIndex, int endIndex, bool create, bool subGroupsRoot = false) {
            if (path == null || path.Length <= 0) {
                return this;
            }
            else if (startIndex >= path.Length) {
                throw new IndexOutOfRangeException($"startIndex cannot be bigger than or equal to the path length ({startIndex} >= {path.Length})");
            }
            else if (startIndex > endIndex) {
                throw new IndexOutOfRangeException($"startIndex cannot be bigger than endIndex ({startIndex} > {endIndex})");
            }
            else {
                ShortcutGroup root = this;
                for (int i = startIndex; i < endIndex; i++) {
                    ShortcutGroup next = root.GetGroupByName(path[i]);
                    if (next == null) {
                        if (!create) {
                            return null;
                        }

                        root = root.CreateGroupByName(path[i], subGroupsRoot);
                    }
                    else {
                        root = next;
                    }
                }

                return root;
            }
        }

        public string GetPathForName(string name) {
            return string.IsNullOrWhiteSpace(this.FocusGroupPath) ? name : (this.FocusGroupPath + '/' + name);
        }
    }
}