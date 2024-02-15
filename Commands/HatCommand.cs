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
    using SCPCosmetics.Cosmetics.Hats;
    using PlayerRoles.FirstPersonControl;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class HatCommand : ParentCommand
    {
        public HatCommand() => LoadGeneratedCommands();

        public override string Command { get; } = "hat";

        public override string[] Aliases { get; } = { "hats" };

        public override string Description { get; } = "Allows players to wear an item as a hat";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Hat.RemoveHatCommand());
            RegisterCommand(new Hat.HatListCommand());
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
                response = "Hats are disabled on this server.";
                return false;
            }

            if (!sender.CheckPermission("scpcosmetics.hat"))
            {
                response = "You do not have access to the Hat command!";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage: .hat <item> OR .hat off/disable/remove/list | Example: .hat scp268";
                return false;
            }

            string arg = arguments.At(0);

            Player player = Player.Get(((PlayerCommandSender)sender).ReferenceHub);

            if (HatsHandler.ShouldRemoveHat(player.Role.Type))
            {
                response = "Please wait until you spawn in as a normal class.";
                return false;
            }

            if (HatsHandler.defaultHatNames.TryGetValue(arg, out var itemType))
            {
                response = $"Set hat successfully to {arg}.";
                HatsHandler.SpawnItemHat(player, new HatInfo(itemType), true);

                return true;
            }
            else if (!Plugin.Instance.Config.SchematicHats)
            {
                goto FailedToFind;
            }

            foreach (KeyValuePair<string, SchematicHatConfig> schemHatConf in Plugin.Instance.Config.SchematicHatList)
            {
                bool missingPerm = false;

                if (schemHatConf.Value.RequiredPermissions != null)
                {
                    foreach (string permCheck in schemHatConf.Value.RequiredPermissions)
                    {
                        if (!sender.CheckPermission(permCheck))
                        {
                            missingPerm = true;
                        }
                    }
                }

                if (!missingPerm)
                {
                    foreach (string hatName in schemHatConf.Value.HatNames)
                    {
                        if (hatName == arg)
                        {
                            response = $"Set hat successfully to {arg}.";
                            HatsHandler.SpawnSchematicHat(player, new SchematicHatInfo(schemHatConf.Value.SchematicName, schemHatConf.Value.Scale, schemHatConf.Value.Position, schemHatConf.Value.Rotation), true);

                            return true;
                        }
                    }
                }
            }

            FailedToFind:
            response = "Couldn't find a hat with this name.";
            return false;
        }
    }
}
