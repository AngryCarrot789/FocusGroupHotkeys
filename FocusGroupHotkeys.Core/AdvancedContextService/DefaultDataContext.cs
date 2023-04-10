using System.Collections.Generic;
using System.Linq;

namespace FocusGroupHotkeys.Core.AdvancedContextService {
    public class DefaultDataContext : IHaveDataContext {
        private Dictionary<string, object> map;
        private readonly List<object> context;

        public IEnumerable<object> Context => this.context;

        public IEnumerable<(string, object)> CustomData => this.map.Select(x => (x.Key, x.Value));

        public DefaultDataContext() {
            this.context = new List<object>();
        }

        public void AddContext(object context) {
            this.context.Add(context);
        }

        public T GetContext<T>() {
            this.TryGetContext<T>(out T value);
            return value;
        }

        public bool TryGetContext<T>(out T value) {
            foreach (object obj in this.context) {
                if (obj is T t) {
                    value = t;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public bool TryGetCustomData<T>(string key, out T value) {
            if (this.map != null && this.map.TryGetValue(key, out object data) && data is T t) {
                value = t;
                return true;
            }

            value = default;
            return false;
        }

        public T GetCustomData<T>(string key) {
            this.TryGetCustomData(key, out T value);
            return value; // ValueType will be default, object will be null
        }

        public void SetCustomData(string key, object value) {
            Dictionary<string, object> dataMap = this.map ?? (this.map = new Dictionary<string, object>());
            if (value == null) {
                dataMap.Remove(key);
            }
            else {
                dataMap[key] = value;
            }
        }

        public void Merge(IHaveDataContext ctx) {
            foreach (object o in ctx.Context) {
                this.context.Add(o);
            }

            foreach ((string a, object b) in ctx.CustomData) {
                this.map[a] = b;
            }
        }
    }
}