# FocusGroupHotkeys
An app for managing multi-stage key and mouse shortcuts (e.g. the similar behaviour found in Intellij IDEA, Visual Studio, etc)

This can be used to create shotcuts like "CTRL + M, CTRL + X" which require you to press M and then X while holding CTRL (irregardless on key release or modifier key pressed)

I also implemented a similar behaviour to Adobe Premiere Pro's Panel focus (and probably many other programs, but adobe seems like the only one that outlines the panel), where clicking different panels will focus them, changing which key bindings can be activated. A "Panel" is defined with a path, which in this app is an attached property (found in `UIFocusGroup`, as `FocusGroupPathProperty`).

Shortcuts and groups are defined as paths, like a file system; You have folders (shortcut groups) and the files (shortcuts)

# Preview
At the bottom is the status indicator (i was lazy so I just created a static MainWindow reference to access the text block). The shortcuts switch the toggle state of the check boxes (all done through multiple ICommands and bool properties, in MainViewModel)
![](FocusGroupHotkeys_2023-03-29_19.43.17.png)

This is the shortcut editor. Also kinda copied IntelliJ IDEA's layout with the yellow background. You can right click a shortcut to add key/mouse strokes, or remove one of the strokes. I created a small context menu item generation library to help with that (IContextGenerator, ContextElement, etc)
![](FocusGroupHotkeys_2023-03-29_19.44.30.png)

## Input strokes
The whole system works around "Key" and "Mouse" strokes. KeyStrokes have a KeyCode, Modifiers, and IsKeyRelease property, and MouseStrokes have a MouseButton, (keyboard) Modifiers, ClickCount and WheelDelta property. I tried to make it as cross-platform as possible, so Key codes (for keyboard and mouse) are plain integers. In the WPF example, i'm just casting to and from the Key and ModifierKeys enum (except for mouse wheel, WHEEL_UP and WHEEL_DOWN are defined with a custom MouseButton integer. Was also thinking about "mouse flicks" but i have no idea how to begin implementing that)

## Keymap storage
The key map are loaded from an XML file just so that it's easy to add and remove shortcuts. You could also add some functions to create the groups purely in C# code. 

Shortcuts are stored in ShortcutGroups, and the root ShortcutGroup is stored in a ShortcutManager.

In the XML file, you don't directly supply the paths, you instead just provide the Name attribute for each group and shortcut, and the C# will generate the path using the '/' char

## Global/Inheritance
Shortcuts and ShortcutGroups have an "IsGlobal" property, which means that they can be fired even if they aren't in the currently focused panel, or even in the panel's hierarchy (hense, global ;D). 

I actually can't quite remember what the inherit feature does, apart from that instead of doing an equality comparison between the focused path and the current shortcut group path (as in, the one being searched during an input stroke), it instead checks if the focused path begins with the current shortcut group path

## Shortcut activated events
Creating events when shortcuts are activated can be done by inheriting from the ShortcutManager and overriding the OnShortcutActivated function. 
The return type is passed to the key event's Handled flag.

## Shortcut activated command binding
I implemented the extra functionality in the AppShortcutManager. To create a shortcut binding, you can use the ShortcutBinding class which you add to a UIElement's InputBindings (easier than using an attached collection property). The shortcut bindings contain a `ShortcutAndUsageId` property, used to register the shortcut binding 
with the AppShortcutManager, and the manager will handle the rest (it will store a callback function into the shortcut binding, which invokes the command in a slightly non-standard way)

Due to the limitation of input bindings, you need to supply the full path in the shortcut binding, because the reference to the input bindings's dependency property parent is internal, meaning the parent group cannot be inferred.
