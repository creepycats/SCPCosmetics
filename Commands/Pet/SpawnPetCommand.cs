namespace SCPCosmetics.Commands.Pet
{
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using PlayerRoles;
    using SCPCosmetics.Cosmetics.Pets;
    using System;

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

            PetsHandler thisHandler = Plugin.Instance.GetCosmeticHandler(typeof(PetsHandler)) as PetsHandler;
            if (thisHandler.CheckPetRateLimited(player.Id))
            {
                response = "You are ratelimited.";
                return false;
            }

            thisHandler.PetRateLimitPlayer(player.Id, 3d);

            if (player.GameObject.TryGetComponent(out PetComponent petComp))
            {
                response = "You already have a pet!";
                return true;
            }

            PetsHandler.SpawnPet("", "default", player.Role.Type, ItemType.None, player, Plugin.Instance.Config.PetScale);
            response = "Spawned in your pet.";
            return true;
        }
    }
}
