using System.Globalization;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using MiraAPI;
using MiraAPI.PluginLoading;
using MiraAPI.Utilities.Assets;
using Reactor;
using Reactor.Localization;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using AmongUsSalem.Patches.Misc;
using static AmongUsSalem.LifeImprovement.MCI.Reactor_Coroutines;
using ModCompatibility = AmongUsSalem.Modules.ModCompatibility;

namespace AmongUsSalem;

/// <summary>
///     Plugin class for Among Us Salem.
/// </summary>
[BepInAutoPlugin("windyways.aus", "Among Us Salem III")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MiraApiPlugin.Id)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class AUSPlugin : BasePlugin, IMiraPlugin
{
    /// <summary>
    ///     Gets the specified Culture for string manipulations.
    /// </summary>
    public static CultureInfo Culture { get; } = new("en-US");

    /// <summary>
    ///     Gets the Harmony instance for patching.
    /// </summary>
    public Harmony Harmony { get; } = new(Id);

    public static ConfigEntry<bool> DeadSeeGhosts { get; set; }
    public static ConfigEntry<bool> ColorPlayerName { get; set; }
    public static ConfigEntry<int> GameSummaryMode { get; set; }
    public static ConfigEntry<float> ButtonUIFactor { get; set; }
    public static ConfigEntry<bool> OffsetButtons { get; set; }

    /// <summary>
    ///     Determines if the current build is a dev build or not. This will change certain visuals as well as always grab news locally to be up to date.
    /// </summary>
    public static bool IsDevBuild => false;
    
    /// <inheritdoc />
    public string OptionsTitleText => "Among Us\nSalem III";

    /// <inheritdoc />
    public ConfigFile GetConfigFile()
    {
        return Config;
    }

    public AUSPlugin()
    {
        
    }

    /// <summary>
    ///     The Load method for the plugin.
    /// </summary>
    public override void Load()
    {
        ReactorCredits.Register("Among Us Salem III", Version, IsDevBuild, ReactorCredits.AlwaysShow);

        IL2CPPChainloader.Instance.Finished += ModCompatibility.Initialize; // Initialise AFTER the mods are loaded to ensure maximum parity (no need for the soft dependency either then)
        IL2CPPChainloader.Instance.Finished += ModNewsFetcher.CheckForNews; // Checks for mod announcements after everything is loaded to avoid Epic Games crashing

        var path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(AUSPlugin))!.Location) + "\\touhats.catalog";
        AddressablesLoader.RegisterCatalog(path);
        AddressablesLoader.RegisterHats("touhats");

        DeadSeeGhosts = Config.Bind("LocalSettings", "DeadSeeGhosts", true, "If you see other ghosts when dead");
        ColorPlayerName = Config.Bind("LocalSettings", "ColorPlayerName", false,
            "If your name is colored with your role color or if it's left as white.");
        ButtonUIFactor = Config.Bind("LocalSettings", "ButtonUIFactor", 0.8f,
            "Scale factor for buttons in-game. Preferably, keep the value between 0.5f and 1.5f.");
        GameSummaryMode = Config.Bind("LocalSettings", "GameSummaryMode", 1,
            "How the Game Summary appears in the Win Screen. 0 is to the left, 1 is split, and 2 is hidden.");
        OffsetButtons = Config.Bind("LocalSettings", "OffsetButtons", false,
            "If venting is disabled (and you're not an Mafia), should there be a blank spot where the vent button usually is?");

        Harmony.PatchAll();

        RoleReferences.Initialize();
        FactionReferences.Initialize();
        
        // For inbuilt MCI
        ClassInjector.RegisterTypeInIl2Cpp<Debugger>();
        ClassInjector.RegisterTypeInIl2Cpp<Component>();
        AddComponent<Component>();
        Debugger = AddComponent<Debugger>();
    }
    
    public enum MsgType { Message, Warning, Error }
    public static void DebugLogMessage(string message, MsgType type = MsgType.Message)
    {
        if (type == MsgType.Error) PluginSingleton<AUSPlugin>.Instance.Log.LogError(message);
        else if (type == MsgType.Warning) PluginSingleton<AUSPlugin>.Instance.Log.LogWarning(message);
        else if (type == MsgType.Message) PluginSingleton<AUSPlugin>.Instance.Log.LogMessage(message);
    }
    
	public static bool InGame()
    {
        return AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started;
    }
    
    public static string RobotName { get; set; } = "Bot";
    public static List<PlayerControl> IsBot = new List<PlayerControl>();
    public static bool Persistence { get; set; } = true;
    public static Debugger Debugger { get; set; }
}