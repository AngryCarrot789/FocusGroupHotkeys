using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FocusGroupHotkeys.Core.AdvancedContextService;

namespace FocusGroupHotkeys.Core.Actions {
    public class ActionManager {
        public static ActionManager Instance { get; set; }

        private readonly Dictionary<string, AnAction> actions;

        public ActionManager() {
            this.actions = new Dictionary<string, AnAction>();
        }

        static ActionManager() {
            Instance = new ActionManager();
        }

        public void Register(string id, AnAction action) {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Action cannot be null or empty", nameof(id));
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            if (this.actions.TryGetValue(id, out AnAction existing))
                throw new Exception($"Action already registered with type '{id}': {existing.GetType()}");

            this.actions[id] = action;
        }

        public virtual AnAction GetAction(string id) {
            return id != null && this.actions.TryGetValue(id, out AnAction action) ? action : null;
        }

        public virtual async Task<bool> Execute(string id, IHaveDataContext context) {
            if (context == null) {
                throw new Exception("Context cannot be null");
            }

            if (this.actions.TryGetValue(id, out AnAction action)) {
                context.SetCustomData("ActionId", id);
                AnActionEventArgs args = new AnActionEventArgs(context);

                #if DEBUG
                return await action.Execute(args);
                #else
                try {
                    return await action.Execute(args);
                }
                catch (Exception e) {
                    return await this.OnActionException(id, action, args, e);
                }
                #endif
            }

            return false;
        }

        protected virtual async Task<bool> OnActionException(string actionId, AnAction action, AnActionEventArgs args, Exception e) {
            #if DEBUG
            string msg = $"An exception occurred while executing {actionId}:\n{e}";
            #else
            string msg = $"An exception occurred while executing {actionId}";
            #endif

            await IoC.MessageDialogs.ShowMessageAsync("Error performing action", msg);
            return true;
        }
    }
}