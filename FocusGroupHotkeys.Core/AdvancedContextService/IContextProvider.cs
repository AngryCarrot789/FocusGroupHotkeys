using System.Collections.Generic;

namespace FocusGroupHotkeys.Core.AdvancedContextService {
    public interface IContextProvider {
        void GetContext(List<IContextEntry> list);
    }
}