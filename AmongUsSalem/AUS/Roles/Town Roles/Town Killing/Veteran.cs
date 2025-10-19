using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using Color = UnityEngine.Color;
using UnityEngine;

namespace AmongUsSalem.Roles;

#region Veteran
#endregion
public sealed class Veteran(IntPtr cppPtr)
    : CrewmateRole(cppPtr), IWikiDiscoverable, ICustomAURole, IContinueGame
{
    public bool continueGame => Charges > 0 || isAlerted;
    public string RoleName { get; set; } = "Veteran";
    public string revealText => "is a paranoid war hero.";
    public string RoleDescription => "Alert to kill visitors.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Color RoleColor { get; set; } = AUSColors.Town;
    public Alignment Alignment => Alignment.TownKilling;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.VeteranRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#06E00C>Veteran</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#06E00C>Town</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#06E00C>Town</color> <color=#1e45d4>Killing</color>" +
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
            "If you go on Alert at night you will deal a Powerful Attack to your visitors." +
            "\nYou gain Basic Defense while on Alert.",
            AUSAssets.Veteran_Alert)
    ];


    public override void OnMeetingStart()
    {
        isAlerted = false;
    }

    public static string ShotInfo()
    {
        return "You have shot someone who visited you last <b><color=#922058>Night</color></b>!";
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
        veteran.Charges--;

        veteran.Attack = Attack.Powerful;
        veteran.Defense = Defense.Basic;
    }
    
    [MethodRpc((uint)AUSRpc.Veteran_Notify, SendImmediately = true)]
    public static void RpcVeteran_Notify(PlayerControl visitor, PlayerControl target, bool attacking)
    {
        if (target.AmOwner())
        {
            if (target.CanKill(visitor))
            {
                MiscUtils.ShowNotification(ShotInfo(), Color.white, AUSAssets.VeteranRoleCard.LoadAsset());
                MiscUtils.RpcApplyDeathReason(target, visitor, DeathReasonShow.ShotByAVeteran);
            }

            MiscUtils.AddFakeChat(target.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Veteran Info"), ShotInfo());

            if (attacking)
            {
                MiscUtils.ShowNotification(AttackedInfo(), Color.white, AUSAssets.VeteranRoleCard.LoadAsset());
                MiscUtils.AddFakeChat(target.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Veteran Info"), AttackedInfo());
            }
        }
    }

    public bool isAlerted;
    public int Charges = (int)OptionGroupSingleton<Veteran_Options>.Instance.Charges;
}

#region Veteran_Alert
#endregion
public sealed class Veteran_Alert : AmongUsSalemRoleButton<Veteran>
{
    public override string Name => "Alert";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Town;
    public override float Cooldown => OptionGroupSingleton<Veteran_Options>.Instance.Cooldown;
    public override int MaxUses => (int)OptionGroupSingleton<Veteran_Options>.Instance.Charges;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Veteran_Alert;

    public override void ClickHandler()
    {
        if (MiscUtils.SuccessfulVisit(Player, Player, false, false))
        {
            base.ClickHandler();
        }
    }

    public override bool CanUse()
    {
        return base.CanUse() && !Role.isAlerted;
    }

    protected override void OnClick()
    {
        Veteran.RpcVeteran_Alert(Player);
        MiscUtils.PostSuccessfulVisit(Player, Player, false, false);
    }
}

#region Veteran_Options
#endregion
public sealed class Veteran_Options : AbstractOptionGroup<Veteran>
{
    public override string GroupName => "Veteran";

    [ModdedNumberOption("<color=#06E00C>Veteran</color> <color=#4a86e8>Alert</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#06E00C>Veteran</color> Max <color=#4a86e8>Alerts</color>", 1f, 30f, 1f)]
    public float Charges { get; set; } = 3;
}