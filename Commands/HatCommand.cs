using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PluginAPI.Commands;
using RemoteAdmin;
using SCPHats.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SCPHats.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class HatCommand : ICommand
    {
        public string Command { get; } = "hat";

        public string[] Aliases { get; } = { };

        public string Description { get; } = "Allows supporters to wear an item as a hat";

        private static Dictionary<string, ItemType> items = new Dictionary<string, ItemType>()
        {
            {"hat", ItemType.SCP268},
            {"268", ItemType.SCP268},
            {"scp268", ItemType.SCP268},
            {"scp-268", ItemType.SCP268},
            {"pill", ItemType.SCP500},
            {"pills", ItemType.SCP500},
            {"scp500", ItemType.SCP500},
            {"500", ItemType.SCP500},
            {"scp-500", ItemType.SCP500},
            {"coin", ItemType.Coin},
            {"quarter", ItemType.Coin},
            {"dime", ItemType.Coin},
            {"ball", ItemType.SCP018},
            {"scp018", ItemType.SCP018},
            {"scp18", ItemType.SCP018},
            {"scp-018", ItemType.SCP018},
            {"scp-18", ItemType.SCP018},
            {"018", ItemType.SCP018},
            {"18", ItemType.SCP018},
            {"medkit", ItemType.Medkit},
            {"adrenaline", ItemType.Adrenaline},
            {"soda", ItemType.SCP207},
            {"cola", ItemType.SCP207},
            {"coke", ItemType.SCP207},
            {"207", ItemType.SCP207},
            {"scp207", ItemType.SCP207},
            {"scp-207", ItemType.SCP207},
            {"butter", ItemType.KeycardScientist}
        };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender))
            {
                response = "This command can only be ran by a player!";
                return true;
            }

            if (!sender.CheckPermission("scphats.hat") && !sender.CheckPermission("scphats.hats")) {
                response = "You do not have access to the Hat command!";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage: .hat <item> <show self?> OR .hat off/disable/remove | Example: .hat scp268 false";
                return false;
            }

            var player = Player.Get(((PlayerCommandSender)sender).ReferenceHub);

            if (arguments.Array[arguments.Offset + 0] == "off" || arguments.Array[arguments.Offset + 0] == "disable" || arguments.Array[arguments.Offset + 0] == "remove") {
                List<Types.HatItemComponent> HatItems = SCPHats.Instance.HatItems;
                HatItemComponent _foundHat = null;
                foreach (HatItemComponent HatItem in HatItems)
                {
                    if (Player.Get(HatItem.player.gameObject) == player) _foundHat = HatItem;
                }
                if (_foundHat != null)
                {
                    HatItems.Remove(_foundHat);
                    UnityEngine.Object.Destroy(_foundHat.item.gameObject);
                    SCPHats.Instance.HatItems = HatItems;
                    response = "Removed hat successfully.";
                } else
                {
                    response = "Couldn't find your hat. Maybe you don't have one on.";
                }
                return true;
            } else
            {
                if (items.TryGetValue(arguments.Array[arguments.Offset + 0], out var itemType))
                {
                    List<Types.HatItemComponent> HatItems = SCPHats.Instance.HatItems;
                    response = $"Set hat successfully to {arguments.Array[arguments.Offset + 0]}.";
                    if (arguments.Array.Length >= 2 && arguments.Array[arguments.Offset + 1] == "true") {
                        HatItems.Add(Hats.SpawnHat(player, new HatInfo(itemType), true));
                    } else {
                        HatItems.Add(Hats.SpawnHat(player, new HatInfo(itemType), false));
                        response += " NOTE: You currently have your hat display set to off for yourself.";
                    };
                    SCPHats.Instance.HatItems = HatItems;
                    return true;
                }
                else {
                    response = "Couldn't find a hat with this name.";
                    return false;
                }
            }

            response = "Something is very wrong. Let me know if you somehow get this result.";
            return false;
        }
    }
}
