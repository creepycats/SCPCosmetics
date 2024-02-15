namespace SCPCosmetics
{
    using Exiled.API.Features;
    using HarmonyLib;
    using MEC;
    using SCPCosmetics.Cosmetics.Glows;
    using SCPCosmetics.Cosmetics.Hats;
    using SCPCosmetics.Cosmetics.Pets;
    using SCPCosmetics.Types;
    using System;
    using System.Collections.Generic;

    public class Plugin : Plugin<Config.Config>
    {
        public override string Name => "SCPCosmetics";
        public override string Author => "creepycats";
        public override Version Version => new(2, 0, 1);

        public static Plugin Instance { get; set; }

        public int PetIDNumber = 1000;
        private Harmony _harmony;
        private CoroutineHandle updateLoop;

        public List<CosmeticHandler> CosmeticHandlers = new List<CosmeticHandler>();

        public override void OnEnabled()
        {
            Instance = this;

            // REGISTER NEW COSMETIC HANDLERS HERE
            CosmeticHandlers = new List<CosmeticHandler>()
            {
                new HatsHandler(),
                new GlowsHandler(),
                new PetsHandler(),
            };
            foreach (CosmeticHandler cosmeticHandler in CosmeticHandlers)
            {
                cosmeticHandler.RegisterCosmetic();
            }

            updateLoop = Timing.RunCoroutine(UpdateDistributor());

            if (_harmony is null)
            {
                _harmony = new("SCPCosmetics");
                _harmony.PatchAll();
            }
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            foreach (CosmeticHandler cosmeticHandler in CosmeticHandlers)
            {
                cosmeticHandler.RegisterCosmetic();
            }
            CosmeticHandlers = new List<CosmeticHandler>();

            Timing.KillCoroutines(updateLoop);
            base.OnDisabled();
        }

        public CosmeticHandler GetCosmeticHandler(Type handlerType)
        {
            foreach (CosmeticHandler cosmeticHandler in CosmeticHandlers)
            {
                if (cosmeticHandler.GetType() == handlerType) return cosmeticHandler;
            }
            return null;
        }

        public static IEnumerator<float> UpdateDistributor()
        {
            for (; ; )
            {
                yield return Timing.WaitForOneFrame;

                foreach(CosmeticHandler handler in Instance.CosmeticHandlers)
                {
                    try
                    {
                        handler.DistributeUpdate();
                    } catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }
    }
}
