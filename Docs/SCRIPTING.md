# SpeedTool scripting overiew
SpeedTool is using the [LUA programming language](https://www.lua.org/) for its scripting capabilities.
Below you will find a brief introduction to timer scripting with SpeedTool

## Important things
Each script must contain two function: `on_load` and `on_frame`. Other than that, everything is up for the programer to do.  
`on_load` function is being executed once the script is loaded.  
`on_frame` function is executed on each frame the target game outputs.

## Sample script
Below you can find a sample script file written for Need for Speed: Underground 2
```lua
function on_load()
    speed2 = module_base_address('SPEED2.exe')
    lastFmv = 0
    oldRaceState = 0
    lastStar = 0
end

function split()
    if (lastFmv == 0 and fmv ~= 0) then
        -- First drive to carlot
        if fmvName == 'SCENE05' then
            timer_split()
        end
        if fmvName == 'SCENE06' then
            timer_split()
        end
    end
end

function split_on_race()
    if (oldRace == 1 and race == 3) then
        timer_split()
        return
    end
    if (oldRace == 2 and race == 3) then
        timer_split()
        return
    end
    if(oldStar == 2 and star == 0) then
        timer_split()
        return
    end
end

function on_frame()
    fmvName = read_ascii(speed2 + 0x4382A0, 7)
    race = read_int(speed2 + 0x49CE00)
    star = read_int(speed2 + 0x438578)
    fmv = read_int(0x008383AC)

    if (fmvName == 'scene01' and fmv ~= 0 and lastFmv == 0) then
        timer_start()
    end

    loading = read_int(0x00832E58)

    if (loading == 0 and fmv ~= 1) then
        timer_set_loading()
    else
        timer_set_not_loading()
    end

    split()
    split_on_race()

    lastFmv = fmv
    oldRace = race
    lastStar = star
end
```

## Functions and descriptions
Please refer to those for available functionality in the scripting engine.

* Reading memory

| Function | Description |
| :---     | :---        |
| read_int(addr) | Reads a 32-bit integer from `addr` |
| read_long(addr) | Reads a 64-bit integer from `addr` |
| read_float(addr) | Reads a 32-bit single precision floating point value from `addr` |
| read_double(addr) | Reads a 64-bit double precision floating point value from `addr` |
| read_ascii(addr, n) | Reads an ASCII string of length `n` from `addr` |
| read_bytes(addr, n) | Reads `n` bytes from `addr` |
| pointer_path(addr[]) | Unwraps a pointer path for `addr`. Since pointer pathes can change in runtime, it's best to not cache the result |
| module_base_address(module) | Returns the base address of a `module`. The search is case-insensitive |


> **_NOTE:_** `read_bytes` will return array that starts indexing with 0, unlike LUA's usual 1

* Timer controls

| Function | Description |
| :---     | :---        |
| timer_set_loading() | Set timer to loading |
| timer_set_not_loading() | Set timer to not loading |
| timer_start() | Starts the timer. If the timer is already started, nothing happens |
| timer_split() | Splits the timer |

* Debug

| Function | Description |
| :---     | :---        |
| debug_message(str) | Sends a debug `str` to main app |
| debug_message_address(addr) | Sends a debug `addr` to the main app |
