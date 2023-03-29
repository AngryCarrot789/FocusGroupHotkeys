using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using FocusGroupHotkeys.Core.Inputs;
using FocusGroupHotkeys.Core.Shortcuts.Managing;
using FocusGroupHotkeys.Core.Shortcuts.Usage;
using FocusGroupHotkeys.Core.Utils;
using FocusGroupHotkeys.MainView;

namespace FocusGroupHotkeys {
    public class AppShortcutManager : ShortcutManager {
        public const int BUTTON_WHEEL_UP = 143;    // Away from the user
        public const int BUTTON_WHEEL_DOWN = 142;  // Towards the user
        public const string DEFAULT_USAGE_ID = "DEF";

        public static AppShortcutManager Instance { get; } = new AppShortcutManager();

        public static Action<ManagedShortcut> OnShortcutCompleted { get; set; }

        /// <summary>
        /// Maps an action ID to a dictionary, which maps a custom ID (relative to each usage of the same shortcut) to the callback function
        /// </summary>
        public static Dictionary<string, Dictionary<string, Action<ManagedShortcut>>> InputBindingCallbackMap { get; }

        public static string CurrentInputBindingUsageID { get; set; } = DEFAULT_USAGE_ID;

        static AppShortcutManager() {
            InputBindingCallbackMap = new Dictionary<string, Dictionary<string, Action<ManagedShortcut>>>();
            KeyStroke.KeyCodeToStringProvider = (x) => ((Key) x).ToString();
            KeyStroke.ModifierToStringProvider = (x) => {
                StringJoiner joiner = new StringJoiner(new StringBuilder(), " + ");
                ModifierKeys keys = (ModifierKeys) x;
                if ((keys & ModifierKeys.Control) != 0) joiner.Append("Ctrl");
                if ((keys & ModifierKeys.Alt) != 0)     joiner.Append("Alt");
                if ((keys & ModifierKeys.Shift) != 0)   joiner.Append("Shift");
                if ((keys & ModifierKeys.Windows) != 0) joiner.Append("Win");
                return joiner.ToString();
            };

            MouseStroke.MouseButtonToStringProvider = (x) => {
                switch (x) {
                    case 0: return "LMB";
                    case 1: return "MWB";
                    case 2: return "RMB";
                    case 3: return "X1";
                    case 4: return "X2";
                    case BUTTON_WHEEL_UP:   return "WHEEL_UP";
                    case BUTTON_WHEEL_DOWN: return "WHEEL_DOWN";
                    default: return $"UNKNOWN_MB[{x}]";
                }
            };

            MouseStroke.ModifierToStringProvider = KeyStroke.ModifierToStringProvider;
        }

        // new KeyStroke((int) Key.Escape, (int) ModifierKeys.None, false)
        public AppShortcutManager() {
            // this.CreateHotkey(null, () => MessageBox.Show("Ello! CTRL + SHIFT + A"), NewStroke(false, Key.A, ModifierKeys.Control, ModifierKeys.Shift), NewStroke(false, Key.X, ModifierKeys.Control));
            // this.CreateHotkey("Panel1", () => MessageBox.Show("Panel 1 says ello!"), NewStroke(true, Key.D, ModifierKeys.Control));
            // this.CreateHotkey("Panel1/Inner2", () => MessageBox.Show("Inner 2 says ello!"), NewStroke(true, Key.B, ModifierKeys.Control));
            // this.CreateHotkey("Panel1/Inner3", () => MessageBox.Show("Inner 3 says ello!"), NewStroke(true, Key.C, ModifierKeys.Control));
        }

        private static void EnforceIdFormat(string id, string paramName) {
            if (string.IsNullOrWhiteSpace(id)) {
                throw new Exception($"{paramName} cannot be null or consist of whitespaces only");
            }
        }

        public static void UnregisterHandler(string shortcutId, string usageId) {
            EnforceIdFormat(shortcutId, nameof(shortcutId));
            EnforceIdFormat(usageId, nameof(usageId));
            if (InputBindingCallbackMap.TryGetValue(shortcutId, out Dictionary<string, Action<ManagedShortcut>> usageMap)) {
                usageMap.Remove(usageId);
            }
        }

        public static void RegisterHandler(string shortcutId, string usageId, Action<ManagedShortcut> handler) {
            EnforceIdFormat(shortcutId, nameof(shortcutId));
            EnforceIdFormat(usageId, nameof(usageId));
            if (!InputBindingCallbackMap.TryGetValue(shortcutId, out Dictionary<string, Action<ManagedShortcut>> usageMap)) {
                InputBindingCallbackMap[shortcutId] = usageMap = new Dictionary<string, Action<ManagedShortcut>>();
            }

            usageMap[usageId] = handler;
        }

        // Typically applied only to windows
        public static void OnIsGlobalShortcutFocusTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is UIElement element) {
                element.PreviewMouseDown -= OnWindowPreviewMouseDown;
                element.MouseDown -= OnWindowMouseDown;
                element.MouseUp -= OnWindowMouseUp;
                element.KeyDown -= OnWindowKeyDown;
                element.KeyUp -= OnWindowKeyUp;
                element.MouseWheel -= OnWindowMouseWheel;
                if (e.NewValue != e.OldValue && (bool) e.NewValue) {
                    element.PreviewMouseDown += OnWindowPreviewMouseDown;
                    element.MouseDown += OnWindowMouseDown;
                    element.MouseUp += OnWindowMouseUp;
                    element.KeyDown += OnWindowKeyDown;
                    element.KeyUp += OnWindowKeyUp;
                    element.MouseWheel += OnWindowMouseWheel;
                }
            }
            else {
                throw new Exception("This property must be applied to type UIElement only, not " + (d?.GetType()));
            }
        }

        private static void OnWindowPreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (e.OriginalSource is DependencyObject hit) {
                UIFocusGroup.ProcessFocusGroupChange(hit);
            }
        }

        private static void OnWindowMouseDown(object sender, MouseButtonEventArgs e) {
            if (e.OriginalSource is DependencyObject hit) {
                string focusedPath = UIFocusGroup.ProcessFocusGroupChange(hit);
                MouseStroke stroke = new MouseStroke((int) e.ChangedButton, (int) Keyboard.Modifiers, e.ClickCount);
                if (Instance.OnMouseStroke(focusedPath, stroke)) {
                    e.Handled = true;
                }
            }
        }

        // Handling mouse up makes the shortcuts way harder to manage, because there's so many edge cases to consider
        // e.g how do you handle double/triple/etc click while ignoring mouse down/up if the next usage expects a mouse up/down,
        // while also checking checking all of the active usages. Just too much extra work... might try and re-implement it some day
        // 
        // It works with keys, because they can only be "clicked" once, unlike mouse input, which can have multiple clicks. Also
        // thought about implementing a key stroke click count... but operating systems don't typically do that so i'd have to implement it myself :(
        private static void OnWindowMouseUp(object sender, MouseButtonEventArgs e) {
            // if (e.OriginalSource is DependencyObject hit) {
            //     string focusedPath = UIFocusGroup.ProcessFocusGroupChange(hit);
            //     MouseStroke stroke = new MouseStroke((int) e.ChangedButton, (int) Keyboard.Modifiers, e.ClickCount);
            //     if (Instance.OnMouseStroke(focusedPath, stroke)) {
            //         e.Handled = true;
            //     }
            // }
        }

        private static void OnWindowMouseWheel(object sender, MouseWheelEventArgs e) {
            if (e.OriginalSource is DependencyObject hit) {
                int button;
                if (e.Delta < 0) {
                    button = BUTTON_WHEEL_DOWN;
                }
                else if (e.Delta > 0) {
                    button = BUTTON_WHEEL_UP;
                }
                else {
                    return;
                }

                try {
                    CurrentInputBindingUsageID = UIFocusGroup.GetUsageID(hit) ?? DEFAULT_USAGE_ID;
                    MouseStroke stroke = new MouseStroke(button, (int) Keyboard.Modifiers, 0, e.Delta);
                    if (Instance.OnMouseStroke(UIFocusGroup.FocusedGroupPath, stroke)) {
                        e.Handled = true;
                    }
                }
                finally {
                    CurrentInputBindingUsageID = DEFAULT_USAGE_ID;
                }
            }
        }

        private static void OnWindowKeyUp(object sender, KeyEventArgs e) {
            OnKeyEvent(e.OriginalSource as DependencyObject, e, true);
        }

        private static void OnWindowKeyDown(object sender, KeyEventArgs e) {
            OnKeyEvent(e.OriginalSource as DependencyObject, e, false);
        }

        public static void OnKeyEvent(DependencyObject focused, KeyEventArgs e, bool isRelease) {
            if (e.Handled || e.IsRepeat) {
                return;
            }

            Key key = e.Key == Key.System ? e.SystemKey : e.Key;
            if (IsModifierKey(key) || key == Key.DeadCharProcessed) {
                return;
            }

            try {
                CurrentInputBindingUsageID = UIFocusGroup.GetUsageID(focused) ?? DEFAULT_USAGE_ID;
                KeyStroke stroke = new KeyStroke((int) key, (int) Keyboard.Modifiers, isRelease);
                if (Instance.OnKeyStroke(UIFocusGroup.FocusedGroupPath, stroke)) {
                    e.Handled = true;
                }
            }
            finally {
                CurrentInputBindingUsageID = DEFAULT_USAGE_ID;
            }
        }

        public static bool IsModifierKey(Key key) {
            switch (key) {
                case Key.LeftCtrl:
                case Key.RightCtrl:
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LWin:
                case Key.RWin:
                case Key.Clear:
                case Key.OemClear:
                case Key.Apps:
                    return true;
                default:
                    return false;
            }
        }

        public override bool OnShortcutActivated(ManagedShortcut shortcut) {
            MainWindow.INSTANCE.ACTIVITY_BAR.Text = "Processed shortcut: " + shortcut;
            // ShortcutInputGesture input = ShortcutInputGesture.CurrentInputGesture;
            // if (input?.ShortcutKeyBinding != null && shortcut.Path == input.ShortcutKeyBinding.ShortcutID) {
            //     input.IsCompleted = true;
            // }

            if (InputBindingCallbackMap.TryGetValue(shortcut.Path, out Dictionary<string, Action<ManagedShortcut>> usageMap)) {
                if (usageMap.TryGetValue(CurrentInputBindingUsageID, out Action<ManagedShortcut> callback)) {
                    callback(shortcut);
                }
            }

            return base.OnShortcutActivated(shortcut);
        }

        public override bool OnNoSuchShortcutForKeyStroke(in KeyStroke stroke) {
            MainWindow.INSTANCE.ACTIVITY_BAR.Text = "No such shortcut for key stroke: " + stroke;
            return base.OnNoSuchShortcutForKeyStroke(in stroke);
        }

        public override bool OnNoSuchShortcutForMouseStroke(in MouseStroke stroke) {
            MainWindow.INSTANCE.ACTIVITY_BAR.Text = "No such shortcut for mouse stroke: " + stroke;
            return base.OnNoSuchShortcutForMouseStroke(in stroke);
        }

        public override bool OnCancelUsageForNoSuchNextKeyStroke(IShortcutUsage usage, ManagedShortcut shortcut, in KeyStroke stroke) {
            MainWindow.INSTANCE.ACTIVITY_BAR.Text = "No such shortcut for next key stroke: " + stroke;
            return base.OnCancelUsageForNoSuchNextKeyStroke(usage, shortcut, in stroke);
        }

        public override bool OnCancelUsageForNoSuchNextMouseStroke(IShortcutUsage usage, ManagedShortcut shortcut, in MouseStroke stroke) {
            MainWindow.INSTANCE.ACTIVITY_BAR.Text = "No such shortcut for next mouse stroke: " + stroke;
            return base.OnCancelUsageForNoSuchNextMouseStroke(usage, shortcut, in stroke);
        }

        public override void OnShortcutUsageCreated(IShortcutUsage usage, ManagedShortcut shortcut) {
            base.OnShortcutUsageCreated(usage, shortcut);
        }

        public override bool OnShortcutUsagesCreated() {
            StringJoiner joiner = new StringJoiner(new StringBuilder(), ", ");
            foreach (KeyValuePair<IShortcutUsage, ManagedShortcut> pair in this.ActiveUsages) {
                joiner.Append(pair.Key.CurrentStroke.ToString());
            }

            MainWindow.INSTANCE.ACTIVITY_BAR.Text = "Waiting for next input: " + joiner.ToString();
            return base.OnShortcutUsagesCreated();
        }

        public override bool OnSecondShortcutUsageProgressed(IShortcutUsage usage, ManagedShortcut shortcut) {
            return base.OnSecondShortcutUsageProgressed(usage, shortcut);
        }

        public override bool OnSecondShortcutUsagesProgressed() {
            StringJoiner joiner = new StringJoiner(new StringBuilder(), ", ");
            foreach (KeyValuePair<IShortcutUsage, ManagedShortcut> pair in this.ActiveUsages) {
                joiner.Append(pair.Key.CurrentStroke.ToString());
            }

            MainWindow.INSTANCE.ACTIVITY_BAR.Text = "Waiting for next input: " + joiner.ToString();
            return base.OnSecondShortcutUsagesProgressed();
        }
    }
}