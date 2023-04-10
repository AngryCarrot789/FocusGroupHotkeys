using System.Collections.Generic;
using System.Net.Mail;

namespace FocusGroupHotkeys.Core.AdvancedContextService {
    public interface IHaveDataContext {
        IEnumerable<object> Context { get; }

        IEnumerable<(string, object)> CustomData { get; }

        void AddContext(object context);

        T GetContext<T>();

        bool TryGetContext<T>(out T value);

        bool TryGetCustomData<T>(string key, out T value);

        T GetCustomData<T>(string key);

        void SetCustomData(string key, object value);

        void Merge(IHaveDataContext context);
    }
}