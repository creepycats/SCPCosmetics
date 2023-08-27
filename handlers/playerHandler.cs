namespace SCPCosmetics.handlers
{
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;
    using SCPCosmetics.Types;
    using MEC;
    using Mirror;
    using PlayerRoles.FirstPersonControl;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class PlayerHandler
    {
        private readonly Config.Config config;

        public PlayerHandler(Config.Config config) => this.config = config;

        public List<HatItemComponent> HatItems { get; set; } = new List<HatItemComponent>();

        public void ChangingRole(ChangingRoleEventArgs args)
        {
            try
            {
                if (args.Player.IsNPC)
                    goto SkipRemove;

                if (!Hats.ShouldRemoveHat(args.NewRole))
                    goto SkipRemove;

                List<HatItemComponent> hatItems = Plugin.Instance.HatItems;
                HatItemComponent _foundHat = null;

                foreach (HatItemComponent HatItem in hatItems)
                {
                    if (Player.Get(HatItem.player.gameObject) == args.Player)
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
                }
            }
            catch (Exception err)
            {
                if (Plugin.Instance.Config.Debug)
                    Log.Error(err);
            }

        SkipRemove:
            if (Plugin.Instance.PetDictionary.TryGetValue($"pet-{args.Player.UserId}", out Npc petNpc))
            {
                Timing.CallDelayed(0.1f, () =>
                {
                    if (args.Player.ReferenceHub.roleManager.CurrentRole is not FpcStandardRoleBase)
                    {
                        petNpc.ClearInventory();
                        petNpc.GameObject.GetComponent<PetComponent>().stopRunning = true;
                        petNpc.Position = new Vector3(-9999f, -9999f, -9999f);
                        Timing.CallDelayed(0.5f, () =>
                        {
                            NetworkServer.Destroy(petNpc.GameObject);
                            Plugin.Instance.PetDictionary.Remove($"pet-{args.Player.UserId}");
                        });
                    }
                    else
                    {
                        if (Plugin.Instance.Config.PetsMirrorClass)
                        {
                            petNpc.Role.Set(args.NewRole);
                        }
                    }
                });
            }
        }

        public void Died(DiedEventArgs args)
        {
            try
            {
                if (Plugin.Instance.Config.RemoveHatsOnDeath)
                {
                    List<HatItemComponent> HatItems = Plugin.Instance.HatItems;

                    HatItemComponent _foundHat = null;

                    foreach (HatItemComponent HatItem in HatItems)
                    {
                        if (Player.Get(HatItem.player.gameObject) == args.Player)
                            _foundHat = HatItem;
                    }

                    if (_foundHat != null)
                    {
                        HatItems.Remove(_foundHat);

                        if (_foundHat.isSchematic)
                        {
                            _foundHat.hatSchematic.Destroy();
                            _foundHat.isSchematic = false;
                        }

                        NetworkServer.Destroy(_foundHat.item.gameObject);
                    }
                }
            }
            catch (Exception err)
            {
                if (Plugin.Instance.Config.Debug)
                    Log.Error(err);
            }
        }

        // For Some Reason, SCPSL and Exiled right now dont want to work with Locked items in fakeSyncVars
        public void SearchingPickup(SearchingPickupEventArgs args)
        {
            try
            {
                List<HatItemComponent> HatItems = Plugin.Instance.HatItems;
                HatItems.RemoveAll(hatItem => hatItem == null);
                Plugin.Instance.HatItems = HatItems;
                foreach (HatItemComponent hatItem in HatItems)
                {
                    if (args.Pickup.GameObject == hatItem.item.gameObject)
                    {
                        var pickupInfo = hatItem.item.NetworkInfo;
                        pickupInfo.Locked = true;
                        hatItem.item.NetworkInfo = pickupInfo;
                        args.IsAllowed = false; // Just in case
                    }
                }
            }
            catch (Exception err)
            {
                if (Plugin.Instance.Config.Debug) Log.Error(err);
            }
        }


        // PET STUFF
        public void TriggeringTesla(TriggeringTeslaEventArgs args)
        {
            if (Plugin.Instance.PetDictionary.Values.Contains(args.Player))
            {
                args.IsAllowed = false;
            }
        }

        public void Handcuffing(HandcuffingEventArgs args)
        {
            if (Plugin.Instance.PetDictionary.Values.Contains(args.Target))
            {
                args.IsAllowed = false;
            }
        }

        public void DroppingItem(DroppingItemEventArgs args)
        {
            if (Plugin.Instance.PetDictionary.Values.Contains(args.Player))
            {
                args.IsAllowed = false;
            }
        }

        public void MakingNoise(MakingNoiseEventArgs args)
        {
            if (Plugin.Instance.PetDictionary.Values.Contains(args.Player))
            {
                args.IsAllowed = false;
            }
        }

        public void Escaping(EscapingEventArgs args)
        {
            if (Plugin.Instance.PetDictionary.Values.Contains(args.Player))
            {
                args.IsAllowed = false;
            }
        }

        public void Left(LeftEventArgs args)
        {
            if (Plugin.Instance.PetDictionary.Keys.Contains($"pet-{args.Player.UserId}"))
            {
                Plugin.Instance.PetDictionary[$"pet-{args.Player.UserId}"].ClearInventory();
                Plugin.Instance.PetDictionary[$"pet-{args.Player.UserId}"].GameObject.GetComponent<PetComponent>().stopRunning = true;
                Plugin.Instance.PetDictionary[$"pet-{args.Player.UserId}"].Position = new Vector3(-9999f, -9999f, -9999f);
                Timing.CallDelayed(0.5f, () =>
                {
                    NetworkServer.Destroy(Plugin.Instance.PetDictionary[$"pet-{args.Player.UserId}"].GameObject);
                    Plugin.Instance.PetDictionary.Remove($"pet-{args.Player.UserId}");
                });
            }
        }
    }
}
