using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Catalyst(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Catalyst";
    public string revealText => "is overflowing with energy.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are a crazed galvanist overflowing with energy.";
    public Color RoleColor { get; set; } = RoleColors.Town;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Alignment Alignment => Alignment.TownOutlier;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.CatalystRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"Attack: {Attack}\n" +
            $"Defense: {Defense}\n" +
            $"The {RoleName} is a {Alignment.ToSpacedString()} rrole that can Overcharge others to make their ability cooldowns 2x faster!\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Overcharge",
            "You can Overcharge a player at Night.\n" +
            "Your target will become Overcharged. They will be notified of this when the Day begins.\n" +
            "The following Night, your target will have their ability cooldowns deplete twice as fast.",
            AUSAssets.Catalyst_Overcharge),
    ];

    public static string Info()
    {
        return $"Your body is overflowing with energy, you are Overcharged!";
    }
}

public sealed class Catalyst_Overcharge : TownOfUsRoleButton<Catalyst, PlayerControl>
{
    public override string Name => "Overcharge";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => OptionGroupSingleton<Catalyst_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Catalyst_Overcharge;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, false, true)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        if (Target == null)
            return;

        Target.RpcAddModifier<OverchargedModifier>(Player);
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !x.HasModifier<OverchargedModifier>(x => x.Caster == Player));
    }
}

public sealed class Catalyst_Options : AbstractOptionGroup<Catalyst>
{
    public override string GroupName => "Catalyst";

    [ModdedNumberOption("Catalyst Overcharge Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("Catalyst Overcharge Deplete Multiplier", 2f, 5f, 0.25f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float Multiplier { get; set; } = 2f;
}