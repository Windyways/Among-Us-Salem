using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Framer(IntPtr cppPtr) : ImpostorRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Framer";
    public string revealText => "has a desire or deceive.";
    public string RoleDescription => "Town Of Salem";
    public string RoleLongDescription => "You are a skilled counterfeiter who manipulates information.";
    public Color RoleColor { get; set; } = RoleColors.Mafia;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public Faction Faction { get; set; } = Faction.Mafia;
    public Alignment Alignment => Alignment.MafiaDeception;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        Icon = AUSAssets.FramerRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can falsify investigative info, potentially leading to false lynches.\n" +
            "Kill anyone that will not submit to the Mafia." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- You have access to Mafia chat.\n" +
            $"- If there are no capable Mafia Killing roles, you will be promoted to Mafioso.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Frame",
            "You can Frame a player at Night.\n" +
            "Your target will appear as a member of the Mafia to Investigative roles.",
            AUSAssets.Framer_Frame),
    ];
}

public sealed class Framer_Frame : TownOfUsRoleButton<Framer, PlayerControl>
{
    public override string Name => "Frame";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Framer_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Framer_Frame;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, false, true)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        if (Target == null)
            return;

        Target.RpcAddModifier<FramedModifier>(Player);
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(false, Distance, predicate: x =>
            !x.HasModifier<FramedModifier>(x => x.Caster == Player));
    }
}

public sealed class Framer_Options : AbstractOptionGroup<Framer>
{
    public override string GroupName => "Framer";

    [ModdedNumberOption("Framer Frame Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedToggleOption("Frames Last Until An Investigative Visits")]
    public bool RemovedOnVisit { get; set; } = true;
}