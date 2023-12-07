namespace SCPCosmetics.Commands.Glow
{
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;
    using SCPCosmetics.Types;
    using SCPCosmetics.Types.Glows;
    using System;
    using UnityEngine;

    public class RainbowGlowCommand : ICommand
    {
        public string Command => "rainbow";

        public string[] Aliases => new string[]
        {
            "rgb"
        };

        public string Description => "Sets your Glow color to RGB GAMER MODE";

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

            if (Glows.ShouldRemoveGlow(player.Role.Type))
            {
                response = "Please wait until you spawn in as a normal class.";
                return false;
            }

            if (Plugin.Instance.GlowDictionary.TryGetValue(player.UserId, out GlowComponent glowComp) && glowComp != null && glowComp.gameObject != null)
            {
                glowComp.glowLight.Color = Color.red;
                glowComp.reflectClass = GlowColorMode.Rainbow;
            }
            else
            {
                glowComp = Glows.SpawnGlow(player, Color.red);
                glowComp.reflectClass = GlowColorMode.Rainbow;
            }
            response = $"Set Glow successfully to Rainbow.";
            return true;
        }
    }
}
