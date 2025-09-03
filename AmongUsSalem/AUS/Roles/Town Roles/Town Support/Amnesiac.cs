using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using Color = UnityEngine.Color;

namespace AmongUsSalem.Roles;

#region Amnesiac
#endregion
public sealed class Amnesiac(IntPtr cppPtr)
    : CrewmateRole(cppPtr), IAUSRole, IWikiDiscoverable
{
    public string RoleName { get; set; } = TouLocale.Get(TouNames.Amnesiac, "Amnesiac");
    public string revealText => "does not remember their role.";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction RoleFaction { get; set; } = Faction.Town;
    public Color RoleColor { get; set; } = AUSColors.Town;
    public Alignment Alignment => Alignment.TownSupport;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;


    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.AmnesiacRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#06E00C>Amnesiac</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#06E00C>Town</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#06E00C>Town</color> <color=#1e45d4>Support</color>" +
            "\n<color=#fdbc00>Goal:</color> Hang every criminal and evildoer." +
            $"\n\nAttributes:" +
            "\nYou will remember a Town role once one has died." +
            "\nYou will prioritize remembering Town Power roles first." +
            "\nThis might not be the first Town role listed in the graveyard." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Remember",
            "Each Day, you will automatically attempt to remember you were like a Town role that is in the graveyard until you remember a role.",
            AUSAssets.Amnesiac_Remember)
    ];

    public override void OnMeetingStart()
    {
        AUSPlugin.DebugLogMessage("Amnesiac OnMeetingStart called!");
        if (Player.HasDied())
            return;

        var rememberTarget = RememberablePlayers.OrderBy(x => x.Is(Alignment.TownPower)).FirstOrDefault();
        if (rememberTarget != null)
        {
            var targetRole = rememberTarget.GetRoleWhenAlive();
            if (Player.AmOwner() && targetRole.Player is IAUSRole ausRole)
            {
                MiscUtils.ShowNotification(Info(ausRole, rememberTarget), Color.white, AUSAssets.AmnesiacRoleCard.LoadAsset());
                MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Amnesiac Info"), Info(ausRole, rememberTarget));
            }

            Player.RpcChangeRole((ushort)targetRole.Role);
            if (Player.HasModifier<VampireRecruit>()) // This is so the Amnesiac carries over the Vampire colored role.
            {
                Player.RpcRemoveModifier<VampireRecruit>();
                
                var vampires = PlayerControl.AllPlayerControls.ToArray().Count(x => x.HasModifier<VampireRecruit>() && !x.HasDied());
                Player.RpcAddModifier<VampireRecruit>(vampires); 
            }
            foreach (var amnesiacs in MiscUtils.GetPlayersWithRole<Amnesiac>())
            {
                var amnesiac = amnesiacs.GetRole<Amnesiac>();
                amnesiac.RememberablePlayers.Remove(rememberTarget);
            }
        }
    }

    public void OnTargetDeath(PlayerControl target, DeathReason? reason)
    {
        if (target.Is(Faction.Town))
        {
            RememberablePlayers.Add(target);
        }
    }

    public static string Info(IAUSRole ausRole, PlayerControl remembered)
    {
        var roleColor = ausRole.RoleFaction != Faction.Neutral ? MiscUtils.GetFactionColour(remembered) : MiscUtils.GetRoleColour(ausRole.RoleName);
        return $"You <b><color=#4a86e8>Remembered</color></b> that you were like the <b><color=#" + roleColor.ToHtmlStringRGBA() + $">{ausRole.RoleName}</color></b>.";
    }

    public List<PlayerControl> RememberablePlayers = new List<PlayerControl>();
}