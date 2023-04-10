using System.Collections.Generic;
using FocusGroupHotkeys.Core.AdvancedContextService.Base;

namespace FocusGroupHotkeys.Core.AdvancedContextService {
    public interface IContextProvider {
        List<IContextEntry> GetContext(List<IContextEntry> list);
    }
}