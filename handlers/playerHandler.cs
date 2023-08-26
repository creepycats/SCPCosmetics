using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using Mirror;
using PlayerRoles.FirstPersonControl;
using SCPCosmetics.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCPCosmetics.handlers
{
    public class playerHandler
    {
        private List<Player> Players = new List<Player>();
        private readonly Config.Config config;

        public playerHandler(Config.Config config) => this.config = config;

        public List<Types.HatItemComponent> HatItems { get; set; } = new List<Types.HatItemComponent>();

        public void ChangingRole(ChangingRoleEventArgs args)
        {
            try
            {
                if (args.Player.SessionVariables.TryGetValue("npc", out object isNpc) != true)
                {
                    if (Hats.ShouldRemoveHat(args.NewRole))
                    {
                        List<Types.HatItemComponent> HatItems = SCPCosmetics.Instance.HatItems;
                        HatItemComponent _foundHat = null;
                        foreach (HatItemComponent HatItem in HatItems)
                        {
                            if (Player.Get(HatItem.player.gameObject) == args.Player) _foundHat = HatItem;
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
                            SCPCosmetics.Instance.HatItems = HatItems;
                        }
                    }
                }
            } catch (Exception err) {
                if (SCPCosmetics.Instance.Config.Debug) Log.Error(err);
            }

            if (SCPCosmetics.Instance.PetDictionary.TryGetValue($"pet-{args.Player.UserId}", out Npc petNpc))
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
                            SCPCosmetics.Instance.PetDictionary.Remove($"pet-{args.Player.UserId}");
                        });
                    }
                    else
                    {
                        if (SCPCosmetics.Instance.Config.PetsMirrorClass)
                        {
                            petNpc.Role.Set(args.NewRole);
                        }
                    }
                });
            }
        }

        public void Died(DiedEventArgs args)
        {
            try {
                if (SCPCosmetics.Instance.Config.RemoveHatsOnDeath)
                {
                    List<Types.HatItemComponent> HatItems = SCPCosmetics.Instance.HatItems;
                    HatItemComponent _foundHat = null;
                    foreach (HatItemComponent HatItem in HatItems)
                    {
                        if (Player.Get(HatItem.player.gameObject) == args.Player) _foundHat = HatItem;
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
                        SCPCosmetics.Instance.HatItems = HatItems;
                    }
                }
            } catch (Exception err) {
                if (SCPCosmetics.Instance.Config.Debug) Log.Error(err);
            }
        }

        // For Some Reason, SCPSL and Exiled right now dont want to work with Locked items in fakeSyncVars
        public void SearchingPickup(SearchingPickupEventArgs args) {
            try
            {
                List<Types.HatItemComponent> HatItems = SCPCosmetics.Instance.HatItems;
                HatItems.RemoveAll(hatItem => hatItem == null);
                SCPCosmetics.Instance.HatItems = HatItems;
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
                if (SCPCosmetics.Instance.Config.Debug) Log.Error(err);
            }
        }


        // PET STUFF
        public void TriggeringTesla(TriggeringTeslaEventArgs args)
        {
            if (SCPCosmetics.Instance.PetDictionary.Values.Contains(args.Player))
            {
                args.IsAllowed = false;
            }
        }
        public void Handcuffing(HandcuffingEventArgs args)
        {
            if (SCPCosmetics.Instance.PetDictionary.Values.Contains(args.Target))
            {
                args.IsAllowed = false;
            }
        }
        public void DroppingItem(DroppingItemEventArgs args)
        {
            if (SCPCosmetics.Instance.PetDictionary.Values.Contains(args.Player))
            {
                args.IsAllowed = false;
            }
        }
        public void MakingNoise(MakingNoiseEventArgs args)
        {
            if (SCPCosmetics.Instance.PetDictionary.Values.Contains(args.Player))
            {
                args.IsAllowed = false;
            }
        }
        public void Escaping(EscapingEventArgs args)
        {
            if (SCPCosmetics.Instance.PetDictionary.Values.Contains(args.Player))
            {
                args.IsAllowed = false;
            }
        }
        public void Left(LeftEventArgs args)
        {
            if (SCPCosmetics.Instance.PetDictionary.Keys.Contains($"pet-{args.Player.UserId}"))
            {
                SCPCosmetics.Instance.PetDictionary[$"pet-{args.Player.UserId}"].ClearInventory();
                SCPCosmetics.Instance.PetDictionary[$"pet-{args.Player.UserId}"].GameObject.GetComponent<PetComponent>().stopRunning = true;
                SCPCosmetics.Instance.PetDictionary[$"pet-{args.Player.UserId}"].Position = new Vector3(-9999f, -9999f, -9999f);
                Timing.CallDelayed(0.5f, () =>
                {
                    NetworkServer.Destroy(SCPCosmetics.Instance.PetDictionary[$"pet-{args.Player.UserId}"].GameObject);
                    SCPCosmetics.Instance.PetDictionary.Remove($"pet-{args.Player.UserId}");
                });
            }
        }
    }
}
