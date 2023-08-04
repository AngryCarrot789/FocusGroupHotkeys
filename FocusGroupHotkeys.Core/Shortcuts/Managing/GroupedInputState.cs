using System;
using System.Threading.Tasks;
using FocusGroupHotkeys.Core.Shortcuts.Inputs;

namespace FocusGroupHotkeys.Core.Shortcuts.Managing {
    public class GroupedInputState {
        private IInputStroke activationStroke;
        private IInputStroke deactivationStroke;

        /// <summary>
        /// The collection that owns this managed input state
        /// </summary>
        public ShortcutGroup Group { get; }

        /// <summary>
        /// The name of this grouped input state. This will not be null or empty and will not consist of only whitespaces;
        /// this is always some sort of valid string (even if only 1 character)
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// This key state's full path (the parent's path (if available/not root) and this shortcut's name). Will not be null and will always containing valid characters
        /// </summary>
        public string FullPath { get; }

        /// <summary>
        /// The state of this input state
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The input stroke that activates this key state (as in, sets <see cref="IsActive"/> to true)
        /// </summary>
        /// <exception cref="ArgumentNullException">Value cannot be null</exception>
        public IInputStroke ActivationStroke {
            get => this.activationStroke;
            set => this.activationStroke = value ?? throw new ArgumentNullException(nameof(value), "Activation stroke cannot be null");
        }

        /// <summary>
        /// The input stroke that deactivates this key state (as in, sets <see cref="IsActive"/> to false)
        /// </summary>
        /// <exception cref="ArgumentNullException">Value cannot be null</exception>
        public IInputStroke DeactivationStroke {
            get => this.deactivationStroke;
            set => this.deactivationStroke = value ?? throw new ArgumentNullException(nameof(value), "Activation stroke cannot be null");
        }

        public GroupedInputState(ShortcutGroup group, string name, IInputStroke activationStroke, IInputStroke deactivationStroke) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null, empty, or consist of only whitespaces");
            this.Group = group ?? throw new ArgumentNullException(nameof(group), "Collection cannot be null");
            this.Name = name;
            this.FullPath = group.GetPathForName(name);
            this.ActivationStroke = activationStroke;
            this.DeactivationStroke = deactivationStroke;
        }

        public Task OnDeactivate() => Task.CompletedTask;

        public Task OnActivate() => Task.CompletedTask;

        public override string ToString() {
            return $"{nameof(GroupedInputState)} ({this.FullPath}: {(this.IsActive ? "pressed" : "released")} [{this.activationStroke}, {this.deactivationStroke}])";
        }
    }
}