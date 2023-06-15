using Exiled.API.Features;
using Exiled.Events;
using Exiled.Events.EventArgs;
using Exiled.Events.EventArgs.Player;
using Exiled.API.Features.Roles;
using System.IO;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using MEC;
using SCPHats.Types;

namespace SCPHats.handlers
{
    public class playerHandler
    {
        private List<Player> Players = new List<Player>();
        private readonly Config.Config config;

        public playerHandler(Config.Config config) => this.config = config;
        public void Spawned(SpawnedEventArgs args)
        {
            // Just a test method for spawning in a Hat
            Hats.SpawnHat(args.Player, new HatInfo(ItemType.SCP268), true);
        }
    }
}
