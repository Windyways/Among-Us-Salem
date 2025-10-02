/*using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Consort
#endregion
public sealed class Consort(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IWikiDiscoverable, IAUSRole
{
    public string RoleName { get; set; } = Consort, "Consort");
    public string revealText => "has a desire or deceive.";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public Faction Faction { get; set; } = Faction.Mafia;
    public Color RoleColor { get; set; } = AUSColors.Mafia;
    public Alignment Alignment => Alignment.MafiaSupport;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        Icon = AUSAssets.ConsortRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#DD0000>Consort</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#DD0000>Mafia</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#DD0000>Mafia</color> <color=#1e45d4>Deception</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill anyone that will not submit to the Mafia." +
            $"\n\nAttributes:" +
            "\nTBD." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Distract",
            "You can Distract a player during the round. You will kill your target.",
            AUSAssets.Consort_Distract)
    ];

    [MethodRpc((uint)AUSRpc.Consort_Distract, SendImmediately = true)]
    public static void RpcConsort_Distract(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Consort)
        {
            Logger<AUSPlugin>.Error("RpcConsort_Distract - Invalid Consort");
            return;
        }

        var consort = player.GetRole<Consort>();
        consort.DistractedPlayer = target;
    }

    [MethodRpc((uint)AUSRpc.Consort_Notify, SendImmediately = true)]
    public static bool RpcConsort_Notifyy(PlayerControl visitor, PlayerControl target)
    {
        foreach (var consorts in MiscUtils.GetPlayersWithRole<Consort>())
        {
            var consort = consorts.GetRole<Consort>();
            if (consort.DistractedPlayer == target)
            {
                if (visitor.AmOwner())
                {
                    MiscUtils.ShowNotification(Escort.Info(), Color.white, AUSAssets.EscortRoleCard.LoadAsset());
                    MiscUtils.AddFakeChat(target.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Escort Info"), Escort.Info());
                }
            }
        }

        return false;
    }

    public PlayerControl DistractedPlayer;
}

#region Consort_Distract
#endregion
public sealed class Consort_Distract : AmongUsSalemRoleButton<Consort, PlayerControl>
{
    public override string Name => "Distract";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Consort_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Consort_Distract;

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

        Consort.RpcConsort_Distract(Player, Target);
        MiscUtils.PostSuccessfulVisit(Player, Target, false, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(false, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (target == null) return base.IsTargetValid(target);
        return base.IsTargetValid(target) &&
            Role.DistractedPlayer != target;
    }
}

#region Consort_Options
#endregion
public sealed class Consort_Options : AbstractOptionGroup<Consort>
{
    public override string GroupName => Consort, "Consort");

    [ModdedNumberOption("<color=#DD0000>Consort</color> <color=#4a86e8>Distract</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}*/