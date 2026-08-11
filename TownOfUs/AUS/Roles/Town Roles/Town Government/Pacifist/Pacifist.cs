using Il2CppInterop.Runtime.Attributes;
using System.Collections;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class Pacifist(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Pacifist";
    public string revealText => "intends to rally everyone to protesting.";
    public string RoleDescription => "Better Town Of Salem 2";
    public string RoleLongDescription => "You are a voice of peace rallying the Town to rise against evil.";
    public Color RoleColor { get; set; } = RoleColors.Town;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Alignment Alignment => Alignment.TownGovernment;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.PacifistRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can Rally the Town in success to force evils to attempt to vote out the Pacifist, if failed, Town will immediately win.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Rally",
            "You can Rally players at Night.\n" +
            "If all alive Town members are Rallied and no evils are Rallied, a Protest will begin tomorrow.\n" +
            "During a Protest, only evils and the Pacifist can vote. If the Pacifist is not voted out, the Town will win. If the Pacifist is lynched, the Day will continue as normal.",
            AUSAssets.Pacifist_Rally),

        new("Self Reflection",
            "You can Self Reflect yourself at Night.\n" +
            "You will appear to be the Town role you selected to evil roles that learn your role.",
            AUSAssets.Pacifist_SelfReflection),
    ];

    public string GetAttributes()
    {
        return
            $"- You cannot Rally until a Town member has died or it is past Day 4.\n" +
            $"- You cannot Rally during a TT Hunt.";
    }

    public static string Info(NotificationType type)
    {
        if (type == NotificationType.Pacifist_BeginProtest) return $"The Pacifist has united the Town. A Protest has begun!";
        if (type == NotificationType.Pacifist_RallyFail) return $"Your Rally failed. Not all Town has answered your call.";
        if (type == NotificationType.Pacifist_RallySuccess) return $"Your Rally has united the Town. The Protest will begin tomorrow!";
        return $"A Protest is underway. Voting will be anonymous, only the Pacifist and their enemies may vote.";
    }

    [MethodRpc((uint)AUSRpc.RpcTryStartProtest)]
    public static void RpcTryStartProtest(PlayerControl player, bool success)
    {
        if (player.Data.Role is not Pacifist)
        {
            Logger<AUSPlugin>.Error("RpcTryStartProtest - Invalid Pacifist");
            return;
        }

        var pacifist = player.GetRole<Pacifist>();
        if (pacifist.Player.AmOwner())
        {
            if (success) pacifist.Player.Notify(Info(NotificationType.Pacifist_RallySuccess), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.PacifistRoleCard.LoadAsset());
            else pacifist.Player.Notify(Info(NotificationType.Pacifist_RallyFail), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.PacifistRoleCard.LoadAsset());
        }

        if (success)
        {
            PlayerControl.LocalPlayer.Notify(Info(NotificationType.Pacifist_BeginProtest), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.PacifistRoleCard.LoadAsset());
            PlayerControl.LocalPlayer.Notify(Info(NotificationType.Pacifist_UnderwayProtest), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.PacifistRoleCard.LoadAsset());
        }
    }

    public IEnumerator OpenMenu()
    {
        if (Minigame.Instance != null)
            yield break;

        var shapeMenu = GuesserMenu.Create();
        shapeMenu.Begin(IsRoleValid, ClickRoleHandle);

        void ClickRoleHandle(RoleBehaviour role)
        {
            Player.RpcAddModifier<SelfReflectionModifier>(RoleId.Get(role.GetType()));
            shapeMenu.Close();
        }
    }

    private bool IsRoleValid(RoleBehaviour role)
    {
        if (role.IsDead) return false;
        if (role is IGhostRole) return false;
        if (role is not ICustomAURole) return false;

        if (role is Amnesiac && OptionGroupSingleton<Amnesiac_Options>.Instance.Mode == AmnesiacMode.TOS1) return false;
        if (role is ICustomAURole customRole2 && customRole2.Faction != Faction.Town) return false;
        return true;
    }

    public void Role_OnMeetingStart()
    {
        if (DayNightMechanic.DayCount == 1 || Player.HasDied())
            return;

        bool anyEvilsRallied = ModifierUtils.GetActiveModifiers<RalliedModifier>(x => x.Caster == Player && !x.Player.Is(Faction.Town)).Any();
        bool anyProtest = ModifierUtils.GetActiveModifiers<ProtestModifier>(x => x.Caster == Player).Any();
        var ralliedPlayers = ModifierUtils.GetPlayersWithModifier<RalliedModifier>(x => x.Caster == Player);
        var allTown = PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Town) && !x.HasDied() && x != Player);
        var allAnyTown = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Town) && x != Player).ToList();
        if (ralliedPlayers.Count() == allTown && !anyProtest && !anyEvilsRallied)
        {
            foreach (var rallied in allAnyTown) rallied.RpcAddModifier<ProtestModifier>(Player);
            RpcTryStartProtest(Player, true);
        }
        else if (ralliedPlayers.Any())
        {
            RpcTryStartProtest(Player, false);
            foreach (var rallied in ralliedPlayers.ToList()) rallied.RpcRemoveModifier<RalliedModifier>();
        }
    }

    public void Role_OnRoundStart()
    {
        if (ModifierUtils.GetActiveModifiers<ProtestModifier>(x => x.Caster == Player).Any())
        {
            if (!Player.HasDied()) MiscUtils.EndGame(GameOverReason.CrewmatesByVote);
            else
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.HasModifier<ProtestModifier>()) player.RpcRemoveModifier<ProtestModifier>();
                }

                if (AmongUsClient.Instance.AmHost)
                {
                    DayNightMechanic.DayCount--;
                    DayNightMechanic.StartDayOne(PlayerControl.LocalPlayer);
                }
            }
        }
    }

    public RoleBehaviour reflectedRole;
    public void Function(PlayerControl target, int Button)
    {
        if (Button is 1)
        {
            if (target.HasModifier<RalliedModifier>(x => x.Caster == Player)) target.RpcRemoveModifier<RalliedModifier>();
            else target.RpcAddModifier<RalliedModifier>(Player);
        }
    }
}

public sealed class Pacifist_Rally : TownOfUsRoleButton<Pacifist, PlayerControl>
{
    public override string Name => "Rally";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => 0.5f;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Pacifist_Rally;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 1, false, false);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance);
    }

    public override bool CanUse()
    {
        var deadTown = PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Town) && x.HasDied());
        if (deadTown == 0 && DayNightMechanic.DayCount < 4) return false;
        return base.CanUse();
    }
}

public sealed class Pacifist_SelfReflection : TownOfUsRoleButton<Pacifist>
{
    public override string Name => "Self Reflection";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => 1f;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Pacifist_SelfReflection;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Player, false, false)) base.ClickHandler();
    }

    protected override void OnClick() => Click(Player);
    public void Click(PlayerControl player, PlayerControl Target = null)
    {
        Coroutines.Start(Role.OpenMenu());
    }
}