using System;
using Exiled.API.Interfaces;

using static StrongerZombies.Handlers.ZombieHandler;

namespace StrongerZombies
{
    public class BalanceSettings : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        #region Balance Tweaks
        public DoorModifier BreakableDoorModifier { get; set; } = DoorModifier.OpenThenLock;

        public int ZombiesNeeded { get; set; } = 5;

        public float AbilityCooldown { get; set; } = 24f;
        public float MaxDistance { get; set; } = 4.35f * 4.35f;
        public float UnlockAfterSeconds { get; set; } = 3;
        public float RateLimit { get; set; } = 2.5f;
        public ushort DisplayDuration { get; set; } = 5;
        #endregion

        /*#region Broadcasts
        public Exiled.API.Features.Broadcast NotEnoughZombies { get; set; } =
            new Exiled.API.Features.Broadcast("<color=red>There isn't enough zombies for this ability!\nYou need {zombiecount} to open this door</color>", 5);

        public Exiled.API.Features.Broadcast OnCooldown { get; set; } =
            new Exiled.API.Features.Broadcast("<color=red>This ability is currently on cooldown!\n{duration} remains before next use</color>", 5);
        #endregion*/
        public string NotEnoughZombiesText { get; set; } = "<color=red>There is not enough zombies for this ability! You need {zombiecount} zombies to open this door</color>";
        public string OnCooldownText { get; set; } = "<color=red>This ability is currently on cooldown!</color>";
    }
}
