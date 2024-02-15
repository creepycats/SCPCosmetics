namespace SCPCosmetics.Commands.Glow
{
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using SCPCosmetics.Cosmetics.Glows;
    using SCPCosmetics.Types.Glows;
    using System;
    using UnityEngine;

    public class ClassGlowCommand : ICommand
    {
        public string Command => "class";

        public string[] Aliases => new string[]
        {
            "default"
        };

        public string Description => "Sets your Glow color to reflect your class color";

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

            if (GlowsHandler.ShouldRemoveGlow(player.Role.Type))
            {
                response = "Please wait until you spawn in as a normal class.";
                return false;
            }

            if (player.GameObject.TryGetComponent(out GlowComponent cosmeticComponent))
            {
                cosmeticComponent.GlowLight.Color = Color.gray;
                cosmeticComponent.ColorMode = GlowColorMode.Class;
            }
            else
            {
                cosmeticComponent = GlowsHandler.SpawnGlow(player, Color.gray);
                cosmeticComponent.ColorMode = GlowColorMode.Class;
            }
            response = $"Set Glow successfully to Reflect Class.";
            return true;
        }
    }
}
