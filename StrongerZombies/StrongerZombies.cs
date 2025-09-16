using LabApi.Features;
using LabApi.Loader.Features.Plugins;
using StrongerZombies.Handlers;
using Version = System.Version;

namespace StrongerZombies;

public class StrongerZombies : Plugin<BalanceSettings>
{
    public override string Name => "StrongerZombies";
    public override string Author => "Beryl, Vicious Vikki";
    public override string Description => " Allows SCP-049-2 to open doors that would normally be locked to SCPs";
    public override Version Version => new(1, 0, 0);
    public override Version RequiredApiVersion => LabApiProperties.CurrentVersion;

    public static StrongerZombies Instance { get; private set; }

    public override void Enable()
    {
        Instance = this;
        ZombieHandler.RegisterEvents();
    }

    public override void Disable()
    {
        ZombieHandler.UnregisterEvents();
    }
}