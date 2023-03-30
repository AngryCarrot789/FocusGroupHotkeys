using System.Collections.Generic;
using System.Windows.Input;

namespace FocusGroupHotkeys.Core.AdvancedContextService {
    /// <summary>
    /// The default implementation for a context entry (aka menu item), which also supports modifying the header,
    /// input gesture text, command and command parameter to reflect the UI menu item
    /// </summary>
    public class ContextEntry : BaseViewModel, IBaseContextEntry {
        private string header;
        private string inputGestureText;
        private ICommand command;
        private object commandParameter;

        /// <summary>
        /// The menu item's header, aka text
        /// </summary>
        public string Header {
            get => this.header;
            set => this.RaisePropertyChanged(ref this.header, value);
        }

        /// <summary>
        /// The preview input gesture text, which is typically on the right side of a menu item (used for shortcuts)
        /// </summary>
        public string InputGestureText {
            get => this.inputGestureText;
            set => this.RaisePropertyChanged(ref this.inputGestureText, value);
        }

        /// <summary>
        /// The command that the context menu item will invoke
        /// </summary>
        public ICommand Command {
            get => this.command;
            set => this.RaisePropertyChanged(ref this.command, value);
        }

        /// <summary>
        /// A parameter to pass to the command
        /// </summary>
        public object CommandParameter {
            get => this.commandParameter;
            set => this.RaisePropertyChanged(ref this.commandParameter, value);
        }

        public IEnumerable<IBaseContextEntry> Children { get; }

        public ContextEntry() {

        }

        public ContextEntry(IEnumerable<IBaseContextEntry> children = null) : this() {
            this.Children = children;
        }

        public ContextEntry(string header, ICommand command, IEnumerable<IBaseContextEntry> children = null) : this(children) {
            this.header = header;
            this.command = command;
        }

        public ContextEntry(string header, ICommand command, object commandParameter, IEnumerable<IBaseContextEntry> children = null) : this(children) {
            this.header = header;
            this.command = command;
            this.commandParameter = commandParameter;
        }

        public ContextEntry(string header, string inputGestureText, ICommand command, IEnumerable<IBaseContextEntry> children = null) : this(children) {
            this.header = header;
            this.inputGestureText = inputGestureText;
            this.command = command;
        }

        public ContextEntry(string header, string inputGestureText, ICommand command, object commandParameter, IEnumerable<IBaseContextEntry> children = null) : this(children) {
            this.header = header;
            this.inputGestureText = inputGestureText;
            this.command = command;
            this.commandParameter = commandParameter;
        }
    }
}