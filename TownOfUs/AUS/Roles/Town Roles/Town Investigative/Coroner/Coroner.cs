using Il2CppInterop.Runtime.Attributes;
using System.Collections;
using System.Text;
using TownOfUs.Modifiers;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Coroner(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Coroner";
    public string revealText => "is a skilled surgeon.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are an anatomist who uncovers the truth behind a townie's death.";
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
        Icon = AUSAssets.CoronerRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var info = ICustomAURole.SetNewTabText(this);

        info.AppendLine();

        if (AutopsiedRoles.Count == 0 && ExaminationResults.Count == 0)
        {
            info.AppendLine("No investigative results yet.");
            return info;
        }

        if (AutopsiedRoles.Count > 0)
        {
            info.AppendLine("<color=#FFD966>Autopsied Roles</color>");

            foreach (var role in AutopsiedRoles.Distinct())
            {
                info.AppendLine($"- {role.NiceName}".ApplyKeywords());
            }

            info.AppendLine();
        }

        if (ExaminationResults.Count > 0)
        {
            info.AppendLine("<color=#FFD966>Examined Players</color>");

            foreach (var result in ExaminationResults)
            {
                string icon = result.Match ? "<color=#00ff00>Y</color>" : "<color=#ff0000>N</color>";

                info.AppendLine($"{icon} {result.Player.Data.PlayerName}");
            }
        }

        return info;
    }

    public string GetAdvancedDescription()
    {
        return
            $"Attack: {Attack}\n" +
            $"Defense: {Defense}\n" +
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can find out who killed their autopsy targets.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- None.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Examine",
            "You can Examine a player at Night.\n\n" +
            "You will know if your target killed any players on your autopsy list.",
            AUSAssets.Coroner_Examine),

        new("Autopsy",
            "You can Autopsy a dead player during the Day.\n\n" +
            "You will know what role killed your target and add their killer to your autopsy list.",
            AUSAssets.Coroner_Autopsy),
    ];

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

    public void Role_OnMeetingStart()
    {
        if (Player.HasDied())
            return;

        SmartProsecutor.Start();
        if (Player.AmOwner) Coroutines.Start(GenButtons());
    }

    public IEnumerator GenButtons(float delay = 3f)
    {
        yield return new WaitForSeconds(delay);
        meetingMenu.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && DayNightMechanic.DayCount >= 2);
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

        if (target.TryGetModifier<DeathHandlerModifier>(out var modifier))
        {
            QueuedAutopsiedRoles.Add(modifier.KillerPlayer.GetRoleWhenAlive());
        }

        if (Player.AmOwner)
        {
            meetingMenu?.HideButtons();
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId == Player.PlayerId || Player.Data.IsDead || !(voteArea!.AmDead);
    }

    public MeetingMenu meetingMenu;
    public List<RoleBehaviour> QueuedAutopsiedRoles = new List<RoleBehaviour>();
    public List<RoleBehaviour> AutopsiedRoles = new List<RoleBehaviour>();

    public record ExaminationResult(PlayerControl Player, bool Match);

    public List<ExaminationResult> ExaminationResults = new();

    public void Role_OnRoundStart()
    {
        if (QueuedAutopsiedRoles.Count > 0)
        {
            AutopsiedRoles.AddRange(QueuedAutopsiedRoles);

            Player.Notify(Coroner_Notifications.Autopsy(QueuedAutopsiedRoles.FirstOrDefault()), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.CoronerRoleCard.LoadAsset());
            QueuedAutopsiedRoles.Clear();
        }
    }

    public void Function(PlayerControl target, int Button)
    {
        if (Button is 1)
        {
            bool match = AutopsiedRoles.Any(role => role == target.GetRoleWhenAlive());
            ExaminationResults.Add(new ExaminationResult(target, match));

            if (match)
            {
                Player.Notify(Coroner_Notifications.Evidence(target), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.CoronerRoleCard.LoadAsset());
            }
            else
            {
                Player.Notify(Coroner_Notifications.NotKiller(target), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.CoronerRoleCard.LoadAsset());
            }
        }
    }
}

public sealed class Coroner_Examine : TownOfUsRoleButton<Coroner, PlayerControl>
{
    public override string Name => "Examine";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => OptionGroupSingleton<Coroner_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Coroner_Examine;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 1, false, true);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance);
    }
}

public static class Coroner_Notifications
{
    public static string Evidence(PlayerControl target) => 
        $"You found evidence that shows {target.Name()} is a killer and their role is a {target.Data.Role.NiceName}.".ApplyKeywords();
    public static string NotKiller(PlayerControl target) =>
        $"You couldn't find any evidence that {target.Name()} is a killer.".ApplyKeywords();
    public static string Autopsy(RoleBehaviour role)
    {
        return $"You have added a {role.NiceName} to your list of autopsy results. You can now find them at night.".ApplyKeywords();
    }
}

public sealed class Coroner_Options : AbstractOptionGroup<Coroner>
{
    public override string GroupName => "Coroner";

    [ModdedNumberOption("Coroner Examine Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}