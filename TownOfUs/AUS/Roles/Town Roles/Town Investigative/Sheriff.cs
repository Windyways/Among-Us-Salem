using Il2CppInterop.Runtime.Attributes;
using Rewired.Utils.Platforms.Windows;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;
using static UnityEngine.UIElements.UIR.Allocator2D;

namespace AmongUsSalem.Roles;

public sealed class Sheriff(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Sheriff";
    public string revealText => "is a protector of the town.";
    public string RoleDescription => "";
    public string RoleLongDescription => "";
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
        Icon = AUSAssets.SheriffRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfUsRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that prioritizes searching players for incriminating evidence, being a big threat against the Mafia and the Coven.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- Enchanters and Soul Collectors can make your target appear suspicious.\n" +
            $"- Illusionists can make your targets appear innocent.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Search",
            "You can Search a player at Night.\n" +
            "You will see if your target has incriminating evidence or not.\n" +
            "Coven without the Necronomicon, Mafia not the Godfather, Wandering Soul members, and Neutral Evils appear as suspicious.\n" +
            "Apocalypse and Vampires appear suspicious if they are not solo.",
            AUSAssets.Sheriff_Search),
    ];

    public static bool IsSuspicious(PlayerControl target)
    {
        if (target.Is(Faction.Mafia)) return true; // Not Godfather!
        if (target.Is(Faction.Coven) && !target.HasModifier<Necronomicon>()) return true;
        if (target.Is(Alignment.NeutralEvil)) return true;
        if (target.Is(Alignment.NeutralApocalypse)) return true;
        return false;
    }

    public static string Info(PlayerControl player, PlayerControl target)
    {
        if (IsSuspicious(target))
        {
            target.AddModifier<IncriminatingEvidence>();
            player.AddModifier<TI>();

            return $"{target.Name()} seems suspicious!";
        }
        return $"You cannot find evidence of wrongdoing. {target.Name()} seems innocent.";
    }
}

public sealed class Sheriff_Search : TownOfUsRoleButton<Sheriff, PlayerControl>
{
    public override string Name => "Search";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => OptionGroupSingleton<Sheriff_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Sheriff_Search;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, false, true)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        if (Target == null)
            return;

        Player.Notify(Sheriff.Info(Player, Target), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.SheriffRoleCard.LoadAsset());
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance);
    }
}

public sealed class Sheriff_Options : AbstractOptionGroup<Sheriff>
{
    public override string GroupName => "Sheriff";

    [ModdedNumberOption("Sheriff Search Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}