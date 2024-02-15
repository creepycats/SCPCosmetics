namespace SCPCosmetics.Commands.Hat
{
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;
    using SCPCosmetics.Cosmetics.Hats;
    using SCPCosmetics.Types;
    using System;

    public class RemoveHatCommand : ICommand
    {
        public string Command => "remove";

        public string[] Aliases => new string[]
        {
            "off",
            "disable"
        };

        public string Description => "Removes your existing hat.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Player.TryGet(sender, out Player player))
            {
                response = "This command can only be ran by a player!";
                return true;
            }

            if (!Plugin.Instance.Config.EnableHats)
            {
                response = "Hats are disabled on this server.";
                return false;
            }

            if (!sender.CheckPermission("scpcosmetics.hat"))
            {
                response = "You do not have access to the Hat command!";
                return false;
            }

            player.GameObject.TryGetComponent(out HatComponent cosmeticComponent);
            bool result = cosmeticComponent != null;

            if (result)
                UnityEngine.Object.Destroy(cosmeticComponent);

            response = result
                ? "Removed hat successfully."
                : "Couldn't find your hat. Maybe you don't have one on.";
            return result;
        }
    }
}
