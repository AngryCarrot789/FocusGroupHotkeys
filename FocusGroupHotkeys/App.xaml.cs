﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using FocusGroupHotkeys.Converters;
using FocusGroupHotkeys.Core;
using FocusGroupHotkeys.Core.Services;
using FocusGroupHotkeys.Core.Shortcuts.Managing;
using FocusGroupHotkeys.Core.Shortcuts.ViewModels;
using FocusGroupHotkeys.Shortcuts;
using FocusGroupHotkeys.Shortcuts.Dialogs;
using FocusGroupHotkeys.Shortcuts.Views;
using FocusGroupHotkeys.Views.Dialogs.FilePicking;
using FocusGroupHotkeys.Views.Dialogs.Message;
using FocusGroupHotkeys.Views.Dialogs.UserInputs;

namespace FocusGroupHotkeys {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private static void UpdateShortcutResourcesRecursive(ResourceDictionary dictionary, ShortcutGroup group) {
            foreach (ShortcutGroup innerGroup in group.Groups) {
                UpdateShortcutResourcesRecursive(dictionary, innerGroup);
            }

            foreach (GroupedShortcut shortcut in group.Shortcuts) {
                UpdatePath(dictionary, shortcut.Path);
            }
        }

        private static void UpdatePath(ResourceDictionary dictionary, string shortcut) {
            string resourcePath = "ShortcutPaths." + shortcut;
            if (dictionary.Contains(resourcePath)) {
                dictionary[resourcePath] = ShortcutPathToInputGestureTextConverter.ShortcutToInputGestureText(shortcut);
            }
        }

        private void App_OnStartup(object sender, StartupEventArgs e) {
            IoC.MessageDialogs = new MessageDialogService();
            IoC.Dispatcher = new DispatcherDelegate(this);
            IoC.FilePicker = new FilePickDialogService();
            IoC.UserInput = new UserInputDialogService();
            InputStrokeViewModel.KeyToReadableString = KeyStrokeRepresentationConverter.ToStringFunction;
            InputStrokeViewModel.MouseToReadableString = MouseStrokeRepresentationConverter.ToStringFunction;
            IoC.KeyboardDialogs = new KeyboardDialogService();
            IoC.MouseDialogs = new MouseDialogService();
            IoC.ShortcutManager = WPFShortcutManager.Instance;
            IoC.OnShortcutManagedChanged = (x) => {
                if (!string.IsNullOrWhiteSpace(x)) {
                    UpdatePath(this.Resources, x);
                }
            };

            const string path = @"F:\VSProjsV2\FocusGroupHotkeys\FocusGroupHotkeys\SomeXML.xml";
            using (Stream stream = File.OpenRead(path)) {
                ShortcutGroup result = WPFKeyMapDeserialiser.Instance.Deserialise(stream);
                WPFShortcutManager.Instance.SetRoot(result);
            }

            // try {
            //     WPFShortcutManager.Instance.Root = null;
            //     using (Stream stream = File.OpenRead(path)) {
            //         ShortcutGroup result = WPFKeyMapDeserialiser.Instance.Deserialise(stream);
            //         WPFShortcutManager.Instance.Root = result;
            //     }
            // }
            // catch (Exception ex) {
            //     MessageBox.Show(ex.ToString(), "Failed to read config or no such file");
            // }

            // CommandManager.RegisterClassCommandBinding(typeof(ShortcutBinding), new CommandBinding());

            this.MainWindow = new MainWindow();
            this.MainWindow.Show();
            UpdateShortcutResourcesRecursive(this.Resources, IoC.ShortcutManager.Root);
        }

        private class DispatcherDelegate : IDispatcher {
            private readonly App app;

            public DispatcherDelegate(App app) {
                this.app = app;
            }

            public void InvokeLater(Action action) {
                this.app.Dispatcher.Invoke(action, DispatcherPriority.Normal);
            }

            public void Invoke(Action action) {
                this.app.Dispatcher.Invoke(action);
            }

            public T Invoke<T>(Func<T> function) {
                return this.app.Dispatcher.Invoke(function);
            }

            public async Task InvokeAsync(Action action) {
                await this.app.Dispatcher.InvokeAsync(action);
            }

            public async Task<T> InvokeAsync<T>(Func<T> function) {
                return await this.app.Dispatcher.InvokeAsync(function);
            }
        }
    }
}
