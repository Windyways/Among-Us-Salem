using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using ObjectWorkshop.Modifiers.Impostor;
using ObjectWorkshop.Modules.Components;
using UnityEngine;

namespace ObjectWorkshop.LifeImprovement.Roles;

#region Canopy
#endregion
public sealed class Canopy(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IOWRole, IWikiDiscoverable
{
    public bool attractsMetal => true;
    public string RoleName => TouLocale.Get(TouNames.Canopy, "Canopy");
    public string revealText => "is a cloud conjurer.";
    public string RoleDescription => "Summon clouds to prevent vision.";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => OWColors.Infiltrator;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.InfiltratorDisruption;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = OWAssets.Canopy,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Infiltrator Disruption role that can kill, and can place a cloud to make it harder for crew to see what others are doing."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Attack",
            "You can Attack a player during the round. You will kill your target.",
            OWAssets.KillSprite),

        new("Cast",
            $"You can Cast a dark cloud during the round. You will summon a dark cloud at your position, which would make players invisible while they're in it. The dark cloud will vaporize after {OptionGroupSingleton<Canopy_Options>.Instance.Duration}s.",
            OWAssets.Canopy_Cast),
    ];

    public override void OnMeetingStart()
    {
        foreach (var darkCloud in DarkCloud.AllDarkClouds)
        {
            darkCloud.DestroyGameObject();
        }
    }

    public void LobbyStart()
    {
        DarkCloud.CleanUp();
    }

    #region RpcCast
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.Cast, SendImmediately = true)]
    public static void RpcCast(PlayerControl player)
    {
        if (player.Data.Role is not Canopy)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcCast - Invalid Canopy");
            return;
        }

        var canopy = player.GetRole<Canopy>();

        WitnessKillEvent.WitnessSuspiciousActivity(canopy.Player, canopy.Player, false, false, true, true);
        DarkCloud.Begin(canopy.Player);
    }

    [HideFromIl2Cpp] public List<RoleBehaviour> ChosenRoles { get; } = [];
    public RoleBehaviour? RandomRole { get; set; }
    public RoleBehaviour? SelectedRole { get; set; }

    public void UpdateRole()
    {
        if (!SelectedRole)
        {
            return;
        }

        var currenttime = Player.killTimer;

        var roleType = RoleId.Get(SelectedRole!.GetType());
        
        Player.RpcChangeRole(roleType, false);
        Player.RpcAddModifier<CacheModifier>(SelectedRole.NiceName, RoleManager.Instance.GetRole((RoleTypes)RoleId.Get(SelectedRole!.GetType())), Faction.Infiltrator);

        SelectedRole = null;
        Player.SetKillTimer(currenttime);
    }
}

#region Canopy_Cast
#endregion
public sealed class Canopy_Cast : ObjectWorkshopRoleButton<Canopy>
{
    public override string Name => "Cast";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Infiltrator;
    public override float Cooldown => OptionGroupSingleton<Canopy_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Canopy_Cast;
    public override float EffectDuration => OptionGroupSingleton<Canopy_Options>.Instance.Duration;

    protected override void OnClick()
    {
        Canopy.RpcCast(PlayerControl.LocalPlayer);
    }
}

#region Canopy_Options
#endregion
public sealed class Canopy_Options : AbstractOptionGroup<Canopy>
{
    public override string GroupName => TouLocale.Get(TouNames.Canopy, "Canopy");

    [ModdedNumberOption("<color=#ff5050>Canopy</color> <color=#4a86e8>Cast</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
    
    [ModdedNumberOption("<color=#ff5050>Canopy</color> <color=#4a86e8>Cast</color> Duration", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Duration { get; set; } = 10f;
    
    [ModdedNumberOption("<color=#ff5050>Canopy</color> Dark Cloud Size", 0.1f, 30f, 0.1f, MiraNumberSuffixes.Multiplier, "0.0")]
    public float Size { get; set; } = 3f;
}