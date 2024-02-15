namespace SCPCosmetics.Commands.Glow
{
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using SCPCosmetics.Cosmetics.Glows;
    using System;

    public class RemoveGlowCommand : ICommand
    {
        public string Command => "remove";

        public string[] Aliases => new string[]
        {
            "off",
            "disable"
        };

        public string Description => "Removes your existing glow.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Player.TryGet(sender, out Player player))
            {
                response = "This command can only be ran by a player!";
                return true;
            }

            if (!Plugin.Instance.Config.EnableGlows)
            {
                response = "Glows are disabled on this server.";
                return false;
            }

            if (!sender.CheckPermission("scpcosmetics.glow"))
            {
                response = "You do not have access to the Glow command!";
                return false;
            }

            player.GameObject.TryGetComponent(out GlowComponent cosmeticComponent);
            bool result = cosmeticComponent != null;

            if (result)
                UnityEngine.Object.Destroy(cosmeticComponent);

            response = result
                ? "Removed Glow successfully."
                : "Couldn't find your Glow. Maybe you don't have one on.";
            return result;
        }
    }
}
