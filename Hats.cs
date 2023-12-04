namespace SCPCosmetics
{
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using MapEditorReborn.API.Features;
    using MapEditorReborn.API.Features.Objects;
    using MEC;
    using Mirror;
    using PlayerRoles;
    using SCPCosmetics.Types;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public static class Hats
    {
        public static readonly IReadOnlyDictionary<string, ItemType> defaultHatNames = new Dictionary<string, ItemType>()
        {
            {"hat", ItemType.SCP268},
            {"268", ItemType.SCP268},
            {"scp268", ItemType.SCP268},
            {"scp-268", ItemType.SCP268},
            {"pill", ItemType.SCP500},
            {"pills", ItemType.SCP500},
            {"scp500", ItemType.SCP500},
            {"500", ItemType.SCP500},
            {"scp-500", ItemType.SCP500},
            {"light", ItemType.SCP2176},
            {"bulb", ItemType.SCP2176},
            {"lightbulb", ItemType.SCP2176},
            {"2176", ItemType.SCP2176},
            {"scp-2176", ItemType.SCP2176},
            {"coin", ItemType.Coin},
            {"quarter", ItemType.Coin},
            {"dime", ItemType.Coin},
            {"medkit", ItemType.Medkit},
            {"adrenaline", ItemType.Adrenaline},
            {"soda", ItemType.SCP207},
            {"cola", ItemType.SCP207},
            {"coke", ItemType.SCP207},
            {"207", ItemType.SCP207},
            {"scp207", ItemType.SCP207},
            {"scp-207", ItemType.SCP207},
            {"butter", ItemType.KeycardScientist}
        };

        public static IEnumerator<float> LockHats()
        {
            while (true)
            {
                foreach (HatItemComponent _hatItem in Plugin.Instance.HatItems)
                {
                    try
                    {
                        var pickupInfo = _hatItem.item.NetworkInfo;
                        pickupInfo.Locked = true;
                        _hatItem.item.NetworkInfo = pickupInfo;
                    }
                    catch (Exception e)
                    {
                        if (Plugin.Instance.Config.Debug)
                            Log.Error(e);
                    }
                }

                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        public static bool RemoveHatForPlayer(Player player)
        {
            List<HatItemComponent> hatItems = Plugin.Instance.HatItems;
            HatItemComponent _foundHat = null;

            foreach (HatItemComponent HatItem in hatItems)
            {
                if (Player.Get(HatItem.player.gameObject) == player)
                {
                    _foundHat = HatItem;
                    break;
                }
            }

            if (_foundHat != null)
            {
                hatItems.Remove(_foundHat);

                if (_foundHat.isSchematic)
                {
                    _foundHat.hatSchematic.Destroy();
                    _foundHat.isSchematic = false;
                }

                NetworkServer.Destroy(_foundHat.item.gameObject);
                return true;
            }

            return false;
        }

        internal static Vector3 GetHatPosForRole(RoleTypeId role)
        {
            return role switch
            {
                RoleTypeId.Scp173 => new Vector3(0, .55f, -.05f),
                RoleTypeId.Scp106 => new Vector3(0, .45f, .18f),
                RoleTypeId.Scp096 => new Vector3(.15f, .425f, .325f),
                RoleTypeId.Scp939 => new Vector3(0, .5f, .125f),// TODO: Fix.
                RoleTypeId.Scp049 => new Vector3(0, .125f, -.05f),
                RoleTypeId.None => new Vector3(-1000, -1000, -1000),
                RoleTypeId.Spectator => new Vector3(-1000, -1000, -1000),
                RoleTypeId.Scp0492 => new Vector3(0, .1f, -.16f),
                _ => new Vector3(0, .15f, -.07f),
            };
        }

        public static HatItemComponent SpawnHat(Player player, HatInfo hat, bool showHat = false)
        {
            if (hat.Item == ItemType.None) return null;

            var pos = GetHatPosForRole(player.Role);
            var itemOffset = Vector3.zero;
            var rot = Quaternion.Euler(0, 0, 0);
            var scale = Vector3.one;
            var item = hat.Item;

            // TODO: Fix this when whatever NW's change is figured out.
            if (item == ItemType.MicroHID || item.IsAmmo())
            {
                return null;
            }

            switch (item)
            {
                case ItemType.KeycardScientist:
                    scale += new Vector3(1.5f, 20f, 1.5f);
                    rot = Quaternion.Euler(0, 90, 0);
                    itemOffset = new Vector3(0, .1f, 0);
                    break;

                case ItemType.KeycardMTFCaptain:
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

                case ItemType.SCP2176:
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
                WeightKg = itemModel.Weight,
                Serial = ItemSerialGenerator.GenerateNext()
            };

            var pickup = UnityEngine.Object.Instantiate(itemModel.PickupDropModel, Vector3.zero, Quaternion.identity);
            pickup.transform.localScale = scale;
            pickup.NetworkInfo = psi;

            NetworkServer.Spawn(pickup.gameObject);
            //pickup.InfoReceivedHook(new PickupSyncInfo(), psi);
            //pickup.RefreshPositionAndRotation();

            return SpawnHat(player, pickup, itemOffset, rot, showHat);
        }

        public static HatItemComponent SpawnHat(Player player, ItemPickupBase pickup, Vector3 posOffset, Quaternion rotOffset, bool showHat = false)
        {
            if (!player.GameObject.TryGetComponent(out HatPlayerComponent playerComponent))
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
            playerComponent.item.pos = GetHatPosForRole(player.Role);
            playerComponent.item.itemOffset = posOffset;
            playerComponent.item.rot = rotOffset.eulerAngles;
            playerComponent.item.showHat = showHat;
            playerComponent.item.isSchematic = false;

            return playerComponent.item;
        }

        // MER Hat Item Stuff
        public static HatItemComponent SpawnHat(Player player, SchematicHatInfo hat, bool showHat = false)
        {
            if (MapUtils.GetSchematicDataByName(hat.SchematicName) == null)
                return null;

            var pos = GetHatPosForRole(player.Role);
            var itemOffset = Vector3.zero;
            var rot = Quaternion.Euler(0, 0, 0);
            var scale = Vector3.one;

            if (hat.Scale != Vector3.one) scale = hat.Scale;
            if (hat.Position != Vector3.zero) itemOffset = hat.Position;
            //if (!hat.Rotation.IsZero()) rot = hat.Rotation;
            //if (hat.Scale != Vector3.one || hat.Position != Vector3.zero) item = hat.Item; //|| !hat.Rotation.IsZero()) item = hat.Item;

            var itemModel = InventoryItemLoader.AvailableItems[ItemType.Coin];

            var psi = new PickupSyncInfo()
            {
                ItemId = ItemType.Coin,
                Serial = ItemSerialGenerator.GenerateNext(),
                WeightKg = itemModel.Weight
            };

            var pickup = UnityEngine.Object.Instantiate(itemModel.PickupDropModel, Vector3.zero, Quaternion.identity);
            pickup.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            pickup.NetworkInfo = psi;

            NetworkServer.Spawn(pickup.gameObject);
            //pickup.InfoReceivedHook(new PickupSyncInfo(), psi);
            //pickup.RefreshPositionAndRotation();

            var hatModel = ObjectSpawner.SpawnSchematic(hat.SchematicName, Vector3.zero, Quaternion.identity, scale);

            return SpawnHat(player, pickup, hatModel, itemOffset, rot, showHat);
        }

        public static HatItemComponent SpawnHat(Player player, ItemPickupBase pickup, SchematicObject schematic, Vector3 posOffset, Quaternion rotOffset, bool showHat = false)
        {

            if (!player.GameObject.TryGetComponent(out HatPlayerComponent playerComponent))
            {
                playerComponent = player.GameObject.AddComponent<HatPlayerComponent>();
            }

            if (playerComponent.item != null)
            {
                UnityEngine.Object.Destroy(playerComponent.item.gameObject);
                playerComponent.item = null;
            }

            //var rigidbody = schematic.gameObject.GetComponent<Rigidbody>();
            //rigidbody.useGravity = false;
            //rigidbody.isKinematic = true;

            playerComponent.item = schematic.gameObject.AddComponent<HatItemComponent>();
            playerComponent.item.item = pickup;
            playerComponent.item.hatSchematic = schematic;
            playerComponent.item.player = playerComponent;
            playerComponent.item.pos = GetHatPosForRole(player.Role);
            playerComponent.item.itemOffset = posOffset;
            playerComponent.item.rot = rotOffset.eulerAngles;
            playerComponent.item.showHat = showHat;
            playerComponent.item.isSchematic = true;

            return playerComponent.item;
        }

        public static bool ShouldRemoveHat(RoleTypeId _rtid)
        {
            return (RoleTypeId.Scp079 == _rtid) || (Plugin.Instance.Config.RemoveHatsOnDeath && (_rtid == RoleTypeId.None || _rtid == RoleTypeId.Spectator || _rtid == RoleTypeId.Overwatch || _rtid == RoleTypeId.Filmmaker));
        }
    }
}
