using System;
using FocusGroupHotkeys.Core.Services;
using FocusGroupHotkeys.Core.Views.Dialogs.FilePicking;
using FocusGroupHotkeys.Core.Views.Dialogs.Message;
using FocusGroupHotkeys.Core.Views.Dialogs.UserInputs;
using FocusGroupHotkeys.Core.Shortcuts.Dialogs;
using FocusGroupHotkeys.Core.Shortcuts.Managing;

namespace FocusGroupHotkeys.Core {
    public static class IoC {
        public static SimpleIoC Instance { get; } = new SimpleIoC();

        public static ShortcutManager ShortcutManager {
            get => Instance.Provide<ShortcutManager>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IKeyboardDialogService KeyboardDialogs {
            get => Instance.Provide<IKeyboardDialogService>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IMouseDialogService MouseDialogs {
            get => Instance.Provide<IMouseDialogService>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IDispatcher Dispatcher {
            get => Instance.Provide<IDispatcher>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IClipboardService Clipboard {
            get => Instance.Provide<IClipboardService>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IMessageDialogService MessageDialogs {
            get => Instance.Provide<IMessageDialogService>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IFilePickDialogService FilePicker {
            get => Instance.Provide<IFilePickDialogService>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static IUserInputDialogService UserInput {
            get => Instance.Provide<IUserInputDialogService>();
            set => Instance.Register(value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null"));
        }

        public static Action<string> OnShortcutManagedChanged { get; set; }
        public static Action<string> BroadcastShortcutActivity { get; set; }
    }
}