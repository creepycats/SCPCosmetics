namespace SCPCosmetics.Commands.Pet
{
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using Mirror;
    using SCPCosmetics.Cosmetics.Pets;
    using SCPCosmetics.Types;
    using System;
    using UnityEngine;

    public class PetClearCommand : ICommand
    {
        public string Command => "clear";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Clears all pets.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Plugin.Instance.Config.EnablePets)
            {
                response = "Pets are disabled on this server.";
                return false;
            }

            if (!sender.CheckPermission("scpcosmetics.staff.clear"))
            {
                response = "You do not have access to the pet clear command!";
                return false;
            }

            PetsHandler thisHandler = Plugin.Instance.GetCosmeticHandler(typeof(PetsHandler)) as PetsHandler;
            foreach (Player plr in Player.List)
            {
                if (plr.GameObject.TryGetComponent(out PetComponent oldComponent))
                {
                    UnityEngine.Object.Destroy(oldComponent);
                }
                if (thisHandler.PlayerLinkedCosmetics.ContainsKey(plr.UserId))
                {
                    thisHandler.PlayerLinkedCosmetics.Remove(plr.UserId);
                }
            }

            response = $"Cleared Pets";
            return true;
        }
    }
}
