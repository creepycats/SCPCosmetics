namespace SCPCosmetics.Commands.Pet
{
    using CommandSystem;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using PlayerRoles;
    using SCPCosmetics.Cosmetics.Pets;
    using System;

    public class PetClassCommand : ICommand
    {
        public string Command => "class";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Change your pet's class!";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Player.TryGet(sender, out Player player))
            {
                response = "This command can only be ran by a player!";
                return true;
            }

            // Pet items config check.
            if (!Plugin.Instance.Config.PetClassCommandEnabled)
            {
                response = "Pets cannot be set to another class on this server!";
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

            // Arguments check.
            if (arguments.Count < 1)
            {
                response = "Usage: .pet class <itemtype>";
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

            if (!player.GameObject.TryGetComponent(out PetComponent petComp))
            {
                response = "You don't currently have a pet spawned in!";
                return true;
            }
            Npc petNpc = petComp.PetNPC;

            if (PetsHandler.allowedPetClasses.TryGetValue(arguments.At(0), out RoleTypeId ChoseRole))
            {
                petNpc.Role.Set(ChoseRole, SpawnReason.ForceClass, RoleSpawnFlags.None);

                response = $"Set pet's class to type '{arguments.At(0)}'";
                return true;
            }
            else
            {
                response = "Couldn't find an allowed class with this name. Maybe the class was disabled by server staff.";
                return false;
            }
        }
    }
}
