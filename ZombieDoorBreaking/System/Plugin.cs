using System;
using System.Collections.Generic;
using System.Linq;
using EXILED;
using MEC;

namespace ZombieDoorBreaking {
    public class Plugin : EXILED.Plugin {

        public EventHandlers EventHandlers;

        public bool IsEnabled, unlockLater, canClose, griefProtection, forceDestroy;
        public Mode currentMode;
        public int amountNeeded;
        public float distanceNeeded, unlockAfter;
        public uint neededBroadcastDuration;
        public string neededBroadcast;

        public override void OnEnable() {
            try {
                Log.Debug("ZombieDoorBreaking plugin detected, loading configuration file...");
                ReloadConfig();
                Log.Debug("Initializing EventHandlers...");
                EventHandlers = new EventHandlers(this);

                if(IsEnabled)
                    Register();
                Events.RemoteAdminCommandEvent += EventHandlers.OnCommand;

                Log.Info("Plugin loaded correctly!");
            } catch ( Exception e ) {
                Log.Error("Problem loading plugin: " + e.StackTrace);
            }
        }

        public override void OnDisable() {

            Unregister();
            Events.RemoteAdminCommandEvent -= EventHandlers.OnCommand;

            EventHandlers = null;
        }

        public void Register() =>
                Events.DoorInteractEvent += EventHandlers.DoorInteract;

        public void Unregister() =>
                Events.DoorInteractEvent -= EventHandlers.DoorInteract;

        public override void OnReload() {
        }

        private Mode GetMode() {
            Enum.TryParse(Config.GetString("zdb_mode", "lock"), true, out Mode mode);
            return mode;
        }

        public void ReloadConfig() {
            Config.Reload();
            IsEnabled = Config.GetBool("zdb_enabled", true);
            unlockLater = Config.GetBool("zdb_unlock", true);
            unlockAfter = Config.GetFloat("zdb_unlock_after", 4f);
            canClose = Config.GetBool("zdb_canclose", true);
            currentMode = GetMode();
            amountNeeded = Config.GetInt("zdb_amount", 4);
            distanceNeeded = Config.GetFloat("zdb_distance", 4f);
            forceDestroy = Config.GetBool("zdb_forcedestroy", false) && currentMode == Mode.LOCK_BREAK;
            griefProtection = Config.GetBool("zdb_griefprotection", true);
            neededBroadcast = Config.GetString("zdb_broadcast_text", $"<color=red>Necesitas a %amount SCP-049-2 más para abrir la puerta.</color>");
            neededBroadcastDuration = Config.GetUInt("zdb_broadcast_duration", 4);
        }

        public override string getName { get; } = "ZombieDoorBreaking";
    }
}