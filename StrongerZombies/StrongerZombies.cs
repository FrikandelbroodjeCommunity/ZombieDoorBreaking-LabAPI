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
        public override string Author { get; } = "Beryl, Vicious Vikki";
        public override string Prefix { get; } = "strongerzombies";
        public override Version Version { get; } = new Version(2, 1, 2);
        public override Version RequiredExiledVersion => new Version(8, 8, 1);

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