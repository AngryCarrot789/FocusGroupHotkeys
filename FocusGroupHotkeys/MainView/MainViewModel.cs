using System.Windows.Input;
using FocusGroupHotkeys.Core;
using FocusGroupHotkeys.Core.Shortcuts.ViewModels;

namespace FocusGroupHotkeys.MainView {
    public class MainViewModel : BaseViewModel {
        public ICommand P1Action1 { get; }
        public ICommand P1Action2 { get; }
        public ICommand P1Action3 { get; }
        public ICommand P1Action4 { get; }
        public ICommand P1Action5 { get; }
        public ICommand P2Action1 { get; }
        public ICommand P3Action1 { get; }
        public ICommand P4Action1 { get; }
        public ICommand P5Action1 { get; }
        public ICommand P6Action1 { get; }

        private bool isP1Action1Checked;
        private bool isP1Action2Checked;
        private bool isP1Action3Checked;
        private bool isP1Action4Checked;
        private bool isP1Action5Checked;
        private bool isP2Action1Checked;
        private bool isP3Action1Checked;
        private bool isP4Action1Checked;
        private bool isP5Action1Checked;
        private bool isP6Action1Checked;

        public bool IsP1Action1Checked { get => this.isP1Action1Checked; set => this.RaisePropertyChanged(ref this.isP1Action1Checked, value); }
        public bool IsP1Action2Checked { get => this.isP1Action2Checked; set => this.RaisePropertyChanged(ref this.isP1Action2Checked, value); }
        public bool IsP1Action3Checked { get => this.isP1Action3Checked; set => this.RaisePropertyChanged(ref this.isP1Action3Checked, value); }
        public bool IsP1Action4Checked { get => this.isP1Action4Checked; set => this.RaisePropertyChanged(ref this.isP1Action4Checked, value); }
        public bool IsP1Action5Checked { get => this.isP1Action5Checked; set => this.RaisePropertyChanged(ref this.isP1Action5Checked, value); }
        public bool IsP2Action1Checked { get => this.isP2Action1Checked; set => this.RaisePropertyChanged(ref this.isP2Action1Checked, value); }
        public bool IsP3Action1Checked { get => this.isP3Action1Checked; set => this.RaisePropertyChanged(ref this.isP3Action1Checked, value); }
        public bool IsP4Action1Checked { get => this.isP4Action1Checked; set => this.RaisePropertyChanged(ref this.isP4Action1Checked, value); }
        public bool IsP5Action1Checked { get => this.isP5Action1Checked; set => this.RaisePropertyChanged(ref this.isP5Action1Checked, value); }
        public bool IsP6Action1Checked { get => this.isP6Action1Checked; set => this.RaisePropertyChanged(ref this.isP6Action1Checked, value); }

        public ShortcutManagerViewModel ShortcutManager { get; }

        public MainViewModel() {
            this.P1Action1 = new RelayCommand(() => { this.IsP1Action1Checked = !this.IsP1Action1Checked; });
            this.P1Action2 = new RelayCommand(() => { this.IsP1Action2Checked = !this.IsP1Action2Checked; });
            this.P1Action3 = new RelayCommand(() => { this.IsP1Action3Checked = !this.IsP1Action3Checked; });
            this.P1Action4 = new RelayCommand(() => { this.IsP1Action4Checked = !this.IsP1Action4Checked; });
            this.P1Action5 = new RelayCommand(() => { this.IsP1Action5Checked = !this.IsP1Action5Checked; });
            this.P2Action1 = new RelayCommand(() => { this.IsP2Action1Checked = !this.IsP2Action1Checked; });
            this.P3Action1 = new RelayCommand(() => { this.IsP3Action1Checked = !this.IsP3Action1Checked; });
            this.P4Action1 = new RelayCommand(() => { this.IsP4Action1Checked = !this.IsP4Action1Checked; });
            this.P5Action1 = new RelayCommand(() => { this.IsP5Action1Checked = !this.IsP5Action1Checked; });
            this.P6Action1 = new RelayCommand(() => { this.IsP6Action1Checked = !this.IsP6Action1Checked; });
            this.ShortcutManager = new ShortcutManagerViewModel();
            this.ShortcutManager.LoadFromRoot(AppShortcutManager.Instance.RootGroup);
        }
    }
}