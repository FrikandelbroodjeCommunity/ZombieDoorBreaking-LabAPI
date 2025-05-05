using System;

using Exiled.API.Features;

using StrongerZombies.Handlers;

namespace StrongerZombies
{
    public class StrongerZombies : Plugin<BalanceSettings>
    {
        public override string Name { get; } = "StrongerZombies";
        public override string Author { get; } = "Beryl, Vicious Vikki";
        public override string Prefix { get; } = "strongerzombies";
        public override Version Version { get; } = new Version(2, 2, 0);
        public override Version RequiredExiledVersion => new Version(9, 6, 0);

        private ZombieHandler _handlers;

        public override void OnEnabled()
        {
            Log.Debug("Initializing any event handlers...");
            _handlers = new ZombieHandler(this);

            _handlers.Subscribe();
        }

        public override void OnDisabled()
        {
            _handlers.Unsubscribe();

            _handlers = null;
        }
    }
}