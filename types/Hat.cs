using CustomPlayerEffects;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp096;
using PlayerRoles.PlayableScps.Scp939;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using InventorySystem.Items.Pickups;
using InventorySystem.Items;
using InventorySystem;
using Mirror;

namespace SCPHats.Types
{
    public class Hats 
    {
        internal static Vector3 GetHatPosForRole(RoleTypeId role)
        {
            switch (role)
            {
                case RoleTypeId.Scp173:
                    return new Vector3(0, .55f, -.05f);
                case RoleTypeId.Scp106:
                    return new Vector3(0, .45f, .18f);
                case RoleTypeId.Scp096:
                    return new Vector3(.15f, .425f, .325f);
                case RoleTypeId.Scp939:
                    // TODO: Fix.
                    return new Vector3(0, -.5f, 1.125f);
                case RoleTypeId.Scp049:
                    return new Vector3(0, .125f, -.05f);
                case RoleTypeId.None:
                    return new Vector3(-1000, -1000, -1000);
                case RoleTypeId.Spectator:
                    return new Vector3(-1000, -1000, -1000);
                case RoleTypeId.Scp0492:
                    return new Vector3(0, .1f, -.16f);
                default:
                    return new Vector3(0, .15f, -.07f);
            }
        }
        public static void SpawnHat(Player player, HatInfo hat, bool showHat = false)
        {
            if (hat.Item == ItemType.None) return;

            var pos = Hats.GetHatPosForRole(player.Role);
            var itemOffset = Vector3.zero;
            var rot = Quaternion.Euler(0, 0, 0);
            var scale = Vector3.one;
            var item = hat.Item;

            // TODO: Fix this when whatever NW's change is figured out.
            if (item == ItemType.MicroHID || item == ItemType.Ammo9x19 || item == ItemType.Ammo12gauge ||
                item == ItemType.Ammo44cal || item == ItemType.Ammo556x45 || item == ItemType.Ammo762x39)
            {
                return;
            }

            switch (item)
            {
                case ItemType.KeycardScientist:
                    scale += new Vector3(1.5f, 20f, 1.5f);
                    rot = Quaternion.Euler(0, 90, 0);
                    itemOffset = new Vector3(0, .1f, 0);
                    break;

                case ItemType.KeycardNTFCommander:
                    scale += new Vector3(1.5f, 200f, 1.5f);
                    rot = Quaternion.Euler(0, 90, 0);
                    itemOffset = new Vector3(0, .9f, 0);
                    break;

                case ItemType.SCP268:
                    scale += new Vector3(-.1f, -.1f, -.1f);
                    rot = Quaternion.Euler(-90, 0, 90);
                    itemOffset = new Vector3(0, 0, .1f);
                    break;

                case ItemType.Adrenaline:
                case ItemType.Medkit:
                case ItemType.Coin:
                case ItemType.SCP018:
                    itemOffset = new Vector3(0, .1f, 0);
                    break;

                case ItemType.SCP500:
                    itemOffset = new Vector3(0, .075f, 0);
                    break;

                case ItemType.SCP207:
                    itemOffset = new Vector3(0, .225f, 0);
                    rot = Quaternion.Euler(-90, 0, 0);
                    break;
            }

            if (hat.Scale != Vector3.one) scale = hat.Scale;
            if (hat.Position != Vector3.zero) itemOffset = hat.Position;
            //if (!hat.Rotation.IsZero()) rot = hat.Rotation;
            if (hat.Scale != Vector3.one || hat.Position != Vector3.zero) item = hat.Item; //|| !hat.Rotation.IsZero()) item = hat.Item;

            var itemModel = InventoryItemLoader.AvailableItems[item];

            var psi = new PickupSyncInfo()
            {
                ItemId = item,
                Serial = ItemSerialGenerator.GenerateNext(),
                Weight = itemModel.Weight
            };

            var pickup = UnityEngine.Object.Instantiate(itemModel.PickupDropModel, Vector3.zero, Quaternion.identity);
            pickup.transform.localScale = scale;
            pickup.NetworkInfo = psi;

            NetworkServer.Spawn(pickup.gameObject);
            pickup.InfoReceived(new PickupSyncInfo(), psi);
            pickup.RefreshPositionAndRotation();

            SpawnHat(player, pickup, itemOffset, rot, showHat);
        }

        public static void SpawnHat(Player player, ItemPickupBase pickup, Vector3 posOffset, Quaternion rotOffset, bool showHat = false)
        {
            HatPlayerComponent playerComponent;

            if (!player.GameObject.TryGetComponent(out playerComponent))
            {
                playerComponent = player.GameObject.AddComponent<HatPlayerComponent>();
            }

            if (playerComponent.item != null)
            {
                UnityEngine.Object.Destroy(playerComponent.item.gameObject);
                playerComponent.item = null;
            }

            var rigidbody = pickup.gameObject.GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;

            playerComponent.item = pickup.gameObject.AddComponent<HatItemComponent>();
            playerComponent.item.item = pickup;
            playerComponent.item.player = playerComponent;
            playerComponent.item.pos = Hats.GetHatPosForRole(player.Role);
            playerComponent.item.itemOffset = posOffset;
            playerComponent.item.rot = rotOffset;
            playerComponent.item.showHat = showHat;
        }
    }
 
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

    public class HatItemComponent : MonoBehaviour
    {
        internal HatPlayerComponent player;
        internal Vector3 pos;
        internal Vector3 itemOffset;
        internal Quaternion rot;
        internal ItemPickupBase item;
        internal bool showHat;
    }

    public class HatPlayerComponent : MonoBehaviour
    {
        internal HatItemComponent item;

        private bool _threw = false;

        private void Start()
        {
            Timing.RunCoroutine(MoveHat().CancelWith(this).CancelWith(gameObject));
        }

        private IEnumerator<float> MoveHat()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(.1f);

                try
                {
                    if (item == null || item.gameObject == null) continue;

                    var player = Player.Get(gameObject);
                    var pickup = item.item;
                    var pickupInfo = pickup.NetworkInfo;
                    var pickupType = pickup.GetType();

                    pickupInfo.Locked = true;

                    if (player.Role == RoleTypeId.None || player.Role == RoleTypeId.Spectator)
                    {
                        pickup.transform.position = Vector3.one * 6000f;
                        pickupInfo.ServerSetPositionAndRotation(Vector3.one * 6000f, Quaternion.identity);

                        pickup.NetworkInfo = pickupInfo;

                        continue;
                    }

                    var camera = player.CameraTransform;

                    var rotAngles = camera.rotation.eulerAngles;
                    if (PlayerRolesUtils.GetTeam(player.Role) == Team.SCPs) rotAngles.x = 0;

                    var rotation = Quaternion.Euler(rotAngles);

                    var rot = rotation * item.rot;
                    var transform1 = pickup.transform;
                    var pos = (player.Role != RoleTypeId.Scp079 ? rotation * (item.pos + item.itemOffset) : (item.pos + item.itemOffset)) + camera.position;

                    transform1.position = pos;
                    transform1.rotation = rot;

                    pickupInfo.ServerSetPositionAndRotation(pos, rot);

                    var fakePickupInfo = pickup.NetworkInfo;
                    fakePickupInfo.ServerSetPositionAndRotation(Vector3.zero, Quaternion.identity);
                    fakePickupInfo.Locked = true;
                    var ownerPickupInfo = item.showHat ? pickupInfo : fakePickupInfo;

                    foreach (var player1 in Player.List)
                    {
                        // IsPlayerNPC is 2nd because it has a lot of null checks.
                        if (player1 == null || player1.UserId == null || player1.IsDead) continue;

                        if (player1 == player)
                        {
                            MirrorExtensions.SendFakeSyncVar(player1, pickup.netIdentity, pickupType, "NetworkInfo", ownerPickupInfo);
                        }
                        else if (PlayerRolesUtils.GetTeam(player1.Role) == PlayerRolesUtils.GetTeam(player.Role))
                        {
                            MirrorExtensions.SendFakeSyncVar(player1, pickup.netIdentity, pickupType, "NetworkInfo", pickupInfo);
                        }
                        else if (player1.ReferenceHub.roleManager.CurrentRole is FpcStandardRoleBase role)
                            switch (player1.Role.Type)
                            {
                                case RoleTypeId.Scp939 when role.VisibilityController is Scp939VisibilityController vision && !vision.ValidateVisibility(player.ReferenceHub):
                                    MirrorExtensions.SendFakeSyncVar(player1, pickup.netIdentity, pickupType, "NetworkInfo", fakePickupInfo);
                                    break;
                                case RoleTypeId.Scp096 when role.VisibilityController is Scp096VisibilityController vision && !vision.ValidateVisibility(player.ReferenceHub):
                                    MirrorExtensions.SendFakeSyncVar(player1, pickup.netIdentity, pickupType, "NetworkInfo", fakePickupInfo);
                                    break;
                                default:
                                    MirrorExtensions.SendFakeSyncVar(player1, pickup.netIdentity, pickupType, "NetworkInfo", pickupInfo);
                                    break;
                            }
                    }
                }
                catch (Exception e)
                {
                    if (!_threw)
                    {
                        Log.Error(e.ToString());
                        _threw = true;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (item != null && item.gameObject != null)
            {
                UnityEngine.Object.Destroy(item.gameObject);
            }
        }
    }
}