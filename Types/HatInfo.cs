using UnityEngine;

namespace SCPHats.Types
{
    public struct HatInfo
    {
        public ItemType Item { get; }
        public Vector3 Scale { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public HatInfo(ItemType item, Vector3 scale = default, Vector3 position = default, Quaternion rotation = default)
        {
            Item = item;
            Scale = scale == default ? Vector3.one : scale;
            Position = position == default ? Vector3.zero : position;
            Rotation = rotation == default ? Quaternion.identity : rotation;
        }
    }
}