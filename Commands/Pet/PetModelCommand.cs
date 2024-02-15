namespace SCPCosmetics.Commands.Pet
{
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using NorthwoodLib.Pools;
    using PlayerRoles;
    using SCPCosmetics.Cosmetics.Extensions;
    using SCPCosmetics.Cosmetics.Pets;
    using SCPCosmetics.Types;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class PetModelCommand : ICommand
    {
        public string Command => "model";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Use a custom model for your pet.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Player.TryGet(sender, out Player player))
            {
                response = "This command can only be ran by a player!";
                return true;
            }

            // Pet items config check.
            if (!Plugin.Instance.Config.SchematicPets)
            {
                response = "Custom Model Pets are not supported on this server.";
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

            // Arguments check.
            if (arguments.Count == 0)
            {
                response = "Usage: .pet model <name> OR .pet model list";
            }

            if (player.Role.Team == Team.Dead)
            {
                response = "Please wait until you spawn in as a normal class.";
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

            if (arguments.At(0) == "list")
            {
                StringBuilder builder = StringBuilderPool.Shared.Rent();

                builder.AppendLine("Available Pet Models:");

                foreach (SchematicPetConfig schemPetConf in Plugin.Instance.Config.SchematicPetList.Values)
                {
                    if (schemPetConf.RequiredPermissions is null)
                        goto SkipPermCheck;

                    bool missingPerm = false;

                    foreach (string permCheck in schemPetConf.RequiredPermissions)
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
                    foreach (string petName in schemPetConf.ModelNames)
                    {
                        builder.AppendLine(petName);
                    }
                }

                response = StringBuilderPool.Shared.ToStringReturn(builder);
                return true;
            }

            if (arguments.At(0) == "off" || arguments.At(0) == "disable" || arguments.At(0) == "remove")
            {
                petComp.ChangePetModel();
                response = "Disabled Custom Pet Model.";
                return true;
            }

            foreach (KeyValuePair<string, SchematicPetConfig> schemPetConf in Plugin.Instance.Config.SchematicPetList)
            {
                bool missingPerm = false;

                if (schemPetConf.Value.RequiredPermissions != null)
                {
                    foreach (string permCheck in schemPetConf.Value.RequiredPermissions)
                    {
                        if (!sender.CheckPermission(permCheck))
                        {
                            missingPerm = true;
                        }
                    }
                }

                if (!missingPerm)
                {
                    foreach (string modelName in schemPetConf.Value.ModelNames)
                    {
                        if (modelName == arguments.At(0))
                        {
                            petComp.ChangePetModel(schemPetConf.Value);
                            response = $"Set Pet Model successfully to {arguments.At(0)}.";
                            return true;
                        }
                    }
                }
            }

            response = "Couldn't find an allowed model with this name.";
            return false;
        }
    }
}
