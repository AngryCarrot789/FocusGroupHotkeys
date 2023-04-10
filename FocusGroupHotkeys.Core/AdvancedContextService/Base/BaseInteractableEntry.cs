using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FocusGroupHotkeys.Core.AdvancedContextService.Base {
    public class BaseInteractableEntry : BaseViewModel, IContextEntry, IHaveDataContext {
        private Dictionary<string, object> additionalData;
        private readonly ObservableCollection<object> context;

        public IEnumerable<object> Context => this.context;

        public IEnumerable<(string, object)> CustomData => this.additionalData.Select(x => (x.Key, x.Value));

        protected BaseInteractableEntry(object dataContext) {
            this.context = new ObservableCollection<object> {
                dataContext
            };
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
            if (this.additionalData != null && this.additionalData.TryGetValue(key, out object data) && data is T t) {
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
            Dictionary<string, object> map = this.additionalData ?? (this.additionalData = new Dictionary<string, object>());
            if (value == null) {
                map.Remove(key);
            }
            else {
                map[key] = value;
            }
        }

        public void Merge(IHaveDataContext ctx) {
            foreach (object o in ctx.Context) {
                this.context.Add(o);
            }

            foreach ((string a, object b) in ctx.CustomData) {
                this.additionalData[a] = b;
            }
        }
    }
}