namespace SCPCosmetics.Commands.Pet
{
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using PlayerRoles;
    using System;
    using UnityEngine;

    public class SpawnPetCommand : ICommand
    {
        public string Command => "spawn";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Spawns in a pet.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Player.TryGet(sender, out Player player))
            {
                response = "This command can only be ran by a player!";
                return true;
            }

            if (!Plugin.Instance.Config.EnablePets)
            {
                response = "Pets are disabled on this server.";
                return false;
            }

            if (!sender.CheckPermission("scpcosmetics.pet"))
            {
                response = "You do not have access to the Pet command!";
                return false;
            }

            if (player.Role.Team == Team.Dead)
            {
                response = "Please wait until you spawn in as a normal class.";
                return false;
            }

            if (Plugin.Instance.CheckPetRateLimited(player.Id))
            {
                response = "You are ratelimited.";
                return false;
            }

            Plugin.Instance.PetRateLimitPlayer(player.Id, 3d);

            if (Plugin.Instance.PetDictionary.ContainsKey($"pet-{player.UserId}"))
            {
                response = "You already have a pet!";
                return true;
            }

            Pets.SpawnPet("", "default", player.Role.Type, ItemType.None, player, new Vector3(0.5f, 0.5f, 0.5f));
            response = "Spawned in your pet.";
            return true;
        }
    }
}
