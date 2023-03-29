using System.Threading.Tasks;

namespace FocusGroupHotkeys.Core.Views.Windows {
    public interface IWindow : IViewBase {
        void CloseWindow();

        Task CloseWindowAsync();
    }
}