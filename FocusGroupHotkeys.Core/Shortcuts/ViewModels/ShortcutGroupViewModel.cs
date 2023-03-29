using System.Collections.ObjectModel;
using System.Linq;
using FocusGroupHotkeys.Core.Shortcuts.Managing;

namespace FocusGroupHotkeys.Core.Shortcuts.ViewModels {
    public class ShortcutGroupViewModel : BaseViewModel {
        private readonly ObservableCollection<object> children;

        public ShortcutGroup GroupReference { get; set; }

        private ShortcutGroupViewModel parent;
        public ShortcutGroupViewModel Parent {
            get => this.parent;
            set => this.RaisePropertyChanged(ref this.parent, value);
        }

        public string FocusGroupPath { get; }

        public string FocusGroupName { get; }

        public bool IsGlobal { get; }

        public bool InheritFromParent { get; }

        public ReadOnlyObservableCollection<object> Children { get; }

        public ShortcutGroupViewModel(string focusGroupPath, bool isGlobal, bool inherit = true) {
            this.children = new ObservableCollection<object>();
            this.Children = new ReadOnlyObservableCollection<object>(this.children);
            this.IsGlobal = isGlobal;
            this.InheritFromParent = inherit;
            this.FocusGroupPath = focusGroupPath;
            if (string.IsNullOrWhiteSpace(focusGroupPath)) {
                this.FocusGroupName = null;
            }
            else {
                int split = focusGroupPath.LastIndexOf('/');
                this.FocusGroupName = split == -1 ? focusGroupPath : focusGroupPath.Substring(split + 1);
            }
        }

        public static ShortcutGroupViewModel CreateFrom(ShortcutGroup group) {
            ShortcutGroupViewModel groupViewModel = new ShortcutGroupViewModel(group.FocusGroupPath, group.IsGlobal, group.InheritFromParent) {
                GroupReference = group
            };

            foreach (ShortcutGroup innerGroup in group.Groups) {
                groupViewModel.AddItem(CreateFrom(innerGroup));
            }

            foreach (ManagedShortcut shortcut in group.Shortcuts) {
                groupViewModel.AddItem(new ShortcutViewModel(shortcut));
            }

            return groupViewModel;
        }

        public ShortcutGroup SaveToRealGroup() {
            ShortcutGroup group = new ShortcutGroup(this.FocusGroupPath, this.IsGlobal, this.InheritFromParent);
            foreach (ShortcutGroupViewModel innerGroup in this.children.OfType<ShortcutGroupViewModel>()) {
                group.AddGroup(innerGroup.SaveToRealGroup());
            }

            foreach (ShortcutViewModel shortcut in this.children.OfType<ShortcutViewModel>()) {
                IShortcut realShortcut = shortcut.SaveToRealShortcut();
                ManagedShortcut managed = group.AddShortcut(shortcut.Name, realShortcut);
                managed.Description = shortcut.Description;
            }

            return group;
        }

        public void AddItem(ShortcutViewModel shortcut) {
            shortcut.Parent = this;
            this.children.Add(shortcut);
        }

        public void AddItem(ShortcutGroupViewModel group) {
            group.Parent = this;
            this.children.Add(group);
        }
    }
}