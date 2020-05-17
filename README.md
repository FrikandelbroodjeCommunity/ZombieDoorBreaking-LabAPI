# ZombieDoorBreaking
This plugin gives SCP-049-2 a new ability to work alongside other infected humans, improving on it's `"swarm"` thematic by allowing them to open doors that usually can't be open by normal SCPs if there's enough around. Overall, pretty simple! (It still needs [EXILED](https://github.com/galaxy119/EXILED "EXILED") to work tho!).

### Installation
As with any EXILED plugin, you must place the ZombieDoorBreaking.dll file inside of your "%appdata%/Roaming/Plugins" folder.

### Commands
| Command | Description | Permission |
| ------------- | ------------------------------ | -------------------- |
| `zdb` | Plugin's main command, displays in-game help | `zdb.command` |
| `zdb toggle` | Toggles this plugin | `zdb.command` |
| `zdb reload` | Reloads configuration values to match your config-file | `zdb.command` |

**TIP:** Use `zdb.*` to receive all permissions.

### Configuration
These are the variables that should be added to your 7777-config.yml. Or simply download/copy the [config-file example](https://github.com/SebasCapo/ZombieDoorBreaking/blob/master/Examples/7777-config.yml)
| Variable  | Description | Default value |
| ------------- | ------------- | ------------- |
| zdb_toggle | Toggles whether the plugin is enabled or not | `true` |
| zdb_amount | How many SCP-049-2 are needed to open a door (counting the one opening the door too!) | `4` |
| zdb_distance | Distance needed to count the SCP-049-2 a part of the group | `4` (Also recommend `3.8`) |
| zdb_lock | Should the door be destroyed (or locked if can't be destroyed)? | `true` |

### That'd be all
Thanks for passing by, have a nice day! :)
