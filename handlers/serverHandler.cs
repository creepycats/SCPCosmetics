namespace SCPCosmetics.handlers
{
    public class ServerHandler
    {
        private readonly Config.Config config;

        public ServerHandler(Config.Config config) => this.config = config;

        public void WaitingForPlayers()
        {
            Plugin.Instance.HatItems?.Clear();
            Plugin.Instance.PetRatelimit?.Clear();
        }
    }
}
