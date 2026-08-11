/*using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Inquisitor(IntPtr cppPtr) : NeutralRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Inquisitor";
    public string revealText => "";
    public string RoleDescription => "Better Town Of Salem 2";
    public string RoleLongDescription => "You are a missionary who is determined to eradicate those accused of heresy.";
    public Color RoleColor { get; set; } = RoleColors.Inquisitor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Neutral;
    public Alignment Alignment => Alignment.NeutralEvil;

    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.Basic;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.Basic;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.InquisitorRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that \n" +
            "Eliminate all Heretics." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- You will automatically counterattack RoleBlockers.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities =>
    [
        new("Vanquish",
            "You can go on Cautious at Night.\n" +
            "You will prevent yourself from counter attacking RoleBlockers.",
            AUSAssets.Inquisitor_Cautious),

        new("Cautious",
            "You can go on Cautious at Night.\n" +
            "You will prevent yourself from counter attacking RoleBlockers.",
            AUSAssets.Inquisitor_Cautious),
    ];

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet() || AnyWon(gameOverReason);
    }

    public static string Info(NotificationType type)
    {
        return "You have slain a non-Heretic, thus you may no longer Vanquish anyone.";
    }

    public override void Initialize(PlayerControl player) // This patches the ability sprite since it's loaded on runtime.
    {
        RoleBehaviourStubs.Initialize(this, player);

        CustomButtonSingleton<Inquisitor_Attack>.Instance.OverrideSprite(AUSAssets.Inquisitor_Attack.LoadAsset());
        CustomButtonSingleton<Inquisitor_Cautious>.Instance.OverrideSprite(AUSAssets.Inquisitor_Cautious.LoadAsset());
    }

    public void Role_OnMeetingStart()
    {
        if (OptionGroupSingleton<Inquisitor_Options>.Instance.Mode == InquisitorMode.TOS1)
            return;

        if (!killedThisNight) Bloodlust = 0;
        killedThisNight = false;

        ManageAttack();
    }

    public void Role_OnRoundStart()
    {
        if (OptionGroupSingleton<Inquisitor_Options>.Instance.Mode == InquisitorMode.TOS1)
            return;

        if (Player.AmOwner && !Player.HasDied())
            Player.Notify(Info(Bloodlust), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.InquisitorRoleCard.LoadAsset());
    }

    public int Bloodlust;
    public bool killedThisNight;

    public void ManageAttack()
    {
        if (Bloodlust >= 2) AttackDefenseMechanic.RpcApplyAttack(Player, Attack.Powerful, true, true);
        else AttackDefenseMechanic.RpcApplyAttack(Player, Attack.Basic, true, true);
    }
}

public sealed class Inquisitor_Attack : TownOfUsRoleButton<Inquisitor, PlayerControl>
{
    public override string Name => "Attack";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Inquisitor();
    public override float Cooldown =>
        (OptionGroupSingleton<Inquisitor_Options>.Instance.Mode == InquisitorMode.TOS1 ?
        OptionGroupSingleton<Inquisitor_Options>.Instance.Cooldown_TOS1.GetFloatData() :
        OptionGroupSingleton<Inquisitor_Options>.Instance.Cooldown_TOS2.GetFloatData());
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Inquisitor_Attack;

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);
        if (KeybindIcon != null && OptionGroupSingleton<Inquisitor_Options>.Instance.Mode == InquisitorMode.TOS2)
        {
            KeybindIcon.transform.localPosition = new Vector3(0.4f, 0.45f, -9f);
        }
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        if (playerControl.IsRole<Inquisitor>() && OptionGroupSingleton<Inquisitor_Options>.Instance.Mode == InquisitorMode.TOS2)
        {
            var role = playerControl.GetRole<Inquisitor>();
            Button?.usesRemainingText.gameObject.SetActive(true);
            Button?.usesRemainingSprite.gameObject.SetActive(true);
            Button!.usesRemainingText.text = role.Bloodlust.ToString();
        }

        base.FixedUpdate(playerControl);
    }

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, true, true)) base.ClickHandler();
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick() => Click(Player, Target);
    public void Click(PlayerControl player, PlayerControl Target = null)
    {
        if (Target == null)
            return;

        if (Role.Bloodlust >= 2)
        {
            int kills = Player.Rampage(Target, DeathReasonShow.StabbedByAInquisitor);
            Role.Bloodlust += kills;
            Role.Bloodlust -= 2;

            if (kills > 0) Role.killedThisNight = true;
        }
        else if (Player.CanKill(Target))
        {
            if (OptionGroupSingleton<Inquisitor_Options>.Instance.Mode == InquisitorMode.TOS2)
            {
                Role.Bloodlust++;
                Role.killedThisNight = true;

                Player.Notify(Inquisitor.Info(Role.Bloodlust), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.InquisitorRoleCard.LoadAsset());
            }

            Player.RpcCustomMurder(Target);
            VisitingMechanic.RpcAddDeathReason(Target, (int)DeathReasonShow.StabbedByAInquisitor);
        }
        else Player.Notify(Feedback.TooMuchDefense(Player, Target), NotifyMode.InstantlyAndMeeting);

        Role.ManageAttack();
    }
}

public sealed class Inquisitor_Cautious : TownOfUsRoleButton<Inquisitor>
{
    public override string Name => "Cautious";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => RoleColors.Inquisitor();
    public override float Cooldown => 1f;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Inquisitor_Cautious;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Player, false, false)) base.ClickHandler();
    }

    protected override void OnClick() => Click(Player);
    public void Click(PlayerControl player, PlayerControl Target = null)
    {
        if (Player.HasModifier<CautiousModifier>()) Player.RpcRemoveModifier<CautiousModifier>();
        else Player.RpcAddModifier<CautiousModifier>(Player);
    }
}

public sealed class Inquisitor_Options : AbstractOptionGroup<Inquisitor>
{
    public override string GroupName => "Inquisitor";

    [ModdedEnumOption("Inquisitor Mode", typeof(InquisitorMode), ["TOS", "TOS2"])]
    public InquisitorMode Mode { get; set; } = InquisitorMode.TOS2;

    // --- TOS ---
    public ModdedNumberOption Cooldown_TOS1 { get; } = new("Inquisitor Attack Cooldown", 25f, 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)
    { Visible = () => OptionGroupSingleton<Inquisitor_Options>.Instance.Mode == InquisitorMode.TOS1 };

    public ModdedToggleOption CanVent_TOS1 { get; } = new("Inquisitor Can Vent", true)
    { Visible = () => OptionGroupSingleton<Inquisitor_Options>.Instance.Mode == InquisitorMode.TOS1 };

    // --- TOS2 ---
    public ModdedNumberOption Cooldown_TOS2 { get; } = new("Inquisitor Attack Cooldown", 25f, 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)
    { Visible = () => OptionGroupSingleton<Inquisitor_Options>.Instance.Mode == InquisitorMode.TOS2 };

    public ModdedToggleOption CanVent_TOS2 { get; } = new("Inquisitor Can Vent", true)
    { Visible = () => OptionGroupSingleton<Inquisitor_Options>.Instance.Mode == InquisitorMode.TOS2 };
}*/