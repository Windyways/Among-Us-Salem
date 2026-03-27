using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class SerialKiller(IntPtr cppPtr) : NeutralRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Serial Killer";
    public string revealText => "wants to kill everyone.";
    public string RoleDescription => OptionGroupSingleton<SerialKiller_Options>.Instance.Mode == SerialKillerMode.TOS1 ? "Town Of Salem" : "Town Of Salem 2";
    public string RoleLongDescription => "You are a psychopath who wants everyone in the town to die.";
    public Color RoleColor { get => RoleColors.SerialKiller(); set { } }
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.SerialKiller;
    public Alignment Alignment => Alignment.NeutralKilling;

    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.Basic;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.Basic;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent =
            (OptionGroupSingleton<SerialKiller_Options>.Instance.Mode == SerialKillerMode.TOS1 ?
            OptionGroupSingleton<SerialKiller_Options>.Instance.CanVent_TOS1.Value :
            OptionGroupSingleton<SerialKiller_Options>.Instance.CanVent_TOS2.Value),

        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.SerialKillerRoleCard
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
            (OptionGroupSingleton<SerialKiller_Options>.Instance.Mode == SerialKillerMode.TOS1 ? 
                $"The {RoleName} is a {Alignment.ToSpacedString()} role that can attack others, and may go on Cautious to prevent counterattacking RoleBlockers.\n" :
                $"The {RoleName} is a {Alignment.ToSpacedString()} role that can deal Basic Attacks to targets early on and deal Powerful Attacks later in the game, being a threat to all.\n") +
            "Kill everyone in the town. You will win with other Serial Killers." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            (OptionGroupSingleton<SerialKiller_Options>.Instance.Mode == SerialKillerMode.TOS1 ? 
                "" :
                $"- If you do not kill at Night, your Bloodlust will reset.\n") +
            $"- You will automatically counterattack RoleBlockers.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities =>
    [
        new("Attack", GetAttackDescription(), AUSAssets.SerialKiller_Attack),

        new("Cautious",
            "You can go on Cautious at Night.\n" +
            "You will prevent yourself from counter attacking RoleBlockers.",
            AUSAssets.SerialKiller_Cautious),
    ];

    private static string GetAttackDescription()
    {
        var opt = OptionGroupSingleton<SerialKiller_Options>.Instance;

        if (opt.Mode == SerialKillerMode.TOS1)
        {
            return "You can Attack a player at Night.\n" +
            "You will deal a Basic Attack to your target.\n";
        }

        return "You can Attack a player at Night.\n" +
            "You will deal a Basic Attack to your target.\n" +
            "If you successfully kill your target, gain 1 Bloodlust.\n" +
            "If you have 2 Bloodlust, your next attack will deal a Powerful Attack to your target and Rampage nearby players, as well as resetting your Bloodlust.\n" +
            "If you are RoleBlocked and you aren’t on Cautious, counterattack them with a Basic Attack.";
    }

    public static bool AnyWon(GameOverReason gameOverReason)
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.IsRole<SerialKiller>() && NeutralGameOver.WinConditionMet(player.Data.Role)) return true;
        }
        return false;
    }

    public bool WinConditionMet() => NeutralGameOver.WinConditionMet(this);
    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet() || AnyWon(gameOverReason);
    }

    public static string Info(int kills)
    {
        if (kills == 0) return "Your bloodlust is sated.";
        if (kills == 1) return "Your bloodlust is swelling.";
        return "Your bloodlust has reached its apex. You will deal a rampaging powerful attack tonight.";
    }

    public override void Initialize(PlayerControl player) // This patches the ability sprite since it's loaded on runtime.
    {
        RoleBehaviourStubs.Initialize(this, player);

        CustomButtonSingleton<SerialKiller_Attack>.Instance.OverrideSprite(AUSAssets.SerialKiller_Attack.LoadAsset());
        CustomButtonSingleton<SerialKiller_Cautious>.Instance.OverrideSprite(AUSAssets.SerialKiller_Cautious.LoadAsset());
    }

    public void Role_OnMeetingStart()
    {
        if (OptionGroupSingleton<SerialKiller_Options>.Instance.Mode == SerialKillerMode.TOS1)
            return;

        if (!killedThisNight) Bloodlust = 0;
        killedThisNight = false;

        ManageAttack();
    }

    public void Role_OnRoundStart()
    {
        if (OptionGroupSingleton<SerialKiller_Options>.Instance.Mode == SerialKillerMode.TOS1)
            return;

        if (Player.AmOwner && !Player.HasDied())
            Player.Notify(Info(Bloodlust), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.SerialKillerRoleCard.LoadAsset());
    }

    public int Bloodlust;
    public bool killedThisNight;

    public void ManageAttack()
    {
        if (Bloodlust >= 2) AttackDefenseMechanic.RpcApplyAttack(Player, Attack.Powerful, true, true);
        else AttackDefenseMechanic.RpcApplyAttack(Player, Attack.Basic, true, true);
    }
}

public sealed class SerialKiller_Attack : TownOfUsRoleButton<SerialKiller, PlayerControl>, IButtonClick
{
    public override string Name => "Attack";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.SerialKiller();
    public override float Cooldown =>
        (OptionGroupSingleton<SerialKiller_Options>.Instance.Mode == SerialKillerMode.TOS1 ?
        OptionGroupSingleton<SerialKiller_Options>.Instance.Cooldown_TOS1.GetFloatData() :
        OptionGroupSingleton<SerialKiller_Options>.Instance.Cooldown_TOS2.GetFloatData());
    public override LoadableAsset<Sprite> Sprite => AUSAssets.SerialKiller_Attack;

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);
        if (KeybindIcon != null && OptionGroupSingleton<SerialKiller_Options>.Instance.Mode == SerialKillerMode.TOS2)
        {
            KeybindIcon.transform.localPosition = new Vector3(0.4f, 0.45f, -9f);
        }
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        if (playerControl.IsRole<SerialKiller>() && OptionGroupSingleton<SerialKiller_Options>.Instance.Mode == SerialKillerMode.TOS2)
        {
            var role = playerControl.GetRole<SerialKiller>();
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
            int kills = Player.Rampage(Target, DeathReasonShow.StabbedByASerialKiller);
            Role.Bloodlust += kills;
            Role.Bloodlust -= 2;

            if (kills > 0) Role.killedThisNight = true;
        }
        else if (Player.CanKill(Target))
        {
            if (OptionGroupSingleton<SerialKiller_Options>.Instance.Mode == SerialKillerMode.TOS2)
            {
                Role.Bloodlust++;
                Role.killedThisNight = true;

                Player.Notify(SerialKiller.Info(Role.Bloodlust), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.SerialKillerRoleCard.LoadAsset());
            }

            Player.RpcCustomMurder(Target);
            VisitingMechanic.RpcAddDeathReason(Target, (int)DeathReasonShow.StabbedByASerialKiller);
        }
        else Player.Notify(Feedback.TooMuchDefense(Player, Target), NotifyMode.InstantlyAndMeeting);

        Role.ManageAttack();
    }
}

public sealed class SerialKiller_Cautious : TownOfUsRoleButton<SerialKiller>, IButtonClick
{
    public override string Name => "Cautious";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => RoleColors.SerialKiller();
    public override float Cooldown => 1f;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.SerialKiller_Cautious;

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

public sealed class SerialKiller_Options : AbstractOptionGroup<SerialKiller>
{
    public override string GroupName => "Serial Killer";

    [ModdedEnumOption("Serial Killer Mode", typeof(SerialKillerMode), ["TOS", "TOS2"])]
    public SerialKillerMode Mode { get; set; } = SerialKillerMode.TOS2;

    // --- TOS ---
    public ModdedNumberOption Cooldown_TOS1 { get; } = new("Serial Killer Attack Cooldown", 25f, 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)
    { Visible = () => OptionGroupSingleton<SerialKiller_Options>.Instance.Mode == SerialKillerMode.TOS1 };

    public ModdedToggleOption CanVent_TOS1 { get; } = new("Serial Killer Can Vent", true)
    { Visible = () => OptionGroupSingleton<SerialKiller_Options>.Instance.Mode == SerialKillerMode.TOS1 };

    // --- TOS2 ---
    public ModdedNumberOption Cooldown_TOS2 { get; } = new("Serial Killer Attack Cooldown", 25f, 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)
    { Visible = () => OptionGroupSingleton<SerialKiller_Options>.Instance.Mode == SerialKillerMode.TOS2 };

    public ModdedToggleOption CanVent_TOS2 { get; } = new("Serial Killer Can Vent", true)
    { Visible = () => OptionGroupSingleton<SerialKiller_Options>.Instance.Mode == SerialKillerMode.TOS2 };
}

public enum SerialKillerMode
{
    TOS1,
    TOS2,
    BTOS2, // Never making this LMAO (unless i copy Le Killer's Doppelganger code but idk)

    Empty
}