using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class SerialKiller(IntPtr cppPtr) : CovenRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Serial Killer";
    public string revealText => "wants to kill everyone.";
    public string RoleDescription => "";
    public string RoleLongDescription => "You are a psychopath who wants everyone in the town to die.";
    public Color RoleColor { get; set; } = RoleColors.SerialKiller;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Neutral;
    public Alignment Alignment => Alignment.NeutralKilling;

    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.Basic;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.Basic;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<SerialKiller_Options>.Instance.CanVent,
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can deal Basic Attacks to targets early on and deal Powerful Attacks later in the game, being a threat to all.\n" +
            "Kill everyone in the town. You will win with other Serial Killers." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Attack",
            "You can Attack a player at Night.\n" +
            "You will deal a Basic Attack to your target.\n" +
            "If you successfully kill your target, gain 1 Bloodlust.\n" +
            "If you have 2 Bloodlust, your next attack will deal a Powerful Attack to your target and Rampage nearby players, as well as resetting your Bloodlust.\n" +
            "If you are RoleBlocked and you aren’t on Cautious, counterattack them with a Basic Attack.",
            AUSAssets.SerialKiller_Attack),

        new("Cautious",
            "You can go on Cautious at Night.\n" +
            "You will prevent yourself from counter attacking RoleBlockers.",
            AUSAssets.SerialKiller_Cautious),
    ];

    public static bool WinConditionMet(RoleBehaviour role)
    {
        var aliveNK = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<SerialKiller>());
        if (aliveNK == 0) return false;

        var result = MiscUtils.GetAlivePlayersToEnd().Count <= aliveNK && MiscUtils.KillersAliveCount == aliveNK;
        return result || LogicGameFlowPatches.EndGameEarlyCheck(role);
    }

    public static bool AnyWon(GameOverReason gameOverReason)
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.IsRole<SerialKiller>() && WinConditionMet(player.Data.Role)) return true;
        }
        return false;
    }

    public bool WinConditionMet() => WinConditionMet(this);
    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet() || AnyWon(gameOverReason);
    }

    public static string Info(int kills)
    {
        if (kills < 2) return "Your bloodlust is sated.";
        if (kills == 2) return "Your bloodlust is swelling.";
        return "Your bloodlust has reached its apex. You will deal a rampaging powerful attack tonight.";
    }

    public void Role_OnMeetingStart()
    {
        if (!killedThisNight) Bloodlust = 0;
        killedThisNight = false;

        ManageAttack();
    }

    public void Role_OnRoundStart()
    {
        if (Player.AmOwner() && !Player.HasDied())
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

public sealed class SerialKiller_Attack : TownOfUsRoleButton<SerialKiller, PlayerControl>
{
    public override string Name => "Attack";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.SerialKiller;
    public override float Cooldown => OptionGroupSingleton<SerialKiller_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.SerialKiller_Attack;

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);
        if (KeybindIcon != null)
        {
            KeybindIcon.transform.localPosition = new Vector3(0.4f, 0.45f, -9f);
        }
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        if (playerControl.IsRole<SerialKiller>())
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

    protected override void OnClick()
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
            Role.Bloodlust++;
            Role.killedThisNight = true;

            Player.RpcCustomMurder(Target);
            VisitingMechanic.RpcAddDeathReason(Target, (int)DeathReasonShow.StabbedByASerialKiller);

            Player.Notify(SerialKiller.Info(Role.Bloodlust + 1), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.SerialKillerRoleCard.LoadAsset());
        }
        else Player.Notify(Feedback.TooMuchDefense(Player, Target), NotifyMode.InstantlyAndMeeting);

        Role.ManageAttack();
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance);
    }
}

public sealed class SerialKiller_Cautious : TownOfUsRoleButton<SerialKiller>
{
    public override string Name => "Cautious";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => RoleColors.SerialKiller;
    public override float Cooldown => OptionGroupSingleton<SerialKiller_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.SerialKiller_Cautious;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Player, false, false)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        if (Player.HasModifier<CautiousModifier>()) Player.RpcRemoveModifier<CautiousModifier>();
        else Player.RpcAddModifier<CautiousModifier>(Player);
    }
}

public sealed class SerialKiller_Options : AbstractOptionGroup<SerialKiller>
{
    public override string GroupName => "SerialKiller";

    [ModdedNumberOption("Serial Killer Attack Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedToggleOption("Serial Killer Can Vent")]
    public bool CanVent { get; set; } = true;
}