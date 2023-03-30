using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using FocusGroupHotkeys.Converters;
using FocusGroupHotkeys.Core.Inputs;
using FocusGroupHotkeys.Core.Views.Dialogs;
using FocusGroupHotkeys.Views;

namespace FocusGroupHotkeys.Shortcuts.Dialogs {
    /// <summary>
    /// Interaction logic for KeyStrokeInputWindow.xaml
    /// </summary>
    public partial class KeyStrokeInputWindow : BaseDialog {
        public KeyStroke Stroke { get; set; }

        public bool IsKeyUp => this.IsKeyReleaseCheckBox?.IsChecked ?? false;

        public KeyStrokeInputWindow() {
            this.InitializeComponent();
            this.DataContext = new BaseConfirmableDialogViewModel(this);
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e) {
            if (AppShortcutManager.GetKeyStrokeForEvent(e, out KeyStroke stroke, this.IsKeyUp)) {
                this.Stroke = stroke;
                this.UpdateText(stroke);
            }

            e.Handled = true;
        }

        public void UpdateText(KeyStroke stroke) {
            if (stroke.Equals(default)) {
                this.InputBox.Text = "";
            }
            else {
                this.InputBox.Text = KeyStrokeRepresentationConverter.ToStringFunction(stroke.KeyCode, stroke.Modifiers, stroke.IsKeyRelease, false);
            }
        }

        private void OnRadioButtonCheckChanged(object sender, RoutedEventArgs e) {
            this.UpdateText(new KeyStroke(this.Stroke.KeyCode, this.Stroke.Modifiers, this.IsKeyUp));
        }
    }
}
