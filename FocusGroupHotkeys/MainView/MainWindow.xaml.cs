using System.Windows;
using FocusGroupHotkeys.Views;

namespace FocusGroupHotkeys.MainView {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowEx {
        public static MainWindow INSTANCE;

        public MainWindow() {
            this.DataContext = new MainViewModel();
            this.InitializeComponent();
            INSTANCE = this;
        }
    }
}
