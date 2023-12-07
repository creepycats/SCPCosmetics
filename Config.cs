namespace SCPCosmetics.Config
{
    using Exiled.API.Interfaces;
    using SCPCosmetics.Types;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class Config : IConfig
    {

        // Required Config
        /// <summary>
        ///  Will the plugin run?
        /// </summary>
        public bool IsEnabled { get; set; } = true;
        /// <summary>
        ///  Will the plugin print Debug Text?
        /// </summary>
        public bool Debug { get; set; } = false;
        /// <summary>
        ///  Will players be able to wear hats?
        /// </summary>
        [Description("Will players be able to wear hats?")]
        public bool EnableHats { get; set; } = true;
        /// <summary>
        /// <summary>
        ///  Will hats be deleted when players die?
        /// </summary>
        // </summary>
        [Description("Will hats be deleted when players die?")]
        public bool RemoveHatsOnDeath { get; set; } = true;
        /// <summary>
        ///  Will players be able to use MapEditorReborn schematics as hats?
        /// </summary>
        [Description("Will players be able to use MapEditorReborn schematics as hats?")]
        public bool SchematicHats { get; set; } = false;
        /// <summary>
        ///  List of Schematic Hats
        /// </summary>
        [Description("List of Schematic Hats")]
        public Dictionary<string, SchematicHatConfig> SchematicHatList { get; set; } = new Dictionary<string, SchematicHatConfig>()
        {
            ["example"] = new SchematicHatConfig()
            {
                RequiredPermissions = new List<string>() { "scpcosmetics.test" }
            }
        };
        /// <summary>
        ///  Will players be able to spawn pets?
        /// </summary>
        [Description("Will players be able to spawn pets?")]
        public bool EnablePets { get; set; } = true;
        /// <summary>
        ///  Will pets mirror their owners' class? RECOMMENDED: True - Prevents SCP-096 and SCP-173 issues
        /// </summary>
        [Description("Will pets mirror their owners' class? RECOMMENDED: True - Prevents SCP-096 and SCP-173 issues")]
        public bool PetsMirrorClass { get; set; } = true;
        /// <summary>
        ///  Will pets mirror their owners' class? RECOMMENDED: True - Prevents SCP-096 and SCP-173 issues
        /// </summary>
        [Description("Will players be able to change their pet's class? RECOMMENDED: False - Prevents SCP-096 and SCP-173 issues")]
        public bool PetClassCommandEnabled { get; set; } = false;
        /// <summary>
        ///  Will players be able to name their pets?
        /// </summary>
        [Description("Will players be able to name their pets?")]
        public bool NamePets { get; set; } = true;
        /// <summary>
        ///  Will players be able to give their pet an item to hold?
        /// </summary>
        [Description("Will players be able to give their pet an item to hold?")]
        public bool PetsCanHoldItems { get; set; } = true;
        /// <summary>
        ///  Will players be able to wear Glows?
        /// </summary>
        [Description("Will players be able to wear Glows?")]
        public bool EnableGlows { get; set; } = true;
        /// <summary>
        /// <summary>
        ///  Will glows be deleted when players die?
        /// </summary>
        // </summary>
        [Description("Will glows be deleted when players die?")]
        public bool RemoveGlowsOnDeath { get; set; } = true;
    }
}
