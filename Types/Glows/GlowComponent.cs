namespace SCPCosmetics.Types
{
    using Exiled.API.Features;
    using Exiled.API.Features.Toys;
    using MEC;
    using PlayerRoles;
    using SCPCosmetics;
    using SCPCosmetics.Types.Glows;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class GlowComponent : MonoBehaviour
    {
        internal Exiled.API.Features.Toys.Light glowLight;

        internal GlowColorMode reflectClass = GlowColorMode.Color;

        private bool _threw = false;

        private void Start()
        {
            Timing.RunCoroutine(MoveGlow().CancelWith(this).CancelWith(gameObject));
        }

        private IEnumerator<float> MoveGlow()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(.05f);

                try
                {
                    if (glowLight == null || glowLight.AdminToyBase == null) continue;

                    var player = Player.Get(gameObject);

                    if (player.Role == RoleTypeId.None || player.Role == RoleTypeId.Spectator || player.CurrentRoom.RoomLightController._flickerDuration > 0f)
                    {
                        glowLight.Position = Vector3.one * 6000f;

                        continue;
                    }

                    glowLight.Position = player.Position + new Vector3(0f, -0.8f, 0f);

                    if (reflectClass == GlowColorMode.Class)
                    {
                        glowLight.Color = player.Role.Color;
                    } 
                    else if (reflectClass == GlowColorMode.Rainbow)
                    {
                        float amountToShift = 0.2f * Time.deltaTime;
                        glowLight.Color = SCPCosmetics.Glows.ShiftHueBy(glowLight.Color, amountToShift);
                    }
                }
                catch (Exception e)
                {
                    if (!_threw)
                    {
                        Log.Error(e.ToString());
                        _threw = true;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (glowLight != null && glowLight.AdminToyBase != null)
            {
                Destroy(glowLight.AdminToyBase.gameObject);
            }
        }
    }
}