namespace SCPCosmetics.Commands.Pet
{
    using CommandSystem;
    using Exiled.API.Features;
    using NorthwoodLib.Pools;
    using System;
    using System.Text;

    public class PetDebugCommand : ICommand
    {
        public string Command => "debug";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "Debug command for SCPCosmetics pets.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            StringBuilder builder = StringBuilderPool.Shared.Rent();

            //builder.Append($"Pet Debug\nNumber of Pets In Play: {Plugin.Instance.PetDictionary.Values.Count}\nPets in Play:\n");

            //foreach (string petId in Plugin.Instance.PetDictionary.Keys)
            //{
            //    Npc pet = Plugin.Instance.PetDictionary[petId];

            //    if (pet == null)
            //        continue;

            //    builder.AppendLine($"{pet.RankName} - {pet.Nickname} - {pet.Id} - {petId}");
            //}

            response = StringBuilderPool.Shared.ToStringReturn(builder);
            return true;
        }
    }
}
