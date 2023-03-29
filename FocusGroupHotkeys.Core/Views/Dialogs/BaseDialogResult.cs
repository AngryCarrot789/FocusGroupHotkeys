namespace FocusGroupHotkeys.Core.Views.Dialogs {
    /// <summary>
    /// The base class for a dialog result. You don't have to use this class, but it may make things easier
    /// </summary>
    public abstract class BaseDialogResult {
        /// <summary>
        /// True if the user confirmed the UI action, otherwise false to indicate the user cancelled the action
        /// <para>
        /// By default, this is set to true in the default constructor. It must be explicitly set to false to indicate a failure
        /// </para>
        /// </summary>
        public bool IsSuccess { get; set; }

        protected BaseDialogResult() : this(true) {

        }

        protected BaseDialogResult(bool isSuccess) {
            this.IsSuccess = isSuccess;
        }
    }
}