namespace SCPCosmetics.Commands.Pet
{
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using PlayerRoles;
    using RemoteAdmin;
    using SCPCosmetics.Cosmetics.Pets;
    using System;

    public class RemovePetCommand : ICommand
    {
        public string Command => "remove";

        public string[] Aliases => new string[]
        {
            "off",
            "disable"
        };

        public string Description => "Removes your existing pet.";

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
                UnityEngine.Object.Destroy(petComp);
                response = "Removed pet successfully.";
                return true;
            }

            response = "Couldn't find your pet. Maybe you don't have one spawned in.";
            return false;
        }
    }
}
