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

        public EventHandlers( Plugin plugin ) {
            this.plugin = plugin;
        }
        public void DoorInteract(ref DoorInteractionEvent ev) {
            try {
                if(ev.Player.GetRole() == RoleType.Scp0492 && ev.Door.doorType == 2 && !ev.Door.NetworkisOpen) {
                    int amount = 0;
                    foreach(ReferenceHub hub in Player.GetHubs(RoleType.Scp0492))
                        if(Vector3.Distance(ev.Player.GetPosition(), hub.GetPosition()) <= plugin.distanceNeeded)
                            amount++;

                    if(amount >= plugin.amountNeeded)
                        ev.Allow = true;
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
                if(args[0].EqualsIgnoreCase("zdb")) {
                    ev.Allow = false;
                    if(!checkPermission(ev, sender, "command"))
                        return;
                    if(args.Length == 1) {
                        ev.Sender.RAMessage("Try using \"zdb <toggle/reload>\"");
                        return;
                    }
                    if(args[1].EqualsIgnoreCase("toggle")) {
                        plugin.IsEnabled = !plugin.IsEnabled;
                        if(plugin.IsEnabled)
                            plugin.Register();
                        else
                            plugin.Unregister();
                        Plugin.Config.SetString("zdb_toggle", plugin.IsEnabled.ToString());
                        plugin.ReloadConfig();
                        ev.Sender.RAMessage("zdb_toggle has now been set to: " + plugin.IsEnabled.ToString());
                        return;
                    } else if(args[1].EqualsIgnoreCase("reload")) {
                        plugin.ReloadConfig();
                        ev.Sender.RAMessage("<color=green>Configuration values have been reloaded.</color>");
                        return;
                    }
                }
                return;
            } catch(Exception e) {
                Log.Error("Command error: " + e.StackTrace);
            }
        }
        #endregion

        public bool checkPermission( RACommandEvent ev, ReferenceHub sender, string perm ) {
            if(!sender.CheckPermission("zdb.*") || !sender.CheckPermission("zdb." + perm)) {
                ev.Sender.RAMessage("<color=red>Access denied.</color>");
                return false;
            }
            return true;
        }
    }
}