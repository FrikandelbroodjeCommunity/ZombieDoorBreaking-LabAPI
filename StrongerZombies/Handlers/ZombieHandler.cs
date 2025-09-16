using System.Collections.Generic;
using System.Linq;
using PlayerRoles;
using Interactables.Interobjects.DoorUtils;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace StrongerZombies.Handlers;

public static class ZombieHandler
{
    private static readonly Dictionary<Player, float> Cooldown = new();
    private static readonly List<CoroutineHandle> Coroutines = new();

    private static BalanceSettings Config => StrongerZombies.Instance.Config ?? new BalanceSettings();

    private static float _rateLimit;
    private static bool _roundEnded;

    public static void RegisterEvents()
    {
        ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
        ServerEvents.RoundEnded += OnRoundEnd;
        PlayerEvents.InteractingDoor += DoorInteract;
    }

    public static void UnregisterEvents()
    {
        ServerEvents.WaitingForPlayers -= OnWaitingForPlayers;
        ServerEvents.RoundEnded -= OnRoundEnd;
        PlayerEvents.InteractingDoor -= DoorInteract;
    }

    private static void OnWaitingForPlayers()
    {
        _roundEnded = false;
        Cooldown.Clear();
    }

    private static void OnRoundEnd(RoundEndedEventArgs ev)
    {
        _roundEnded = true;

        // We clear these on Round end as it's unpredictable whether Exiled will destroy any referenced objects before waiting for players.
        foreach (var item in Coroutines)
        {
            Timing.KillCoroutines(item);
        }

        Coroutines.Clear();
    }

    private static void DoorInteract(PlayerInteractingDoorEventArgs ev)
    {
        if (_roundEnded || ev.Player.Role != RoleTypeId.Scp0492
                        || ev.Door.Permissions.HasFlag(DoorPermissionFlags.ScpOverride)
                        || ev.Door.Permissions == DoorPermissionFlags.None || ev.Door.IsLocked ||
                        ev.Door.IsOpened || _rateLimit > Time.time)
        {
            Logger.Debug("Cannot break the door: No permission", Config.Debug);
            return;
        }

        if (Cooldown.TryGetValue(ev.Player, out var cd) && cd > Time.time)
        {
            ev.Player.SendHint(Config.OnCooldownText, Config.DisplayDuration);
            Logger.Debug("Cannot Break Door: Cooldown", Config.Debug);
            return;
        }

        var nearbyZombies = Player.List
            .Where(x => x.Role == RoleTypeId.Scp0492 &&
                        (x.Position - ev.Door.Position).sqrMagnitude < Config.MaxDistance &&
                        (!Cooldown.TryGetValue(x, out var cooldown) || cooldown <= Time.time))
            .ToArray();

        if (Config.ZombiesNeeded > nearbyZombies.Length)
        {
            _rateLimit = Time.time + Config.RateLimit;
            ev.Player.SendHint(string.Format(Config.NotEnoughZombiesText, Config.ZombiesNeeded),
                Config.DisplayDuration);
            Logger.Debug("Cannot Break Door: Not Enough Zombies", Config.Debug);
            return;
        }

        ev.IsAllowed = false;

        if (ev.Door is Gate pryableDoor)
        {
            switch (Config.PryableGateModifier)
            {
                case GateModifier.Pry:
                    pryableDoor.TryPry(ev.Player);
                    Logger.Debug("Prying Gate Open");
                    break;
                case GateModifier.OpenThenLock:
                    Open(ev.Door, true);
                    Logger.Debug("Opening and Locking Gate");
                    break;
                case GateModifier.Open:
                    Open(ev.Door);
                    Logger.Debug("Opening Gate");
                    break;
                case GateModifier.Nothing:
                    Logger.Debug("Doing nothing as config is set to nothing");
                    break;
            }
        }
        else if (ev.Door.Base is IDamageableDoor damageableDoor)
        {
            switch (Config.BreakableDoorModifier)
            {
                case DoorModifier.Break:
                    damageableDoor.ServerDamage(damageableDoor.RemainingHealth + 1, DoorDamageType.ServerCommand);
                    Logger.Debug("Destroying Door");
                    break;
                case DoorModifier.OpenThenLock:
                    Open(ev.Door, true);
                    Logger.Debug("Opening & Locking Door");
                    break;
                case DoorModifier.Open:
                    Open(ev.Door);
                    Logger.Debug("Opening Door");
                    break;
                case DoorModifier.Nothing:
                    Logger.Debug("Doing nothing as config is set to nothing");
                    break;
            }
        }

        var newCooldown = Time.time + Config.AbilityCooldown;
        foreach (var player in nearbyZombies)
        {
            player.SendHint(Config.OnBreakDoorText, Config.DisplayDuration);
            Cooldown[player] = newCooldown;
        }
    }

    private static void Open(Door door, bool shouldLock = false)
    {
        door.IsOpened = true;

        if (!shouldLock)
            return;

        door.Lock(DoorLockReason.AdminCommand, true);

        if (Config.UnlockAfterSeconds <= 0) return;

        var routine = Timing.CallDelayed(Config.UnlockAfterSeconds,
            () => { door.Lock(DoorLockReason.AdminCommand, false); });

        Coroutines.Add(routine);
        Logger.Debug("Adding coroutine for Unlocking Doors after Opening and Locking Door", Config.Debug);
    }
}