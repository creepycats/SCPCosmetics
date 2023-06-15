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
using SCPHats.Types;

namespace SCPHats
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
}
