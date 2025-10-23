using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Ambusher
#endregion
public sealed class Ambusher(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IWikiDiscoverable, ICustomAURole
{
    public string RoleName { get; set; } = "Ambusher";
    public string revealText => "lies in wait";
    public string RoleDescription => "Ambush to kill visitors!";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public Faction Faction { get; set; } = Faction.Mafia;
    public Color RoleColor { get; set; } = AUSColors.Mafia;
    public Alignment Alignment => Alignment.MafiaKilling;

    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        Icon = AUSAssets.AmbusherRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#DD0000>Ambusher</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#DD0000>Mafia</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#DD0000>Mafia</color> <color=#1e45d4>Killing</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill anyone that will not submit to the Mafia." +
            $"\n\nAttributes:" +
            "\nIf all Mafia Killing roles are dead, you will be promoted to Mafioso." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Ambush",
            "You can Ambush a player at Night." +
            "\n\nYou will deal a Basic Attack to a player that visits your target, or reports your target's body." +
            "\n\nAll players visiting your target are aware of your identity.",
            AUSAssets.Ambusher_Ambush)
    ];

    public override void OnMeetingStart()
    {
        AmbushedPlayer = null;
    }

    public enum Type { PreparedAmbush, AmbushedSomeone }
    public static string Info(Type type, PlayerControl ambusher, PlayerControl target)
    {
        if (type == Type.PreparedAmbush) return $"You saw {ambusher.GetDefaultAppearance().PlayerName} prepare an <b><color=#4a86e8>Ambush</color></b> while visiting {target.GetDefaultAppearance().PlayerName}!";
        return $"You ambushed someone who visited {target.GetDefaultAppearance().PlayerName} last Night!";
    }

    [MethodRpc((uint)AUSRpc.Ambusher_Ambush, SendImmediately = true)]
    public static void RpcAmbusher_Ambush(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Ambusher)
        {
            Logger<AUSPlugin>.Error("Ambusher_Ambush - Invalid Ambusher");
            return;
        }

        var ambusher = player.GetRole<Ambusher>();
        ambusher.AmbushedPlayer = target;
    }

    [MethodRpc((uint)AUSRpc.Ambusher_Notify, SendImmediately = true)]
    public static bool RpcAmbusher_Notify(PlayerControl visitor, PlayerControl target)
    {
        foreach (var ambushers in MiscUtils.GetPlayersWithRole<Ambusher>())
        {
            var ambusher = ambushers.GetRole<Ambusher>();
            if (ambusher.AmbushedPlayer == target)
            {
                if (ambusher.Player.AmOwner())
                {
                    if (ambusher.Player.CanKill(visitor)) MiscUtils.ShowNotification(Info(Type.AmbushedSomeone, ambusher.Player, target), Color.white, AUSAssets.AmbusherRoleCard.LoadAsset());
                    MiscUtils.AddFakeChat(ambusher.Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Mafia, "Ambusher Info"), Info(Type.AmbushedSomeone, ambusher.Player, target));
                }

                if (visitor.AmOwner())
                {
                    if (ambusher.Player.CanKill(visitor))
                    {
                        ambusher.Player.RpcAddModifier<InvisibleStatus>(OptionGroupSingleton<AUSOptions>.Instance.InvisDuration);
                        MiscUtils.RpcApplyDeathReason(ambusher.Player, visitor, DeathReasonShow.KilledByAnAmbusher);
                    }
                    else if (Debugger.IsDebuggerActive) CalculatedVoting.QueueEvidenceAgainst.Add(visitor, ambusher.Player);

                    MiscUtils.ShowNotification(Info(Type.PreparedAmbush, ambusher.Player, target), Color.white, AUSAssets.AmbusherRoleCard.LoadAsset());
                    MiscUtils.AddFakeChat(visitor.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Mafia, "Ambusher Info"), Info(Type.PreparedAmbush, ambusher.Player, target));
                }

                ambusher.AmbushedPlayer = null;
            }
        }

        return false;
    }

    public PlayerControl AmbushedPlayer;
}

#region Ambusher_Ambush
#endregion
public sealed class Ambusher_Ambush : AmongUsSalemRoleButton<Ambusher, PlayerControl>
{
    public override string Name => "Ambush";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Ambusher_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Ambusher_Ambush;

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

        Ambusher.RpcAmbusher_Ambush(Player, Target);
        MiscUtils.PostSuccessfulVisit(Player, Target, false, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(false, Distance, predicate: x =>
            x != Role.AmbushedPlayer);
    }
}

#region Ambusher_Options
#endregion
public sealed class Ambusher_Options : AbstractOptionGroup<Ambusher>
{
    public override string GroupName => "Ambusher";

    [ModdedNumberOption("<color=#DD0000>Ambusher</color> <color=#4a86e8>Ambush</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}