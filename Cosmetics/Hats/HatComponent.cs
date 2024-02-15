using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using InventorySystem.Items.Pickups;
using MapEditorReborn.API.Extensions;
using MapEditorReborn.API.Features.Objects;
using Mirror;
using PlayerRoles;
using SCPCosmetics.Cosmetics.Extensions;
using SCPCosmetics.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCPCosmetics.Cosmetics.Hats
{
    public class HatComponent : CosmeticComponent
    {
        // PICKUP VERSION
        public ushort? _hatPickupSerial = null;
        public Pickup HatPickup {
            get 
            {
                if (_hatPickupSerial == null) return null;
                return Pickup.Get((ushort)_hatPickupSerial); 
            }
        }
        public bool _hasSyncedPickup = false;

        // SCHEMATIC VERSION
        public SchematicObject HatSchematic;
        public List<string> _invisibleTo = new List<string>();
        public List<string> SchematicInvisibleTo
        {
            get
            {
                if (_invisibleTo is not null)
                    _invisibleTo = _invisibleTo.Where(uid => Player.TryGet(uid, out Player plr)).ToList();

                if (_invisibleTo is null)
                    _invisibleTo = new List<string>();

                return _invisibleTo;
            }
            set
            {
                _invisibleTo = value;
            }
        }

        // SHARED
        public Vector3 HatPosition = Vector3.zero;
        public Vector3 HatItemOffset = Vector3.zero;
        public Vector3 HatRotation = Vector3.zero;

        public bool ShowHat;

        // CALL UPDATE
        public override void UpdateCosmetic()
        {
            base.UpdateCosmetic();

            Player player = Player.Get(gameObject);

            if (player.Role == RoleTypeId.None || player.Role == RoleTypeId.Spectator)
            {
                if (HatPickup != null)
                {
                    HatPickup.Position = Vector3.one * 6000f;
                }
                if (HatSchematic != null)
                {
                    HatSchematic.Position = Vector3.one * 6000f;

                    HatSchematic.UpdateObject();
                }
            } else
            {
                var camera = player.CameraTransform;

                var rotAngles = camera.rotation.eulerAngles;
                if (PlayerRolesUtils.GetTeam(player.Role) == Team.SCPs) rotAngles.x = 0;

                var rotation = Quaternion.Euler(rotAngles);

                var rot = rotation * Quaternion.Euler(HatRotation);
                var pos = (player.Role != RoleTypeId.Scp079 ? rotation * (HatPosition + HatItemOffset) : (HatPosition + HatItemOffset)) + camera.position;

                if (HatPickup != null)
                {
                    HatPickup.Position = pos;
                    HatPickup.Rotation = rot;
                    if (HatPickup.PhysicsModule is PickupStandardPhysics standardPhysics)
                    {
                        HatPickup.PhysicsModule.ServerSendRpc(new Action<NetworkWriter>(standardPhysics.ServerWriteRigidbody));
                    }
                }

                if (HatSchematic != null)
                {
                    HatSchematic.Position = pos;
                    HatSchematic.Rotation = rot;

                    HatSchematic.UpdateObject();

                    foreach (Player plr in Player.List)
                    {
                        if (this.IsVisibleToPlayer(plr))
                        {
                            if (SchematicInvisibleTo.Contains(plr.UserId))
                            {
                                plr.SpawnSchematic(HatSchematic);
                                SchematicInvisibleTo.Remove(plr.UserId);
                            }
                        }
                        else
                        {
                            if (!SchematicInvisibleTo.Contains(plr.UserId))
                            {
                                plr.DestroySchematic(HatSchematic);
                                SchematicInvisibleTo.Add(plr.UserId);
                            }
                        }
                    }
                }
            }
        }

        // DESTROY
        public override void OnDestroy()
        {
            // DESTROY PICKUP
            if (HatPickup != null && HatPickup.GameObject != null)
            {
                Destroy(HatPickup.GameObject);
            }

            // DESTROY SCHEMATIC
            if (HatSchematic != null)
            {
                HatSchematic.Destroy();
                HatSchematic = null;
            }

            base.OnDestroy();
        }
    }
}
