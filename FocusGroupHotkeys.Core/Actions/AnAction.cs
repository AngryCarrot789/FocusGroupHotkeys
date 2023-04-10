using System;
using System.Threading.Tasks;

namespace FocusGroupHotkeys.Core.Actions {
    public abstract class AnAction {
        private static readonly Func<string> ProvideNullString = () => null;

        public Func<string> Header { get; }

        public Func<string> Description { get; }

        protected AnAction(Func<string> header, Func<string> description) {
            this.Header = header ?? ProvideNullString;
            this.Description = description ?? ProvideNullString;
        }

        public static AnAction Lambda(Func<AnActionEventArgs, Task<bool>> action, string header = null, string description = null) {
            return new LambdaAction(() => header, () => description, action);
        }

        public static AnAction LambdaI18N(Func<AnActionEventArgs, Task<bool>> action, Func<string> header = null, Func<string> description = null) {
            return new LambdaAction(header, description, action);
        }

        /// <summary>
        /// Executes this specific action with the given action event args
        /// </summary>
        /// <param name="e">Event arguments that this action can use</param>
        /// <returns></returns>
        public abstract Task<bool> Execute(AnActionEventArgs e);

        private class LambdaAction : AnAction {
            public Func<AnActionEventArgs, Task<bool>> MyAction { get; }

            public LambdaAction(Func<string> header, Func<string> description, Func<AnActionEventArgs, Task<bool>> action) : base(header, description) {
                this.MyAction = action ?? throw new ArgumentNullException(nameof(action), "Action function cannot be null");
            }

            public override Task<bool> Execute(AnActionEventArgs e) {
                return this.MyAction(e);
            }
        }
    }
}