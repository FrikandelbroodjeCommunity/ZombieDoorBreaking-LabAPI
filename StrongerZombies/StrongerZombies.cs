using System;
using System.Collections.Generic;
using System.Linq;

using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events;

using MEC;

using StrongerZombies.Handlers;

namespace StrongerZombies
{
    public class StrongerZombies : Plugin<BalanceSettings>
    {
        public override string Name { get; } = "StrongerZombies";
        public override string Author { get; } = "Beryl";
        public override string Prefix { get; } = "strongerzombies";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(4, 2, 3);

        private ZombieHandler handlers;

        public override void OnEnabled()
        {
            Log.Debug("Initializing any event handlers...");
            handlers = new ZombieHandler(this);

            handlers.Subscribe();
        }

        public override void OnDisabled()
        {
            handlers.Unsubscribe();

            handlers = null;
        }
    }
}