namespace SCPCosmetics.Commands
{
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;
    using System;

    [CommandHandler(typeof(ClientCommandHandler))]
    public class PetCommand : ParentCommand
    {
        public PetCommand() => LoadGeneratedCommands();

        public override string Command { get; } = "pet";

        public override string[] Aliases { get; } = { "pets" };

        public override string Description { get; } = "Allows supporters to have a pet follow them around";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new Pet.RemovePetCommand());
            RegisterCommand(new Pet.SpawnPetCommand());
            RegisterCommand(new Pet.PetNameCommand());
            RegisterCommand(new Pet.PetItemCommand());
            RegisterCommand(new Pet.PetModelCommand());
            RegisterCommand(new Pet.PetClassCommand());
            RegisterCommand(new Pet.PetDebugCommand());
            RegisterCommand(new Pet.PetClearCommand());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not PlayerCommandSender)
            {
                response = "This command can only be ran by a player!";
                return true;
            }
            if (!Plugin.Instance.Config.EnablePets)
            {
                response = "Pets are disabled on this server.";
                return false;
            }

            if (!sender.CheckPermission("scpcosmetics.pet"))
            {
                response = "You do not have access to the Pet command!";
                return false;
            }

            if (arguments.Count < 1)
            {
                response = "Usage: .pet off/disable/remove/spawn/name/item/model/class/clear | Example: .pet spawn / .pet name <valid-role-color> <name> / .pet item <item>";
                return false;
            }

            response = "Invalid syntax - see .pet for all possible commands.";
            return false;
        }
    }
}
