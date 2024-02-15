using AdminToys;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using Mirror;
using PlayerRoles;
using SCPCosmetics.Cosmetics.Glows;
using SCPCosmetics.Types;
using System.Collections.Generic;
using UnityEngine;
using PlayerEvents = Exiled.Events.Handlers.Player;

namespace SCPCosmetics.Cosmetics.Hats
{
    public class HatsHandler : CosmeticHandler
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
            {"scp2176", ItemType.SCP2176},
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

        public static bool ShouldRemoveHat(RoleTypeId _rtid)
        {
            return (RoleTypeId.Scp079 == _rtid) || (Plugin.Instance.Config.RemoveHatsOnDeath && (_rtid == RoleTypeId.None || _rtid == RoleTypeId.Spectator || _rtid == RoleTypeId.Overwatch || _rtid == RoleTypeId.Filmmaker));
        }

        public static bool IsHat(ItemPickupBase ipb)
        {
            Pickup check = Pickup.Get(ipb);
            foreach (Player player in Player.List)
            {
                if (player.GameObject.TryGetComponent(out HatComponent hatComp))
                {
                    return hatComp._hatPickupSerial == check.Serial;
                }
            }
            return false;
        }

        public static bool TryGetHat(ItemPickupBase ipb, out HatComponent hatComp)
        {
            Pickup check = Pickup.Get(ipb);
            hatComp = null;
            foreach (Player player in Player.List)
            {
                if (player.GameObject.TryGetComponent(out HatComponent hatComp2) && hatComp2.HatPickup == check)
                {
                    hatComp = hatComp2;
                    return true;
                }
            }
            return false;
        }

        public static HatComponent SpawnItemHat(Player player, HatInfo hat, bool showHat = false)
        {
            if (hat.Item == ItemType.None) return null;

            HatsHandler thisHandler = Plugin.Instance.GetCosmeticHandler(typeof(HatsHandler)) as HatsHandler;
            if (player.GameObject.TryGetComponent(out HatComponent oldComponent))
            {
                Object.Destroy(oldComponent);
            }
            if (thisHandler.PlayerLinkedCosmetics.ContainsKey(player.UserId))
            {
                thisHandler.PlayerLinkedCosmetics.Remove(player.UserId);
            }

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

            if (hat.Scale != Vector3.one || hat.Position != Vector3.zero) item = hat.Item;

            var itemModel = InventoryItemLoader.AvailableItems[item];

            var psi = new PickupSyncInfo()
            {
                ItemId = item,
                WeightKg = itemModel.Weight,
                Serial = ItemSerialGenerator.GenerateNext()
            };
            psi.Locked = true;

            var pickup = UnityEngine.Object.Instantiate(itemModel.PickupDropModel, Vector3.zero, Quaternion.identity);
            pickup.transform.localScale = scale;
            pickup.NetworkInfo = psi;

            NetworkServer.Spawn(pickup.gameObject);

            HatComponent cosmeticComponent;

            cosmeticComponent = player.GameObject.AddComponent<HatComponent>();

            var rigidbody = pickup.gameObject.GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = false;
            rigidbody.detectCollisions = false;

            cosmeticComponent._hatPickupSerial = Pickup.Get(pickup).Serial;
            cosmeticComponent.HatItemOffset = itemOffset;
            cosmeticComponent.HatRotation = rot.eulerAngles;
            cosmeticComponent.ShowHat = showHat;

            thisHandler.PlayerLinkedCosmetics.Add(player.UserId, cosmeticComponent);

            return cosmeticComponent;
        }

        public static HatComponent SpawnSchematicHat(Player player, SchematicHatInfo hat, bool showHat = false)
        {
            HatsHandler thisHandler = Plugin.Instance.GetCosmeticHandler(typeof(HatsHandler)) as HatsHandler;
            if (player.GameObject.TryGetComponent(out HatComponent oldComponent))
            {
                Object.Destroy(oldComponent);
            }
            if (thisHandler.PlayerLinkedCosmetics.ContainsKey(player.UserId))
            {
                thisHandler.PlayerLinkedCosmetics.Remove(player.UserId);
            }

            var itemOffset = Vector3.zero;
            var rot = Quaternion.Euler(0, 0, 0);
            var scale = Vector3.one;

            if (hat.Scale != Vector3.one) scale = hat.Scale;
            if (hat.Position != Vector3.zero) itemOffset = hat.Position;

            SchematicObject hatModel = ObjectSpawner.SpawnSchematic(hat.SchematicName, Vector3.zero, Quaternion.identity, scale, isStatic: false);
            foreach (AdminToyBase atb in hatModel.AdminToyBases)
            {
                atb.MovementSmoothing = 60;
            }

            HatComponent cosmeticComponent;

            cosmeticComponent = player.GameObject.AddComponent<HatComponent>();

            cosmeticComponent.HatItemOffset = itemOffset;
            cosmeticComponent.HatRotation = rot.eulerAngles;
            cosmeticComponent.ShowHat = showHat;
            cosmeticComponent.HatSchematic = hatModel;

            thisHandler.PlayerLinkedCosmetics.Add(player.UserId, cosmeticComponent);

            return cosmeticComponent;
        }

        public override void RegisterCosmetic()
        {
            PlayerEvents.Died += EventDied;
            base.RegisterCosmetic();
        }

        public override void UnregisterCosmetic()
        {
            PlayerEvents.Died -= EventDied;
            base.UnregisterCosmetic();
        }

        public void EventDied(DiedEventArgs args)
        {
            if (Plugin.Instance.Config.RemoveHatsOnDeath)
            {
                if (args.Player.GameObject.TryGetComponent(out HatComponent oldComponent))
                {
                    Object.Destroy(oldComponent);
                }
                if (PlayerLinkedCosmetics.ContainsKey(args.Player.UserId))
                {
                    PlayerLinkedCosmetics.Remove(args.Player.UserId);
                }
            }
        }
    }
}
