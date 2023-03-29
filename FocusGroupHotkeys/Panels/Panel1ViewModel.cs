using FocusGroupHotkeys.Core;

namespace FocusGroupHotkeys.Panels {
    public class Panel1ViewModel : BaseViewModel {
        private bool isAChecked;
        public bool IsAChecked {
            get => this.isAChecked;
            set => this.RaisePropertyChanged(ref this.isAChecked, value);
        }

        private bool isBChecked;
        public bool IsBChecked {
            get => this.isBChecked;
            set => this.RaisePropertyChanged(ref this.isBChecked, value);
        }

        private bool isCChecked;
        public bool IsCChecked {
            get => this.isCChecked;
            set => this.RaisePropertyChanged(ref this.isCChecked, value);
        }
    }
}