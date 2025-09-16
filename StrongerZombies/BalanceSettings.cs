using System.ComponentModel;
using StrongerZombies.Handlers;

namespace StrongerZombies;

public class BalanceSettings
{
    [Description("If the plugin should send out debug prints (There's a lot covering basically every case)")]
    public bool Debug { get; set; } = false;

    [Description("Sets the Door Open Modifier, Default is OpenThenLock, there is also Open, Break, and Nothing")]
    public DoorModifier BreakableDoorModifier { get; set; } = DoorModifier.OpenThenLock;

    [Description("Sets the Gate Open Modifier, Default is Pry, there is also Open, OpenThenLock, and Nothing")]
    public GateModifier PryableGateModifier { get; set; } = GateModifier.Pry;

    [Description("Set the required amount of Zombies to Open/Break the door")]
    public int ZombiesNeeded { get; set; } = 5;

    [Description("The cooldown between door opening/breaks")]
    public float AbilityCooldown { get; set; } = 24f;

    [Description("The distance you need to be to the door to open/break it")]
    public float MaxDistance { get; set; } = 4.35f * 4.35f;

    [Description("If the config for BreakbableDoorModifier is OpenThenLock, this gets how long the door should be locked for")]
    public float UnlockAfterSeconds { get; set; } = 3;

    [Description("How often the check is ran between attempts to open/break the door")]
    public float RateLimit { get; set; } = 2.5f;

    [Description("Sets the duration that the broadcasts should be shown to users")]
    public ushort DisplayDuration { get; set; } = 5;

    [Description("Sets the text that will be shown to the users if there is not enough zombies to open/break the door")]
    public string NotEnoughZombiesText { get; set; } = "<color=red>There is not enough zombies for this ability! You need {0} zombies to open this door</color>";

    [Description("Sets the text that will be shown to users if their ability is on cooldown")]
    public string OnCooldownText { get; set; } = "<color=red>This ability is currently on cooldown!</color>";
    
    [Description("Message shown to all zombies who assisted in opening the door")]
    public string OnBreakDoorText { get; set; } = "<color=red>You helped open a door, your ability is now on cooldown!</color>";
}