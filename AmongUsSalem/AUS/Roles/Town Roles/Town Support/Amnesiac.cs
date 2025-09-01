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
    public string RoleName => TouLocale.Get(TouNames.Amnesiac, "Amnesiac");
    public string revealText => "placeholder.";
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

    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;

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
            "<color=#06e00c>Amnesiac</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#06e00c>Town</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#06e00c>Town</color> <color=#1e45d4>Support</color>" +
            "\n<color=#fdbc00>Goal:</color> Hang every criminal and evildoer." +
            $"\n\nAttributes:" +
            "\nNone." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public void OnMeetingStart(MeetingHud __instance)
    {
        if (Player.HasDied())
            return;

        var deadTown = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Data.IsDead && x.Is(Faction.Town)).ToList();
        if (deadTown.Count() > 0)
        {
            deadTown.Shuffle();

            var targetRole = deadTown[0].GetRoleWhenAlive();
            if (Player.AmOwner() && targetRole.Player is IAUSRole ausRole)
            {
                MiscUtils.ShowNotification(Info(ausRole), Color.white, AUSAssets.AmnesiacRoleCard.LoadAsset());
                MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Amnesiac Info"), Info(ausRole));
            }

            Player.RpcChangeRole((ushort)targetRole.Role);
        }
    }

    public static string Info(IAUSRole ausRole)
    {
        return $"You <b><color=#4a86e8>Remembered</color></b> that you were like the {MiscUtils.GetRoleColour(ausRole.RoleName)}.";
    }
}