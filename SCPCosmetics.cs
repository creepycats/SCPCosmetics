using Exiled.API.Features;
using HarmonyLib;
using MEC;
using System;
using System.Collections.Generic;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;

namespace SCPCosmetics
{
    public class SCPCosmetics : Plugin<Config.Config>
    {
        public override string Name => "SCPCosmetics";
        public override string Author => "creepycats";
        public override Version Version => new Version(1, 1, 0);

        public static SCPCosmetics Instance { get; set; }

        public List<Types.HatItemComponent> HatItems { get; set; } = new List<Types.HatItemComponent>();

        public List<string> PetRatelimit { get; set; } = new List<string>();
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
            _harmony = new("SCPCosmetics");
            _harmony.PatchAll();
            Log.Info("Plugin Enabled!");
        }
        public override void OnDisabled()
        {
            if (Config.Debug)
                Log.Info("Unregistering events...");
            UnregisterEvents();
            HatItems = new List<Types.HatItemComponent>();
            Timing.KillCoroutines();
            _harmony.UnpatchAll();
            _harmony = null;
            Log.Info("Disabled Plugin Successfully");
        }

        // NotesToSelf
        // OBJECT.EVENT += FUNCTION > Add Function to Callback
        // OBJECT.EVENT -= FUNCTION > Remove Function from Callback

        private handlers.serverHandler ServerHandler;
        private handlers.playerHandler PlayerHandler;

        public void RegisterEvents() 
        {
            ServerHandler = new handlers.serverHandler(Config);
            PlayerHandler = new handlers.playerHandler(Config);

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
    }
}