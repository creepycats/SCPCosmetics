using System.Collections.Generic;

namespace SCPCosmetics.handlers
{
    public class serverHandler
    {
        private readonly Config.Config config;

        public serverHandler(Config.Config config) => this.config = config;
        public void WaitingForPlayers() {
            SCPCosmetics.Instance.HatItems = new List<Types.HatItemComponent>(); // Resets list after each round to hopefully minimize cross round lag
            SCPCosmetics.Instance.PetRatelimit = new List<string>(); // Resets list after each round to prevent infinite ratelimits
        }
    }
}
