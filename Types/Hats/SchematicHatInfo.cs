using UnityEngine;

namespace SCPCosmetics.Types
{
    public struct SchematicHatInfo
    {
        public string SchematicName { get; }
        public Vector3 Scale { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public SchematicHatInfo(string schematicName, Vector3 scale = default, Vector3 position = default, Quaternion rotation = default)
        {
            SchematicName = schematicName;
            Scale = scale == default ? Vector3.one : scale;
            Position = position == default ? Vector3.zero : position;
            Rotation = rotation == default ? Quaternion.identity : rotation;
        }
    }
}