using System;
using System.Collections.Generic;
using EXILED;
using MEC;

namespace ZombieDoorBreaking {
    public class Plugin : EXILED.Plugin {

        public EventHandlers EventHandlers;

        public bool IsEnabled;
        public int amountNeeded;
        public float distanceNeeded;

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

        public void ReloadConfig() {
            Config.Reload();
            IsEnabled = Config.GetBool("zdb_enabled", true);
            amountNeeded = Config.GetInt("zdb_amount", 4);
            distanceNeeded = Config.GetFloat("zdb_distance", 4f);
        }

        public override string getName { get; } = "ZombieDoorBreaking";
    }
}