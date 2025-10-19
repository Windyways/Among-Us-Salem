using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Arsonist
#endregion
public sealed class Arsonist(IntPtr cppPtr)
    : NeutralRole(cppPtr), IWikiDiscoverable, ICustomAURole
{
    public string RoleName { get; set; } = "Arsonist";
    public string revealText => "likes to watch things burn.";
    public string RoleDescription => "Douse and Ignite everyone.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Neutral;
    public Color RoleColor { get; set; } = AUSColors.Arsonist;
    public Alignment Alignment => Alignment.NeutralKilling;
    public Attack Attack { get; set; } = Attack.Unstoppable;
    public Defense Defense { get; set; } = Defense.Basic;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Unstoppable;
    public Defense ogDefense { get; set; } = Defense.Basic;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.ArsonistRoleCard,
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
            "<color=#ee7600>Arsonist</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#A9A9A9>Neutral</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#A9A9A9>Neutral</color> <color=#1e45d4>Killing</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill everyone in the town." +
            $"\n\nAttributes:" +
            "\nYou will passively Douse players that visit you." +
            "\nYou will clean Gasoline off yourself if you do not Douse or Ignite during the Night." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Douse",
            "You can Douse a player at Night." +
            "\n\nYou will Douse your target.",
            AUSAssets.Arsonist_Douse),
            
        new("Ignite",
            "You can Ignite at Night." +
            "\n\nAll Doused players will die, even if they were Doused from a different Arsonist.",
            AUSAssets.Arsonist_Ignite)
    ];

    public bool WinConditionMet()
    {
        var aliveArsonists = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Arsonist>());
        if (aliveArsonists == 0) return false;

        var result = MiscUtils.GetAlivePlayersToEnd().Count <= aliveArsonists && MiscUtils.KillersAliveCount() == aliveArsonists;
        return result || AmongUsSalem.Patches.LogicGameFlowPatches.EndGameEarlyCheck(this);
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    public override void OnMeetingStart()
    {
        if (Player.AmOwner() && Player.IsDoused())
        {
            MiscUtils.ShowNotification(Info(Type.WasDoused), Color.white, AUSAssets.ArsonistRoleCard.LoadAsset());
            MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Arsonist, "Arsonist Info"), Info(Type.WasDoused));
        }

        if (!hasDoused)
        {
            foreach (var arsonists in MiscUtils.GetPlayersWithRole<Arsonist>())
            {
                var arsonist = arsonists.GetRole<Arsonist>();
                arsonist.DousedPlayers.Remove(Player.PlayerId);
            }

            if (Player.AmOwner())
            {
                MiscUtils.ShowNotification(Info(Type.Cleanse), Color.white, AUSAssets.ArsonistRoleCard.LoadAsset());
                MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Arsonist, "Arsonist Info"), Info(Type.Cleanse));
            }
        }
        hasDoused = false;
    }

    public enum Type { PassiveDouse, Cleanse, WasDoused }
    public static string Info(Type type)
    {
        if (type == Type.PassiveDouse) return "You have <b><color=#4a86e8>Doused</color></b> a player who visited you.";
        if (type == Type.Cleanse) return "You have cleaned the gasoline off of yourself.";
        return "You were <b><color=#4a86e8>Doused</color></b> in gas!";
    }

    [MethodRpc((uint)AUSRpc.Arsonist_Douse, SendImmediately = true)]
    public static void RpcArsonist_Douse(PlayerControl player, PlayerControl target, bool passiveDouse = false)
    {
        if (player.Data.Role is not Arsonist)
        {
            Logger<AUSPlugin>.Error("RpcArsonist_Douse - Invalid Arsonist");
            return;
        }

        var arsonist = player.GetRole<Arsonist>();
        arsonist.DousedPlayers.Add(target.PlayerId);
        if (passiveDouse && player.AmOwner())
        {
            MiscUtils.ShowNotification(Info(Type.PassiveDouse), Color.white, AUSAssets.ArsonistRoleCard.LoadAsset());
            MiscUtils.AddFakeChat(player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Arsonist, "Arsonist Info"), Info(Type.PassiveDouse));
        }
    }

    public List<byte> DousedPlayers = new List<byte>();
    public bool hasDoused = true; // So it doesn't show the notification Day 1, gets set to false afterward anyway.
}

#region Arsonist_Douse
#endregion
public sealed class Arsonist_Douse : AmongUsSalemRoleButton<Arsonist, PlayerControl>
{
    public override string Name => "Douse";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Arsonist;
    public override float Cooldown => OptionGroupSingleton<Arsonist_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Arsonist_Douse;

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

        if (Player.AmOwner)
        {
            var button = CustomButtonSingleton<Arsonist_Ignite>.Instance;
            button.ResetCooldownAndOrEffect();
        }

        Role.hasDoused = true;
        Arsonist.RpcArsonist_Douse(Role.Player, Target);
        MiscUtils.PostSuccessfulVisit(Player, Target, false, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (target == null) return base.IsTargetValid(target);
        return base.IsTargetValid(target) &&
            !Role.DousedPlayers.Contains(target.PlayerId);
    }
}

#region Arsonist_Ignite
#endregion
public sealed class Arsonist_Ignite : AmongUsSalemRoleButton<Arsonist>
{
    public override string Name => "Ignite";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Arsonist;
    public override float Cooldown => OptionGroupSingleton<Arsonist_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Arsonist_Ignite;

    public override void ClickHandler()
    {
        if (MiscUtils.SuccessfulVisit(Player, Player, false, false))
        {
            base.ClickHandler();
        }
    }

    protected override void OnClick()
    {
        if (Player.AmOwner)
        {
            var button = CustomButtonSingleton<Arsonist_Douse>.Instance;
            button.ResetCooldownAndOrEffect();
        }

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (Player.CanKill(player) && player.IsDoused()) MiscUtils.RpcApplyDeathReason(Player, player, DeathReasonShow.IncineratedByAnArsonist, true, false);
        }

        Role.hasDoused = true;
        Role.DousedPlayers.Clear();
        MiscUtils.PostSuccessfulVisit(Player, Player, false, false);
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role.DousedPlayers.Count > 0;
    }
}

#region Arsonist_Options
#endregion
public sealed class Arsonist_Options : AbstractOptionGroup<Arsonist>
{
    public override string GroupName => "Arsonist";

    [ModdedNumberOption("<color=#ee7600>Arsonist</color> <color=#4a86e8>Douse</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}