using FocusGroupHotkeys.Core.AdvancedContextService.Base;

namespace FocusGroupHotkeys.Core.AdvancedContextService {
    /// <summary>
    /// A separator element between menu items
    /// </summary>
    public class ContextEntrySeparator : IContextEntry {
        public static ContextEntrySeparator Instance { get; } = new ContextEntrySeparator();
    }
}