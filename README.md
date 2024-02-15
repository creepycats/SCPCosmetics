[![Github All Releases](https://img.shields.io/github/downloads/creepycats/SCPHats/total.svg)](https://github.com/creepycats/SCPHats/releases) [![Maintenance](https://img.shields.io/badge/Maintained%3F-yes-green.svg)](https://github.com/creepycats/SCPHats/graphs/commit-activity) [![GitHub license](https://img.shields.io/github/license/Naereen/StrapDown.js.svg)](https://github.com/creepycats/SCPHats/blob/main/LICENSE)
<a href="https://github.com/creepycats/SCPHats/releases"><img src="https://img.shields.io/github/v/release/creepycats/SCPHats?include_prereleases&label=Release" alt="Releases"></a>
<a href="https://discord.gg/PyUkWTg"><img src="https://img.shields.io/discord/656673194693885975?color=%23aa0000&label=EXILED" alt="Support"></a>

# SCPCosmetics
Formerly a SCP:SL Exiled Plugin Porting the Hats from SCPStats as well as adding support for MapEditorReborn models to be used as Hats.
Now, a full Cosmetics plugin involving both Hats and Pets!

Made for v13.0.0 of SCP:SL and v7.0.0 of Exiled and onward by creepycats.

This plugin contains code originally from SCPStats, [you can find its source code here](https://github.com/SCPStats/Plugin/).

## Installation
**This Plugin Requires MapEditorReborn, [you can download it here](https://github.com/Michal78900/MapEditorReborn/releases)**

After you've installed MapEditorReborn, go to [Releases](https://github.com/creepycats/SCPHats/releases) and download the latest Plugin file.

Simply drop it into your Exiled Plugins folder alongside MapEditorReborn.

**I will NOT cover installing MapEditorReborn schematics.**

## Permissions
`scpcosmetics.hat`

`scpcosmetics.pet`

You can create custom permissions for Schematic Hats.

## Configuration
Your default Config for SCPHats should look like this:
```yml
s_c_p_cosmetics:
  is_enabled: true
  debug: false
  # Will players be able to use MapEditorReborn schematics as hats?
  schematic_hats: false
  # List of Schematic Hats
  schematic_hat_list:
    example:
      # Name of the Schematic to use for the Hat
      schematic_name: 'example'
      # List of names the command will accept for this hat
      hat_names:
      - 'test'
      # List of permissions required for this hat to be used
      required_permissions:
      - 'scpcosmetics.test'
      # The Positional Offset for the Schematic
      position:
        x: 0
        y: 0
        z: 0
      # The Rotational Offset for the Schematic
      rotation: &o0
        euler_angles:
          x: 0
          y: 0
          z: 0
        normalized: *o0
      # The Scale for the Schematic
      scale:
        x: 1
        y: 1
        z: 1
  # Will players be able to spawn pets?
  enable_pets: true
  # Will pets mirror their owners' class? RECOMMENDED: True - Prevents SCP-096 and SCP-173 issues (NOT FULLY IMPLEMENTED 100%)
  pets_mirror_class: true
  # Will players be able to name their pets?
  name_pets: true
  # Will players be able to give their pet an item to hold?
  pets_can_hold_items: true
```
If you want to use the schematic hats, simply change `schematic_hats` to true. Make sure your schematics are properly configured.

You can have multiple schematic hats. The `schematic_name` is the name of the MapEditorReborn schematic from your Schematics folder. `hat_names` is a list of different names that will trigger this hat in game when using the command. You can add custom permissions such as in the example shown.
