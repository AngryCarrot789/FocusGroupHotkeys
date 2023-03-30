using System.Windows.Input;
using FocusGroupHotkeys.Core;

namespace FocusGroupHotkeys.AdvancedContextService {
    public class ContextEntry : BaseViewModel {
        /// <summary>
        /// The command that the context menu item will invoke
        /// </summary>
        public ICommand Command { get; }

        /// <summary>
        /// A parameter to pass to the command
        /// </summary>
        public object CommandParameter { get; }
    }
}