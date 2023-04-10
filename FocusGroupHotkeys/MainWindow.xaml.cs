using FocusGroupHotkeys.Core;
using FocusGroupHotkeys.Views;

namespace FocusGroupHotkeys {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowEx {
        public static MainWindow INSTANCE;

        public MainWindow() {
            this.DataContext = new MainViewModel();
            this.InitializeComponent();

            IoC.BroadcastShortcutActivity = (x) => {
                this.ACTIVITY_BAR.Text = x ?? "";
            };

            INSTANCE = this;
        }
    }
}
