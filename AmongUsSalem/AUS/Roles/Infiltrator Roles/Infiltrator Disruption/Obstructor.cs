using System.Text;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace ObjectWorkshop.LifeImprovement.Roles;

#region Obstructor
#endregion
public sealed class Obstructor(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IOWRole, IWikiDiscoverable
{
    public bool attractsMetal => true;
    public string RoleName => TouLocale.Get(TouNames.Obstructor, "Obstructor");
    public string revealText => "block passageways to disrupt opposing threats.";
    public string RoleDescription => "Place traffic cones to block movement!";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => OWColors.Infiltrator;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.InfiltratorDisruption;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = OWAssets.Obstructor,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Infiltrator Disruption role that can kill, and can place traffic cones to prevent non-infiltrators from passing it."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Attack",
            "You can Attack a player during the round. You will kill your target.",
            OWAssets.KillSprite),

        new("Barricade",
            $"You can plant a Barricade during the round. You will plant a traffic cone at your position, blocking people from passing it. Using this ability again will override the original Barricade. Infiltrators are immune to your Barricade. Players are immune to your Barricades if a sabotage is active. Barricades reset at the start of each round. Your Barricades are affected by Sandstorms.",
            OWAssets.Obstructor_Barricade),
    ];

    public void LobbyStart()
    {
        Barricade.CleanUp();
    }

    #region RpcBarricade
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.Barricade, SendImmediately = true)]
    public static void RpcBarricade(PlayerControl player)
    {
        if (player.Data.Role is not Obstructor)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcBarricade - Invalid Obstructor");
            return;
        }

        var obstructor = player.GetRole<Obstructor>();

        WitnessKillEvent.WitnessSuspiciousActivity(obstructor.Player, obstructor.Player, false, false, true, true);
		obstructor.ActiveBarricades.Add(Barricade.Begin(obstructor.Player));
    }

    public List<Barricade> ActiveBarricades = new List<Barricade>();
}

#region Obstructor_Barricade
#endregion
public sealed class Obstructor_Barricade : ObjectWorkshopRoleButton<Obstructor>
{
    public override string Name => "Barricade";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Infiltrator;
    public override float Cooldown => OptionGroupSingleton<Obstructor_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Obstructor_Barricade;

    protected override void OnClick()
    {
        Obstructor.RpcBarricade(Role.Player);
    }
}

#region Obstructor_Options
#endregion
public sealed class Obstructor_Options : AbstractOptionGroup<Obstructor>
{
    public override string GroupName => TouLocale.Get(TouNames.Obstructor, "Obstructor");

    [ModdedNumberOption("<color=#ff5050>Obstructor</color> <color=#4a86e8>Barricade</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
    
    [ModdedNumberOption("<color=#ff5050>Obstructor</color> Max <color=#4a86e8>Barricades</color> At Once", 1f, 3f, 1f)]
    public float MaxAtOnce { get; set; } = 1f;
}