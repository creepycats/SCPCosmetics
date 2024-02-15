namespace SCPCosmetics.Commands.Pet
{
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using PlayerRoles;
    using SCPCosmetics.Cosmetics.Pets;
    using System;
    using System.Linq;
    using static Misc;

    public class PetNameCommand : ICommand
    {
        public string Command => "name";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Name your pet!";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Player.TryGet(sender, out Player player))
            {
                response = "This command can only be ran by a player!";
                return true;
            }

            // Name pets config check.
            if (!Plugin.Instance.Config.NamePets)
            {
                response = "Players cannot set pet names on this server!";
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

            // Arguments check.
            if (arguments.Count < 2)
            {
                response = "Usage: .pet name <color> <name>";
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

            if (Enum.TryParse(arguments.At(0), true, out PlayerInfoColorTypes rankColor))
            {
                string checkName = string.Join(" ", arguments.Skip(1));
                if (checkName.Length < 25)
                {
                    petNpc.RankName = checkName;
                    petNpc.RankColor = arguments.At(0);
                    response = $"Set pet's name to '{string.Join(" ", arguments.Skip(1))}' with color '{arguments.At(0)}' - REMEMBER, YOU WILL BE HELD ACCOUNTABLE FOR INAPPROPRIATE NAMES";
                    return true;
                }
                response = "Pet name should be under 25 characters!";
                return false;
            }
            else
            {
                response = "Couldn't find an allowed color with this name.";
                return false;
            }
        }
    }
}
