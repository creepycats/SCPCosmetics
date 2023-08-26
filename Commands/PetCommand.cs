using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Permissions.Extensions;
using MEC;
using Mirror;
using PlayerRoles;
using RemoteAdmin;
using SCPCosmetics.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SCPCosmetics.Pets;

namespace SCPCosmetics.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class PetCommand : ICommand
    {
        public string Command { get; } = "pet";

        public string[] Aliases { get; } = { "pets" };

        public string Description { get; } = "Allows supporters to have a pet follow them around";

        private static Dictionary<string, ItemType> items = new Dictionary<string, ItemType>()
        {
            {"pill", ItemType.SCP500},
            {"pills", ItemType.SCP500},
            {"scp500", ItemType.SCP500},
            {"500", ItemType.SCP500},
            {"scp-500", ItemType.SCP500},
            {"coin", ItemType.Coin},
            {"quarter", ItemType.Coin},
            {"dime", ItemType.Coin},
            {"medkit", ItemType.Medkit},
            {"adrenaline", ItemType.Adrenaline},
            {"soda", ItemType.SCP207},
            {"cola", ItemType.SCP207},
            {"coke", ItemType.SCP207},
            {"207", ItemType.SCP207},
            {"scp207", ItemType.SCP207},
            {"scp-207", ItemType.SCP207},
            {"keycard", ItemType.KeycardScientist}
        };
        private static List<string> allowedColors = new List<string>() {
            "cyan",
            "aqua",
            "deep_pink",
            "tomato",
            "yellow",
            "magenta",
            "blue_green",
            "orange",
            "lime",
            "green",
            "emerald",
            "carmine",
            "nickel",
            "mint",
            "army_green",
            "pumpkin",
            "pink",
            "red",
            "default",
            "brown",
            "silver",
            "light_green",
            "crimson"
        };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!(sender is PlayerCommandSender))
            {
                response = "This command can only be ran by a player!";
                return true;
            }
            if (!SCPCosmetics.Instance.Config.EnablePets)
            {
                response = "Pets are disabled on this server.";
                return false;
            }

            var player = Player.Get(((PlayerCommandSender)sender).ReferenceHub);
            if (SCPCosmetics.Instance.PetRatelimit.Contains(player.UserId))
            {
                response = "Ratelimited.";
                return false;
            }
            SCPCosmetics.Instance.PetRatelimit.Add(player.UserId);
            Timing.CallDelayed(3f, () =>
            {
                SCPCosmetics.Instance.PetRatelimit.Remove(player.UserId);
            });

            if ((!sender.CheckPermission("scpcosmetics.pet") && !sender.CheckPermission("scpcosmetics.pets")) && (!sender.CheckPermission("scphats.pet") && !sender.CheckPermission("scphats.pets")))
            {
                response = "You do not have access to the Pet command!";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage: .pet off/disable/remove/spawn/name/item/clear | Example: .pet spawn / .pet name <valid-role-color> <name> / .pet item <item>";
                return false;
            }

            if (player.Role.Team == Team.Dead && arguments.At(0) != "debug")
            {
                response = "Please wait until you spawn in as a normal class.";
                return false;
            }

            if (arguments.At(0) == "off" || arguments.At(0) == "disable" || arguments.At(0) == "remove")
            {
                if (SCPCosmetics.Instance.PetDictionary.TryGetValue($"pet-{player.UserId}",out Npc foundPet))
                {
                    foundPet.ClearInventory();
                    foundPet.GameObject.GetComponent<PetComponent>().stopRunning = true;
                    foundPet.Position = new Vector3(-9999f, -9999f, -9999f);
                    Timing.CallDelayed(0.5f, () =>
                    {
                        NetworkServer.Destroy(foundPet.GameObject);
                        SCPCosmetics.Instance.PetDictionary.Remove($"pet-{player.UserId}");
                    });
                    response = "Removed pet successfully.";
                }
                else
                {
                    response = "Couldn't find your pet. Maybe you don't have one spawned in.";
                }
                return true;
            }
            else if (arguments.At(0) == "spawn")
            {
                if (SCPCosmetics.Instance.PetDictionary.TryGetValue($"pet-{player.UserId}", out Npc foundPet))
                {
                    response = "You already have a pet!";
                    return true;
                }
                SpawnPet("", "default", player.Role.Type, ItemType.None, player, new Vector3(0.5f, 0.5f, 0.5f));
                response = "Spawned in your pet.";
                return true;
            }
            else if (arguments.At(0) == "name")
            {
                if (!SCPCosmetics.Instance.PetDictionary.TryGetValue($"pet-{player.UserId}", out Npc petNpc))
                {
                    response = "You don't currently have a pet spawned in!";
                    return true;
                }
                if (!SCPCosmetics.Instance.Config.NamePets)
                {
                    response = "Players cannot set pet names on this server!";
                    return true;
                }
                if (allowedColors.Contains(arguments.At(1)))
                {
                    petNpc.RankName = String.Join(" ", arguments.Skip(2));
                    petNpc.RankColor = arguments.At(1);
                    response = $"Set pet's name to '{String.Join(" ", arguments.Skip(2))}' with color '{arguments.At(1)}' - REMEMBER, YOU WILL BE HELD ACCOUNTABLE FOR INAPPROPRIATE NAMES";
                }
                else
                {
                    response = "Couldn't find an allowed color with this name.";
                }
                return true;
            }
            else if (arguments.At(0) == "item")
            {
                if (!SCPCosmetics.Instance.PetDictionary.TryGetValue($"pet-{player.UserId}", out Npc petNpc))
                {
                    response = "You don't currently have a pet spawned in!";
                    return true;
                }
                if (!SCPCosmetics.Instance.Config.PetsCanHoldItems)
                {
                    response = "Pets cannot hold items on this server!";
                    return true;
                }
                if (items.TryGetValue(arguments.At(1) ,out ItemType HeldItem))
                {
                    if (HeldItem == ItemType.None)
                    {
                        petNpc.ClearInventory();
                    } 
                    else
                    {
                        Timing.CallDelayed(0.5f, () =>
                        {
                            petNpc.ClearInventory();
                            petNpc.CurrentItem = Item.Create(HeldItem, petNpc);
                        });
                    }
                    response = $"Set pet's held item to type '{arguments.At(1)}'";
                }
                else
                {
                    response = "Couldn't find an allowed item with this name. Maybe the item was disabled by server staff.";
                }
                return true;
            }
            else if (arguments.At(0) == "debug")
            {
                response = $"Pet Debug \n Number of Pets In Play: {SCPCosmetics.Instance.PetDictionary.Values.Count} \n Pets in Play: \n";

                foreach (string petId in SCPCosmetics.Instance.PetDictionary.Keys)
                {
                    response += $"{SCPCosmetics.Instance.PetDictionary[petId].RankName} - {SCPCosmetics.Instance.PetDictionary[petId].Nickname} - {SCPCosmetics.Instance.PetDictionary[petId].Id} - {petId}";
                }
                return false;
            }
            else if (arguments.At(0) == "clear")
            {
                if (!sender.CheckPermission("scpcosmetics.staff.clear"))
                {
                    response = "You do not have access to the Clear subcommand!";
                    return false;
                } else
                {
                    response = $"Killed {SCPCosmetics.Instance.PetDictionary.Count} Pets";
                    foreach (string petId in SCPCosmetics.Instance.PetDictionary.Keys)
                    {
                        SCPCosmetics.Instance.PetDictionary[petId].ClearInventory();
                        SCPCosmetics.Instance.PetDictionary[petId].GameObject.GetComponent<PetComponent>().stopRunning = true;
                        SCPCosmetics.Instance.PetDictionary[petId].Position = new Vector3(-9999f, -9999f, -9999f);
                        Timing.CallDelayed(0.5f, () =>
                        {
                            NetworkServer.Destroy(SCPCosmetics.Instance.PetDictionary[petId].GameObject);
                            SCPCosmetics.Instance.PetDictionary.Remove(petId);
                        });
                    }
                    return false;
                }
            } else
            {
                response = "Invalid Subcommand.";
                return false;
            }

            response = "Something is very wrong. Let me know if you somehow get this result.";
            return false;
        }
    }
}
