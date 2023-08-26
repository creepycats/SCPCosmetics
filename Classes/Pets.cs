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
using SCPCosmetics.Types;
using System;
using System.Linq;
using UnityEngine;

namespace SCPCosmetics
{
    public class Pets
    {
        //public static IEnumerator<float> RemovePetItemsDropped()
        //{
        //    for (; ; )
        //    {
        //        foreach (Item item in Item.List)
        //        {
        //            try
        //            {
        //                var pickupInfo = _hatItem.item.NetworkInfo;
        //                pickupInfo.Locked = true;
        //                _hatItem.item.NetworkInfo = pickupInfo;
        //            }
        //            catch (Exception e) { }
        //        }
        //        yield return Timing.WaitForSeconds(0.1f);
        //    }
        //}
        public static bool IsPet(ReferenceHub refHub)
        {
            return SCPCosmetics.Instance.PetDictionary.Values.Contains(Player.Get(refHub));
        }
        public static bool IsPet(ScpSubroutineBase targetTrack)
        {
            ReferenceHub refHub;
            targetTrack.Role.TryGetOwner(out refHub);
            return SCPCosmetics.Instance.PetDictionary.Values.Contains(Player.Get(refHub));
        }
        
        public static void SpawnPet(string Name, string Color, RoleTypeId Role, ItemType? HeldItem, Player target, Vector3 scale)
        {
            //SCPCosmetics.Instance.PetIDNumber++;
            Npc SpawnedPet = SpawnFix($"{target.Nickname}'s Pet", Role, SCPCosmetics.Instance.PetIDNumber);
            SCPCosmetics.Instance.PetDictionary.Add($"pet-{target.UserId}", SpawnedPet);

            SpawnedPet.Scale = scale;
            SpawnedPet.IsGodModeEnabled = true;
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
            Npc npc = new Npc(gameObject)
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
                npc.ReferenceHub.characterClassManager.SyncedUserId = "ID_Dedicated";
                npc.ReferenceHub.characterClassManager.InstanceMode = ClientInstanceMode.DedicatedServer;
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

