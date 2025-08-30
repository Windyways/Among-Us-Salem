using System.Text;
using UnityEngine;
using Color = UnityEngine.Color;

namespace ObjectWorkshop.LifeImprovement.Roles;

#region Alarum
#endregion
public sealed class Alarum(IntPtr cppPtr) 
    : CrewmateRole(cppPtr), IOWRole, IWikiDiscoverable
{
    public bool attractsMetal => true;
    public string RoleName => TouLocale.Get(TouNames.Alarum, "Alarum");
    public string revealText => "has a hard time sleeping.";
    public string RoleDescription => "Place clocks to catch Infiltrators!";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => OWColors.Crewmate;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = OWAssets.Alarum,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Investigative role that can place down clocks that would emit sound waves if an Infiltrator gets within its radius." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Activate",
            "You can Activate your alarm clock during the round. You will place down your alarm clock on the ground, visible to only you. If an Infiltrator gets into range, the alarm clock radius will turn red. Alarm clocks reset at the start of each meeting.",
            OWAssets.Alarum_Activate)
    ];

    public override void OnMeetingStart()
    {
        var button = CustomButtonSingleton<Alarum_Activate>.Instance;
        button.IncreaseUses(ClocksPlaced);

        ClocksPlaced = 0;
		AlarmClock.CleanUp();
    }

    public void LobbyStart()
    {
        AlarmClock.CleanUp();
    }

    #region RpcActivate
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.Activate, SendImmediately = true)]
    public static void RpcActivate(PlayerControl player)
    {
        if (player.Data.Role is not Alarum)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcActivate - Invalid Alarum");
            return;
        }

        var alarum = player.GetRole<Alarum>();

        if ((PlayerControl.LocalPlayer.IsFaction(Faction.Infiltrator, true) && OptionGroupSingleton<Alarum_Options>.Instance.InfiltSeeClock) || PlayerControl.LocalPlayer == player)
        {
            AlarmClock.Begin(alarum.Player);
        }
    }

    public int ClocksPlaced;
}

#region Alarum_Activate
#endregion
public sealed class Alarum_Activate : ObjectWorkshopRoleButton<Alarum>
{
    public override string Name => "Activate";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Crewmate;
    public override float Cooldown => OptionGroupSingleton<Alarum_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Alarum_Activate;
    public override int MaxUses => (int)OptionGroupSingleton<Alarum_Options>.Instance.MaxAlarmsAtOnce;

    protected override void OnClick()
    {
        Role.ClocksPlaced++;
        Alarum.RpcActivate(Role.Player);
    }
}

#region Alarum_Options
#endregion
public sealed class Alarum_Options : AbstractOptionGroup<Alarum>
{
    public override string GroupName => TouLocale.Get(TouNames.Alarum, "Alarum");

    [ModdedNumberOption("<color=#b3ffff>Alarum</color> <color=#4a86e8>Activate</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#b3ffff>Alarum</color> <color=#4a86e8>Activate</color> Radius", 0.1f, 60f, 0.1f, MiraNumberSuffixes.Multiplier, "0.0")]
    public float Radius { get; set; } = 3f;

    [ModdedNumberOption("<color=#b3ffff>Alarum</color> Max Alarms At Once", 1f, 15f, 1f)]
    public float MaxAlarmsAtOnce { get; set; } = 3f;

    [ModdedToggleOption("<color=#ff5050>Infiltrators</color> Can See Alarms")]
    public bool InfiltSeeClock { get; set; } = false;
}