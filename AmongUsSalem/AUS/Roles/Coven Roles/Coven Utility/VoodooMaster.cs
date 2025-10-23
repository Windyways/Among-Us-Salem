using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region VoodooMaster
#endregion
public sealed class VoodooMaster(IntPtr cppPtr)
    : CovenRole(cppPtr), IWikiDiscoverable, ICustomAURole, ICovenRole
{
    public string RoleName { get; set; } = "Voodoo Master";
    public string revealText => "works with dolls.";
    public string RoleDescription => "Prevent players from chatting.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Coven;
    public Color RoleColor { get; set; } = AUSColors.Coven;
    public Alignment Alignment => Alignment.CovenUtility;
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public NecronomiconPriority NecronomiconPriority => NecronomiconPriority.VoodooMaster;
    public bool Necronomicon { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.VoodooMasterRoleCard,
        CanUseSabotage = OptionGroupSingleton<CovenOptions>.Instance.CanSabotage,
        CanUseVent = OptionGroupSingleton<CovenOptions>.Instance.CanVent,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#B545FF>Voodoo Master</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#B545FF>Coven</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#B545FF>Coven</color> <color=#1e45d4>Utility</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill all who would oppose the Coven." +
            $"\n\nAttributes:" +
            "\nWith the Necronomicon, your victim is dealt a Basic Attack." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Voodoo",
            "Create a Voodoo doll of your target at night and use it to silence them." +
            "\n\nYour target cannot talk the following day." +
            "\n\nYour target cannot vote during the Voting phase the following day." +
            "\n\nYou cannot choose the same person two nights in a row.",
            AUSAssets.VoodooMaster_Voodoo)
    ];

    public bool WinConditionMet()
    {
        var aliveCoven = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.Is(Faction.Coven));
        if (aliveCoven == 0) return false;

        var result = MiscUtils.GetAlivePlayersToEnd().Count <= aliveCoven && MiscUtils.KillersAliveCount() == aliveCoven;
        return result || AmongUsSalem.Patches.LogicGameFlowPatches.EndGameEarlyCheck(this);
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    [MethodRpc((uint)AUSRpc.VoodooMaster_Voodoo, SendImmediately = true)]
    public static void RpcVoodooMaster_Voodoo(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not VoodooMaster)
        {
            Logger<AUSPlugin>.Error("RpcVoodooMaster_Voodoo - Invalid Voodoo Master");
            return;
        }

        var voodoomaster = player.GetRole<VoodooMaster>();
        voodoomaster.SilencedPlayer = target;
    }

    public enum Type { Silenced, WellRested }
    public static string Info(Type type)
    {
        if (type == Type.Silenced) return "Your voice seems to be lost! You can't speak.";
        return "A <b><color=#B545FF>Voodoo Master</color></b> tried to Silence you, but your voice is well rested!";
    }

    public PlayerControl SilencedPlayer;
    public PlayerControl PreviouslySilencedPlayer;
}

#region VoodooMaster_Voodoo
#endregion
public sealed class VoodooMaster_Voodoo : AmongUsSalemRoleButton<VoodooMaster, PlayerControl>
{
    public override string Name => "Voodoo";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Coven;
    public override float Cooldown => Role.Necronomicon ? OptionGroupSingleton<CovenOptions>.Instance.Cooldown : OptionGroupSingleton<VoodooMaster_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.VoodooMaster_Voodoo;

    public override void ClickHandler()
    {
        if (Target != null && Timer <= 0)
        {
            if (MiscUtils.SuccessfulVisit(Player, Target, Role.Necronomicon, true))
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

        if (Role.Necronomicon)
        {
            if (Player.CanKill(Target)) MiscUtils.RpcApplyDeathReason(Player, Target, DeathReasonShow.KilledByTheCoven);
            else
            {
                MiscUtils.ShowNotification(MessageTexts.TooMuchDefense(Player, Target), Color.white);
                MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Neutral, "General Info"), MessageTexts.TooMuchDefense(Player, Target));
            }
        }

        VoodooMaster.RpcVoodooMaster_Voodoo(Player, Target);
        MiscUtils.PostSuccessfulVisit(Player, Target, Role.Necronomicon, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, predicate: x =>
            x != Role.SilencedPlayer &&
            (x.Data.Role is ICustomAURole customRole && customRole.Faction != Role.Faction));
    }
}

#region VoodooMaster_Events
#endregion
public static class VoodooMaster_Events
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return; // Only run when round starts.
        }

        foreach (var voodooMasters in MiscUtils.GetPlayersWithRole<VoodooMaster>())
        {
            var voodooMaster = voodooMasters.GetRole<VoodooMaster>();
            if (voodooMaster.SilencedPlayer != null)
            {
                voodooMaster.PreviouslySilencedPlayer = voodooMaster.SilencedPlayer;
                voodooMaster.SilencedPlayer = null;
            }
            else voodooMaster.PreviouslySilencedPlayer = null;
        }
    }

    [RegisterEvent]
    public static void StartMeetingEventHandler(StartMeetingEvent @event) // Show target that their silenced!
    {
        var player = PlayerControl.LocalPlayer;
        if (player.AmOwner() && player.IsSilenced())
        {
            foreach (var voodooMasters in MiscUtils.GetPlayersWithRole<VoodooMaster>())
            {
                var voodooMaster = voodooMasters.GetRole<VoodooMaster>();
                if (voodooMaster.PreviouslySilencedPlayer == player)
                {
                    voodooMaster.SilencedPlayer = null;
                    MiscUtils.AddFakeChat(player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Coven, "Voodoo Master Info"), VoodooMaster.Info(VoodooMaster.Type.WellRested));
                }
                else
                {
                    MiscUtils.AddFakeChat(player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Coven, "Voodoo Master Info"), VoodooMaster.Info(VoodooMaster.Type.Silenced));
                }
            }
        }
    }

    [RegisterEvent]
    public static void HandleVoteEvent(HandleVoteEvent @event)
    {
        if (!@event.VoteData.Owner.IsSilenced())
        {
            return;
        }

        @event.VoteData.SetRemainingVotes(0);
        @event.Cancel();
    }
}

#region VoodooMaster_Options
#endregion
public sealed class VoodooMaster_Options : AbstractOptionGroup<VoodooMaster>
{
    public override string GroupName => "Voodoo Master";

    [ModdedNumberOption("<color=#B545FF>Voodoo Master</color> <color=#4a86e8>Voodoo</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}