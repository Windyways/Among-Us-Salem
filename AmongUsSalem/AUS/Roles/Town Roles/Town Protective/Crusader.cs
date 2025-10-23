using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using Color = UnityEngine.Color;
using UnityEngine;

namespace AmongUsSalem.Roles;

#region Crusader
#endregion
public sealed class Crusader(IntPtr cppPtr)
    : CrewmateRole(cppPtr), IWikiDiscoverable, ICustomAURole
{
    public string RoleName { get; set; } = "Crusader";
    public string revealText => "is a divine protector.";
    public string RoleDescription => "Fortify to kill visitors.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Color RoleColor { get; set; } = AUSColors.Town;
    public Alignment Alignment => Alignment.TownProtective;

    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;


    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.CrusaderRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#06E00C>Crusader</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#06E00C>Town</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#06E00C>Town</color> <color=#1e45d4>Protective</color>" +
            "\n<color=#fdbc00>Goal:</color> Hang every criminal and evildoer." +
            $"\n\nAttributes:" +
            "\nYou know if your target is attacked." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Fortify",
            "You can Fortify a player at Night." +
            "\n\nYou will grant your target Powerful Defense." +
            "\n\nYou will deal a Basic Attack to a player that visits your target, or reports your target's body.",
            AUSAssets.Crusader_Fortify)
    ];

    public override void OnMeetingStart()
    {
        FortifiedPlayer = null;
    }

    public enum Type { AttackedButProtected, TargetAttacked }
    public static string Info(Type type)
    {
        if (type is Type.AttackedButProtected) return "Someone attacked you, but a <b><color=#06e00c>Crusader</color></b> protected you!";
        return "You attacked someone visiting your target.";
    }

    [MethodRpc((uint)AUSRpc.Crusader_Fortify, SendImmediately = true)]
    public static void RpcCrusader_Fortify(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Crusader)
        {
            Logger<AUSPlugin>.Error("RpcCrusader_Fortify - Invalid Crusader");
            return;
        }

        var crusader = player.GetRole<Crusader>();
        crusader.FortifiedPlayer = target;

        if (target.Data.Role is ICustomAURole ausRole) ausRole.ApplyDefense(Defense.Powerful);
    }

    [MethodRpc((uint)AUSRpc.Crusader_Notify, SendImmediately = true)]
    public static void RpcCrusader_Notify(PlayerControl visitor, PlayerControl target, bool attacking)
    {
        foreach (var crusaders in MiscUtils.GetPlayersWithRole<Crusader>())
        {
            var crusader = crusaders.GetRole<Crusader>();
            if (crusader.FortifiedPlayer == target && crusader.Player == visitor)
                return;
                
            if (crusader.FortifiedPlayer == target)
            {
                if (crusader.Player.AmOwner())
                {
                    if (crusader.Player.CanKill(visitor)) MiscUtils.ShowNotification(Info(Type.TargetAttacked), Color.white, AUSAssets.CrusaderRoleCard.LoadAsset());
                    MiscUtils.AddFakeChat(crusader.Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Crusader Info"), Info(Type.TargetAttacked));
                }

                if (target.AmOwner())
                {
                    if (crusader.Player.CanKill(visitor))
                    {
                        crusader.Player.RpcAddModifier<InvisibleStatus>(OptionGroupSingleton<AUSOptions>.Instance.InvisDuration);
                        MiscUtils.RpcApplyDeathReason(crusader.Player, visitor, DeathReasonShow.KilledByACrusader);
                    }
                    if (attacking) MiscUtils.AddFakeChat(target.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Crusader Info"), Info(Type.AttackedButProtected));
                }

                crusader.FortifiedPlayer = null;
            }
        }
    }

    public PlayerControl FortifiedPlayer;
}

#region Crusader_Fortify
#endregion
public sealed class Crusader_Fortify : AmongUsSalemRoleButton<Crusader, PlayerControl>
{
    public override string Name => "Fortify";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Town;
    public override float Cooldown => OptionGroupSingleton<Crusader_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Crusader_Fortify;

    public override void ClickHandler()
    {
        if (Target != null && Timer <= 0)
        {
            if (MiscUtils.SuccessfulVisit(Player, Target, false, true))
            {
                base.ClickHandler();
            }
        }
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        Crusader.RpcCrusader_Fortify(Player, Target);
        MiscUtils.PostSuccessfulVisit(Player, Target, false, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, predicate: x =>
            x != Role.FortifiedPlayer);
    }
}

#region Crusader_Options
#endregion
public sealed class Crusader_Options : AbstractOptionGroup<Crusader>
{
    public override string GroupName => "Crusader";

    [ModdedNumberOption("<color=#06E00C>Crusader</color> <color=#4a86e8>Fortify</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}