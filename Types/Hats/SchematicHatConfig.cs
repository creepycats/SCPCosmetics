using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace SCPCosmetics.Types
{
    public sealed class SchematicHatConfig
    {
        [Description("Name of the Schematic to use for the Hat")]
        public string SchematicName { get; set; }

        [Description("List of names the command will accept for this hat. CANNOT be named off/disable/remove/list. Items + Command Arguments take priority over schematics")]
        public List<string> HatNames { get; set; }
        [Description("List of permissions required for this hat to be used")]
        public List<string> RequiredPermissions { get; set; }

        [Description("The Positional Offset for the Schematic")]
        public Vector3 Position { get; set; }

        [Description("The Rotational Offset for the Schematic")]
        public Quaternion Rotation { get; set; }

        [Description("The Scale for the Schematic")]
        public Vector3 Scale { get; set; }

        public SchematicHatConfig()
        {
            SchematicName = "example";
            RequiredPermissions = new List<string>();
            HatNames = new List<string>(){ "test" };
            Position = new Vector3(0f,0f,0f);
            Rotation = Quaternion.Euler(0, 0, 0);
            Scale = new Vector3(1f, 1f, 1f);
        }

        public SchematicHatConfig(string _si, List<string> _perm, Vector3 _pos, Quaternion _rot, Vector3 _scale)
        {
            SchematicName = _si;
            RequiredPermissions = _perm;
            Position = _pos;
            Rotation = _rot;
            Scale = _scale;
        }
    }
}