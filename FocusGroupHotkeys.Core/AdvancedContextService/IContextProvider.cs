using System.Collections.Generic;

namespace FocusGroupHotkeys.Core.AdvancedContextService {
    public interface IContextProvider {
        IEnumerable<IBaseContextEntry> RootContextEntries { get; }
    }
}