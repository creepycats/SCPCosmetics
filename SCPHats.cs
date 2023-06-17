using Exiled.API.Features;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;
using System;
using System.Collections.Generic;

namespace SCPHats
{
    public class SCPHats : Plugin<Config.Config>
    {
        public override string Name => "SCPHats";
        public override string Author => "creepycats";
        public override Version Version => new Version(1, 0, 1);

        public static SCPHats Instance { get; set; }

        public List<Types.HatItemComponent> HatItems { get; set; } = new List<Types.HatItemComponent>();

        public override void OnEnabled()
        {
            Instance = this;
            Log.Info("SCPHats v" + Version + " - made for v13 by creepycats");
            if (Config.Debug)
                Log.Info("Registering events...");
            RegisterEvents();
            HatItems = new List<Types.HatItemComponent>();
            Log.Info("Plugin Enabled!");
        }
        public override void OnDisabled()
        {
            if (Config.Debug)
                Log.Info("Unregistering events...");
            UnregisterEvents();
            HatItems = new List<Types.HatItemComponent>();
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
        }
        public void UnregisterEvents()
        {
            Server.WaitingForPlayers -= ServerHandler.WaitingForPlayers;

            Player.ChangingRole -= PlayerHandler.ChangingRole;
            Player.SearchingPickup -= PlayerHandler.SearchingPickup;
            Player.Died -= PlayerHandler.Died;
        }
    }
}