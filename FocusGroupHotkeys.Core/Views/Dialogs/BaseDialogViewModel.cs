namespace FocusGroupHotkeys.Core.Views.Dialogs {
    public class BaseDialogViewModel : BaseViewModel {
        public IDialog Dialog { get; }

        public BaseDialogViewModel(IDialog dialog) {
            this.Dialog = dialog;
        }
    }
}