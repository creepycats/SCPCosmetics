using Exiled.API.Features.Roles;
using MapEditorReborn.Commands.ModifyingCommands.Position;
using MapEditorReborn.Commands.ModifyingCommands.Rotation;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SCPCosmetics.Types
{
    public sealed class PetConfig
    {
        [Description("The Pet's ID")]
        public string PetId { get; set; }
        [Description("List of names the command will accept for this pet. CANNOT be named off/disable/remove/list. Command Arguments take priority over pets/schematics")]
        public List<string> PetNames { get; set; }
        public RoleTypeId Role { get; set; }
        public Vector3 Scale { get; set; }
        public ItemType ItemInHand { get; set; }
        public PetConfig()
        {
            PetId = "example";
            PetNames = new List<string>() { "test" };
            Role = RoleTypeId.ClassD;
            Scale = new Vector3(1f, 1f, 1f);
            ItemInHand = ItemType.None;
        }

        public PetConfig(string _id, List<string> _names, RoleTypeId _role, Vector3 _scale, ItemType _held)
        {
            PetId = _id;
            PetNames = _names;
            Role = _role;
            Scale = _scale;
            ItemInHand = _held;
        }
    }
}
