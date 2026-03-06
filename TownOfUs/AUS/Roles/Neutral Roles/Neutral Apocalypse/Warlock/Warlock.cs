using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Warlock(IntPtr cppPtr) : CovenRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Warlock";
    public string revealText => "is a sorcerer who curses people.";
    public string RoleDescription => "Better Town Of Salem 2";
    public string RoleLongDescription => "You are an acolyte of Death who curses the Town.";
    public Color RoleColor { get; set; } = RoleColors.Apocalypse;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Neutral;
    public Alignment Alignment => Alignment.NeutralApocalypse;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseSabotage = OptionGroupSingleton<ApocOptions>.Instance.CanSabotage,
        CanUseVent = OptionGroupSingleton<Warlock_Options>.Instance.CanVent,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.WarlockRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can frame others to disrupt Investigatives, gains Grimoires when their targets vote players, and turns into Death once they have 6 Grimoires.\n" +
            "Bring forth the Apocalypse, kill everyone in the town. You will win with other Neutral Apocalypse members." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- Every Night the Grimoire will charge itself passively.\n" +
            $"- If you have {OptionGroupSingleton<Warlock_Options>.Instance.Required} Grimoires, you will turn into Death, horseman of the apocalypse.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Curse",
            "You can Curse a player at Night.\n" +
            "If a Curse player visits someone, frame their target, making them seen as a member of the Apocalypse to Investigative roles. If your target doesn’t visit, frame them instead.\n" +
            "You will know who your targets Curse, if any.\n" +
            "If a Cursed player votes any player during the Day, your Grimoire will empower at nightfall.\n" +
            "There can only be up to 1 Cursed player at once. If you attempt to Curse a second player, the curse will override the first.",
            AUSAssets.Warlock_Curse),
    ];

    public bool WinConditionMet() => ApocGameOver.WinConditionMet(this);
    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet() || ApocGameOver.AnyApocWon(gameOverReason);
    }

    public static string Info(NotificationType type, PlayerControl player, PlayerControl target)
    {
        if (type == NotificationType.Warlock_PassiveGain) return "The Grimoire has empowered itself.";
        if (type == NotificationType.Warlock_TargetVoted) return $"{player.Name()} has voted Guilty, which has empowered your Grimoire!";
        return $"{player.Name()} visited {target.Name()} and framed them!";
    }

    [MethodRpc((uint)AUSRpc.RpcNotifyWarlock)]
    public static void RpcNotify(PlayerControl warlock, PlayerControl player, PlayerControl target, int notifyType)
    {
        if (warlock.AmOwner() && !warlock.HasDied())
        {
            var notify = (NotificationType)notifyType;
            switch (notify)
            {
                case NotificationType.Warlock_PassiveGain:
                    warlock.Notify(Info(notify, null, null), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.WarlockRoleCard.LoadAsset());
                    break;
                case NotificationType.Warlock_TargetVisited:
                    warlock.Notify(Info(notify, player, target), NotifyMode.OnlyMeeting, sprite: AUSAssets.WarlockRoleCard.LoadAsset());
                    break;
                case NotificationType.Warlock_TargetVoted:
                    warlock.Notify(Info(notify, player, target), NotifyMode.OnlyMeeting, sprite: AUSAssets.WarlockRoleCard.LoadAsset());
                    break;
            }
        }
    }

    [MethodRpc((uint)AUSRpc.RpcEmpowerGrimoire)]
    public static void RpcEmpowerGrimoire(PlayerControl player, PlayerControl voter)
    {
        if (player.Data.Role is not Warlock)
        {
            Logger<AUSPlugin>.Error("RpcEmpowerGrimoire - Invalid Warlock");
            return;
        }

        var warlock = player.GetRole<Warlock>();
        if (warlock.Player.AmOwner())
        {
            warlock.Grimoires++;

            warlock.Player.Notify(Warlock.Info(NotificationType.Warlock_TargetVoted, voter, null), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.WarlockRoleCard.LoadAsset());
        }
    }

    public void Role_OnMeetingStart()
    {
        if (Player.HasDied())
            return;

        if (DayNightMechanic.DayCount > 1)
        {
            Grimoires++;
            Player.Notify(Info(NotificationType.Warlock_PassiveGain, null, null), NotifyMode.OnlyMeeting, sprite: AUSAssets.WarlockRoleCard.LoadAsset());
        }

        if (!Player.HasDied() && Grimoires >= (int)OptionGroupSingleton<Warlock_Options>.Instance.Required)
        {
            Player.RpcChangeRole(RoleId.Get<Death>());
        }
    }

    public void Role_OnRoundStart()
    {
        foreach (var cursed in ModifierUtils.GetActiveModifiers<CursedModifier>(x => x.Caster == Player))
            cursed.Player.RpcRemoveModifier<CursedModifier>();
    }

    public int Grimoires;
}

public sealed class Warlock_Curse : TownOfUsRoleButton<Warlock, PlayerControl>
{
    public override string Name => "Curse";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Apocalypse;
    public override float Cooldown => OptionGroupSingleton<Warlock_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Warlock_Curse;

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
        if (playerControl.IsRole<Warlock>())
        {
            var role = playerControl.GetRole<Warlock>();
            Button?.usesRemainingText.gameObject.SetActive(true);
            Button?.usesRemainingSprite.gameObject.SetActive(true);
            Button!.usesRemainingText.text = role.Grimoires.ToString() + $"/{OptionGroupSingleton<Warlock_Options>.Instance.Required}";
        }

        base.FixedUpdate(playerControl);
    }

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, false, true)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        if (Target == null)
            return;

        Target.RpcAddModifier<WarlockFramedModifier>(Player);
        Target.RpcAddModifier<CursedModifier>(Player);
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !x.Is(Alignment.NeutralApocalypse) && !x.HasModifier<CursedModifier>(x => x.Caster == Player));
    }
}

public static class Warlock_Events
{
    [RegisterEvent()]
    public static void HandleVoteEvent(HandleVoteEvent @event)
    {
        var votingPlayer = @event.Player;
        var suspectPlayer = @event.TargetPlayerInfo;

        if (votingPlayer.TryGetModifier<CursedModifier>(out var cursed))
        {
            if (@event.TargetId == MeetingHud.Instance.SkipVoteButton.TargetPlayerId)
            {
                return;
            }

            var warlock = cursed.Caster.GetRole<Warlock>();
            if (warlock.Player.AmOwner())
            {
                warlock.Grimoires++;
                warlock.Player.Notify(Warlock.Info(NotificationType.Warlock_TargetVoted, votingPlayer, null), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.WarlockRoleCard.LoadAsset());
            }
        }
    }
}

public sealed class Warlock_Options : AbstractOptionGroup<Warlock>
{
    public override string GroupName => "Warlock";

    [ModdedNumberOption("Warlock Curse Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("Warlock Grimoires Required", 1f, 10f, 1f, MiraNumberSuffixes.None)]
    public float Required { get; set; } = 6f;

    [ModdedToggleOption("Warlock Can Vent")]
    public bool CanVent { get; set; } = false;
}