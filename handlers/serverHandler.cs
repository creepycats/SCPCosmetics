using Exiled.API.Features;
using Exiled.Events;
using Exiled.Events.EventArgs;

namespace SCPHats.handlers
{
    public class serverHandler
    {
        private readonly Config.Config config;

        public serverHandler(Config.Config config) => this.config = config;
        public void RoundStarted() {
            // Test
        }
    }
}
