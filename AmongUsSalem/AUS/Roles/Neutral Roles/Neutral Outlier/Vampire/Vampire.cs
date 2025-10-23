using System.Text;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using System.Collections;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Vampire
#endregion
public sealed class Vampire(IntPtr cppPtr)
    : NeutralRole(cppPtr), IWikiDiscoverable, ICustomAURole
{
    public string RoleName { get; set; } = "Vampire";
    public string revealText => "drinks blood.";
    public string RoleDescription => "Convert townies to your team.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Neutral;
    public Color RoleColor { get; set; } = AUSColors.Vampire;
    public Alignment Alignment => Alignment.NeutralOutlier;
    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.Basic;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.Basic;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;


    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.VampireRoleCard,
        CanUseVent = OptionGroupSingleton<Vampire_Options>.Instance.CanVent,
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
            "<color=#a22929>Vampire</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#A9A9A9>Neutral</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#A9A9A9>Neutral</color> <color=#1e45d4>Outlier</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill everyone in the town." +
            $"\n\nAttributes:" +
            "\nYou have Basic Defense as long as you have an alive Vampire." +
            "\nUpon death, your oldest Recruit will be promoted to Vampire." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Drain",
            "You can Drain a player at Night. You will deal a Basic Attack to your target.",
            AUSAssets.Vampire_Drain),

        new("Convert",
            "You can Convert a player at Night. If your target is a Town member with no Defense, you will make them Vampire-Aligned. Else, you will deal a Basic Attack to your target.",
            AUSAssets.Vampire_Convert)
    ];

    public bool WinConditionMet()
    {
        var aliveVampires = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Vampire>());
        var aliveRecs = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.HasModifier<VampireRecruit>());
        if (aliveVampires == 0 && aliveRecs == 0) return false;

        var result = MiscUtils.GetAlivePlayersToEnd().Count <= (aliveVampires + aliveRecs) && MiscUtils.KillersAliveCount() == (aliveVampires + aliveRecs);
        return result || AmongUsSalem.Patches.LogicGameFlowPatches.EndGameEarlyCheck(this);
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    [MethodRpc((uint)AUSRpc.Vampire_Convert, SendImmediately = true)]
    public static void RpcVampire_Convert(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Vampire)
        {
            Logger<AUSPlugin>.Error("RpcVampire_Convert - Invalid Vampire");
            return;
        }

        var vampire = player.GetRole<Vampire>();
        vampire.Charges--;
        
        if (vampire.Player.Data.Role is ICustomAURole ausRole)
        {
            ausRole.ApplyDefense(Defense.None, true, true);
        }

        var vampires = PlayerControl.AllPlayerControls.ToArray().Count(x => x.HasModifier<VampireRecruit>() && !x.HasDied());
        target.RpcAddModifier<VampireRecruit>(vampires);
    }

    public void OnDeath(DeathReason? reason)
    {
        foreach (var vrec in ModifierUtils.GetActiveModifiers<VampireRecruit>())
        {
            if (vrec.id == 0 && !vrec.Player.HasDied()) vrec.Promote();
        }
        ModifierUtils.GetActiveModifiers<VampireRecruit>().Do(x => x.LowerIds());
    }

    public void OnTargetDeath(PlayerControl target, DeathReason? reason)
    {
        var vampires = PlayerControl.AllPlayerControls.ToArray().Count(x => x.HasModifier<VampireRecruit>() && !x.HasDied());
        if (vampires == 0 && Player.Data.Role is ICustomAURole ausRole)
        {
            ausRole.ApplyDefense(Defense.Basic, true);
        }
    }
    
    public int Charges = (int)OptionGroupSingleton<Vampire_Options>.Instance.Charges;
}

#region Vampire_Drain
#endregion
public sealed class Vampire_Drain : AmongUsSalemRoleButton<Vampire, PlayerControl>
{
    public override string Name => "Drain";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Vampire;
    public override float Cooldown => OptionGroupSingleton<Vampire_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Vampire_Drain;

    public override void ClickHandler()
    {
        if (Target != null && Timer <= 0)
        {
            if (MiscUtils.SuccessfulVisit(Player, Target, true, true))
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

        if (Player.AmOwner)
        {
            var button = CustomButtonSingleton<Vampire_Convert>.Instance;
            WitnessKillEvent.ResetButtonTimer(Player, button, button.Cooldown);
        }

        if (Player.CanKill(Target)) MiscUtils.RpcApplyDeathReason(Player, Target, DeathReasonShow.BittenByAVampire);
        else
        {
            MiscUtils.ShowNotification(MessageTexts.TooMuchDefense(Player, Target), Color.white);
            MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Neutral, "General Info"), MessageTexts.TooMuchDefense(Player, Target));
        }
        MiscUtils.PostSuccessfulVisit(Player, Target, true, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !x.HasModifier<VampireRecruit>());
    }
}

#region Vampire_Convert
#endregion
public sealed class Vampire_Convert : AmongUsSalemRoleButton<Vampire, PlayerControl>
{
    public override string Name => "Convert";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Vampire;
    public override float Cooldown => OptionGroupSingleton<Vampire_Options>.Instance.ConvertCD;
    public override int MaxUses => (int)OptionGroupSingleton<Vampire_Options>.Instance.Charges;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Vampire_Convert;

    public override void ClickHandler()
    {
        if (Target != null && Timer <= 0)
        {
            if (MiscUtils.SuccessfulVisit(Player, Target, !IsConvertable(Target), true))
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

        if (Player.AmOwner)
        {
            var button = CustomButtonSingleton<Vampire_Drain>.Instance;
            button.ResetCooldownAndOrEffect();
        }

        if (IsConvertable(Target)) Vampire.RpcVampire_Convert(Role.Player, Target);
        else
        {
            if (Player.CanKill(Target)) MiscUtils.RpcApplyDeathReason(Player, Target, DeathReasonShow.BittenByAVampire);
            else
            {
                MiscUtils.ShowNotification(MessageTexts.TooMuchDefense(Player, Target), Color.white);
                MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Neutral, "General Info"), MessageTexts.TooMuchDefense(Player, Target));
            }
            IncreaseUses();
        }
        
        MiscUtils.PostSuccessfulVisit(Player, Target, false, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !x.HasModifier<VampireRecruit>());
    }

    public static bool IsConvertable(PlayerControl target)
    {
        return target.Is(Faction.Town) && target.Data.Role is ICustomAURole ausRole && ausRole.Defense == Defense.None;
    }
}

#region Vampire_Options
#endregion
public sealed class Vampire_Options : AbstractOptionGroup<Vampire>
{
    public override string GroupName => "Vampire";

    [ModdedNumberOption("<color=#a22929>Vampire</color> <color=#4a86e8>Drain</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#a22929>Vampire</color> <color=#4a86e8>Convert</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float ConvertCD { get; set; } = 25f;

    [ModdedNumberOption("<color=#a22929>Vampire</color> Max <color=#4a86e8>Converts</color>", 1f, 15f, 1f)]
    public float Charges { get; set; } = 3f;

    [ModdedNumberOption("<color=#a22929>Vampire</color> Vision", 0.25f, 5f, 0.25f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float Vision { get; set; } = 1f;

    [ModdedToggleOption("<color=#a22929>Vampire</color> Can Vent")]
    public bool CanVent { get; set; } = false;
}