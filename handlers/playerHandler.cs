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

        public List<Types.HatItemComponent> HatItems { get; set; } = new List<Types.HatItemComponent>();

        public void Spawned(SpawnedEventArgs args)
        {
            // Just a test method for spawning in a Hat
            HatItems.Add(Hats.SpawnHat(args.Player, new HatInfo(ItemType.SCP268), true));
        }

        // For Some Reason, SCPSL and Exiled right now dont want to work with Locked items in fakeSyncVars
        public void SearchingPickup(SearchingPickupEventArgs args) {
            foreach (HatItemComponent hatItem in HatItems)
            {
                if (hatItem.item.gameObject == null) {
                    HatItems.Remove(hatItem);
                    continue;
                };
                
                if (args.Pickup.GameObject == hatItem.item.gameObject) 
                {
                    var pickupInfo = hatItem.item.NetworkInfo;
                    pickupInfo.Locked = true;
                    hatItem.item.NetworkInfo = pickupInfo;
                    args.IsAllowed = false; // Just in case
                }
            }
        }
    }
}
