namespace SCPCosmetics.Commands
{
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using SCPCosmetics.Types;
    using RemoteAdmin;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using SCPCosmetics.Types.Glows;
    using SCPCosmetics.Cosmetics.Glows;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class GlowCommand : ParentCommand
    {
        public GlowCommand() => LoadGeneratedCommands();

        public override string Command { get; } = "glow";

        public override string[] Aliases { get; } = { };

        public override string Description { get; } = "Gives players a glowing effect at their feet.";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Glow.RemoveGlowCommand());
            RegisterCommand(new Glow.ClassGlowCommand());
            RegisterCommand(new Glow.RainbowGlowCommand());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not PlayerCommandSender)
            {
                response = "This command can only be ran by a player!";
                return true;
            }

            if (!Plugin.Instance.Config.EnableHats)
            {
                response = "Glows are disabled on this server.";
                return false;
            }

            if (!sender.CheckPermission("scpcosmetics.glow"))
            {
                response = "You do not have access to the Glow command!";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage: .glow <hex-color> OR .glow off/disable/class | Example: .glow #34b4eb";
                return false;
            }

            string arg = arguments.At(0);

            var player = Player.Get(((PlayerCommandSender)sender).ReferenceHub);

            if (GlowsHandler.ShouldRemoveGlow(player.Role.Type))
            {
                response = "Please wait until you spawn in as a normal class.";
                return false;
            }

            if (ColorUtility.TryParseHtmlString(arg, out Color newCol))
            {
                if (player.GameObject.TryGetComponent(out GlowComponent cosmeticComponent))
                {
                    cosmeticComponent.GlowLight.Color = newCol;
                    cosmeticComponent.ColorMode = GlowColorMode.Color;
                }
                else
                {
                    cosmeticComponent = GlowsHandler.SpawnGlow(player, newCol);
                    cosmeticComponent.ColorMode = GlowColorMode.Color;
                }
                response = $"Set Glow successfully to {arg}.";
                return true;
            }

            response = "Invalid Glow color! Must be hex color format";
            return false;
        }
    }
}
