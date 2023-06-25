using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Permissions.Extensions;
using Mirror;
using PlayerRoles;
using RemoteAdmin;
using SCPCosmetics.Types;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static SCPCosmetics.Pets;
using static UnityEngine.GraphicsBuffer;

namespace SCPCosmetics.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class PetCommand : ICommand
    {
        public string Command { get; } = "pet";

        public string[] Aliases { get; } = { "pets" };

        public string Description { get; } = "Allows supporters to have a pet follow them around";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "Erm what the scallop? (Feature not implemented yet)";
            return true;

            if (!(sender is PlayerCommandSender))
            {
                response = "This command can only be ran by a player!";
                return true;
            }
            var player = Player.Get(((PlayerCommandSender)sender).ReferenceHub);

            SpawnDum("Nice Cock", RoleTypeId.Scp939, player);

            response = "Guhh???";
            return false;
        }
    }
}
