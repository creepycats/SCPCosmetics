namespace SCPCosmetics
{
    using Exiled.API.Features;
    using HarmonyLib;
    using MEC;
    using System;
    using System.Collections.Generic;
    using Player = Exiled.Events.Handlers.Player;
    using Server = Exiled.Events.Handlers.Server;

    public class Plugin : Plugin<Config.Config>
    {
        public override string Name => "SCPCosmetics";
        public override string Author => "creepycats";
        public override Version Version => new(1, 1, 0);

        public static Plugin Instance { get; set; }

        public List<Types.HatItemComponent> HatItems { get; set; } = new List<Types.HatItemComponent>();

        public Dictionary<int, DateTime> PetRatelimit { get; set; } = new();
        public Dictionary<string, Npc> PetDictionary { get; set; } = new Dictionary<string, Npc>();
        public int PetIDNumber = 1000;
        private Harmony _harmony;

        public override void OnEnabled()
        {
            Instance = this;
            Log.Info("SCPCosmetics v" + Version + " - made for v13.1.1 by creepycats");

            if (Config.Debug)
                Log.Info("Registering events...");

            RegisterEvents();
            HatItems = new List<Types.HatItemComponent>();
            Timing.RunCoroutine(Hats.LockHats());

            if (_harmony is null)
            {
                _harmony = new("SCPCosmetics");
                _harmony.PatchAll();
            }
        }

        public override void OnDisabled()
        {
            if (Config.Debug)
                Log.Info("Unregistering events...");
            UnregisterEvents();
            HatItems = new List<Types.HatItemComponent>();
            Timing.KillCoroutines();
        }

        private handlers.ServerHandler ServerHandler;
        private handlers.PlayerHandler PlayerHandler;

        public void RegisterEvents()
        {
            ServerHandler = new handlers.ServerHandler(Config);
            PlayerHandler = new handlers.PlayerHandler(Config);

            Server.WaitingForPlayers += ServerHandler.WaitingForPlayers;

            Player.ChangingRole += PlayerHandler.ChangingRole;
            Player.SearchingPickup += PlayerHandler.SearchingPickup;
            Player.Died += PlayerHandler.Died;

            // Pet Events
            Player.TriggeringTesla += PlayerHandler.TriggeringTesla;
            Player.Handcuffing += PlayerHandler.Handcuffing;
            Player.DroppingItem += PlayerHandler.DroppingItem;
            Player.MakingNoise += PlayerHandler.MakingNoise;
            Player.Escaping += PlayerHandler.Escaping;
            Player.Left += PlayerHandler.Left;
        }

        public void UnregisterEvents()
        {
            Server.WaitingForPlayers -= ServerHandler.WaitingForPlayers;

            Player.ChangingRole -= PlayerHandler.ChangingRole;
            Player.SearchingPickup -= PlayerHandler.SearchingPickup;
            Player.Died -= PlayerHandler.Died;

            // Pet Events
            Player.TriggeringTesla -= PlayerHandler.TriggeringTesla;
            Player.Handcuffing -= PlayerHandler.Handcuffing;
            Player.DroppingItem -= PlayerHandler.DroppingItem;
            Player.MakingNoise -= PlayerHandler.MakingNoise;
            Player.Escaping -= PlayerHandler.Escaping;
            Player.Left -= PlayerHandler.Left;
        }

        public bool CheckPetRateLimited(int playerId)
        {
            if (PetRatelimit.TryGetValue(playerId, out DateTime undoTime))
            {
                return (undoTime - DateTime.Now).TotalSeconds > 0d;
            }

            return false;
        }

        public void PetRateLimitPlayer(int playerId, double time)
        {
            PetRatelimit[playerId] = DateTime.Now.AddSeconds(time);
        }
    }
}