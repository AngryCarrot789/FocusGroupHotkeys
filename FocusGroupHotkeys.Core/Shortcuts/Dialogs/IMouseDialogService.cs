using FocusGroupHotkeys.Core.Shortcuts.Inputs;

namespace FocusGroupHotkeys.Core.Shortcuts.Dialogs {
    public interface IMouseDialogService {
        MouseStroke? ShowGetMouseStrokeDialog();
    }
}