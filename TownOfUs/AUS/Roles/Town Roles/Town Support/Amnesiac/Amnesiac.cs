using System.Text;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Amnesiac(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Amnesiac";
    public string revealText => "does not remember their role.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You do not remember who you are.";
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
        Icon = AUSAssets.AmnesiacRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that becomes the role of a dead Town member, which can be very useful if a highly valuable Town role dies very early.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- You will prioritize Remembering Town Executive, Catalyst & Town Government roles.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Remember",
            "You will automatically attempt to Remember a Town role in the Graveyard during the Day.\n" +
            "You will become a random Town role in the Graveyard.\n" +
            "If a player dies during the Day, they will have a chance to be Remembered along with the people that die during the Night.\n" +
            "If the Teams Modifier is enabled, you may Remember any role.",
            AUSAssets.Amnesiac_Remember),
    ];

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        AUSPlugin.DebugLogMessage("Amnesiac OnMeetingStart called!");
        if (Player.HasDied())
            return;

        AUSPlugin.DebugLogMessage("Potential Remember Targets: " + RememberablePlayers.Count);
        var rememberTarget = RememberablePlayers.Where(x => !x.HasModifier<RememberedModifier>())
            .OrderBy(x => x.IsTPow())
            .ThenBy(x => x.Is(Alignment.TownOutlier)).FirstOrDefault();
        if (rememberTarget != null)
        {
            var targetRole = rememberTarget.GetRoleWhenAlive();
            if (Player.AmOwner())
            {
                var roleWhenAlive = rememberTarget.GetRoleWhenAlive();
                Player.Notify(Info(roleWhenAlive.NiceName), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.AmnesiacRoleCard.LoadAsset());
            }

            // Prevents other Amnesiacs from remembering the same TPOW.
            if (rememberTarget.IsTPow() || rememberTarget.Is(Alignment.TownOutlier)) rememberTarget.RpcAddModifier<RememberedModifier>();

            Player.RpcChangeRole((ushort)targetRole.Role);
            if (Player.Data.Role is ICustomAURole customRole) customRole.Role_OnMeetingStart();
        }
    }

    public void OnTargetDeath(PlayerControl target)
    {
        var targetRole = target.GetRoleWhenAlive();
        if (targetRole is ICustomAURole customRole && customRole.Faction == Faction.Town && targetRole is not Amnesiac)
        {
            RememberablePlayers.Add(target);
        }
    }

    public static string Info(string text)
    {
        return $"You Remembered that you were like the {text}.";
    }

    public List<PlayerControl> RememberablePlayers = new List<PlayerControl>();
}

public static class Amnesiac_Events
{
    [RegisterEvent]
    public static void EjectionEvent(EjectionEvent @event)
    {
        NetworkedPlayerInfo exiled = @event.ExileController.initData.networkedPlayer;
        if (exiled != null)
        {
            PlayerControl player = exiled.Object;
            foreach (var amnesiac in MiscUtils.GetRoles<Amnesiac>()) amnesiac.OnTargetDeath(player);
        }
    }

    [RegisterEvent]
    public static void AfterMurderEvent(AfterMurderEvent @event)
    {
        foreach (var amnesiac in MiscUtils.GetRoles<Amnesiac>()) amnesiac.OnTargetDeath(@event.Target);
    }
}