namespace FocusGroupHotkeys.Core.AdvancedContextService {
    /// <summary>
    /// A separator element between menu items
    /// </summary>
    public class ContextEntrySeparator : IBaseContextEntry {
        public static ContextEntrySeparator Instance { get; } = new ContextEntrySeparator();
    }
}