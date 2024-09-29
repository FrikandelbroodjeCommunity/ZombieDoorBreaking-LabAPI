# ZombieDoorBreaking

![Downloads](https://img.shields.io/github/downloads/SnivyFilms/ZombieDoorBreaking/total.svg)

This Exiled plugin gives SCP-049-2 a new ability to work alongside other infected humans, improving on it's `"swarm"` thematic by allowing them to open doors that usually can't be open by normal SCPs if there's enough around. Overall, pretty simple!

# Configs
```yml
is_enabled: true
debug: false
breakable_door_modifier: OpenThenLock
zombies_needed: 5
ability_cooldown: 24
max_distance: 24
unlock_after_seconds: 3
rate_limit: 2.5
not_enough_zombies: "<color=red>There isn't enough zombies for this ability! You need {zombiecount} to open this door</color>"
on_cooldown: "<color=red>This ability is currently on cooldown!"
DisplayDuration: 5
```

# Breakable door modifiers
| Variable | Meaning |
| ------------- | ------------- |
| `OpenThenLock` | Opens the door, then it locks it based off the unlock_after_seconds value |
| `Open` | Opens the door, but does not lock it, unlock_after_seconds does nothing |
| `Break` | Breaks open the door, unlock_after_seconds does nothing |

# String variables
| Variable | Meaning |
| ------------- | ------------- |
| `{zombiecount}` | Gets the required amount zombies needed to open/break the door | 
