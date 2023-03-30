using System.Collections.Generic;

namespace FocusGroupHotkeys.AdvancedContextService {
    public interface IContextProvider {
        IEnumerable<ContextEntry> GetContextEntries();
    }
}