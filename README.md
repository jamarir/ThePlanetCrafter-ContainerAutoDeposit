# ThePlanetCrafter-ContainerAutoDeposit
The Planet Crafter BepInEx / HarmonyX Plugin - ContainerAutoDeposit

## TLDR

This repo contains the patch code used for [this BepInEx plugin](https://www.nexusmods.com/planetcrafter/mods/121).

## Description

[The Planet Crafter](https://store.steampowered.com/app/1284190/The_Planet_Crafter/) is basically a game where the player goes through the following steps: Collect -> Craft -> Unlock -> Repeat.

In-game, the player may store his inventory's items into containers. However, the game doesn't inherently provide a way to store every player's item that are already in the destination container, with a single action. 

This mod allows the player to perform that action in a single shot, by pressing LCTRL while clicking the MoveAll button (which allows you to deposit all your items) on the left's inventory (the player's one).

## Usage

To use this mod, BepInEx must be installed in the game's directory, i.e.:
- The content of [BepInEx_win_x64_5.4.23.3.zip](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.3) must have been copied into the game's directory.
- The game must have been run at least once with the above files copied.

Then, [the release DLL](https://github.com/jamarir/ThePlanetCrafter-ContainerAutoDeposit/releases) can be copied into the created `BepInEx\plugins\` folder.

## Development

To develop this patch:
- A [Visual Studio "Class Library (.NET Framework)" project](https://docs.bepinex.dev/v5.4.11/articles/dev_guide/plugin_tutorial/1_setup.html), using the working 4.7.2 version, has been created.
- The following game libraries has been [imported into a local `lib\` folder](https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/2_plugin_start.html#referencing-from-local-install) in the project:
```
The Planet Crafter\BepInEx\core\BepInEx.dll
The Planet Crafter\BepInEx\core\0Harmony.dll
The Planet Crafter\Planet Crafter_Data\Managed\Assembly-CSharp.dll
The Planet Crafter\Planet Crafter_Data\Managed\Unity.InputSystem.dll
The Planet Crafter\Planet Crafter_Data\Managed\UnityEngine.CoreModule.dll
The Planet Crafter\Planet Crafter_Data\Managed\UnityEngine.dll
```

Handful [BepInEx Debugging Tools](https://docs.bepinex.dev/articles/dev_guide/dev_tools.html) can be used, such as:
- The [Runtime Unity Editor tool](https://github.com/ManlyMarco/RuntimeUnityEditor), to debug dynamically the game's instances while playing.
- The [Configuration Manager tool](https://github.com/BepInEx/BepInEx.ConfigurationManager), to configure BepInEx tools.

Finally, the following Post-Build event can be used to copy the builded DLL, and launch the game directly:
> Naturally, the paths should be updated accordingly if non-existent.
```powershell
xcopy /q /y "$(TargetPath)" "D:\Program Files (x86)\Steam\steamapps\common\The Planet Crafter\BepInEx\plugins\"
"D:\Program Files (x86)\Steam\steamapps\common\The Planet Crafter\Planet Crafter.exe"
```

## References

- https://store.steampowered.com/app/1284190/The_Planet_Crafter/
- https://www.nexusmods.com/planetcrafter/mods/121
- https://docs.bepinex.dev/
- https://harmony.pardeike.net/articles/intro.html
- https://github.com/ManlyMarco/RuntimeUnityEditor
- https://github.com/BepInEx/BepInEx.ConfigurationManager