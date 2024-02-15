using AdminToys;
using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using MapEditorReborn.API.Features;
using MapEditorReborn.Commands.ModifyingCommands.Position;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp096;
using PlayerRoles.PlayableScps.Scp939;
using SCPCosmetics.Cosmetics.Glows;
using SCPCosmetics.Cosmetics.Hats;
using SCPCosmetics.Cosmetics.Pets;
using SCPCosmetics.Types;
using System;
using UnityEngine;

namespace SCPCosmetics.Cosmetics.Extensions
{
    public static class CosmeticExtensions
    {
        public static bool IsVisibleToPlayer(this HatComponent hic, Player player1)
        {
            // IsPlayerNPC is 2nd because it has a lot of null checks.
            if (player1 == null || player1.UserId == null || player1.IsDead) return false;

            Player cosmeticPlayer = Player.Get(hic.gameObject);
            if (cosmeticPlayer.TryGetEffect(Exiled.API.Enums.EffectType.Invisible, out StatusEffectBase stEff) && stEff.Intensity > 0)
            {
                return false;
            }
            else if (player1.ReferenceHub == cosmeticPlayer.ReferenceHub)
            {
                return false;
            }
            else if (PlayerRolesUtils.GetTeam(player1.Role) == PlayerRolesUtils.GetTeam(cosmeticPlayer.Role))
            {
                return true;
            }
            else if (player1.ReferenceHub.roleManager.CurrentRole is FpcStandardRoleBase role)
                switch (player1.Role.Type)
                {
                    case RoleTypeId.Scp939 when role.VisibilityController is Scp939VisibilityController vision && !vision.ValidateVisibility(cosmeticPlayer.ReferenceHub):
                        return false;
                    case RoleTypeId.Scp096 when role.VisibilityController is Scp096VisibilityController vision && !vision.ValidateVisibility(cosmeticPlayer.ReferenceHub):
                        return false;
                    case RoleTypeId.Scp106:
                        return false;
                    default:
                        return true;
                }

            return false;
        }

        public static bool IsVisibleToPlayer(this GlowComponent hic, Player player1)
        {
            // IsPlayerNPC is 2nd because it has a lot of null checks.
            if (player1 == null || player1.UserId == null || player1.IsDead) return false;

            Player cosmeticPlayer = Player.Get(hic.gameObject);
            if (cosmeticPlayer.TryGetEffect(Exiled.API.Enums.EffectType.Invisible, out StatusEffectBase stEff) && stEff.Intensity > 0)
            {
                return false;
            }
            else if (player1.ReferenceHub == cosmeticPlayer.ReferenceHub)
            {
                return true;
            }
            else if (PlayerRolesUtils.GetTeam(player1.Role) == PlayerRolesUtils.GetTeam(cosmeticPlayer.Role))
            {
                return true;
            }
            else if (player1.ReferenceHub.roleManager.CurrentRole is FpcStandardRoleBase role)
                switch (player1.Role.Type)
                {
                    case RoleTypeId.Scp939 when role.VisibilityController is Scp939VisibilityController vision && !vision.ValidateVisibility(cosmeticPlayer.ReferenceHub):
                        return false;
                    case RoleTypeId.Scp096 when role.VisibilityController is Scp096VisibilityController vision && !vision.ValidateVisibility(cosmeticPlayer.ReferenceHub):
                        return false;
                    case RoleTypeId.Scp106:
                        return false;
                    default:
                        return true;
                }

            return false;
        }

        public static bool IsVisibleToPlayer(this PetComponent pc, Player player1)
        {
            // IsPlayerNPC is 2nd because it has a lot of null checks.
            if (player1 == null || player1.UserId == null || player1.IsDead) return false;

            Player Owner = Player.Get(pc.gameObject);
            if (Owner.TryGetEffect(Exiled.API.Enums.EffectType.Invisible, out StatusEffectBase stEff) && stEff.Intensity > 0)
            {
                return false;
            }
            else if (player1 == pc.PetNPC)
            {
                return true;
            }
            else if (PlayerRolesUtils.GetTeam(player1.Role) == PlayerRolesUtils.GetTeam(pc.PetNPC.Role))
            {
                return true;
            }
            else if (player1.ReferenceHub.roleManager.CurrentRole is FpcStandardRoleBase role)
                switch (player1.Role.Type)
                {
                    case RoleTypeId.Scp939 when role.VisibilityController is Scp939VisibilityController vision && !vision.ValidateVisibility(pc.PetNPC.ReferenceHub):
                        return false;
                    case RoleTypeId.Scp096 when role.VisibilityController is Scp096VisibilityController vision && !vision.ValidateVisibility(pc.PetNPC.ReferenceHub):
                        return false;
                    case RoleTypeId.Scp106:
                        return false;
                    default:
                        return true;
                }

            return false;
        }

        public static void LookAt(this Npc npc, Vector3 position)
        {
            if (npc.Role is FpcRole fpc)
            {
                Vector3 direction = position - npc.Position;
                Quaternion quat = Quaternion.LookRotation(direction, Vector3.up);
                npc.LookAt(quat);
            }
        }

        public static void LookAt(this Npc npc, Quaternion rotation)
        {
            if (npc.Role is not FpcRole fpc)
                return;

            if (rotation.eulerAngles.z != 0f)
                rotation = Quaternion.LookRotation(rotation * Vector3.forward, Vector3.up);

            Vector2 angles = new Vector2(-rotation.eulerAngles.x, rotation.eulerAngles.y);

            ushort hor = (ushort)Mathf.RoundToInt(Mathf.Repeat(angles.y, 360f) * (ushort.MaxValue / 360f));
            ushort vert = (ushort)Mathf.RoundToInt(Mathf.Clamp(Mathf.Repeat(angles.x + 90f, 360f) - 2f, 0f, 176f) * (ushort.MaxValue / 176f));

            fpc.FirstPersonController.FpcModule.MouseLook.ApplySyncValues(hor, vert);
        }

        public static void ChangePetModel(this PetComponent petComp, SchematicPetConfig petModel)
        {
            if (MapUtils.GetSchematicDataByName(petModel.SchematicName) == null)
                return;

            if (petComp == null)
                return;

            if (petComp.PetModel != null)
            {
                try
                {
                    petComp.PetModel.Destroy();
                }
                catch (Exception e)
                {
                    Log.Error("Couldn't Remove a Pet Schematic - " + e.ToString());
                }
            }

            petComp.PetModel = ObjectSpawner.SpawnSchematic(petModel.SchematicName, Vector3.zero, Quaternion.identity, petModel.Scale, isStatic: false);
            foreach (AdminToyBase atb in petComp.PetModel.AdminToyBases)
            {
                atb.MovementSmoothing = 60;
            }
            petComp.ModelConfig = petModel;

            petComp.PetNPC.EnableEffect<Invisible>();
        }

        public static void ChangePetModel(this PetComponent petComp)
        {
            if (petComp == null)
                return;

            try
            {
                petComp.PetModel.Destroy();
            }
            catch (Exception e)
            {
                Log.Error("Couldn't Remove a Pet Schematic - " + e.ToString());
            }

            petComp.PetModel = null;
            petComp.ModelConfig = null;

            petComp.PetNPC.DisableEffect<Invisible>();
        }
    }
}
