using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FocusGroupHotkeys.Core.Shortcuts.Managing {
    /// <summary>
    /// A class for storing and managing shortcuts
    /// </summary>
    public abstract class ShortcutManager {
        private List<GroupedShortcut> allShortcuts;
        private Dictionary<string, List<GroupedShortcut>> actionToShortcut;

        private ShortcutGroup root;
        public ShortcutGroup Root {
            get => this.root;
            protected set {
                ShortcutGroup old = this.root;
                this.root = value;
                this.OnRootChanged(old, value);
            }
        }

        public ShortcutManager() {
            this.Root = ShortcutGroup.CreateRoot();
        }

        public ShortcutGroup FindGroupByPath(string path) {
            return this.Root.GetGroupByPath(path);
        }

        public GroupedShortcut FindShortcutByPath(string path) {
            return this.Root.GetShortcutByPath(path);
        }

        public GroupedShortcut FindFirstShortcutByAction(string actionId) {
            return this.Root.FindFirstShortcutByAction(actionId);
        }

        protected virtual void OnRootChanged(ShortcutGroup oldRoot, ShortcutGroup root) {
            this.InvalidateShortcutCache();
        }

        /// <summary>
        /// This will invalidate the cached shortcuts, meaning they will be regenerated when needed
        /// <para>
        /// This should be called if a shortcut or shortcut group was modified (e.g. a new shortcut group and added or a shortcut was removed)
        /// </para>
        /// </summary>
        public virtual void InvalidateShortcutCache() {
            this.allShortcuts = null;
            this.actionToShortcut = null;
        }

        /// <summary>
        /// Creates a new shortcut processor for this manager
        /// </summary>
        /// <returns></returns>
        public abstract ShortcutProcessor NewProcessor();

        public IEnumerable<GroupedShortcut> GetAllShortcuts() {
            if (this.allShortcuts != null) {
                return this.allShortcuts;
            }

            this.allShortcuts = new List<GroupedShortcut>();
            if (this.root != null) {
                GetAllShortcuts(this.root, this.allShortcuts);
            }
            return this.allShortcuts;
        }

        public List<GroupedShortcut> GetShortcutsByAction(string actionId) {
            return this.GetCachedShortcutMap().TryGetValue(actionId, out List<GroupedShortcut> value) ? value : null;
        }

        public static void GetAllShortcuts(ShortcutGroup rootGroup, ICollection<GroupedShortcut> accumulator) {
            foreach (GroupedShortcut shortcut in rootGroup.Shortcuts) {
                accumulator.Add(shortcut);
            }

            foreach (ShortcutGroup innerGroup in rootGroup.Groups) {
                GetAllShortcuts(innerGroup, accumulator);
            }
        }

        private Dictionary<string, List<GroupedShortcut>> GetCachedShortcutMap() {
            if (this.actionToShortcut != null) {
                return this.actionToShortcut;
            }

            this.actionToShortcut = new Dictionary<string, List<GroupedShortcut>>();
            foreach (GroupedShortcut shortcut in this.GetAllShortcuts()) {
                string id = shortcut.ActionId;
                if (string.IsNullOrWhiteSpace(id)) {
                    continue;
                }

                if (!this.actionToShortcut.TryGetValue(id, out List<GroupedShortcut> list)) {
                    this.actionToShortcut[id] = list = new List<GroupedShortcut>();
                }

                list.Add(shortcut);
            }

            return this.actionToShortcut;
        }
    }
}