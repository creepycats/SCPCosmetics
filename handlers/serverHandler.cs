using Exiled.API.Features;
using Exiled.Events;
using Exiled.Events.EventArgs;
using System.Collections.Generic;

namespace SCPHats.handlers
{
    public class serverHandler
    {
        private readonly Config.Config config;

        public serverHandler(Config.Config config) => this.config = config;
        public void WaitingForPlayers() {
            SCPHats.Instance.HatItems = new List<Types.HatItemComponent>(); // Resets list after each round to hopefully minimize cross round lag
        }
    }
}
