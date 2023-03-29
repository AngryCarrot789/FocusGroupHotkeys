using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using FocusGroupHotkeys.Bindings;
using FocusGroupHotkeys.Core.Shortcuts.Managing;
using FocusGroupHotkeys.MainView;

namespace FocusGroupHotkeys {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private void App_OnStartup(object sender, StartupEventArgs e) {
            string path = @"F:\VSProjsV2\FocusGroupHotkeys\FocusGroupHotkeys\SomeXML.xml";

            try {
                AppShortcutManager.Instance.RootGroup = null;
                using (Stream stream = File.OpenRead(path)) {
                    ShortcutGroup result = WPFKeyMapDeserialiser.Instance.Deserialise(stream);
                    AppShortcutManager.Instance.RootGroup = result;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "Failed to read config or no such file");
            }

            CommandManager.RegisterClassCommandBinding(typeof(ShortcutBinding), new CommandBinding());

            this.MainWindow = new MainWindow();
            this.MainWindow.Show();
        }
    }
}
