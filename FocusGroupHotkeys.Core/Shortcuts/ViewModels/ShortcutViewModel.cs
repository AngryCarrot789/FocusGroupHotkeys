using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FocusGroupHotkeys.Core.AdvancedContextService;
using FocusGroupHotkeys.Core.Shortcuts.Inputs;
using FocusGroupHotkeys.Core.Shortcuts.Managing;

namespace FocusGroupHotkeys.Core.Shortcuts.ViewModels {
    public class ShortcutViewModel : BaseShortcutItemViewModel, IContextProvider {
        public GroupedShortcut TheShortcut { get; }

        public ObservableCollection<InputStrokeViewModel> InputStrokes { get; }

        public string Name { get; }

        public string DisplayName { get; }

        public string Path { get; }

        public string Description { get; }

        private bool isGlobal;
        public bool IsGlobal {
            get => this.isGlobal;
            set => this.RaisePropertyChanged(ref this.isGlobal, value);
        }

        private bool inherit;
        public bool Inherit {
            get => this.inherit;
            set => this.RaisePropertyChanged(ref this.inherit, value);
        }

        private RepeatMode repeatMode;
        public RepeatMode RepeatMode {
            get => this.repeatMode;
            set => this.RaisePropertyChanged(ref this.repeatMode, value);
        }

        public ICommand AddKeyStrokeCommand { get; set; }

        public ICommand AddMouseStrokeCommand { get; set; }

        public RelayCommand<InputStrokeViewModel> RemoveStrokeCommand { get; }

        public ShortcutViewModel(ShortcutManagerViewModel manager, ShortcutGroupViewModel parent, GroupedShortcut reference) : base(manager, parent) {
            this.TheShortcut = reference;
            this.Name = reference.Name;
            this.DisplayName = reference.DisplayName ?? reference.Name;
            this.Path = reference.FullPath;
            this.Description = reference.Description;
            this.isGlobal = reference.IsGlobal;
            this.inherit = reference.IsInherited;
            this.repeatMode = reference.RepeatMode;
            this.InputStrokes = new ObservableCollection<InputStrokeViewModel>();
            this.AddKeyStrokeCommand = new RelayCommand(this.AddKeyStrokeAction);
            this.AddMouseStrokeCommand = new RelayCommand(this.AddMouseStrokeAction);
            this.RemoveStrokeCommand = new RelayCommand<InputStrokeViewModel>(this.RemoveStrokeAction, (x) => x != null);
            foreach (IInputStroke stroke in reference.Shortcut.InputStrokes) {
                this.InputStrokes.Add(InputStrokeViewModel.CreateFrom(stroke));
            }
        }

        public void GetContext(List<IContextEntry> list) {
            list.Add(new CommandContextEntry("Add key stroke...", this.AddKeyStrokeCommand));
            list.Add(new CommandContextEntry("Add mouse stroke...", this.AddMouseStrokeCommand));
            if (this.InputStrokes.Count > 0) {
                list.Add(SeparatorEntry.Instance);
                foreach (InputStrokeViewModel stroke in this.InputStrokes) {
                    list.Add(new CommandContextEntry("Remove " + stroke.ToReadableString(), this.RemoveStrokeCommand, stroke));
                }
            }
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
            IShortcut shortcut = this.TheShortcut.Shortcut;
            this.TheShortcut.Shortcut = this.SaveToRealShortcut() ?? KeyboardShortcut.EmptyKeyboardShortcut;
            this.Manager.OnShortcutModified(this, shortcut);
        }

        public void RemoveStrokeAction(InputStrokeViewModel stroke) {
            if (this.InputStrokes.Remove(stroke)) {
                this.UpdateShortcutReference();
            }
        }

        public IShortcut SaveToRealShortcut() {
            bool hasKey = this.InputStrokes.Any(x => x is KeyStrokeViewModel);
            bool hasMouse = this.InputStrokes.Any(x => x is MouseStrokeViewModel);
            // These 3 different shortcut types only really exist for a performance reason. You can
            // always fall back to MouseKeyboardShortcut, and just ignore the other types
            if (hasKey && hasMouse) {
                return new MouseKeyboardShortcut(this.InputStrokes.Select(a => a.ToInputStroke()));
            }
            else if (hasKey) {
                return new KeyboardShortcut(this.InputStrokes.Select(a => ((KeyStrokeViewModel) a).ToKeyStroke()));
            }
            else if (hasMouse) {
                return new MouseShortcut(this.InputStrokes.Select(a => ((MouseStrokeViewModel) a).ToMouseStroke()));
            }
            else {
                return null;
            }
        }
    }
}