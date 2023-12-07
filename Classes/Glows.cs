namespace SCPCosmetics
{
    using Exiled.API.Features;
    using Exiled.API.Features.Toys;
    using Mirror;
    using PlayerRoles;
    using SCPCosmetics.Types;
    using System.Collections.Generic;
    using UnityEngine;
    using YamlDotNet.Core.Tokens;

    public static class Glows
    {
        public static bool ShouldRemoveGlow(RoleTypeId _rtid)
        {
            return (RoleTypeId.Scp079 == _rtid) || (Plugin.Instance.Config.RemoveGlowsOnDeath && (_rtid == RoleTypeId.None || _rtid == RoleTypeId.Spectator || _rtid == RoleTypeId.Overwatch || _rtid == RoleTypeId.Filmmaker));
        }

        public static GlowComponent SpawnGlow(Player player, Color color)
        {
            GlowComponent glowComp = player.GameObject.AddComponent<GlowComponent>();

            glowComp.glowLight = Exiled.API.Features.Toys.Light.Create(player.Position, null, null, true, color);
            glowComp.glowLight.ShadowEmission = false;
            glowComp.glowLight.Range = 1.15f;
            glowComp.glowLight.Intensity = 5f;

            Plugin.Instance.GlowDictionary.Add(player.UserId, glowComp);

            return glowComp;
        }

        public static bool RemoveGlowForPlayer(string userId)
        {
            if (Plugin.Instance.GlowDictionary.TryGetValue(userId, out GlowComponent foundGlow))
            {
                NetworkServer.Destroy(foundGlow.glowLight.AdminToyBase.gameObject);
                Plugin.Instance.GlowDictionary.Remove(userId);
                return true;
            }

            return false;
        }
        public static bool RemoveGlowForPlayer(Player player)
        {
            return RemoveGlowForPlayer(player.UserId);
        }

        public static Color ShiftHueBy(Color color, float amount)
        {
            Color.RGBToHSV(color, out float hue, out float sat, out float val);
            return Color.HSVToRGB(hue + amount, sat, val);
        }
    }
}
