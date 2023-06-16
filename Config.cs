using Exiled.API.Interfaces;
using SCPHats.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SCPHats.Config
{
    public class Config : IConfig
    {

        // Required Config
        // <summary>
        //  Will the plugin run?
        // </summary>
        public bool IsEnabled { get; set; } = true;
        // <summary>
        //  Will the plugin print Debug Text?
        // </summary>
        public bool Debug { get; set; } = false;
        // <summary>
        //  Will players be able to use MapEditorReborn schematics as hats?
        // </summary>
        [Description("Will players be able to use MapEditorReborn schematics as hats?")]
        public bool SchematicHats { get; set; } = false;
        // <summary>
        //  List of Schematic Hats
        // </summary>
        [Description("List of Schematic Hats")]
        public List<SchematicHatConfig> SchematicHatList { get; set; } = new List<SchematicHatConfig>(){
            new SchematicHatConfig()
        };
    }
}
