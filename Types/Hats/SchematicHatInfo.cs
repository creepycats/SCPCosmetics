namespace SCPCosmetics.Types
{
    using UnityEngine;

    public struct SchematicHatInfo
    {
        public string SchematicName { get; }
        public Vector3 Scale { get; }
        public Vector3 Position { get; }
        public Vector3 Rotation { get; }

        public SchematicHatInfo(string schematicName, Vector3 scale = default, Vector3 position = default, Vector3 rotation = default)
        {
            SchematicName = schematicName;
            Scale = scale == default ? Vector3.one : scale;
            Position = position == default ? Vector3.zero : position;
            Rotation = rotation == default ? Vector3.zero : rotation;
        }
    }
}