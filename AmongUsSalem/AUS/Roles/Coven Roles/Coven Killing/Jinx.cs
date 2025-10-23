using System.Text;
using AmongUsSalem.Utilities;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Jinx
#endregion
public sealed class Jinx(IntPtr cppPtr)
    : CovenRole(cppPtr), IWikiDiscoverable, ICustomAURole, ICovenRole
{
    public string RoleName { get; set; } = "Jinx";
    public string revealText => "lies in wait";
    public string RoleDescription => "Jinx players to kill a visitor!";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Coven;
    public Color RoleColor { get; set; } = AUSColors.Coven;
    public Alignment Alignment => Alignment.CovenKilling;

    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public NecronomiconPriority NecronomiconPriority => NecronomiconPriority.Jinx;
    public bool Necronomicon { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.JinxRoleCard,
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
            "<color=#B545FF>Jinx</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#B545FF>Coven</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#B545FF>Coven</color> <color=#1e45d4>Killing</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill all who would oppose the Coven." +
            $"\n\nAttributes:" +
            "\nWith the Necronomicon, you will also deal a Basic Attack to your target." +
            "\nYou will obtain the Necronomicon 15th, after the <color=#B545FF>Coven Leader</color>." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Jinx",
            "You can Jinx a player at Night." +
            "\n\nYou will deal a Basic Attack to a player that visits your target, or reports your target's body." +
            "\n\nAll players visiting your target are aware of your identity.",
            AUSAssets.Jinx_Jinx)
    ];

    public override void OnMeetingStart()
    {
        JinxedPlayer = null;
        if (Necronomicon) CovenNecronomiconMechanic.AdjustButtons(Player);
    }

    public enum Type { PreparedJinx, JinxedSomeone }
    public static string Info(Type type, PlayerControl jinx, PlayerControl target)
    {
        if (type == Type.PreparedJinx) return $"You saw the <b><color=#ab42ef>Jinx</color></b> {jinx.GetDefaultAppearance().PlayerName} visit {target.GetDefaultAppearance().PlayerName}!";
        return $"You jinxed someone that visited {target.GetDefaultAppearance().PlayerName}!";
    }

    [MethodRpc((uint)AUSRpc.Jinx_Jinx, SendImmediately = true)]
    public static void RpcJinx_Jinx(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Jinx)
        {
            Logger<AUSPlugin>.Error("Jinx_Jinx - Invalid Jinx");
            return;
        }

        var jinx = player.GetRole<Jinx>();
        jinx.JinxedPlayer = target;
    }

    [MethodRpc((uint)AUSRpc.Jinx_Notify, SendImmediately = true)]
    public static bool RpcJinx_Notify(PlayerControl visitor, PlayerControl target)
    {
        foreach (var jinxs in MiscUtils.GetPlayersWithRole<Jinx>())
        {
            var jinx = jinxs.GetRole<Jinx>();
            if (jinx.JinxedPlayer == target)
            {
                if (jinx.Player.AmOwner())
                {
                    if (jinx.Player.CanKill(visitor)) MiscUtils.ShowNotification(Info(Type.JinxedSomeone, jinx.Player, target), Color.white, AUSAssets.JinxRoleCard.LoadAsset());
                    MiscUtils.AddFakeChat(jinx.Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Coven, "Jinx Info"), Info(Type.JinxedSomeone, jinx.Player, target));
                }

                if (visitor.AmOwner())
                {
                    if (jinx.Player.CanKill(visitor))
                    {
                        jinx.Player.RpcAddModifier<InvisibleStatus>(OptionGroupSingleton<AUSOptions>.Instance.InvisDuration);
                        MiscUtils.RpcApplyDeathReason(jinx.Player, visitor, DeathReasonShow.KilledByAJinx);
                    }
                    else if (Debugger.IsDebuggerActive)
                    {
                        if (!CalculatedVoting.QueueEvidenceAgainst.ContainsValue(jinx.Player) && !jinx.Player.IsImpureToTown() && !jinx.Player.Is(Faction.Town))
                        {
                            CalculatedVoting.QueueEvidenceAgainst.Add(visitor, jinx.Player);
                        }
                    }

                    MiscUtils.ShowNotification(Info(Type.PreparedJinx, jinx.Player, target), Color.white, AUSAssets.JinxRoleCard.LoadAsset());
                    MiscUtils.AddFakeChat(visitor.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Coven, "Jinx Info"), Info(Type.PreparedJinx, jinx.Player, target));
                }

                jinx.JinxedPlayer = null;
            }
        }

        return false;
    }

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

    public PlayerControl JinxedPlayer;
}

#region Jinx_Jinx
#endregion
public sealed class Jinx_Jinx : AmongUsSalemRoleButton<Jinx, PlayerControl>
{
    public override string Name => "Jinx";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Coven;
    public override float Cooldown => Role.Necronomicon ? OptionGroupSingleton<CovenOptions>.Instance.Cooldown : OptionGroupSingleton<Jinx_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Jinx_Jinx;

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

        Jinx.RpcJinx_Jinx(Player, Target);
        MiscUtils.PostSuccessfulVisit(Player, Target, Role.Necronomicon, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, predicate: x => 
            x != Role.JinxedPlayer && 
            (x.Data.Role is ICustomAURole customRole && customRole.Faction != Role.Faction));
    }
}

#region Jinx_Options
#endregion
public sealed class Jinx_Options : AbstractOptionGroup<Jinx>
{
    public override string GroupName => "Jinx";

    [ModdedNumberOption("<color=#B545FF>Jinx</color> <color=#4a86e8>Jinx</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}