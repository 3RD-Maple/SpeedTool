# SpeedTool
A scriptable OpenGL-accelerated speedrunning timer that runs inside your games!

>⚠️**WARNING**⚠️ This project is still a _**Work In Progress**_. Please expect rough edges, missing features, bugs, and breakage of backwards compatibility.

You can find the newest release on [itch.io](https://3rdmaple.itch.io/speedtool)

_We are looking for contributors. Please consider checking out [CONTRIBUTING](CONTRIBUTING.md) if you wish to help improve the project._

## Features
* **Frame perfect.** Unlike other scriptable timers, SpeedTool is injected directly into the target game and synchronizes perfectly with game's refresh rates. Therefore, the script is executed at the end of every frame the game outputs
* **Safe scripting.** SpeedTool uses a safe [LUA programming language](https://www.lua.org/) sandbox to execute scripts, which means scripts can't do any harm
* **Unlimited subsplits.** Speedtool comes with builtin support for infinitely nesting splits

## Supported platforms

Speedtool is designed to be cross-platform and it's confirmed to run on Windows and Linux operating systems! It *should* support MacOS, but we have no way of testing it.

**Scripting is only supported on Windows** (expanding to other systems _might_ happen)

List of already supported graphics platforms:
* OpenGL
* DirectX 9
* DirectX 11

Support for other graphics platforms will be added in near future

> **ℹ️ NOTE ℹ️** Some games might be impossible to inject to, due to anti-cheat software or other game-specific implementation quirks. Please note that we can only test games that we have in our possession.

## Building

SpeedTool is based on .NET 8.0, however the injected timer is built using .NET Framework 4.6.

Build with:
```
dotnet build
```

It _**is**_ as easy as that!

If you want to run SpeedTool, please execute the following command:
```
dotnet run --project SpeedTool
```

**Due to complications with .NET Framework versions, to get scripting support you need to manually copy the output of TimerInjector project into SpeedTool output directory.**

## Running on Linux

### Wayland

SpeedTool runs natively on Wayland (the bundled GLFW supports it out of the box).

Global hotkeys work differently on Wayland: the compositor does not allow applications to install global
keyboard hooks, so SpeedTool reads key events directly from the kernel's evdev devices instead. This requires
read access to `/dev/input/event*`, which usually means being a member of the `input` group:

```
sudo usermod -aG input $USER
```

(Log out and back in for the group change to take effect.) If evdev access is unavailable, SpeedTool falls back
to the regular global hook, which only sees X11/XWayland applications.

The hotkey backend can be forced with the `SPEEDTOOL_HOTKEY_BACKEND` environment variable:

* `SPEEDTOOL_HOTKEY_BACKEND=evdev` - always use evdev
* `SPEEDTOOL_HOTKEY_BACKEND=global` - always use the global hook (X11)

When unset, SpeedTool picks evdev automatically on Wayland sessions.

The active backend is written to the debug log on startup.

## Future plans

Some things we'd like to work on in the near future:

* Improving the UI and UX
* More customisation options
* Automatic game loading based on what game is currently running
* More visualisation options
* Browser source capture support
