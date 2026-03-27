using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Lookout(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Lookout";
    public string revealText => "watches who visits people at night.";
    public string RoleDescription => OptionGroupSingleton<Lookout_Options>.Instance.Mode == LookoutMode.TOS1 ? "Town Of Salem" : "Town Of Salem 2";
    public string RoleLongDescription => "You are an observer who camps outside of houses to gather information.";
    public Color RoleColor { get; set; } = RoleColors.Town;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Alignment Alignment => Alignment.TownInvestigative;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.LookoutRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var info = ICustomAURole.SetNewTabText(this);
        return info;
    }

    public string GetAdvancedDescription()
    {
        return
            $"Attack: {Attack}\n" +
            $"Defense: {Defense}\n" +
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can see who visits their target, being a threat to evils when on a high priority target.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities =>
    [
        new("Watch", GetWatchDescription(), OptionGroupSingleton<Lookout_Options>.Instance.Mode == LookoutMode.TOS1 ?
            AUSAssets.Lookout_Watch_TOS1 : AUSAssets.Lookout_Watch_TOS2)
    ];

    private static string GetWatchDescription()
    {
        var opt = OptionGroupSingleton<Lookout_Options>.Instance;

        if (opt.Mode == LookoutMode.TOS1)
        {
            return "You can Watch a player at Night.\n" +
                   $"You will see up to {opt.MaxRecognize.GetFloatData()} players that visit your target. Astral abilities bypass this.";
        }

        return "You can Watch a player at Night.\n" +
               "You will see all players that visit your target. Astral abilities bypass this.";
    }

    public static string Info(NotificationType type, PlayerControl player, PlayerControl target)
    {
        if (type == NotificationType.Lookout_MoreThan3) return $"More people visited {target.Name()} but you couldn't identify them.";
        if (type == NotificationType.Lookout_NoOneVisited) return $"No one visited {target.Name()} last night.";
        return $"{target.Name()} was visited by {player.Name()} last night!";
    }

    public override void Initialize(PlayerControl player) // This patches the ability sprite since it's loaded on runtime.
    {
        RoleBehaviourStubs.Initialize(this, player);

        var opt = OptionGroupSingleton<Lookout_Options>.Instance;
        CustomButtonSingleton<Lookout_Watch>.Instance.OverrideSprite(AUSAssets.Lookout_Watch.LoadAsset());
        if (opt.WatchMode == (int)LookoutWatchMode.Camouflage && opt.Mode == LookoutMode.TOS2) Player.RpcAddModifier<Camouflage>();
    }

    public void Role_OnMeetingStart()
    {
        if (Player.AmOwner())
        {
            Player.AddModifier<TI>();
            if (VisitedInfo.Count == 0)
            {
                foreach (var watched in ModifierUtils.GetPlayersWithModifier<WatchedModifier>(x => x.Caster == Player))
                {
                    Player.Notify(Info(NotificationType.Lookout_NoOneVisited, watched, watched), NotifyMode.OnlyMeeting, sprite: AUSAssets.LookoutRoleCard.LoadAsset());
                }
            }
            else
            {
                foreach (var info in VisitedInfo)
                {
                    Player.Notify(Info(NotificationType.Lookout_Visited, info.Item1, info.Item2), NotifyMode.OnlyMeeting, sprite: AUSAssets.LookoutRoleCard.LoadAsset());
                    if (info.Item1.HasDied() && MeetingHud.Instance.reporterId != info.Item2.PlayerId) info.Item2.RpcAddModifier<IncriminatingEvidence>();
                }
            }

            VisitedInfo.Clear();
        }
    }

    public List<(PlayerControl, PlayerControl)> VisitedInfo = new List<(PlayerControl, PlayerControl)>();
}

public sealed class Lookout_Watch : TownOfUsRoleButton<Lookout, PlayerControl>, IButtonClick
{
    public override string Name => "Watch";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => 
        (OptionGroupSingleton<Lookout_Options>.Instance.Mode == LookoutMode.TOS1 ?
        OptionGroupSingleton<Lookout_Options>.Instance.Cooldown_TOS1.GetFloatData() :
        OptionGroupSingleton<Lookout_Options>.Instance.Cooldown_TOS2.GetFloatData());
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Lookout_Watch;

    public override void ClickHandler()
    {
        var opt = OptionGroupSingleton<Lookout_Options>.Instance;
        bool isAstral = opt.Mode == LookoutMode.TOS2 && opt.WatchMode == (int)LookoutWatchMode.Astral;
        if (button.IsTargetingValid(Player, Target, false, !isAstral)) base.ClickHandler();
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x => 
            !x.HasModifier<WatchedModifier>(x => x.Caster == Player));
    }

    protected override void OnClick() => Click(Player, Target);
    public void Click(PlayerControl player, PlayerControl Target = null)
    {
        if (Target == null)
            return;

        Target.RpcAddModifier<WatchedModifier>(Player);
    }
}

public sealed class Lookout_Options : AbstractOptionGroup<Lookout>
{
    public override string GroupName => "Lookout";

    [ModdedEnumOption("Lookout Mode", typeof(LookoutMode), ["TOS1", "TOS2"])]
    public LookoutMode Mode { get; set; } = LookoutMode.TOS2;

    // --- TOS1 ---
    public ModdedNumberOption Cooldown_TOS1 { get; } = new("Lookout Watch Cooldown", 25f, 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)
    { Visible = () => OptionGroupSingleton<Lookout_Options>.Instance.Mode == LookoutMode.TOS1 };

    public ModdedNumberOption MaxRecognize { get; } = new("Lookout Max Recognized Players", 0f, 0f, 15f, 1f, MiraNumberSuffixes.None, zeroInfinity: true)
    { Visible = () => OptionGroupSingleton<Lookout_Options>.Instance.Mode == LookoutMode.TOS1 };

    // --- TOS2 ---
    public ModdedNumberOption Cooldown_TOS2 { get; } = new("Lookout Watch Cooldown", 25f, 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)
    { Visible = () => OptionGroupSingleton<Lookout_Options>.Instance.Mode == LookoutMode.TOS2 };

    public ModdedEnumOption WatchMode { get; set; } = new("Lookout Watch Mode",
        (int)LookoutWatchMode.None, typeof(LookoutWatchMode), ["None", "Astral", "Camouflage"])
    { Visible = () => OptionGroupSingleton<Lookout_Options>.Instance.Mode == LookoutMode.TOS2 };
}

public enum LookoutMode
{
    TOS1,
    TOS2
}

public enum LookoutWatchMode
{
    None,
    Astral,
    Camouflage
}