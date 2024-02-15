using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using InventorySystem.Items.Pickups;
using MapEditorReborn.API.Extensions;
using MapEditorReborn.API.Features.Objects;
using Mirror;
using PlayerRoles;
using SCPCosmetics.Cosmetics.Extensions;
using SCPCosmetics.Types;
using SCPCosmetics.Types.Glows;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCPCosmetics.Cosmetics.Glows
{
    public class GlowComponent : CosmeticComponent
    {
        public Exiled.API.Features.Toys.Light GlowLight;

        public GlowColorMode ColorMode = GlowColorMode.Color;

        public List<string> _invisibleTo = new List<string>();
        public List<string> LightInvisibleTo
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

            if (GlowLight == null || GlowLight.AdminToyBase == null) Destroy(this);

            Player player = Player.Get(gameObject);

            if (player.Role == RoleTypeId.None || player.Role == RoleTypeId.Spectator || player.CurrentRoom.RoomLightController._flickerDuration > 0f)
            {
                GlowLight.Position = Vector3.one * 6000f;
            } else
            {
                GlowLight.Position = player.Position + new Vector3(0f, -0.8f, 0f);

                if (ColorMode == GlowColorMode.Class)
                {
                    GlowLight.Color = player.Role.Color;
                }
                else if (ColorMode == GlowColorMode.Rainbow)
                {
                    float amountToShift = 0.05f * Time.deltaTime;
                    GlowLight.Color = GlowsHandler.ShiftHueBy(GlowLight.Color, amountToShift);
                }

                foreach (Player plr in Player.List)
                {
                    if (this.IsVisibleToPlayer(plr))
                    {
                        if (LightInvisibleTo.Contains(plr.UserId))
                        {
                            plr.SpawnNetworkIdentity(GlowLight.Base.netIdentity);
                            LightInvisibleTo.Remove(plr.UserId);
                        }
                    }
                    else
                    {
                        if (!LightInvisibleTo.Contains(plr.UserId))
                        {
                            plr.DestroyNetworkIdentity(GlowLight.Base.netIdentity);
                            LightInvisibleTo.Add(plr.UserId);
                        }
                    }
                }
            }
        }

        // DESTROY
        public override void OnDestroy()
        {
            if (GlowLight != null && GlowLight.AdminToyBase != null)
                Destroy(GlowLight.AdminToyBase.gameObject);

            base.OnDestroy();
        }
    }
}
