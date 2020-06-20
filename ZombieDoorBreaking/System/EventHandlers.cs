using System;
using System.Collections.Generic;
using System.Linq;
using EXILED;
using EXILED.ApiObjects;
using EXILED.Extensions;
using Grenades;
using MEC;
using Mirror;
using UnityEngine;
using Log = EXILED.Log;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace ZombieDoorBreaking {
    public class EventHandlers {

        public Plugin plugin;

        public EventHandlers(Plugin plugin) {
            this.plugin = plugin;
        }

        public void DoorInteract(ref DoorInteractionEvent ev) {
            try {
                if(ev.Player.GetRole() == RoleType.Scp0492 && ev.Door.doorType == 2 && !ev.Door.Networklocked) {
                    if((!plugin.canClose && ev.Door.NetworkisOpen) 
                        || (plugin.griefProtection && ev.Door.DoorName.StartsWith("106")) )
                        return;
                    int amount = 0;
                    Door d = ev.Door;
                    foreach(ReferenceHub hub in Player.GetHubs(RoleType.Scp0492))
                        if(Vector3.Distance(d.transform.position, hub.GetPosition()) <= plugin.distanceNeeded) {
                            amount++;
                        }
                    if(amount >= plugin.amountNeeded) {
                        ev.Allow = true;
                        if(plugin.currentMode != Mode.OPEN && !ev.Door.NetworkisOpen && plugin.canClose) {
                            d.DestroyDoor(plugin.currentMode == Mode.LOCK_BREAK);
                            if(plugin.forceDestroy) {
                                d.destroyed = true;
                                d.Networkdestroyed = true;
                                return;
                            }
                            Timing.CallDelayed(0.5f, () => {
                                d.Networklocked = true;
                                d.locked = true;
                            });
                            if(plugin.unlockLater)
                                Timing.CallDelayed(plugin.unlockAfter, () => {
                                    d.Networklocked = false;
                                    d.locked = false;
                                });
                        }
                    } else {
                        if(plugin.neededBroadcastDuration <= 0) {
                            ev.Player.ClearBroadcasts();
                            ev.Player.Broadcast(plugin.neededBroadcastDuration, plugin.neededBroadcast.Replace("%amount", $"{plugin.amountNeeded - amount}"), false);
                        }
                    }
                }
            } catch(Exception e) {
                Log.Error("ZDB DoorInteract error: " + e.StackTrace);
            }
        }

        #region Commands
        public void OnCommand( ref RACommandEvent ev ) {
            try {
                if(ev.Command.Contains("REQUEST_DATA PLAYER_LIST SILENT")) return;
                string[] args = ev.Command.ToLower().Split(' ');
                ReferenceHub sender = ev.Sender.SenderId == "SERVER CONSOLE" || ev.Sender.SenderId == "GAME CONSOLE" ? Player.GetPlayer(PlayerManager.localPlayer) : Player.GetPlayer(ev.Sender.SenderId);
                if(args[0] == "zdb") {
                    ev.Allow = false;
                    if(!sender.CheckPermission("command")) {
                        ev.Sender.RAMessage("<color=red>Access denied.</color>");
                        return;
                    }
                    if(args.Length > 1) {
                        if(args[1] == "toggle") {
                            plugin.IsEnabled = !plugin.IsEnabled;
                            if(plugin.IsEnabled)
                                plugin.Register();
                            else
                                plugin.Unregister();
                            Plugin.Config.SetString("zdb_toggle", plugin.IsEnabled.ToString());
                            plugin.ReloadConfig();
                            ev.Sender.RAMessage("zdb_toggle has now been set to: " + plugin.IsEnabled.ToString());
                            return;
                        } else if(args[1] == "reload") {
                            plugin.ReloadConfig();
                            ev.Sender.RAMessage("<color=green>Configuration values have been reloaded.</color>");
                            return;
                        }
                    }
                    ev.Sender.RAMessage("Try using \"zdb <toggle/reload>\"");
                    return;
                }
                return;
            } catch(Exception e) {
                Log.Error("Command error: " + e.StackTrace);
            }
        }
        #endregion
    }
}