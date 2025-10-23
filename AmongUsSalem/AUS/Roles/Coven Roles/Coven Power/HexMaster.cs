using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;
using System.Collections;

namespace AmongUsSalem.LifeImprovement.Roles;

#region HexMaster
#endregion
public sealed class HexMaster(IntPtr cppPtr)
    : CovenRole(cppPtr), IWikiDiscoverable, ICustomAURole, ICovenRole
{
    public string RoleName { get; set; } = "Hex Master";
    public string revealText => "is versed in the ways of hexes.";
    public string RoleDescription => "HexBomb and kill all.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Coven;
    public Color RoleColor { get; set; } = AUSColors.Coven;
    public Alignment Alignment => Alignment.CovenPower;
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public NecronomiconPriority NecronomiconPriority => NecronomiconPriority.HexMaster;
    public bool Necronomicon { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.HexMasterRoleCard,
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
            "<color=#B545FF>Hex Master</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#B545FF>Coven</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#B545FF>Coven</color> <color=#1e45d4>Power</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill all who would oppose the Coven." +
            $"\n\nAttributes:" +
            "\nPlayers remain Hexed when you gain the Necronomicon." +
            "\nWith the Necronomicon, you gain Astral visits and Basic Attacks." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Hex",
            "You can Hex a player at Night. If you are alive while all Non-Coven members are Hexed, you will perform a Hex-Bomb at Day, killing all Hexed players." +
            "\n\nIf all Non-Coven players are Hexed, The Necromancer can reanimate the Hex Master to cause all Hexes to go off, killing all hexed players.",
            AUSAssets.HexMaster_Hex)
    ];

    public bool WinConditionMet()
    {
        var aliveCoven = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.Is(Faction.Coven));
        if (aliveCoven == 0) return false;

        var result = Helpers.GetAlivePlayers().Count <= aliveCoven && MiscUtils.KillersAliveCount() == aliveCoven;
        return result || AmongUsSalem.Patches.LogicGameFlowPatches.EndGameEarlyCheck(this);
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    public override void OnMeetingStart()
    {
        Coroutines.Start(PostMeetingIntro());
        if (Necronomicon) CovenNecronomiconMechanic.AdjustButtons(Player);
    }

    public IEnumerator PostMeetingIntro()
    {
        yield return new WaitForSeconds(DayNightMechanic.PostMeetingIntroTime);

        var aliveHexed = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && HexedPlayers.Contains(x.PlayerId));
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && !x.Is(Faction.Coven));
        if (aliveHexed >= alivePlayers)
        {
            List<byte> HexBombedPlayers = HexedPlayers.ToList();
            foreach (var hexed in HexBombedPlayers)
            {
                var player = MiscUtils.PlayerById(hexed);
                if (!player.HasDied())
                {
                    MiscUtils.RpcApplyDeathReason(Player, player, DeathReasonShow.BombedByAHexMaster, false, false);
                }
            }
        }
    }

    [MethodRpc((uint)AUSRpc.HexMaster_Hex, SendImmediately = true)]
    public static void RpcHexMaster_Hex(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not HexMaster)
        {
            Logger<AUSPlugin>.Error("RpcHexMaster_Hex - Invalid HexMaster");
            return;
        }

        var hexMaster = player.GetRole<HexMaster>();
        hexMaster.HexedPlayers.Add(target.PlayerId);
    }

    public List<byte> HexedPlayers = new List<byte>();
}

#region HexMaster_Hex
#endregion
public sealed class HexMaster_Hex : AmongUsSalemRoleButton<HexMaster, PlayerControl>
{
    public override string Name => "Hex";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Coven;
    public override float Cooldown => Role.Necronomicon ? OptionGroupSingleton<CovenOptions>.Instance.Cooldown : OptionGroupSingleton<HexMaster_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.HexMaster_Hex;

    public override void ClickHandler()
    {
        if (Target != null && Timer <= 0)
        {
            if (MiscUtils.SuccessfulVisit(Player, Target, Role.Necronomicon, false)) // Astral visiting!
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

        HexMaster.RpcHexMaster_Hex(Player, Target);
        MiscUtils.PostSuccessfulVisit(Player, Target, Role.Necronomicon, false);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !Role.HexedPlayers.Contains(x.PlayerId) &&
            !(x.Data.Role is ICustomAURole customRole && (customRole.Faction != Role.Faction && !Role.Necronomicon)));
    }
}

#region HexMaster_Options
#endregion
public sealed class HexMaster_Options : AbstractOptionGroup<HexMaster>
{
    public override string GroupName => "Hex Master";

    [ModdedNumberOption("<color=#B545FF>Hex Master</color> <color=#4a86e8>Hex</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}