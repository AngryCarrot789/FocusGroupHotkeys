using System.Threading.Tasks;
using FocusGroupHotkeys.Core.Views.Dialogs;

namespace FocusGroupHotkeys.Views {
    public class BaseDialog : BaseWindowCore, IDialog {
        public void CloseDialog(bool result) {
            this.DialogResult = result;
            this.Close();
        }

        public async Task CloseDialogAsync(bool result) {
            await this.Dispatcher.InvokeAsync(() => this.CloseDialog(result));
        }
    }
}