namespace SCPCosmetics.Commands.Pet
{
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using Mirror;
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

            response = $"Cleared {Plugin.Instance.PetDictionary.Count} Pets";
            foreach (string petId in Plugin.Instance.PetDictionary.Keys)
            {
                Plugin.Instance.PetDictionary[petId].ClearInventory();
                Plugin.Instance.PetDictionary[petId].GameObject.GetComponent<PetComponent>().stopRunning = true;
                Plugin.Instance.PetDictionary[petId].Position = new Vector3(-9999f, -9999f, -9999f);
                NetworkServer.Destroy(Plugin.Instance.PetDictionary[petId].GameObject);
            }
            Plugin.Instance.PetDictionary.Clear();
            return true;
        }
    }
}
