namespace SCPCosmetics.Types
{
    using UnityEngine;

    public struct HatInfo
    {
        public ItemType Item { get; }
        public Vector3 Scale { get; }
        public Vector3 Position { get; }
        public Vector3 Rotation { get; }

        public HatInfo(ItemType item, Vector3 scale = default, Vector3 position = default, Vector3 rotation = default)
        {
            Item = item;
            Scale = scale == default ? Vector3.one : scale;
            Position = position == default ? Vector3.zero : position;
            Rotation = rotation == default ? Vector3.zero : rotation;
        }
    }
}