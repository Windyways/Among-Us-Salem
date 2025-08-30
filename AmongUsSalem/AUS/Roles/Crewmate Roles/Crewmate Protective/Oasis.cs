using System.Text;
using UnityEngine;
using Color = UnityEngine.Color;

namespace ObjectWorkshop.LifeImprovement.Roles;

#region Oasis
#endregion
public sealed class Oasis(IntPtr cppPtr)
    : CrewmateRole(cppPtr), IOWRole, IWikiDiscoverable
{
    public string RoleName => TouLocale.Get(TouNames.Oasis, "Oasis");
    public string revealText => "is the protector of the desert.";
    public string RoleDescription => "Create Sanctuaries, start Sandstorms!";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => OWColors.Crewmate;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateProtective;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = OWAssets.Oasis,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Protective role that can create sanctuaries to stop players from being attacked within, and can start sandstorms to potentially harm Infiltrators." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Sanctify",
            $"You can Sanctify the area around you during the round. You will create a Sanctuary, preventing all players inside from dying from direct attacks, which only lasts {OptionGroupSingleton<Oasis_Options>.Instance.Duration}s. You will be notified if your target is attacked. Your target will be notified if they're attacked. The attacker will be notified their target was protected by an Oasis.",
            OWAssets.Oasis_Sanctify),

        new("Sandstorm",
            $"You can cause a Sandstorm during the round. A sandstorm will brew, slowing everyone down a bit and passively pushing specific objects in the direction of the wind. All evils attempting to attack in a sandstorm will have a {OptionGroupSingleton<Oasis_Options>.Instance.MissChance}% chance to miss. Sandstorms last {OptionGroupSingleton<Oasis_Options>.Instance.SandstormDuration}s before evaporating. You will be notified if your target is attacked. Your target will be notified if they're attacked. The attacker will be notified their target was protected by a Sandstorm.",
            OWAssets.Oasis_Sandstorm),
    ];

    public override void OnMeetingStart()
    {
        Sanctuary.DestroyAll();
        Oasis.RpcStopSandstorm(Player);
    }

    public void LobbyStart()
    {
        Sanctuary.CleanUp();
        Sandstorm.CleanUp();
    }

    #region RpcSanctify
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.Sanctify, SendImmediately = true)]
    public static void RpcSanctify(PlayerControl player)
    {
        if (player.Data.Role is not Oasis)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcSanctify - Invalid Oasis");
            return;
        }

        var oasis = player.GetRole<Oasis>();
        Sanctuary.Begin(oasis.Player);
    }

    #region RpcSandstorm
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.Sandstorm, SendImmediately = true)]
    public static void RpcSandstorm(PlayerControl player)
    {
        if (player.Data.Role is not Oasis)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcSandstorm - Invalid Oasis");
            return;
        }

        var oasis = player.GetRole<Oasis>();
        Sandstorm.Begin(oasis.Player);
    }
    
    #region RpcStopSandstorm
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.StopSandstorm, SendImmediately = true)]
    public static void RpcStopSandstorm(PlayerControl player)
    {
        var sandstorm = Sandstorm.GetObjectByPlayer(player);
        if (sandstorm != null) sandstorm.Stop();
    }

    #region RpcOasisNotif
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.OasisNotif, SendImmediately = true)]
    public static bool RpcOasisNotif(PlayerControl player, PlayerControl target, PlayerControl oasis, int type)
    {
        switch (type)
        {
            case 1:
                if (player.AmOwner || Debugger.IsDebuggerActive)
                {
                    MiscUtils.AddFakeChat(player.Data, MiscUtils.GetTitle(OWColors.Crewmate),
                    $"You {OWColors.GetColorKeyword("Attacked")} {target.GetDefaultAppearance().PlayerName}, but an {OWColors.GetColorKeyword("Oasis")} protected them!");
                }

                if (target.AmOwner || Debugger.IsDebuggerActive)
                {
                    MiscUtils.AddFakeChat(target.Data, MiscUtils.GetTitle(OWColors.Crewmate),
                    $"You were attacked, but a {OWColors.GetColorKeyword("Sanctuary")} protected you!");
                }

                if (oasis.AmOwner || Debugger.IsDebuggerActive)
                {
                    MiscUtils.AddFakeChat(oasis.Data, MiscUtils.GetTitle(OWColors.Crewmate),
                    $"{target.GetDefaultAppearance().PlayerName} was attacked, but you saved them!");
                }
                break;
            case 2:
                if (player.AmOwner || Debugger.IsDebuggerActive)
                {
                    MiscUtils.AddFakeChat(player.Data, MiscUtils.GetTitle(OWColors.Crewmate),
                    $"You {OWColors.GetColorKeyword("Attacked")} {target.GetDefaultAppearance().PlayerName}, but a {OWColors.GetColorKeyword("Sandstorm")} made you miss!");
                }

                if (target.AmOwner || Debugger.IsDebuggerActive)
                {
                    MiscUtils.AddFakeChat(target.Data, MiscUtils.GetTitle(OWColors.Crewmate),
                    $"You were attacked, but a {OWColors.GetColorKeyword("Sandstorm")} protected you!");
                }

                if (oasis.AmOwner || Debugger.IsDebuggerActive)
                {
                    MiscUtils.AddFakeChat(oasis.Data, MiscUtils.GetTitle(OWColors.Crewmate),
                    $"You feel one with the storm as you hear someone miss an attack!");
                }
                break;
        }

        return false;
    }
}

#region Oasis_Sanctify
#endregion
public sealed class Oasis_Sanctify : ObjectWorkshopRoleButton<Oasis>
{
    public override string Name => "Sanctify";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Crewmate;
    public override float Cooldown => OptionGroupSingleton<Oasis_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Oasis_Sanctify;
    public override float EffectDuration => OptionGroupSingleton<Oasis_Options>.Instance.Duration;

    protected override void OnClick()
    {
        Oasis.RpcSanctify(Role.Player);
    }
}

#region Oasis_Sandstorm
#endregion
public sealed class Oasis_Sandstorm : ObjectWorkshopRoleButton<Oasis>
{
    public override string Name => "Sandstorm";
    public override Color TextOutlineColor => OWColors.Crewmate;
    public override float Cooldown => OptionGroupSingleton<Oasis_Options>.Instance.SandstormCD;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Oasis_Sandstorm;
    public override float EffectDuration => OptionGroupSingleton<Oasis_Options>.Instance.SandstormDuration;

    protected override void OnClick()
    {
        Oasis.RpcSandstorm(Role.Player);
    }
}

#region Oasis_Options
#endregion
public sealed class Oasis_Options : AbstractOptionGroup<Oasis>
{
    public override string GroupName => TouLocale.Get(TouNames.Oasis, "Oasis");

    [ModdedNumberOption("<color=#b3ffff>Oasis</color> <color=#4a86e8>Sanctify</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#b3ffff>Oasis</color> <color=#4a86e8>Sandstorm</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float SandstormCD { get; set; } = 25f;
    
    [ModdedNumberOption("<color=#b3ffff>Oasis</color> <color=#4a86e8>Sanctify</color> Duration", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Duration { get; set; } = 10f;
    
    [ModdedNumberOption("<color=#b3ffff>Oasis</color> <color=#4a86e8>Sanctify</color> Radius", 0.1f, 30f, 0.1f, MiraNumberSuffixes.Multiplier, "0.0")]
    public float Radius { get; set; } = 7f;
    
    [ModdedNumberOption("<color=#b3ffff>Oasis</color> <color=#4a86e8>Sandstorm</color> Duration", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float SandstormDuration { get; set; } = 25f;

    [ModdedNumberOption("<color=#b3ffff>Oasis</color> <color=#4a86e8>Sandstorm</color> Miss Chance", 0, 100f, 5f, MiraNumberSuffixes.Percent)]
    public float MissChance { get; set; } = 25f;
}