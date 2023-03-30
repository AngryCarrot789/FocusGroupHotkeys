using FocusGroupHotkeys.Core.Inputs;

namespace FocusGroupHotkeys.Core.Shortcuts.Dialogs {
    public interface IMouseDialogService {
        MouseStroke? ShowGetMouseStrokeDialog();
    }
}