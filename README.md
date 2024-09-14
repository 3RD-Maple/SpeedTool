# SpeedTool
A scriptable OpenGL-accelerated speedrunning timer that runs inside your games!

>⚠️**WARNING**⚠️ This project is still a _**Work In Progress**_. Please expect rough edges, missing features, bugs, and breakage of backwards compatibility.

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

## Future plans

Some things we'd like to work on in the near future:

* Improving the UI and UX
* More customisation options
* Automatic game loading based on what game is currently running
* More visualisation options
* Browser source capture support
