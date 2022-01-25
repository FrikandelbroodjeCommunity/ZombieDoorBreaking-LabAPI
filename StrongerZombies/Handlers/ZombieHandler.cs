using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Exiled.API.Features;
using Exiled.Events.EventArgs;

using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;

using MEC;

using UnityEngine;

namespace StrongerZombies.Handlers
{
    public class ZombieHandler
    {
        public ZombieHandler(StrongerZombies instance) => core = instance;

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
            if (roundEnded || ev.Player.Role != RoleType.Scp0492 || ev.Door.IsLocked || rateLimit > Time.time
                || ev.Door.RequiredPermissions.RequiredPermissions.HasFlagFast(KeycardPermissions.ScpOverride))
                return;

            if (ev.Player.TryGetSessionVariable(CooldownTag, out float cd) && cd > Time.time)
            {
                ev.Player.Broadcast(core.Config.OnCooldown);
                return;
            }

            int acceptedCount = 0;
            foreach (var player in Player.List)
            {
                if (player.Role != RoleType.Scp0492)
                    continue;

                if ((player.Position - ev.Player.Position).sqrMagnitude < core.Config.MaxDistance)
                {
                    // We store the player's ID so we can later give everyone a cooldown, not just the player who used it.
                    player.SessionVariables[OnCdTag] = ev.Player.Id;
                    acceptedCount++;
                }
            }

            if (core.Config.ZombiesNeeded < acceptedCount)
            {
                rateLimit = Time.time + core.Config.RateLimit;

                ev.Player.Broadcast(core.Config.NotEnoughZombies);
                return;
            }

            ev.IsAllowed = false;

            if (ev.Door.Base is PryableDoor pryableDoor)
            {
                pryableDoor.TryPryGate();
            }
            else if (ev.Door.Base is IDamageableDoor damageableDoor)
            {
                switch (core.Config.BreakableDoorModifier)
                {
                    case DoorModifier.Break:
                        damageableDoor.ServerDamage(ev.Door.Health, DoorDamageType.Scp096);
                        break;
                    case DoorModifier.OpenThenLock:
                        Open(ev.Door, true);
                        break;
                    case DoorModifier.Open:
                        Open(ev.Door);
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

            if(core.Config.UnlockAfterSeconds > 0)
                coroutines.Add(Timing.CallDelayed(core.Config.UnlockAfterSeconds, () => door.Unlock()));
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
