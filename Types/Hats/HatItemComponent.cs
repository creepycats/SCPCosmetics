namespace SCPCosmetics.Types
{
    using InventorySystem.Items.Pickups;
    using MapEditorReborn.API.Features.Objects;
    using UnityEngine;

    public class HatItemComponent : MonoBehaviour
    {
        internal HatPlayerComponent player;
        internal Vector3 pos;
        internal Vector3 itemOffset;
        internal Vector3 rot;
        internal ItemPickupBase item;
        internal bool showHat;
        internal SchematicObject hatSchematic;
        internal bool isSchematic;
    }
}