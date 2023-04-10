using FocusGroupHotkeys.Core.AdvancedContextService;

namespace FocusGroupHotkeys.Core.Actions {
    /// <summary>
    /// Action event arguments for when an action is about to be executed
    /// </summary>
    public class AnActionEventArgs {
        /// <summary>
        /// The data context for this specific action execution. This will not be null, but it may be empty (contain no inner data or data context)
        /// </summary>
        public IHaveDataContext DataContext { get; }

        public AnActionEventArgs(IHaveDataContext dataContext) {
            this.DataContext = dataContext ?? new DefaultDataContext();
        }
    }
}