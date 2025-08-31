using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using Color = UnityEngine.Color;
using UnityEngine;

namespace AmongUsSalem.Roles;

#region Veteran
#endregion
public sealed class Veteran(IntPtr cppPtr)
    : CrewmateRole(cppPtr), IAUSRole, IWikiDiscoverable
{
    public string RoleName => TouLocale.Get(TouNames.Veteran, "Veteran");
    public string revealText => "is a paranoid war hero.";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction RoleFaction => Faction.Town;
    public Color RoleColor => AUSColors.Town;
    public Alignment Alignment => Alignment.TownKilling;
    public Attack Attack { get; set; } = Attack.Powerful;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.VeteranRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#06e00c>Veteran</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#06e00c>Town</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#06e00c>Town</color> <color=#1e45d4>Killing</color>" +
            "\n<color=#fdbc00>Goal:</color> Hang every criminal and evildoer." +
            $"\n\nAttributes:" +
            "\nYou will know if you shoot any visitors to anyone who visits you." +
            "\nIf a Town Traitor hunt begins, you will lose all remaining ability charges." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Alert",
            "If you go on Alert at night you will deal a Powerful Attack to." +
            "\nYou gain Basic Defense while on Alert.",
            AUSAssets.Veteran_Alert)
    ];

    public override void OnMeetingStart()
    {
        isAlerted = false;
    }

    public static string ShotInfo()
    {
        return "You have shot someone who visited you last <color=#922058>Night</color>!";
    }

    public static string AttackedInfo()
    {
        return "You were attacked, but your <b><color=#0000ff>Defense</color></b> on <b><color=#4a86e8>Alert</color></b> was too high!";
    }

    [MethodRpc((uint)AUSRpc.Veteran_Alert, SendImmediately = true)]
    public static void RpcVeteran_Alert(PlayerControl player)
    {
        if (player.Data.Role is not Veteran)
        {
            Logger<AUSPlugin>.Error("RpcVeteran_Alert - Invalid Veteran");
            return;
        }

        var veteran = player.GetRole<Veteran>();
        veteran.isAlerted = true;
        veteran.Defense = Defense.Basic;
    }

    [MethodRpc((uint)AUSRpc.Veteran_Notify, SendImmediately = true)]
    public static bool RpcVeteran_Notify(PlayerControl visitor, PlayerControl target, bool attacking)
    {
        if (target.AmOwner())
        {
            if (target.CanKill(visitor)) target.RpcCustomMurder(visitor);

            MiscUtils.ShowNotification(ShotInfo(), Color.white, AUSAssets.VeteranRoleCard.LoadAsset());
            MiscUtils.AddFakeChat(target.CachedPlayerData, "Veteran Info", ShotInfo());

            if (attacking)
            {
                MiscUtils.ShowNotification(AttackedInfo(), Color.white, AUSAssets.VeteranRoleCard.LoadAsset());
                MiscUtils.AddFakeChat(target.CachedPlayerData, "Veteran Info", AttackedInfo());
            }
        }

        return false;
    }

    public bool isAlerted;

    public enum Version
    {
        TownOfSalem,
        TownOfSalem2
    }
}

#region Veteran_Alert
#endregion
public sealed class Veteran_Alert : AmongUsSalemRoleButton<Veteran>
{
    public override string Name => "Alert";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Town;
    public override float Cooldown => OptionGroupSingleton<Veteran_Options>.Instance.Cooldown;
    public override int MaxUses => (int)OptionGroupSingleton<Veteran_Options>.Instance.Charges;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Veteran_Alert;

    public override void ClickHandler()
    {
        if (!Role.isAlerted)
        {
            if (MiscUtils.SuccessfulVisit(Role.Player, Role.Player, false, false))
            {
                base.ClickHandler();
            }
        }
    }

    protected override void OnClick()
    {
        Veteran.RpcVeteran_Alert(Role.Player);
    }
}

#region Veteran_Options
#endregion
public sealed class Veteran_Options : AbstractOptionGroup<Veteran>
{
    public override string GroupName => TouLocale.Get(TouNames.Veteran, "Veteran");

    [ModdedNumberOption("<color=#06e00c>Veteran</color> <color=#4a86e8>Alert</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#06e00c>Veteran</color> Max <color=#4a86e8>Alerts</color>", 1f, 30f, 1f)]
    public float Charges { get; set; } = 3;
}