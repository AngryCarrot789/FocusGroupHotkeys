using System;
using System.Collections.ObjectModel;
using System.Linq;
using FocusGroupHotkeys.Core.Inputs;
using FocusGroupHotkeys.Core.Shortcuts.Managing;

namespace FocusGroupHotkeys.Core.Shortcuts.ViewModels {
    public class ShortcutViewModel : BaseViewModel {
        public ManagedShortcut ShortcutRefernce { get; set; }

        private ShortcutGroupViewModel parent;
        public ShortcutGroupViewModel Parent {
            get => this.parent;
            set => this.RaisePropertyChanged(ref this.parent, value);
        }

        public ObservableCollection<InputStrokeViewModel> InputStrokes { get; }

        public string Name { get; }

        public string Description { get; }

        public ShortcutViewModel(string name, string description) {
            this.Name = name;
            this.Description = description;
            this.InputStrokes = new ObservableCollection<InputStrokeViewModel>();
        }

        public ShortcutViewModel(string name, string description, IShortcut shortcut) : this(name, description) {
            this.InputStrokes.Add(InputStrokeViewModel.CreateFrom(shortcut.PrimaryStroke));
            foreach (IInputStroke stroke in shortcut.SecondaryStrokes) {
                this.InputStrokes.Add(InputStrokeViewModel.CreateFrom(stroke));
            }
        }

        public ShortcutViewModel(ManagedShortcut managedShortcut) : this(managedShortcut.Name, managedShortcut.Description, managedShortcut.Shortcut) {
            this.ShortcutRefernce = managedShortcut;
        }

        public IShortcut SaveToRealShortcut() {
            bool hasKey = false;
            bool hasMouse = false;
            if (this.InputStrokes.Count(x => x is KeyStrokeViewModel) > 0) {
                hasKey = true;
            }

            if (this.InputStrokes.Count(x => x is MouseStrokeViewModel) > 0) {
                hasMouse = true;
            }

            // These 3 different shortcut types only really exist for a performance reason. You can
            // always fall back to MouseKeyboardShortcut, and just ignore the other types
            if (hasKey && hasMouse) {
                return new MouseKeyboardShortcut(this.InputStrokes.Select(a => a.ToInputStroke()));
            }
            else if (hasKey) {
                return new KeyboardShortcut(this.InputStrokes.OfType<KeyStrokeViewModel>().Select(a => a.ToKeyStroke()));
            }
            else if (hasMouse) {
                return new MouseShortcut(this.InputStrokes.OfType<MouseStrokeViewModel>().Select(a => a.ToMouseStroke()));
            }
            else {
                throw new Exception("Missing at least 1 input stroke");
            }
        }
    }
}