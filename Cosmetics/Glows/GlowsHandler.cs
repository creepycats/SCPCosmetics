using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using SCPCosmetics.Cosmetics.Hats;
using SCPCosmetics.Types;
using UnityEngine;
using PlayerEvents = Exiled.Events.Handlers.Player;

namespace SCPCosmetics.Cosmetics.Glows
{
    public class GlowsHandler : CosmeticHandler
    {
        public static bool ShouldRemoveGlow(RoleTypeId _rtid)
        {
            return (RoleTypeId.Scp079 == _rtid) || (Plugin.Instance.Config.RemoveGlowsOnDeath && (_rtid == RoleTypeId.None || _rtid == RoleTypeId.Spectator || _rtid == RoleTypeId.Overwatch || _rtid == RoleTypeId.Filmmaker));
        }

        public static Color ShiftHueBy(Color color, float amount)
        {
            Color.RGBToHSV(color, out float hue, out float sat, out float val);
            return Color.HSVToRGB(hue + amount, sat, val);
        }

        public static GlowComponent SpawnGlow(Player player, Color color)
        {
            GlowsHandler thisHandler = Plugin.Instance.GetCosmeticHandler(typeof(GlowsHandler)) as GlowsHandler;
            if (player.GameObject.TryGetComponent(out GlowComponent oldComponent))
            {
                Object.Destroy(oldComponent);
            }
            if (thisHandler.PlayerLinkedCosmetics.ContainsKey(player.UserId))
            {
                thisHandler.PlayerLinkedCosmetics.Remove(player.UserId);
            }

            GlowComponent glowComp = player.GameObject.AddComponent<GlowComponent>();

            glowComp.GlowLight = Exiled.API.Features.Toys.Light.Create(player.Position, null, null, true, color);
            glowComp.GlowLight.ShadowEmission = false;
            glowComp.GlowLight.Range = 1.15f;
            glowComp.GlowLight.Intensity = 5f;

            thisHandler.PlayerLinkedCosmetics.Add(player.UserId, glowComp);

            return glowComp;
        }

        public override void RegisterCosmetic()
        {
            PlayerEvents.Died += EventDied;
            base.RegisterCosmetic();
        }

        public override void UnregisterCosmetic()
        {
            PlayerEvents.Died -= EventDied;
            base.UnregisterCosmetic();
        }

        public void EventDied(DiedEventArgs args)
        {
            if (Plugin.Instance.Config.RemoveGlowsOnDeath)
            {
                if (args.Player.GameObject.TryGetComponent(out GlowComponent oldComponent))
                {
                    Object.Destroy(oldComponent);
                }
                if (PlayerLinkedCosmetics.ContainsKey(args.Player.UserId))
                {
                    PlayerLinkedCosmetics.Remove(args.Player.UserId);
                }
            }
        }
    }
}
