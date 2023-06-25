using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;
using SCPCosmetics.Types;
using Mirror;
using PlayerRoles;
using System;

namespace SCPCosmetics.handlers
{
    public class playerHandler
    {
        private List<Player> Players = new List<Player>();
        private readonly Config.Config config;

        public playerHandler(Config.Config config) => this.config = config;

        public List<Types.HatItemComponent> HatItems { get; set; } = new List<Types.HatItemComponent>();

        public void ChangingRole(ChangingRoleEventArgs args)
        {
            try
            {
                if (args.Player.SessionVariables.TryGetValue("npc", out object isNpc) != true)
                {
                    if (Hats.ShouldRemoveHat(args.Player.Role.Type))
                    {
                        List<Types.HatItemComponent> HatItems = SCPCosmetics.Instance.HatItems;
                        HatItemComponent _foundHat = null;
                        foreach (HatItemComponent HatItem in HatItems)
                        {
                            if (Player.Get(HatItem.player.gameObject) == args.Player) _foundHat = HatItem;
                        }
                        if (_foundHat != null)
                        {
                            HatItems.Remove(_foundHat);
                            if (_foundHat.isSchematic)
                            {
                                _foundHat.hatSchematic.Destroy();
                                _foundHat.isSchematic = false;
                            }
                            NetworkServer.Destroy(_foundHat.item.gameObject);
                            SCPCosmetics.Instance.HatItems = HatItems;
                        }
                    }
                }
            } catch (Exception err) {
                if (SCPCosmetics.Instance.Config.Debug) Log.Error(err);
            }
        }

        public void Died(DiedEventArgs args)
        {
            try {
                if (SCPCosmetics.Instance.Config.RemoveHatsOnDeath)
                {
                    List<Types.HatItemComponent> HatItems = SCPCosmetics.Instance.HatItems;
                    HatItemComponent _foundHat = null;
                    foreach (HatItemComponent HatItem in HatItems)
                    {
                        if (Player.Get(HatItem.player.gameObject) == args.Player) _foundHat = HatItem;
                    }
                    if (_foundHat != null)
                    {
                        HatItems.Remove(_foundHat);
                        if (_foundHat.isSchematic)
                        {
                            _foundHat.hatSchematic.Destroy();
                            _foundHat.isSchematic = false;
                        }
                        NetworkServer.Destroy(_foundHat.item.gameObject);
                        SCPCosmetics.Instance.HatItems = HatItems;
                    }
                }
            } catch (Exception err) {
                if (SCPCosmetics.Instance.Config.Debug) Log.Error(err);
            }
        }

        // For Some Reason, SCPSL and Exiled right now dont want to work with Locked items in fakeSyncVars
        public void SearchingPickup(SearchingPickupEventArgs args) {
            try
            {
                List<Types.HatItemComponent> HatItems = SCPCosmetics.Instance.HatItems;
                HatItems.RemoveAll(hatItem => hatItem == null);
                SCPCosmetics.Instance.HatItems = HatItems;
                foreach (HatItemComponent hatItem in HatItems)
                {
                    if (args.Pickup.GameObject == hatItem.item.gameObject)
                    {
                        var pickupInfo = hatItem.item.NetworkInfo;
                        pickupInfo.Locked = true;
                        hatItem.item.NetworkInfo = pickupInfo;
                        args.IsAllowed = false; // Just in case
                    }
                }
            }
            catch (Exception err)
            {
                if (SCPCosmetics.Instance.Config.Debug) Log.Error(err);
            }
        }
    }
}
