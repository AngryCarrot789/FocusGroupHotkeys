using System;
using System.Collections.Generic;

namespace FocusGroupHotkeys.Core.Actions {
    public class DefaultActionGroup : ActionGroup {
        private readonly List<AnAction> actions;

        public DefaultActionGroup(Func<string> header, Func<string> description, List<AnAction> actions) : base(header, description) {
            this.actions = actions;
        }

        public override List<AnAction> GetChildren() {
            return this.actions;
        }
    }
}