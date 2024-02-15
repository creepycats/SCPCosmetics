namespace SCPCosmetics.Commands.Hat
{
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.Permissions.Extensions;
    using SCPCosmetics.Types;
    using NorthwoodLib.Pools;
    using System;
    using System.Text;

    public class HatDebugCommand : ICommand
    {
        public string Command => "debug";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Debug command for SCPCosmetics hats.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!Player.TryGet(sender, out Player player))
            {
                response = "This command can only be ran by a player!";
                return true;
            }

            if (!Plugin.Instance.Config.EnableHats)
            {
                response = "Hats are disabled on this server.";
                return false;
            }

            if (!sender.CheckPermission("scpcosmetics.hat"))
            {
                response = "You do not have access to the Hat command!";
                return false;
            }

            StringBuilder builder = StringBuilderPool.Shared.Rent();

            //builder.Append($"Hat Debug\nNumber of Hats In Play: {Plugin.Instance.HatItems.Count}\nPlayers With Hats:\n");

            //foreach (HatItemComponent hatItem in Plugin.Instance.HatItems)
            //{
            //    if (!Player.TryGet(hatItem.player.gameObject, out Player hatPlayer))
            //        continue;

            //    builder.AppendLine($"{hatPlayer.Nickname} - {hatPlayer.Id} - {hatPlayer.UserId}");
            //}

            response = StringBuilderPool.Shared.ToStringReturn(builder);
            return true;
        }
    }
}
