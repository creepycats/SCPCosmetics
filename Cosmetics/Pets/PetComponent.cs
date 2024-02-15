using CustomPlayerEffects;
using Exiled.API.Features;
using MapEditorReborn.API.Extensions;
using MapEditorReborn.API.Features.Objects;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using SCPCosmetics.Cosmetics.Extensions;
using SCPCosmetics.Types;
using SCPCosmetics.Types.Glows;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCPCosmetics.Cosmetics.Pets
{
    public class PetComponent : CosmeticComponent
    {
        public Npc PetNPC;

        public SchematicPetConfig ModelConfig;
        public SchematicObject PetModel;
        public List<string> _invisibleTo = new List<string>();
        public List<string> SchematicInvisibleTo
        {
            get
            {
                if (_invisibleTo is not null)
                    _invisibleTo = _invisibleTo.Where(uid => Player.TryGet(uid, out Player plr)).ToList();

                if (_invisibleTo is null)
                    _invisibleTo = new List<string>();

                return _invisibleTo;
            }
            set
            {
                _invisibleTo = value;
            }
        }

        // CALL UPDATE
        public override void UpdateCosmetic()
        {
            base.UpdateCosmetic();

            Player Owner = Player.Get(gameObject);

            if (Owner.TryGetEffect(Exiled.API.Enums.EffectType.Invisible, out StatusEffectBase stEff) && stEff.Intensity > 0)
            {
                if (PetNPC.TryGetEffect(Exiled.API.Enums.EffectType.Invisible, out StatusEffectBase stEff2) && stEff2.Intensity < 1)
                {
                    PetNPC.EnableEffect<Invisible>();
                }
            }
            else
            {
                if (PetModel == null || ModelConfig == null)
                {
                    PetNPC.DisableEffect<Invisible>();
                }
            }

            float distance = Vector3.Distance(Owner.Position, PetNPC.Position);
            PetNPC.MaxHealth = 9999;
            PetNPC.Health = 9999;
            PetNPC.IsGodModeEnabled = true;
            PetNPC.Stamina = 99;
            try
            {
                Vector3 _targetPos = Owner.CameraTransform.position;
                _targetPos.y = PetNPC.Position.y;
                PetNPC.LookAt(_targetPos);
            }
            catch (Exception e) { }

            if (Owner.ReferenceHub.roleManager.CurrentRole is FpcStandardRoleBase ownerFPC && ownerFPC.FpcModule.ModuleReady && PetNPC.ReferenceHub.roleManager.CurrentRole is FpcStandardRoleBase petFPC && petFPC.FpcModule.ModuleReady)
            {
                if (ownerFPC.FpcModule.CurrentMovementState == PlayerMovementState.Sneaking)
                {
                    petFPC.FpcModule.CurrentMovementState = PlayerMovementState.Sneaking;
                }
                else
                {
                    petFPC.FpcModule.CurrentMovementState = PlayerMovementState.Sprinting;
                }

                if (petFPC.FpcModule.CurrentMovementState == PlayerMovementState.Sneaking)
                {
                    if (distance > 5f) PetNPC.Position = Owner.Position;

                    else if (distance > 1f)
                    {
                        Vector3 pos = PetNPC.Position + PetNPC.ReferenceHub.PlayerCameraReference.forward * ownerFPC.FpcModule.SneakSpeed * Time.deltaTime;
                        PetNPC.Position = pos;
                    }
                }
                else
                {
                    if (distance > 10f)
                        PetNPC.Position = Owner.Position;

                    else if (distance > 2f)
                    {
                        Vector3 pos = PetNPC.Position + PetNPC.ReferenceHub.PlayerCameraReference.forward * ownerFPC.FpcModule.SprintSpeed * Time.deltaTime;
                        PetNPC.Position = pos;
                    }
                }

                if (PetModel != null && ModelConfig != null)
                {
                    PetModel.Position = PetNPC.Position + ModelConfig.Position;
                    PetModel.Rotation = Quaternion.Euler(PetNPC.Rotation.eulerAngles + ModelConfig.Rotation);
                    PetModel.Scale = ModelConfig.Scale;

                    foreach (Player plr in Player.List)
                    {
                        if (this.IsVisibleToPlayer(plr))
                        {
                            if (SchematicInvisibleTo.Contains(plr.UserId))
                            {
                                plr.SpawnSchematic(PetModel);
                                SchematicInvisibleTo.Remove(plr.UserId);
                            }
                        }
                        else
                        {
                            if (!SchematicInvisibleTo.Contains(plr.UserId))
                            {
                                plr.DestroySchematic(PetModel);
                                SchematicInvisibleTo.Add(plr.UserId);
                            }
                        }
                    }
                }
            }
        }

        // DESTROY
        public override void OnDestroy()
        {
            if (PetNPC != null)
            {
                try
                {
                    PetNPC.ClearInventory();
                    PetNPC.Position = new Vector3(-9999f, -9999f, -9999f);
                    Timing.CallDelayed(0.5f, () =>
                    {
                        NetworkServer.Destroy(PetNPC.GameObject);
                    });
                }
                catch (Exception e)
                {
                    Log.Error("Couldn't Remove a Pet NPC - " + e.ToString());
                }
            }

            if (PetModel != null)
            {
                try
                {
                    PetModel.Destroy();
                }
                catch (Exception e)
                {
                    Log.Error("Couldn't Remove a Pet Schematic - " + e.ToString());
                }
            }

            base.OnDestroy();
        }
    }
}
