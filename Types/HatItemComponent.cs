using UnityEngine;
using InventorySystem.Items.Pickups;

namespace SCPHats.Types
{
    public class HatItemComponent : MonoBehaviour
    {
        internal HatPlayerComponent player;
        internal Vector3 pos;
        internal Vector3 itemOffset;
        internal Quaternion rot;
        internal ItemPickupBase item;
        internal bool showHat;
    }
}