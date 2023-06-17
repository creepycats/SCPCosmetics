using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using Mirror;
using RemoteAdmin;
using SCPHats.Types;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SCPHats.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class HatCommand : ICommand
    {
        public string Command { get; } = "hat";

        public string[] Aliases { get; } = { "hats" };

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

            if ((!sender.CheckPermission("scphats.hat") && !sender.CheckPermission("scphats.hats")) && (!sender.CheckPermission("scpstats.hat") && !sender.CheckPermission("scpstats.hats"))) {
                response = "You do not have access to the Hat command!";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage: .hat <item> OR .hat off/disable/remove/list | Example: .hat scp268";
                return false;
            }

            var player = Player.Get(((PlayerCommandSender)sender).ReferenceHub);

            if ((player.Role.Type == PlayerRoles.RoleTypeId.None || player.Role.Type == PlayerRoles.RoleTypeId.Spectator || player.Role.Type == PlayerRoles.RoleTypeId.Overwatch || player.Role.Type == PlayerRoles.RoleTypeId.Filmmaker) && arguments.Array[arguments.Offset + 0] != "debug") {
                response = "Please wait until you spawn in as a normal class.";
                return false;
            }

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
                    if (_foundHat.isSchematic)
                    {
                        _foundHat.hatSchematic.Destroy();
                        _foundHat.isSchematic = false;
                    }
                    //UnityEngine.Object.Destroy(_foundHat.item.gameObject);
                    NetworkServer.Destroy(_foundHat.item.gameObject);
                    SCPHats.Instance.HatItems = HatItems;
                    response = "Removed hat successfully.";
                } else
                {
                    response = "Couldn't find your hat. Maybe you don't have one on.";
                }
                return true;
            } else if (arguments.Array[arguments.Offset + 0] == "list")
            {
                response = "Available Hats: \n";
                foreach (KeyValuePair<string, ItemType> entry in items)
                {
                    // do something with entry.Value or entry.Key
                    response += $"{entry.Key} \n";
                }
                if (SCPHats.Instance.Config.SchematicHats) {
                    foreach (SchematicHatConfig schemHatConf in SCPHats.Instance.Config.SchematicHatList)
                    {   
                        foreach (string HatName in schemHatConf.HatNames)
                        {
                           response += $"{HatName} \n";
                       }
                    }
                }
                return false;
            }
            else if (arguments.Array[arguments.Offset + 0] == "debug")
            {
                response = $"Hat Debug \n Number of Hats In Play: {SCPHats.Instance.HatItems.Count} \n Players With Hats: \n";
                
                foreach (HatItemComponent hatItem in SCPHats.Instance.HatItems)
                {
                    response += $"{Player.Get(hatItem.player.gameObject).Nickname} - {Player.Get(hatItem.player.gameObject).Id} - {Player.Get(hatItem.player.gameObject).UserId}";
                }
                return false;
            }
            else
            {
                if (items.TryGetValue(arguments.Array[arguments.Offset + 0], out var itemType))
                {
                    List<Types.HatItemComponent> HatItems = SCPHats.Instance.HatItems;
                    response = $"Set hat successfully to {arguments.Array[arguments.Offset + 0]}.";
                    HatItems.Add(Hats.SpawnHat(player, new HatInfo(itemType), true));
                    SCPHats.Instance.HatItems = HatItems;
                    return true;
                }
                else if (SCPHats.Instance.Config.SchematicHats) {
                    foreach (SchematicHatConfig schemHatConf in SCPHats.Instance.Config.SchematicHatList) {
                        foreach (string hatName in schemHatConf.HatNames)
                        {
                            if (hatName == arguments.Array[arguments.Offset + 0]) {
                                List<Types.HatItemComponent> HatItems = SCPHats.Instance.HatItems;
                                response = $"Set hat successfully to {arguments.Array[arguments.Offset + 0]}.";
                                HatItems.Add(Hats.SpawnHat(player, new SchematicHatInfo(schemHatConf.SchematicName, schemHatConf.Scale, schemHatConf.Position, schemHatConf.Rotation), true));
                                SCPHats.Instance.HatItems = HatItems;
                                return true;
                            }
                        }
                    }
                    response = "Couldn't find a hat with this name.";
                } 
                else  {
                    response = "Couldn't find a hat with this name.";
                    return false;
                }
            }

            response = "Something is very wrong. Let me know if you somehow get this result.";
            return false;
        }
    }
}
