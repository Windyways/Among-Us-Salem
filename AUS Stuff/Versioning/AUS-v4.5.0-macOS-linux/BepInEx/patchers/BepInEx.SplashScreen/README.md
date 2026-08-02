![Splash screen preview](https://github.com/BepInEx/BepInEx.SplashScreen/assets/39247311/07831558-91e7-48fa-a2de-fc3c6d29a731)

# BepInEx Loading Progress Splash Screen
A BepInEx patcher that displays a loading screen on game startup with information about patchers and plugins being currently loaded. It's best suited for games where patchers and plugins take a long time to initialize.

This patcher is mostly meant for inclusion in modpacks to give end-users immediate feedback after starting a heavily modded game. It can sometimes take a long time for the game window to appear and/or become responsive - especially on slow systems - which can be interpretted by the user as the game crashing.

The patcher and GUI app have evolved from a very old version of [risk-of-thunder/BepInEx.GUI](https://github.com/risk-of-thunder/BepInEx.GUI), though at this point most of the code has been rewritten and this version works in all games. That being said, if you are modding Risk Of Rain 2, use risk-of-thunder/BepInEx.GUI for a better experience.

## How to use
1. Install [BepInEx](https://github.com/BepInEx/BepInEx) 5.4.11 or later, or 6.0.0-be.674 or later (works on both mono and IL2CPP).
2. Download latest release for your BepInEx version.
3. Extract the release so that the patcher files end up inside `BepInEx\patchers`.
4. You should now see the splash screen appear on game start-up, assuming BepInEx is configured properly.

### Splash screen doesn't appear
1. Make sure that `BepInEx.SplashScreen.GUI.exe` and `BepInEx.SplashScreen.Patcher.dll` are both present inside the `BepInEx\patchers` folder.
2. Check if the splash screen isn't disabled in `BepInEx\config\BepInEx.cfg`. If you can't see this file or the SplashScreen Enable setting, it means either BepInEx isn't correctly configured or this patcher is failing to start for some reason.
3. Update BepInEx 5 to latest version and make sure that it is running.
4. If the splash screen still does not appear, check the game log for any errors or exceptions. You can report issues on [GitHub](https://github.com/BepInEx/BepInEx.SplashScreen/issues).

## Contributing
Feel free to start issues, and by all means submit some PRs! Contributions should be submitted to the repository at https://github.com/BepInEx/BepInEx.SplashScreen.

You can discuss changes and talk with other modders on the [official BepInEx Discord server](https://discord.gg/MpFEDAg).

## Compiling
Clone the repository and open the .sln with Visual Studio 2022 (with .NET desktop development and .NET 3.5 development tools installed). Hit `Build Solution` and it should just work.
