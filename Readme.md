# Important
First of all, this "software" is far from ready, once it becomes mature, I'll properly document and package it etc.
For now if you really want to try, you can message me to help you set it up, or try it yourself.
Expect many breaking changes, bugs, and stuff not working (like VNAV, I cannot find the proper events for it...) for now.

# Features
I'll add pictures/videos later, for now you just have to trust me.

## Currently working
### Buttons (internally called toggles)
These show the status of a function (like autopilot master, or flight director)
and allow you to toggle it's state, from off -> on or vice versa and visually update it's state within ~50 milliseconds.
Clicking the button sends an event to the sim, and the sim sends an event back with the new value which gets displayed.
Sometimes the value doesn't change, for example you cannot turn the FD off when AP is on.

- Altitude hold (ALT)
- Autopilot master (AP)
- Approach mode (APR)
- Flight director (FD)
- Flight level change (FLC)
- Heading mode (HDG)
- Wing leveler (LVL) aka. blue button
- Navigation mode (NAV)
- Vertical speed (VS)

### Dials
These allow you to dial in a specifc number shown on the touchscreen display above the dials.
When fiddling with these, the value shown on the Stream Deck change immediately,
and update in the Sim basically immediately if it hasn't been updated in the last 100 ms, or 100 ms after the previous update.

- Heading (HDG) 0-359, slow dialling adds or subtracts 1, fast dialling 10. You can click on the dial to select the airplane's indicated heading. Suffixed with: °.
- Altitude (ALT) 0-510, slow dialling adds or subtracts 1, fast dialling 10. Below 100 the shown value is multiplied by 100 (to get feet) and a "ft" suffix gets added;
At and above 100, a FL (Flight Level) prefix gets added (instead of the "ft" suffix). Clicking the dial sets the airplane's indicated altitude.
- Vertical speed (VS) -99 - +99, dialling only ever changes by 1. The shown value is always prefixed with an '+' or '-', multiplied by 100 to get feet and suffixed with "ft".
Clicking the dial resets it back to 0.
- Speed (SPD) 0-999 (default), slow dialling adds or subtracts 1, fast dialling 10. You can click on the dial to select the airplane's indicated air speed (IAS).
The min and max values get updated with the flown airplane's min and max auto throttle values.

## Currently not working
- Auto throttle (AT) and MAN/FMS switch for it
- Vertical navigation mode (VNAV)

## Next on my list
- Auto throttle (AT + MAN/FMS) - **currently working on this**
- Vertical navigation mode (VNAV). I've looked for hours, but cannot find the right events, I'll probably have to reach out on some forum,
maybe even contact MS themselves (or rather the developer responsible for the Sim) because even on the forums I've only found other lost people in this regard.
- COM/NAV radios

# Requirements
- **Stream Deck plus** (other models should work, but for now the dials are the only way to set HDG, ALT, VS, SPD. I do plan to add these things to regular buttons in the future though.)
- **[StreamController](https://github.com/StreamController/StreamController)**
- **Microsoft Flight Simulator 2024** running through proton on Steam
- **ProtonTricks**

# Structure
## StreamControllerPlugin
I think this is self explanatory.

## WineRealy
This is the main program running in the same wine prefix as the Sim,
that communicates with the StreamController plugin over a local UDP connection,
and the Sim with SimConnect.

## WineRealyHost
Right now this is basically useless.
Its only purpose is to start the WineRelay process in the correct prefix with ProtonTricks.
It used to do more at first (when I was prototyping before the first commit) when I wasn't sure about the architecture.
I am too lazy to change this to a simple bash script but definitely will in the future.
For now I simply start this with `dotnet run` or `dotnet build ../WineRelay && dotnet run` after making changes in the WineRelay.

# Contributing
Feel free to open issues or submit pull requests.
