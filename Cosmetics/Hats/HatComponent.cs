using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Roles;
using InventorySystem.Items.Pickups;
using MapEditorReborn.API.Extensions;
using MapEditorReborn.API.Features.Objects;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
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
                return;
            }

            Transform camera = player.CameraTransform;
            Vector3 HeadPositionOffset = Vector3.zero;
            Vector3 HeadRotationOffset = Vector3.zero;
            if (player.Role.Base is IFpcRole fpcRole)
            {
                List<HitboxIdentity> matchedHeadHitbox = fpcRole.FpcModule.CharacterModelInstance.Hitboxes.Where(hbox => hbox.name.ToLower().Contains("mixamorig:head")).ToList();
                camera = matchedHeadHitbox.FirstOrDefault()?.transform ?? player.CameraTransform;
                switch (player.Role.Type)
                {
                    case RoleTypeId.Scp106:
                        camera = fpcRole.FpcModule.CharacterModelInstance.gameObject.transform.Find("armature/Root/HipsCTRL/Pelvis/Stomach/LowerChest/UpperChest/neck/Head");
                        HeadPositionOffset = new Vector3(0f, -0.25f, -0.2f) + new Vector3(0, .45f, .18f);
                        HeadRotationOffset = new Vector3(0f, 180f, 0f);
                        break;
                    case RoleTypeId.Scp096:
                        camera = fpcRole.FpcModule.CharacterModelInstance.gameObject.transform.Find("SCP-096/root/Hips/Spine01/Spine02/Spine03/Neck01/Neck02/head");
                        HeadPositionOffset = new Vector3(-0.15f, -0.3f, -0.35f) + new Vector3(.15f, .425f, .325f);
                        break;
                    case RoleTypeId.Scp939:
                        camera = fpcRole.FpcModule.CharacterModelInstance.gameObject.transform.Find("Anims/939Rig/HipControl/DEF-Hips/DEF-Stomach/DEF-Chest/DEF-Neck/DEF-Head");
                        HeadPositionOffset = new Vector3(0f, -0.4f, -0.1f) + new Vector3(0, .5f, .125f);
                        break;
                    case RoleTypeId.Scp173:
                        HeadPositionOffset = new Vector3(0, .55f, -.05f);
                        break;
                    case RoleTypeId.Scp049:
                        HeadPositionOffset = new Vector3(0, .125f, -.05f);
                        break;
                    case RoleTypeId.None:
                    case RoleTypeId.Spectator:
                        HeadPositionOffset = new Vector3(-1000, -1000, -1000);
                        break;
                    case RoleTypeId.Scp0492:
                        HeadPositionOffset = new Vector3(0, 0f, -.1f);
                        break;
                    default:
                        HeadPositionOffset = new Vector3(0, .20f, -.03f);
                        break;
                }
            }

            var rotAngles = camera.rotation.eulerAngles;
            if (camera == player.CameraTransform && PlayerRolesUtils.GetTeam(player.Role) == Team.SCPs) rotAngles.x = 0;

            var rotation = Quaternion.Euler(rotAngles) * Quaternion.Euler(HeadRotationOffset);

            var rot = rotation * Quaternion.Euler(HatRotation);
            var pos = (player.Role != RoleTypeId.Scp079 ? rotation * (HatItemOffset + HeadPositionOffset) : (HatItemOffset)) + camera.position;

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
            }

            foreach (Player plr in Player.List)
            {
                if (this.IsVisibleToPlayer(plr))
                {
                    if (SchematicInvisibleTo.Contains(plr.UserId))
                    {
                        if (HatSchematic != null)
                            plr.SpawnSchematic(HatSchematic);

                        if (HatPickup != null)
                            plr.SpawnNetworkIdentity(HatPickup.Base.netIdentity);

                        SchematicInvisibleTo.Remove(plr.UserId);
                    }
                }
                else
                {
                    if (!SchematicInvisibleTo.Contains(plr.UserId))
                    {
                        if (HatSchematic != null)
                            plr.DestroySchematic(HatSchematic);

                        if (HatPickup != null)
                            plr.DestroyNetworkIdentity(HatPickup.Base.netIdentity);

                        SchematicInvisibleTo.Add(plr.UserId);
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
