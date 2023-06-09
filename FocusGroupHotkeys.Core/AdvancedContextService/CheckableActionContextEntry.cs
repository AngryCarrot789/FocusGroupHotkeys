using System.Collections.Generic;
using FocusGroupHotkeys.Core.AdvancedContextService.Base;

namespace FocusGroupHotkeys.Core.AdvancedContextService {
    public class CheckableActionContextEntry : ActionContextEntry {
        private bool isChecked;
        public bool IsChecked {
            get => this.isChecked;
            set {
                this.RaisePropertyChanged(ref this.isChecked, value);
                this.SetCustomData(nameof(this.IsChecked), value);
            }
        }

        public CheckableActionContextEntry(IEnumerable<IContextEntry> children = null) : base(children) {

        }

        public CheckableActionContextEntry(string actionId, IEnumerable<IContextEntry> children = null) : base(actionId, children) {

        }

        public CheckableActionContextEntry(object dataContext, string actionId, IEnumerable<IContextEntry> children = null) : base(dataContext, actionId, children) {

        }

        public CheckableActionContextEntry(object dataContext, string header, string actionId, IEnumerable<IContextEntry> children = null) : base(dataContext, header, actionId, children) {

        }

        public CheckableActionContextEntry(object dataContext, string header, string actionId, string toolTip, IEnumerable<IContextEntry> children = null) : base(dataContext, header, actionId, toolTip, children) {

        }
    }
}