namespace SCPCosmetics.Types
{
    using Exiled.API.Features;
    using MEC;
    using PlayerRoles.FirstPersonControl;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class PetComponent : MonoBehaviour
    {
        internal Npc PetNPC;
        internal Player Owner;

        private bool _threw;
        public bool stopRunning = false;

        private void Start()
        {
            Timing.RunCoroutine(MovePet().CancelWith(this).CancelWith(gameObject));
        }

        private IEnumerator<float> MovePet()
        {
            while (!stopRunning)
            {
                yield return Timing.WaitForSeconds(0.1f);

                try
                {
                    if (Owner == null || PetNPC == null)
                    {
                        Destroy(this);
                    };

                    var distance = Vector3.Distance(Owner.Position, PetNPC.Position);
                    PetNPC.MaxHealth = 9999;
                    PetNPC.Health = 9999;
                    PetNPC.IsGodModeEnabled = true;
                    PetNPC.Stamina = 99;
                    try
                    {
                        Vector3 _targetPos = Owner.CameraTransform.position;
                        _targetPos.y = PetNPC.Position.y;
                        Pets.LookAt(PetNPC, _targetPos);
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
                                var pos = PetNPC.Position + PetNPC.ReferenceHub.PlayerCameraReference.forward / 20 * ownerFPC.FpcModule.SprintSpeed;
                                PetNPC.Position = pos;
                            }
                        }
                        else
                        {
                            if (distance > 10f)
                                PetNPC.Position = Owner.Position;

                            else if (distance > 2f)
                            {
                                var pos = PetNPC.Position + PetNPC.ReferenceHub.PlayerCameraReference.forward / 10 * ownerFPC.FpcModule.SprintSpeed;
                                PetNPC.Position = pos;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (!_threw)
                    {
                        //Log.Error(e.ToString());
                        _threw = true;
                    }
                }
            }
        }

        /*
        private void OnDestroy()
        {
            if (item != null && item.gameObject != null)
            {
                if (item.isSchematic)
                {
                    try
                    {
                        item.hatSchematic.Destroy();
                    }
                    catch (Exception e)
                    {
                        Log.Error("Couldn't Remove a Hat Schematic - " + e.ToString());
                    }
                }
                UnityEngine.Object.Destroy(item.gameObject);
            }
        }*/
    }
}