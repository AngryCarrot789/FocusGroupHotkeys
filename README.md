# FocusGroupHotkeys
An app for managing multi-stage key and mouse shortcuts (e.g. the similar behaviour found in Intellij IDEA, Visual Studio, etc)

This can be used to create shotcuts like "CTRL + M, CTRL + X" which require you to press M and then X (irregardless on key release or modifier key pressed) while holding CTRL

I tried to give this similar behaviour to Adobe Premiere Pro, where clicking different panels will focus them, changing which key bindings do what. A
"Panel" is defined with a path, which in this app is an attached property (found in `UIFocusGroup`, as `FocusGroupPathProperty`).

This is kinda based around a file system, where you have folders (shortcut groups) and the files (shortcuts)

Also I added a bunch of unnessary libraries to the Core project (IoC, the Services folder (IDispatcher, etc), View/Dialog classes, etc) because 
I plan on using them to create the shortcut editor dialogs

# Preview
At the bottom is the status indicator (i was lazy so I just created a static MainWindow reference to access the text block). The shortcuts switch the toggle state of the check boxes (all done through multiple ICommands and bool properties, in MainViewModel)
![](FocusGroupHotkeys_2023-03-29_19.43.17.png)

This is the shortcut editor (can't edit yet... only preview). Also kinda copied IntelliJ IDEA's layout with the yellow background
![](FocusGroupHotkeys_2023-03-29_19.44.30.png)

## Input strokes
The whole system works around "Key" and "Mouse" strokes. KeyStrokes have a KeyCode, Modifiers, and IsKeyRelease property. 
I tried to make it as cross-platform as possible, so Key codes (for keyboard and mouse) are plain integers. 
In in the example in this repo, i'm just casting to and from the Key and ModifierKeys enum

## Keymap storage
The key map are loaded from an XML file (could switch to XAML at some point). 
Shortcuts are stored in ShortcutGroups, and the root ShortcutGroup is stored in a ShortcutManager. At the moment, 
shortcuts cannot be global, but a ShortcutGroup has a "Global" attribute, making it apply across the entire window (not application... yet).

In the XML file, you don't directly supply the paths, and instead it is generated when the group is created (you provide the Name attribute to a group or shortcut)

## Shortcut activated events
Creating events when hotkeys are pressed is done by inheriting from the ShortcutManager and overriding the OnShortcutActivated function. 
The return type is passed to the key event's Handled flag.

## Shortcut activated command binding
I implemented the extra functionality in the AppShortcutManager. To create a shortcut binding, you can use the ShortcutBinding which 
you add to a UIElement's InputBindings. The shortcut bindings contain a `ShortcutAndUsageId` property, used to register the shortcut binding 
with the AppShortcutManager, and the manager will handle the rest (it will store a callback function into the shortcut binding, 
which invokes the command in a slightly non-standard way)

Due to the limitation of input bindings, you need to supply the full path in the shortcut binding, because the reference to the 
input bindings's dependency property parent is internal, meaning the parent group cannot be inferred.
