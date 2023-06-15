using Exiled.API.Interfaces;
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
    }
}
