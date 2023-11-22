namespace SCPCosmetics
{
    using CentralAuth;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Components;
    using Exiled.API.Features.Items;
    using MEC;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps.Subroutines;
    using PlayerRoles.Subroutines;
    using PlayerStatsSystem;
    using SCPCosmetics.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public static class Pets
    {
        static Pets()
        {
            ServerRoles serverRoles = NetworkManager.singleton.playerPrefab.GetComponent<ServerRoles>();

            List<string> allowedColors = new(serverRoles.NamedColors.Length);

            foreach (ServerRoles.NamedColor namedColor in serverRoles.NamedColors)
            {
                if (namedColor.Restricted)
                    continue;

                allowedColors.Add(namedColor.Name);
            }

            allowedPetNameColors = allowedColors;
        }

        public static readonly IReadOnlyDictionary<string, ItemType> allowedPetItems = new Dictionary<string, ItemType>()
        {
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

        public static readonly IReadOnlyList<string> allowedPetNameColors;

        public static bool RemovePetForPlayer(Player player)
        {
            if (Plugin.Instance.PetDictionary.TryGetValue($"pet-{player.UserId}", out Npc foundPet))
            {
                foundPet.ClearInventory();
                foundPet.GameObject.GetComponent<PetComponent>().stopRunning = true;
                foundPet.Position = new Vector3(-9999f, -9999f, -9999f);
                Timing.CallDelayed(0.5f, () =>
                {
                    NetworkServer.Destroy(foundPet.GameObject);
                    Plugin.Instance.PetDictionary.Remove($"pet-{player.UserId}");
                });
                return true;
            }

            return false;
        }

        public static bool IsPet(ReferenceHub refHub)
        {
            return Plugin.Instance.PetDictionary.Values.Contains(Player.Get(refHub));
        }

        public static bool IsPet(SubroutineBase targetTrack)
        {
            return targetTrack.Role.TryGetOwner(out ReferenceHub refHub)
                    && Plugin.Instance.PetDictionary.Values.Contains(Player.Get(refHub));
        }

        public static void SpawnPet(string Name, string Color, RoleTypeId Role, ItemType? HeldItem, Player target, Vector3 scale)
        {
            //SCPCosmetics.Instance.PetIDNumber++;
            Npc SpawnedPet = SpawnFix($"{target.Nickname}'s Pet", Role, Plugin.Instance.PetIDNumber);
            Plugin.Instance.PetDictionary.Add($"pet-{target.UserId}", SpawnedPet);

            SpawnedPet.Scale = scale;
            SpawnedPet.ReferenceHub.characterClassManager._godMode = true;
            SpawnedPet.ReferenceHub.playerStats.GetModule<AdminFlagsStat>().SetFlag(AdminFlags.GodMode, true);
            SpawnedPet.MaxHealth = 9999;
            SpawnedPet.Health = 9999;

            SpawnedPet.RankName = Name;
            SpawnedPet.RankColor = Color;

            Round.IgnoredPlayers.Add(SpawnedPet.ReferenceHub);

            PetComponent petComponent = SpawnedPet.GameObject.AddComponent<PetComponent>();
            petComponent.PetNPC = SpawnedPet;
            petComponent.Owner = target;

            if (!(HeldItem == null || HeldItem == ItemType.None))
            {
                Timing.CallDelayed(0.5f, () =>
                {
                    SpawnedPet.ClearInventory();
                    SpawnedPet.CurrentItem = Item.Create((ItemType)HeldItem, SpawnedPet);
                });
            }
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
                npc.ReferenceHub.authManager.SyncedUserId = "ID_Dedicated";
                npc.ReferenceHub.authManager.InstanceMode = ClientInstanceMode.DedicatedServer;
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

        // source for this method: o5zereth on discord
        public static (ushort horizontal, ushort vertical) ToClientUShorts(Quaternion rotation)
        {
            if (rotation.eulerAngles.z != 0f)
            {
                rotation = Quaternion.LookRotation(rotation * Vector3.forward, Vector3.up);
            }

            float outfHorizontal = rotation.eulerAngles.y;
            float outfVertical = -rotation.eulerAngles.x;

            if (outfVertical < -90f)
            {
                outfVertical += 360f;
            }
            else if (outfVertical > 270f)
            {
                outfVertical -= 360f;
            }

            return (ToHorizontal(outfHorizontal), ToVertical(outfVertical));

            static ushort ToHorizontal(float horizontal)
            {
                const float ToHorizontal = 65535f / 360f;

                horizontal = Mathf.Clamp(horizontal, 0f, 360f);

                return (ushort)Mathf.RoundToInt(horizontal * ToHorizontal);
            }

            static ushort ToVertical(float vertical)
            {
                const float ToVertical = 65535f / 176f;

                vertical = Mathf.Clamp(vertical, -88f, 88f) + 88f;

                return (ushort)Mathf.RoundToInt(vertical * ToVertical);
            }
        }

        public static void LookAt(Npc npc, Vector3 position)
        {
            Vector3 direction = position - npc.Position;
            Quaternion quat = Quaternion.LookRotation(direction, Vector3.up);
            var mouseLook = ((IFpcRole)npc.ReferenceHub.roleManager.CurrentRole).FpcModule.MouseLook;
            (ushort horizontal, ushort vertical) = ToClientUShorts(quat);
            mouseLook.ApplySyncValues(horizontal, vertical);
        }
    }
}

