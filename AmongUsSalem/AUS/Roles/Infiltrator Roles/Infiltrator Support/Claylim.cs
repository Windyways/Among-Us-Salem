using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.LifeImprovement.Roles;

#region Claylim
#endregion
public sealed class Claylim(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IOWRole, IWikiDiscoverable
{
    public bool attractsMetal => true;
    public string RoleName => TouLocale.Get(TouNames.Claylim, "Claylim");
    public string revealText => "hides their devilish identity well.";
    public string RoleDescription => "You have immunity against Infiltrator catchers.";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => OWColors.Claylim;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.InfiltratorSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        MaxRoleCount = 0,
        CanModifyChance = false,
        UseVanillaKillButton = false,
        // Icon = OWAssets.Infiltrator,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Infiltrator Support role that starts off as a modifier. Using the Rebirth ability will replace youe current role with this one, making you be treated as a Neutral instead of an Infiltrator, and can Submerge players to kill them and dispose their body."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Rebirth",
            "You can Rebirth yourself during the round. You will replace your role with this modifier, being calculated as a Neutral instead of an Infiltrator, although you will have no abilities aside from Attack.",
            OWAssets.Placeholder),

        new("Submerge",
            "You can Submerge a player in goo during the round. You will kill your target, and you will dispose of their body, leaving a pool of goo in its place. Using this ability with no players nearby would instead kill a player in range of a pool of goo and would as well hide the body, although only usable once per game.",
            OWAssets.KillSprite)
    ];

    #region RpcClaylimSubmerge
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.ClaylimSubmerge, SendImmediately = true)]
    public static void RpcClaylimSubmerge(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Claylim)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcClaylimSubmerge - Invalid Claylim");
            return;
        }

        var claylim = player.GetRole<Claylim>();
        GooPool.Begin(claylim.Player, target);
    }
}

#region Claylim_Submerge
#endregion
public sealed class Claylim_Submerge : ObjectWorkshopRoleButton<Claylim, PlayerControl>
{
    public override string Name => "Submerge";
    public override Color TextOutlineColor => OWColors.Claylim;
    public override float Cooldown => OptionGroupSingleton<Claylim_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Placeholder;
    public override int MaxUses => (int)OptionGroupSingleton<Claylim_Options>.Instance.MaxGooKills;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }
    
    public override bool CanUse()
    {
        foreach (var pool in GooPool.AllGooPools)
        {
            if (pool != null)
            {
                if (pool.nearbyTarget != null && UsesLeft > 0)
                {
                    return true;
                }
            }
        }

        return base.CanUse() || UsesLeft == 0;
    }

    protected override void OnClick()
    {
        Reactor.Utilities.Logger<ObjectWorkshopPlugin>.Error("if (Target == null)");
        if (Target == null)
        {
            Reactor.Utilities.Logger<ObjectWorkshopPlugin>.Error("foreach (var pool in GooPool.AllGooPools)");
            foreach (var pool in GooPool.AllGooPools)
            {
                Reactor.Utilities.Logger<ObjectWorkshopPlugin>.Error("if (pool != null)");
                if (pool != null)
                {
                    Reactor.Utilities.Logger<ObjectWorkshopPlugin>.Error("if (pool.nearbyTarget == null)");
                    if (pool.nearbyTarget == null)
                    {
                        Reactor.Utilities.Logger<ObjectWorkshopPlugin>.Error("Claylim Ability: Target is null");
                    }
                    else
                    {
                        pool.returnTimer = 2f;
                        pool.killedNearbyPlayer = true;
                        pool.nearbyTarget.transform.position = pool.gameObject.transform.position;
                        PlayerControl.LocalPlayer.RpcCustomMurder(pool.nearbyTarget, true, true, false, false);
                    }
                }
            }

            return;
        }

        Claylim.RpcClaylimSubmerge(PlayerControl.LocalPlayer, Target);
        PlayerControl.LocalPlayer.RpcCustomMurder(Target, true, true, false);
        IncreaseUses();
    }
}


#region Claylim_Options
#endregion
public sealed class Claylim_Options : AbstractOptionGroup<Claylim>
{
    public override string GroupName => TouLocale.Get(TouNames.Claylim, "Claylim");

    [ModdedNumberOption("<color=#5b5353>Claylim</color> Chance", 0f, 100f, 10f, MiraNumberSuffixes.Percent)]
    public float Chance { get; set; } = 0f;

    [ModdedNumberOption("<color=#5b5353>Claylim</color> Amounr", 0f, 15f, 1f)]
    public float Amount { get; set; } = 1f;

    [ModdedNumberOption("<color=#5b5353>Claylim</color> <color=#4a86e8>Submerge</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
    
    [ModdedNumberOption("<color=#5b5353>Claylim</color> Max Pool Goo Kills", 0f, 15f, 1f)]
    public float MaxGooKills { get; set; } = 1f;
}