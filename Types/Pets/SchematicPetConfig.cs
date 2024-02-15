namespace SCPCosmetics.Types
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using UnityEngine;

    public sealed class SchematicPetConfig
    {
        [Description("Name of the Schematic to use for the Pet")]
        public string SchematicName { get; set; }

        [Description("List of names the command will accept for this pet. CANNOT be named off/disable/remove/list.")]
        public List<string> ModelNames { get; set; }
        [Description("List of permissions required for this pet model to be used")]
        public List<string> RequiredPermissions { get; set; }

        [Description("The Positional Offset for the Schematic")]
        public Vector3 Position { get; set; }

        [Description("The Rotational Offset for the Schematic")]
        public Vector3 Rotation { get; set; }

        [Description("The Scale for the Schematic")]
        public Vector3 Scale { get; set; }

        public SchematicPetConfig()
        {
            SchematicName = "example";
            RequiredPermissions = new List<string>();
            ModelNames = new List<string>() { "test" };
            Position = new Vector3(0f, 0f, 0f);
            Rotation = new Vector3(0f,0f,0f);
            Scale = new Vector3(1f, 1f, 1f);
        }

        public SchematicPetConfig(string _si, List<string> _perm, Vector3 _pos, Vector3 _rot, Vector3 _scale)
        {
            SchematicName = _si;
            RequiredPermissions = _perm;
            Position = _pos;
            Rotation = _rot;
            Scale = _scale;
        }
    }
}