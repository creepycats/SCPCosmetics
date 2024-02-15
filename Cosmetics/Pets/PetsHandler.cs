using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Components;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Subroutines;
using PlayerStatsSystem;
using SCPCosmetics.Cosmetics.Hats;
using SCPCosmetics.Types;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Misc;
using PlayerEvents = Exiled.Events.Handlers.Player;

namespace SCPCosmetics.Cosmetics.Pets
{
    public class PetsHandler : CosmeticHandler
    {
        public Dictionary<int, DateTime> PetRatelimit { get; set; } = new();

        public static readonly IReadOnlyDictionary<string, ItemType> allowedPetItems = new Dictionary<string, ItemType>()
        {
            {"off", ItemType.None},
            {"none", ItemType.None},
            {"pill", ItemType.SCP500},
            {"pills", ItemType.SCP500},
            {"scp500", ItemType.SCP500},
            {"500", ItemType.SCP500},
            {"scp-500", ItemType.SCP500},
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
            {"keycard", ItemType.KeycardScientist}
        };

        public static readonly IReadOnlyDictionary<string, RoleTypeId> allowedPetClasses = new Dictionary<string, RoleTypeId>()
        {
            {"dclass", RoleTypeId.ClassD},
            {"d-class", RoleTypeId.ClassD},
            {"class-d", RoleTypeId.ClassD},
            {"classd", RoleTypeId.ClassD},
            {"dboy", RoleTypeId.ClassD},
            {"dboi", RoleTypeId.ClassD},
            {"scientist", RoleTypeId.Scientist},
            {"science", RoleTypeId.Scientist},
            {"facilityguard", RoleTypeId.FacilityGuard},
            {"facility", RoleTypeId.FacilityGuard},
            {"guard", RoleTypeId.FacilityGuard},
            {"ntf", RoleTypeId.NtfSergeant},
            {"mtf", RoleTypeId.NtfSergeant},
            {"chaos", RoleTypeId.ChaosRifleman},
            {"chaosinsurgency", RoleTypeId.ChaosRifleman},
            {"tutorial", RoleTypeId.Tutorial},
            {"scp049", RoleTypeId.Scp049},
            {"scp-049", RoleTypeId.Scp049},
            {"049", RoleTypeId.Scp049},
            {"49", RoleTypeId.Scp049},
            {"scp0492", RoleTypeId.Scp0492},
            {"scp-0492", RoleTypeId.Scp0492},
            {"scp049-2", RoleTypeId.Scp0492},
            {"scp-049-2", RoleTypeId.Scp0492},
            {"0492", RoleTypeId.Scp0492},
            {"492", RoleTypeId.Scp0492},
            {"049-2", RoleTypeId.Scp0492},
            {"49-2", RoleTypeId.Scp0492},
            {"zombie", RoleTypeId.Scp0492},
            {"scp096", RoleTypeId.Scp096},
            {"scp-096", RoleTypeId.Scp096},
            {"096", RoleTypeId.Scp096},
            {"96", RoleTypeId.Scp096},
            {"scp106", RoleTypeId.Scp106},
            {"scp-106", RoleTypeId.Scp106},
            {"106", RoleTypeId.Scp106},
            {"larry", RoleTypeId.Scp106},
            {"scp173", RoleTypeId.Scp173},
            {"scp-173", RoleTypeId.Scp173},
            {"173", RoleTypeId.Scp173},
            {"peanut", RoleTypeId.Scp173},
            {"nut", RoleTypeId.Scp173},
            {"scp939", RoleTypeId.Scp939},
            {"scp-939", RoleTypeId.Scp939},
            {"939", RoleTypeId.Scp939},
            {"dog", RoleTypeId.Scp939},
            {"scp3114", RoleTypeId.Scp3114},
            {"scp-3114", RoleTypeId.Scp3114},
            {"3114", RoleTypeId.Scp3114},
            {"skeleton", RoleTypeId.Scp3114},
            {"sans", RoleTypeId.Scp3114},
            {"papyrus", RoleTypeId.Scp3114}
        };

        public static bool IsPet(ReferenceHub refHub)
        {
            foreach (Player player in Player.List)
            {
                if (player.GameObject.TryGetComponent(out PetComponent petComp))
                {
                    if (petComp.PetNPC.ReferenceHub == refHub) return true;
                }
            }
            return false;
        }

        public static bool IsPet(Player play)
        {
            foreach (Player player in Player.List)
            {
                if (player.GameObject.TryGetComponent(out PetComponent petComp))
                {
                    if (petComp.PetNPC.ReferenceHub == play.ReferenceHub) return true;
                }
            }
            return false;
        }

        public static bool IsPet(SubroutineBase targetTrack)
        {
            targetTrack.Role.TryGetOwner(out ReferenceHub refHub);
            if (refHub != null)
                return IsPet(refHub);
            else
                return false;
        }

        public static void SpawnPet(string Name, string Color, RoleTypeId Role, ItemType? HeldItem, Player target, Vector3 scale)
        {
            PetsHandler thisHandler = Plugin.Instance.GetCosmeticHandler(typeof(PetsHandler)) as PetsHandler;
            if (target.GameObject.TryGetComponent(out PetComponent oldComponent))
            {
                UnityEngine.Object.Destroy(oldComponent);
            }
            if (thisHandler.PlayerLinkedCosmetics.ContainsKey(target.UserId))
            {
                thisHandler.PlayerLinkedCosmetics.Remove(target.UserId);
            }

            Plugin.Instance.PetIDNumber++;
            Npc SpawnedPet = SpawnFix($"{target.Nickname}'s Pet", Role, Plugin.Instance.PetIDNumber);

            SpawnedPet.Scale = scale;
            SpawnedPet.ReferenceHub.characterClassManager._godMode = true;
            SpawnedPet.ReferenceHub.playerStats.GetModule<AdminFlagsStat>().SetFlag(AdminFlags.GodMode, true);
            SpawnedPet.MaxHealth = 9999;
            SpawnedPet.Health = 9999;

            SpawnedPet.RankName = Name;
            if (Color != null && Enum.TryParse(Color, true, out PlayerInfoColorTypes rankColor))
            {
                SpawnedPet.RankColor = Color;
            }

            Round.IgnoredPlayers.Add(SpawnedPet.ReferenceHub);

            PetComponent petComponent = target.GameObject.AddComponent<PetComponent>();
            petComponent.PetNPC = SpawnedPet;

            if (!(HeldItem == null || HeldItem == ItemType.None))
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    SpawnedPet.ClearInventory();
                    SpawnedPet.CurrentItem = Item.Create((ItemType)HeldItem, SpawnedPet);
                });
            }

            thisHandler.PlayerLinkedCosmetics.Add(target.UserId, petComponent);
        }

        public static Npc SpawnFix(string name, RoleTypeId role, int id = 0, Vector3? position = null)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab);
            Npc npc = new(gameObject)
            {
                IsNPC = true
            };
            try
            {
                npc.ReferenceHub.roleManager.InitializeNewRole(RoleTypeId.None, RoleChangeReason.None);
            }
            catch (Exception arg)
            {
                Log.Debug($"Ignore: {arg}");
            }

            if (RecyclablePlayerId.FreeIds.Contains(id))
            {
                RecyclablePlayerId.FreeIds.RemoveFromQueue(id);
            }
            else if (RecyclablePlayerId._autoIncrement >= id)
            {
                id = ++RecyclablePlayerId._autoIncrement;
            }

            NetworkServer.AddPlayerForConnection(new FakeConnection(id), gameObject);
            try
            {
                npc.ReferenceHub.authManager.SyncedUserId = null;
            }
            catch (Exception e)
            {
                Log.Debug($"Ignore: {e}");
            }

            npc.ReferenceHub.nicknameSync.Network_myNickSync = name;
            Player.Dictionary.Add(gameObject, npc);
            Timing.CallDelayed(0.3f, delegate
            {
                npc.Role.Set(role, SpawnReason.ForceClass, position.HasValue ? RoleSpawnFlags.AssignInventory : RoleSpawnFlags.All);
                npc.ClearInventory();
            });
            if (position.HasValue)
            {
                Timing.CallDelayed(0.5f, delegate
                {
                    npc.Position = position.Value;
                });
            }

            return npc;
        }

        public bool CheckPetRateLimited(int playerId)
        {
            if (PetRatelimit.TryGetValue(playerId, out DateTime undoTime))
            {
                return (undoTime - DateTime.Now).TotalSeconds > 0d;
            }

            return false;
        }

        public void PetRateLimitPlayer(int playerId, double time)
        {
            PetRatelimit[playerId] = DateTime.Now.AddSeconds(time);
        }

        public override void RegisterCosmetic()
        {
            PlayerEvents.ChangingRole += EventChangingRole;
            PlayerEvents.Died += EventDied;
            PlayerEvents.TriggeringTesla += EventTriggeringTesla;
            PlayerEvents.Handcuffing += EventHandcuffing;
            PlayerEvents.DroppingItem += EventDroppingItem;
            PlayerEvents.MakingNoise += EventMakingNoise;
            PlayerEvents.Escaping += EventEscaping;
            base.RegisterCosmetic();
        }

        public override void UnregisterCosmetic()
        {
            PlayerEvents.ChangingRole -= EventChangingRole;
            PlayerEvents.Died -= EventDied;
            PlayerEvents.TriggeringTesla -= EventTriggeringTesla;
            PlayerEvents.Handcuffing -= EventHandcuffing;
            PlayerEvents.DroppingItem -= EventDroppingItem;
            PlayerEvents.MakingNoise -= EventMakingNoise;
            PlayerEvents.Escaping -= EventEscaping;
            base.UnregisterCosmetic();
        }

        public void EventChangingRole(ChangingRoleEventArgs args)
        {
            Timing.CallDelayed(0.1f, () =>
            {
                if (args.Player.GameObject.TryGetComponent(out PetComponent oldComponent))
                {
                    if (args.Player.ReferenceHub.roleManager.CurrentRole is not FpcStandardRoleBase)
                    {
                        UnityEngine.Object.Destroy(oldComponent);
                    }
                    else
                    {
                        if (Plugin.Instance.Config.PetsMirrorClass)
                        {
                            oldComponent.PetNPC.Role.Set(args.NewRole);
                        }
                    }
                }
            });
        }
        public void EventDied(DiedEventArgs args)
        {
            if (args.Player.GameObject.TryGetComponent(out PetComponent oldComponent))
            {
                UnityEngine.Object.Destroy(oldComponent);
            }
            if (PlayerLinkedCosmetics.ContainsKey(args.Player.UserId))
            {
                PlayerLinkedCosmetics.Remove(args.Player.UserId);
            }
        }

        public void EventTriggeringTesla(TriggeringTeslaEventArgs args)
        {
            try
            {
                if (args.Player == null)
                    return;

                if (IsPet(args.Player))
                {
                    args.IsAllowed = false;
                }
            }
            catch (Exception err)
            {
                if (Plugin.Instance.Config.Debug) Log.Error(err);
            }
        }

        public void EventHandcuffing(HandcuffingEventArgs args)
        {
            try
            {
                if (args.Target == null)
                    return;

                if (IsPet(args.Target))
                {
                    args.IsAllowed = false;
                }
            }
            catch (Exception err)
            {
                if (Plugin.Instance.Config.Debug) Log.Error(err);
            }
        }

        public void EventDroppingItem(DroppingItemEventArgs args)
        {
            try
            {
                if (args.Player == null)
                    return;

                if (IsPet(args.Player))
                {
                    args.IsAllowed = false;
                }
            }
            catch (Exception err)
            {
                if (Plugin.Instance.Config.Debug) Log.Error(err);
            }
        }

        public void EventMakingNoise(MakingNoiseEventArgs args)
        {
            try
            {
                if (args.Player == null)
                    return;

                if (IsPet(args.Player))
                {
                    args.IsAllowed = false;
                }
            }
            catch (Exception err)
            {
                if (Plugin.Instance.Config.Debug) Log.Error(err);
            }
        }

        public void EventEscaping(EscapingEventArgs args)
        {
            try
            {
                if (args.Player == null)
                    return;

                if (IsPet(args.Player))
                {
                    args.IsAllowed = false;
                }
            }
            catch (Exception err)
            {
                if (Plugin.Instance.Config.Debug) Log.Error(err);
            }
        }
    }
}
