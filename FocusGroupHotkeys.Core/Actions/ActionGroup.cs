using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FocusGroupHotkeys.Core.Actions {
    public abstract class ActionGroup : AnAction {
        protected ActionGroup(Func<string> header, Func<string> description) : base(header, description) {

        }

        public abstract List<AnAction> GetChildren();

        public override Task<bool> Execute(AnActionEventArgs e) {
            return Task.FromResult(false);
        }
    }
}