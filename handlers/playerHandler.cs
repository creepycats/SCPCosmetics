using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;
using SCPHats.Types;
using Mirror;

namespace SCPHats.handlers
{
    public class playerHandler
    {
        private List<Player> Players = new List<Player>();
        private readonly Config.Config config;

        public playerHandler(Config.Config config) => this.config = config;

        public List<Types.HatItemComponent> HatItems { get; set; } = new List<Types.HatItemComponent>();

        public void ChangingRole(ChangingRoleEventArgs args)
        {
            if (args.Player.Role.Type == PlayerRoles.RoleTypeId.None || args.Player.Role.Type == PlayerRoles.RoleTypeId.Spectator || args.Player.Role.Type == PlayerRoles.RoleTypeId.Overwatch || args.Player.Role.Type == PlayerRoles.RoleTypeId.Filmmaker) {
                List<Types.HatItemComponent> HatItems = SCPHats.Instance.HatItems;
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
                    SCPHats.Instance.HatItems = HatItems;
                }
            }
        }

        public void Died(DiedEventArgs args)
        {
            List<Types.HatItemComponent> HatItems = SCPHats.Instance.HatItems;
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
                SCPHats.Instance.HatItems = HatItems;
            }
        }

        // For Some Reason, SCPSL and Exiled right now dont want to work with Locked items in fakeSyncVars
        public void SearchingPickup(SearchingPickupEventArgs args) {
            List<Types.HatItemComponent> HatItems = SCPHats.Instance.HatItems;
            HatItems.RemoveAll(hatItem => hatItem == null);
            SCPHats.Instance.HatItems = HatItems;
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
    }
}
