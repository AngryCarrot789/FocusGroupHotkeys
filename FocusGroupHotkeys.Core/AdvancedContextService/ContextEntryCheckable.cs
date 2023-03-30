using System.Collections.Generic;
using System.Windows.Input;

namespace FocusGroupHotkeys.Core.AdvancedContextService {
    public class ContextEntryCheckable : ContextEntry {
        private bool isChecked;
        public bool IsChecked {
            get => this.isChecked;
            set => this.RaisePropertyChanged(ref this.isChecked, value);
        }

        public ContextEntryCheckable() {
        }

        public ContextEntryCheckable(IEnumerable<IBaseContextEntry> children = null) : base(children) {
        }

        public ContextEntryCheckable(string header, ICommand command, IEnumerable<IBaseContextEntry> children = null) : base(header, command, children) {
        }

        public ContextEntryCheckable(string header, ICommand command, object commandParameter, IEnumerable<IBaseContextEntry> children = null) : base(header, command, commandParameter, children) {
        }

        public ContextEntryCheckable(string header, string inputGestureText, ICommand command, IEnumerable<IBaseContextEntry> children = null) : base(header, inputGestureText, command, children) {
        }

        public ContextEntryCheckable(string header, string inputGestureText, ICommand command, object commandParameter, IEnumerable<IBaseContextEntry> children = null) : base(header, inputGestureText, command, commandParameter, children) {
        }
    }
}