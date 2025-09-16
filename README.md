[![GitHub release](https://flat.badgen.net/github/release/FrikandelbroodjeCommunity/ZombieDoorBreaking-LabAPI/)](https://github.com/FrikandelbroodjeCommunity/ZombieDoorBreaking-LabAPI/releases/latest)
[![LabAPI Version](https://flat.badgen.net/static/LabAPI%20Version/v1.1.2)](https://github.com/northwood-studios/LabAPI)
[![Original](https://flat.badgen.net/static/Original/SnivyFilms?icon=github)](https://github.com/SnivyFilms/ZombieDoorBreaking)
[![License](https://flat.badgen.net/github/license/FrikandelbroodjeCommunity/ZombieDoorBreaking-LabAPI/)](https://github.com/FrikandelbroodjeCommunity/ZombieDoorBreaking-LabAPI/blob/master/LICENSE)

# About ZombieDoorBreaking

This LabAPI plugin gives SCP-049-2 a new ability to work alongside other infected humans, improving on it's `"swarm"`
thematic by allowing them to open doors that usually can't be open by normal SCPs if there's enough around. Overall,
pretty simple!

# Installation

Place the [latest release](https://github.com/FrikandelbroodjeCommunity/ZombieDoorBreaking-LabAPI/releases/latest) in
the LabAPI plugin folder.

# Usage

When playing as SCP-049-2 you can attempt to break/open a door/gate that normally requires keycard permissions by
interacting with the door (default: E key). The door will then open if enough zombies, that no longer have a cooldown,
are in the vicinity.

Doors that are locked, or already open, cannot be broken or closed.

# Config

| Config                    | Default        | Meaning                                                                                                                                                  |
|---------------------------|----------------|----------------------------------------------------------------------------------------------------------------------------------------------------------|
| `debug`                   | `false`        | When enabled, the plugin will show debug message. When using on a server it is recommended to keep this disabled.                                        |
| `breakable_door_modifier` | `OpenThenLock` | What should happen if the zombie succeed in opening the door (see [modifiers](#Breakable-door-modifiers)).                                               |
| `pryable_gate_modifier`   | `Pry`          | What should happen if the zombie succeed in opening a pryable gate (see [modifiers](#Pryable-gate-modifiers)).                                           |
| `zombies_needed`          | `5`            | The amount of zombies that need to be within the `MaxDistance` range of the door in order for it to open.                                                |
| `ability_cooldown`        | `24`           | The time in seconds before a zombie can attempt to open another door. During this time they cannot count towards the `ZombiesNeeded`.                    |
| `max_distance`            | `16`           | The squared maximum amount of distance a zombie can have to the door. This means a value of 16 will represent a max distance of 4 meters (as `4^2 = 16`) |
| `unlock_after_seconds`    | `3`            | When a modifiers is set to `OpenThenLock`, this determines the amount of time the doors will be locked for in seconds.                                   |
| `rate_limit`              | `2.5`          | The amount of time in seconds that needs to be between door open attempts, this rate limit affects <i>all</i> doors globally.                            |
| `display_duration`        | `7`            | The amount of time in seconds the hints will be shown to players. Make sure this is long enough so players have time to read the messages.               |
| `not_enough_zombies_text` | ...            | The message shown when a player attempts to open a door, but there are not enough zombies nearby.                                                        |
| `on_cooldown_text`        | ...            | The message shown when a player attempts to open a door, but they are still on cooldown.                                                                 |
| `on_break_door_text`      | ...            | The message shown to all players that assisted in opening a door.                                                                                        |

## Breakable door modifiers

| Variable       | Meaning                                                                      |
|----------------|------------------------------------------------------------------------------|
| `OpenThenLock` | Opens the door, then it locks it based off the `unlock_after_seconds` value. |
| `Open`         | Opens the door, but does not lock it, `unlock_after_seconds` does nothing.   |
| `Break`        | Breaks open the door, `unlock_after_seconds` does nothing.                   |
| `Nothing`      | Does nothing, effectively disabling the plugin for doors.                    |

## Pryable gate modifiers

| Variable       | Meaning                                                                    |
|----------------|----------------------------------------------------------------------------|
| `OpenThenLock` | Opens the door, then it locks it based off the unlock_after_seconds value. |
| `Open`         | Opens the door, but does not lock it, `unlock_after_seconds` does nothing. |
| `Pry`          | Pries open the door like SCP-096, `unlock_after_seconds` does nothing.     |
| `Nothing`      | Does nothing, effectively disabling the plugin for doors.                  |
