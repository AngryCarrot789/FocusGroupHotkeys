using System.Threading.Tasks;
using FocusGroupHotkeys.Core.Views.Windows;

namespace FocusGroupHotkeys.Views {
    public class BaseWindow : BaseWindowCore, IWindow {
        public void CloseWindow() {
            this.Close();
        }

        public async Task CloseWindowAsync() {
            await this.Dispatcher.InvokeAsync(this.CloseWindow);
        }
    }
}