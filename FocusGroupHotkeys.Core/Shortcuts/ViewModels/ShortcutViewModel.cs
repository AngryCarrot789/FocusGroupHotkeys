using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FocusGroupHotkeys.Core.AdvancedContextService;
using FocusGroupHotkeys.Core.Inputs;
using FocusGroupHotkeys.Core.Shortcuts.Managing;

namespace FocusGroupHotkeys.Core.Shortcuts.ViewModels {
    public class ShortcutViewModel : BaseViewModel, IContextProvider {
        public ManagedShortcut ShortcutRefernce { get; set; }

        public ShortcutManagerViewModel Manager { get; set; }

        private ShortcutGroupViewModel parent;
        public ShortcutGroupViewModel Parent {
            get => this.parent;
            set => this.RaisePropertyChanged(ref this.parent, value);
        }

        public ObservableCollection<InputStrokeViewModel> InputStrokes { get; }

        public string Name { get; }

        public string Description { get; }

        public ICommand AddKeyStrokeCommand { get; set; }

        public ICommand AddMouseStrokeCommand { get; set; }

        public RelayCommandParam<InputStrokeViewModel> RemoveStrokeCommand { get; }

        public IEnumerable<IBaseContextEntry> RootContextEntries {
            get {
                yield return new ContextEntry("Add key stroke...", this.AddKeyStrokeCommand);
                yield return new ContextEntry("Add mouse stroke...", this.AddMouseStrokeCommand);
                if (this.InputStrokes.Count > 0) {
                    yield return ContextEntrySeparator.Instance;
                    foreach (InputStrokeViewModel stroke in this.InputStrokes) {
                        yield return new ContextEntry("Remove " + stroke.ToReadableString(), this.RemoveStrokeCommand, stroke);
                    }
                }
            }
        }

        public ShortcutViewModel(string name, string description) {
            this.Name = name;
            this.Description = description;
            this.InputStrokes = new ObservableCollection<InputStrokeViewModel>();
            this.AddKeyStrokeCommand = new RelayCommand(this.AddKeyStrokeAction);
            this.AddMouseStrokeCommand = new RelayCommand(this.AddMouseStrokeAction);
            this.RemoveStrokeCommand = new RelayCommandParam<InputStrokeViewModel>(this.RemoveStrokeAction);
        }

        public void AddKeyStrokeAction() {
            KeyStroke? result = IoC.KeyboardDialogs.ShowGetKeyStrokeDialog();
            if (result.HasValue) {
                this.InputStrokes.Add(new KeyStrokeViewModel(result.Value));
                this.UpdateShortcutReference();
            }
        }

        public void AddMouseStrokeAction() {
            MouseStroke? result = IoC.MouseDialogs.ShowGetMouseStrokeDialog();
            if (result.HasValue) {
                this.InputStrokes.Add(new MouseStrokeViewModel(result.Value));
                this.UpdateShortcutReference();
            }
        }

        public void UpdateShortcutReference() {
            if (this.Manager != null) {
                IoC.ShortcutManager.RootGroup = this.Manager.SaveToRoot();
                IoC.BroadcastShortcutUpdate();
            }
        }

        public void RemoveStrokeAction(InputStrokeViewModel stroke) {
            this.InputStrokes.Remove(stroke);
            this.UpdateShortcutReference();
        }

        public ShortcutViewModel(string name, string description, IShortcut shortcut) : this(name, description) {
            foreach (IInputStroke stroke in shortcut.InputStrokes) {
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
                return null;
            }
        }
    }
}