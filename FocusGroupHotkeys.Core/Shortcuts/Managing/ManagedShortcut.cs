using System;
using System.Text;

namespace FocusGroupHotkeys.Core.Shortcuts.Managing {
    /// <summary>
    /// A class used to store a reference to a <see cref="Shortcut"/> and its
    /// owning <see cref="ShortcutGroup"/>, and also other shortcut data
    /// </summary>
    public class ManagedShortcut {
        /// <summary>
        /// The collection that owns this managed shortcut
        /// </summary>
        public ShortcutGroup Group { get; }

        /// <summary>
        /// The shortcut itself
        /// </summary>
        public IShortcut Shortcut { get; }

        /// <summary>
        /// The name of the shortcut
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A description of what the shortcut is used for
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The ID for an optional action that this shortcut will trigger when activated
        /// </summary>
        public string ActionID { get; set; }

        public string Path => this.Group.GetPathForName(this.Name);

        public ManagedShortcut(ShortcutGroup collection, string name, IShortcut shortcut) {
            this.Group = collection ?? throw new ArgumentNullException(nameof(collection), "Collection cannot be null");
            this.Shortcut = shortcut ?? throw new ArgumentNullException(nameof(shortcut), "Shortcut cannot be null");
            this.Name = name;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Shortcut).Append(" -> ").Append(this.Path);
            if (!string.IsNullOrWhiteSpace(this.Description)) {
                sb.Append(" (").Append(this.Description).Append(")");
            }

            return sb.ToString();
        }
    }
}