using Il2CppInterop.Runtime.Attributes;
using System.Collections;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Admirer(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Admirer";
    public string revealText => "is infatuated.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are a hopeless romantic searching for your soulmate.";
    public Color RoleColor { get; set; } = RoleColors.Town;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Alignment Alignment => Alignment.TownSupport;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.AdmirerRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var info = ICustomAURole.SetNewTabText(this);

        // Only show info if we have data
        info.AppendLine();
        info.AppendLine("Obsession: " + Obsession.GetAlignment().ToSpacedString().ApplyKeywords());
        if (RejectedPlayers.Count > 0)
        {
            info.AppendLine();
            info.AppendLine("Rejected Players:");
            foreach (var rejected in RejectedPlayers) info.AppendLine($"{rejected.Name()}");
        }

        return info;
    }

    public string GetAdvancedDescription()
    {
        return
            $"Attack: {Attack}\n" +
            $"Defense: {Defense}\n" +
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that is assigned an Obsession and must find them to begin caring for them. They may also Bestow players to grant them permanent beneficial effects.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- You are assigned an Obsession at the start of the game, which they can only be a Town role.\n" +
            $"- Your Obsession cannot be Town Executive or Town Government roles.\n" +
            $"- You cannot Bestow Day 1.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Admire",
            "You can Admire a player at Night.\n" +
            "You will see if your target is your Obsession. If so, you may now Care for them at Night.",
            AUSAssets.Admirer_Admire),
        new("Care",
            "You can Care for your Obsession at Night.\n" +
            "You will grant yourself and your Obsession Powerful Defense. You will know if your Obsession or yourself was attacked..",
            AUSAssets.Admirer_Care),
        new("Bestow",
            "You can Bestow a player during the Day.\n" +
            "You will grant your target Astral visits, RoleBlock Immunity, and Control Immunity for the rest of the game.",
            AUSAssets.Admirer_Bestow),
    ];

    public static string Info(NotificationType type, PlayerControl target)
    {
        if (type is NotificationType.Admirer_Obsession) return $"Your secret Obsession is a {target.GetAlignment().ToSpacedString()}!";
        if (type is NotificationType.Admirer_Rejected) return $"{target.Name()} has rejected you. They were not your Obsession!";
        if (type is NotificationType.Admirer_Accepted) return $"{target.Name()} is your Obsession. You may Care for them now.";
        return $"Error";
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (Player.AmOwner())
        {
            meetingMenu = new MeetingMenu(
                this,
                ClickGuess,
                MeetingAbilityType.Click,
                AUSAssets.Prosecutor_Prosecute,
                null!,
                IsExempt)
            {
                Position = new Vector3(-0.40f, 0f, -3f)
            };
        }
    }

    public void Role_AfterMurder(PlayerControl victim)
    {
        // Get new Obsession. This guy falls in love too easily:skull:
        if (victim == Obsession && !foundObsession) RollForObsession();
    }

    public void Role_OnMeetingStart()
    {
        if (Player.HasDied())
            return;

        if (DayNightMechanic.DayCount == 0 && Player.AmOwner()) RollForObsession();

        SmartAdmirer.Start();
        if (Player.AmOwner) Coroutines.Start(GenButtons());
    }

    private void RollForObsession()
    {
        var targets = PlayerControl.AllPlayerControls.ToArray().Where(x => x != Player && x.Is(Faction.Town) && !x.IsTPow(true) && !x.HasDied()).ToList();
        if (targets.Count == 0)
            return;

        System.Random rndIndex = new();
        var randomTarget = targets[rndIndex.Next(0, targets.Count)];

        Obsession = randomTarget;

        Player.Notify(Info(NotificationType.Admirer_Obsession, Obsession), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.AdmirerRoleCard.LoadAsset());
    }

    public IEnumerator GenButtons(float delay = 3f)
    {
        yield return new WaitForSeconds(delay);
        meetingMenu.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && Charges > 0 && DayNightMechanic.DayCount >= 2);
    }

    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);

        if (Player.AmOwner)
        {
            meetingMenu.HideButtons();
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
            meetingMenu = null!;
        }
    }

    public void ClickGuess(PlayerVoteArea voteArea, MeetingHud __)
    {
        var target = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId).Object;

        Charges--;
        target.RpcAddModifier<BestowedModifier>(Player);

        if (Player.AmOwner)
        {
            meetingMenu?.HideButtons();
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId == Player.PlayerId || Player.Data.IsDead || voteArea!.AmDead;
    }

    public MeetingMenu meetingMenu;
    public int Charges = (int)OptionGroupSingleton<Admirer_Options>.Instance.BestowCharges;

    public bool foundObsession;
    public PlayerControl Obsession;
    public List<PlayerControl> RejectedPlayers = new List<PlayerControl>();
    public void Function(PlayerControl target, int Button)
    {
        if (Button is 1)
        {
            if (Obsession == target)
            {
                foundObsession = true;
                Player.Notify(Info(NotificationType.Admirer_Accepted, target), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.AdmirerRoleCard.LoadAsset());
            }
            else
            {
                RejectedPlayers.Add(target);
                Player.Notify(Info(NotificationType.Admirer_Rejected, target), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.AdmirerRoleCard.LoadAsset());
            }
        }
        else if (Button is 2)
        {
            Obsession.RpcAddModifier<BarrieredModifier>(Player);
            AttackDefenseMechanic.RpcApplyDefense(Obsession, Defense.Powerful);

            Player.RpcAddModifier<BarrieredModifier>(Player);
            AttackDefenseMechanic.RpcApplyDefense(Player, Defense.Powerful, visualize: true);
        }
    }
}

public sealed class Admirer_Admire: TownOfUsRoleButton<Admirer, PlayerControl>
{
    public override string Name => "Admire";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => OptionGroupSingleton<Admirer_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Admirer_Admire;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 1, false, true);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !Role.RejectedPlayers.Contains(x));
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && !Role.foundObsession;
    }
}

public sealed class Admirer_Care : TownOfUsRoleButton<Admirer>
{
    public override string Name => "Care";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => OptionGroupSingleton<Admirer_Options>.Instance.CareCD;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Admirer_Care;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Player, 2, false, false);
    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Role.foundObsession;
    }
}

public sealed class Admirer_Options : AbstractOptionGroup<Admirer>
{
    public override string GroupName => "Admirer";

    [ModdedNumberOption("Admirer Admire Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("Admirer Care Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float CareCD { get; set; } = 25f;

    [ModdedNumberOption("Admirer Max Cares", 0f, 30f, 1f, MiraNumberSuffixes.None, zeroInfinity: true)]
    public float Charges { get; set; } = 2;

    [ModdedNumberOption("Admirer Max Bestows", 0f, 30f, 1f, MiraNumberSuffixes.None, zeroInfinity: true)]
    public float BestowCharges { get; set; } = 2;
}