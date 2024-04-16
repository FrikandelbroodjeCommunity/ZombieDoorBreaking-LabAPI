using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Exiled.API.Interfaces;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using PlayerRoles;
using Exiled.API.Features.Doors;

using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;

using MEC;

using UnityEngine;

namespace StrongerZombies.Handlers
{
    public class ZombieHandler
    {
        public ZombieHandler(StrongerZombies instance) => core = instance;
        public string ZombiesNeededBroadcast = string.Empty;
        public string OnCooldownBroadcast = string.Empty;

        public void Subscribe()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.InteractingDoor += DoorInteract;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnd;
        }

        public void Unsubscribe()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Player.InteractingDoor -= DoorInteract;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnd;
        }

        private void OnWaitingForPlayers() => roundEnded = false;

        private void OnRoundEnd(RoundEndedEventArgs ev)
        {
            roundEnded = true;

            // We clear these on Round end as it's unpredictable whether Exiled will destroy any referenced objects before waiting for players.
            foreach (var item in coroutines)
            {
                Timing.KillCoroutines(item);
            }

            coroutines.Clear();
        }

        private void DoorInteract(InteractingDoorEventArgs ev)
        {
            if (roundEnded || ev.Player.Role != RoleTypeId.Scp0492
                || ev.Door.RequiredPermissions.RequiredPermissions.HasFlagFast(KeycardPermissions.ScpOverride)
                || !ev.Door.IsKeycardDoor || ev.Door.IsLocked || ev.Door.IsOpen || rateLimit > Time.time)
            {
                Log.Debug("Cannot Break Door: Not a Zombie, Door Is Locked, Door is a Normal Hall Door, Rate Limit, Checkpoint Door, Door is Open");
                return;
            }

            if (ev.Player.TryGetSessionVariable(CooldownTag, out float cd) && cd > Time.time)
            {
                if (!string.IsNullOrEmpty(core.Config.OnCooldownText))
                {
                    OnCooldownBroadcast = core.Config.OnCooldownText;
                    ev.Player.Broadcast(new Exiled.API.Features.Broadcast(OnCooldownBroadcast, core.Config.DisplayDuration));
                }
                else
                    Log.Debug("String is empty");
                Log.Debug("Cannot Break Door: Cooldown Config");
                return;
            }

            int acceptedCount = 0;
            foreach (var player in Player.List)
            {
                if (player.Role != RoleTypeId.Scp0492)
                    continue;

                if ((player.Position - ev.Player.Position).sqrMagnitude < core.Config.MaxDistance)
                {
                    // We store the player's ID so we can later give everyone a cooldown, not just the player who used it.
                    player.SessionVariables[OnCdTag] = ev.Player.Id;
                    acceptedCount++;
                }
            }

            if (core.Config.ZombiesNeeded - 1 >= acceptedCount)
            {
                rateLimit = Time.time + core.Config.RateLimit;
                if (!string.IsNullOrEmpty(core.Config.NotEnoughZombiesText))
                {
                    ZombiesNeededBroadcast = core.Config.NotEnoughZombiesText;
                    ZombiesNeededBroadcast = ZombiesNeededBroadcast.Replace("{zombiecount}", core.Config.ZombiesNeeded.ToString());
                    ev.Player.Broadcast(new Exiled.API.Features.Broadcast(ZombiesNeededBroadcast, core.Config.DisplayDuration));
                }
                else
                    Log.Debug("String is empty");
                Log.Debug("Cannot Break Door: Not Enough Zombies Config");
                Log.Debug("Zombies Required:" + core.Config.ZombiesNeeded.ToString());
                return;
            }

            ev.IsAllowed = false;

            if (ev.Door is Gate pryableDoor)
            {
                pryableDoor.TryPry();
                Log.Debug("Opening Gate");
            }
            else if (ev.Door is Exiled.API.Interfaces.IDamageableDoor damageableDoor)
            {
                switch (core.Config.BreakableDoorModifier)
                {
                    case DoorModifier.Break:
                        damageableDoor.Damage(damageableDoor.Health, DoorDamageType.ServerCommand);
                        Log.Debug("Destroying Door");
                        break;
                    case DoorModifier.OpenThenLock:
                        Open(ev.Door, true);
                        Log.Debug("Opening & Locking Door");
                        break;
                    case DoorModifier.Open:
                        Open(ev.Door);
                        Log.Debug("Opening Door");
                        break;
                }
            }

            foreach (var player in Player.List)
            {
                // Here we give the previously stored IDs to good use.
                if (player.TryGetSessionVariable(OnCdTag, out int id) && id == ev.Player.Id)
                {
                    player.SessionVariables[CooldownTag] = Time.time + core.Config.AbilityCooldown;
                }
            }
        }

        private void Open(Door door, bool shouldLock = false)
        {
            door.IsOpen = true;

            if (!shouldLock)
                return;

            door.ChangeLock(Exiled.API.Enums.DoorLockType.Regular079);

            if (core.Config.UnlockAfterSeconds > 0)
            {
                coroutines.Add(Timing.CallDelayed(core.Config.UnlockAfterSeconds, () => door.Unlock()));
                Log.Debug("Adding coroutine for Unlocking Doors after Opening and Locking Door");
            }
        }

        private const string OnCdTag = "sz_oncd";
        private const string CooldownTag = "sz_cd";

        private readonly List<CoroutineHandle> coroutines = new List<CoroutineHandle>();
        private readonly StrongerZombies core;

        private float rateLimit;
        private bool roundEnded;

        public enum DoorModifier
        {
            OpenThenLock, Break, Open
        }
    }
}
