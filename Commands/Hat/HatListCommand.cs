namespace SCPCosmetics.Commands.Hat
{
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using SCPCosmetics.Types;
    using NorthwoodLib.Pools;
    using RemoteAdmin;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using SCPCosmetics.Cosmetics.Hats;

    public class HatListCommand : ICommand
    {
        public string Command => "list";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Shows the list of hats available to the player.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
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

            StringBuilder builder = StringBuilderPool.Shared.Rent();

            builder.AppendLine("Available Hats:");

            foreach (KeyValuePair<string, ItemType> entry in HatsHandler.defaultHatNames)
            {
                builder.AppendLine(entry.Key);
            }

            if (Plugin.Instance.Config.SchematicHats)
            {
                foreach (SchematicHatConfig schemHatConf in Plugin.Instance.Config.SchematicHatList.Values)
                {
                    if (schemHatConf.RequiredPermissions is null)
                        goto SkipPermCheck;

                    bool missingPerm = false;

                    foreach (string permCheck in schemHatConf.RequiredPermissions)
                    {
                        if (!sender.CheckPermission(permCheck))
                        {
                            missingPerm = true;
                            break;
                        }
                    }

                    if (missingPerm)
                        continue;

                    SkipPermCheck:
                    foreach (string hatName in schemHatConf.HatNames)
                    {
                        builder.AppendLine(hatName);
                    }
                }
            }

            response = StringBuilderPool.Shared.ToStringReturn(builder);
            return true;
        }
    }
}
