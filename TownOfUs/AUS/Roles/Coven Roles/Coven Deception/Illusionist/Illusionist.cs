using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Illusionist(IntPtr cppPtr) : CovenRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Illusionist";
    public string revealText => "can alter a person's appearance to others.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are a talented magician using cheap tricks to fool the town.";
    public Color RoleColor { get; set; } = RoleColors.Coven;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Coven;
    public Alignment Alignment => Alignment.CovenDeception;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseSabotage = OptionGroupSingleton<CovenOptions>.Instance.CanSabotage,
        CanUseVent = OptionGroupSingleton<CovenOptions>.Instance.CanVent,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.IllusionistRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can keep their teammates undetected from investigatives.\n" +
            "Kill all who would oppose the Coven." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- With the Necronomicon, you can deal a Basic Attack to non-Coven targets.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Cast",
            "You can Cast an illusion on a Coven member at Night.\n" +
            "Your target will appear as a member of the Town to Investigative roles.\n" +
            "This is an Astral visit.",
            AUSAssets.Illusionist_Cast),
    ];


    public bool WinConditionMet() => CovenGameOver.WinConditionMet(this);
    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet() || CovenGameOver.AnyCovenWon(gameOverReason);
    }
}

public sealed class Illusionist_Cast : TownOfUsRoleButton<Illusionist, PlayerControl>, IButtonClick
{
    public override string Name => "Cast";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Coven;
    public override float Cooldown => Player.HasNecronomicon() ? OptionGroupSingleton<CovenOptions>.Instance.Cooldown : 
        OptionGroupSingleton<Illusionist_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Illusionist_Cast;

    public override void ClickHandler()
    {
        bool isAstral = OptionGroupSingleton<Illusionist_Options>.Instance.CastIsAstral && !Target.Is(Faction.Coven);
        if (button.IsTargetingValid(Player, Target, Player.HasNecronomicon(), isAstral)) base.ClickHandler();
    }

    public override PlayerControl? GetTarget()
    {
        if (Player.HasNecronomicon()) 
            return Player.GetClosestLivingPlayer(true, Distance);

        return Player.GetClosestLivingPlayer(true, Distance, predicate: x => x.Is(Faction.Coven));
    }

    protected override void OnClick() => Click(Player, Target);
    public void Click(PlayerControl player, PlayerControl Target = null)
    {
        if (Target == null)
            return;

        if (Target.Is(Faction.Coven)) Target.RpcAddModifier<IllusionedModifier>(Player);
        else if (Player.HasNecronomicon())
        {
            if (Player.CanKill(Target))
            {
                Player.RpcCustomMurder(Target);
                VisitingMechanic.RpcAddDeathReason(Target, (int)DeathReasonShow.KilledByTheCoven);
            }
            else Player.Notify(Feedback.TooMuchDefense(Player, Target), NotifyMode.InstantlyAndMeeting);
        }

        CustomButtonSingleton<Illusionist_SelfIllusion>.Instance.ResetCooldownAndOrEffect();
    }
}

public sealed class Illusionist_SelfIllusion : TownOfUsRoleButton<Illusionist>, IButtonClick
{
    public override string Name => "Self Illusion";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => RoleColors.Coven;
    public override float Cooldown => OptionGroupSingleton<Illusionist_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Illusionist_Cast;
    public override ButtonLocation Location => ButtonLocation.BottomLeft;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Player, false, false)) base.ClickHandler();
    }

    protected override void OnClick() => Click(Player);
    public void Click(PlayerControl player, PlayerControl Target = null)
    {
        Player.RpcAddModifier<IllusionedModifier>(Player);

        CustomButtonSingleton<Illusionist_Cast>.Instance.ResetCooldownAndOrEffect();
    }
}

public sealed class Illusionist_Options : AbstractOptionGroup<Illusionist>
{
    public override string GroupName => "Illusionist";

    [ModdedNumberOption("Illusionist Cast Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedToggleOption("Illusionist Cast Are Astral")]
    public bool CastIsAstral { get; set; } = true;
}