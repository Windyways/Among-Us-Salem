using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Survivor
#endregion
public sealed class Survivor(IntPtr cppPtr)
    : NeutralRole(cppPtr), IWikiDiscoverable, IAUSRole, INotThreatable
{
    public string RoleName { get; set; } = "Survivor";
    public string revealText => "simply wants to live";
    public string RoleDescription => "";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Neutral;
    public Color RoleColor { get; set; } = AUSColors.Survivor;
    public Alignment Alignment => Alignment.NeutralBenign;
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.SurvivorRoleCard,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#dddd00>Survivor</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#A9A9A9>Neutral</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#A9A9A9>Neutral</color> <color=#1e45d4>Benign</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill everyone in the town." +
            $"\n\nAttributes:" +
            "\nN/A" +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Vest",
            "You can Vest at Night." +
            "\n\nYou will grant yourself Basic Defense for the Night.",
            AUSAssets.Survivor_Vest)
    ];

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return !Player.HasDied();
    }

    public override void OnMeetingStart()
    {
        isVesting = false;
    }

    public static string Info()
    {
        return "You were attacked, but your bulletproof <b><color=#4a86e8>Vest</color></b> saved you!";
    }

    [MethodRpc((uint)AUSRpc.Survivor_Vest, SendImmediately = true)]
    public static void RpcSurvivor_Vest(PlayerControl player)
    {
        if (player.Data.Role is not Survivor)
        {
            Logger<AUSPlugin>.Error("RpcSurvivor_Vest - Invalid Survivor");
            return;
        }

        var survivor = player.GetRole<Survivor>();
        survivor.isVesting = true;

        if (player.Data.Role is IAUSRole ausRole) ausRole.ApplyDefense(Defense.Basic);
    }

    [MethodRpc((uint)AUSRpc.Survivor_Notify, SendImmediately = true)]
    public static void RpcSurvivor_Notify(PlayerControl player)
    {
        if (player.Data.Role is not Survivor)
        {
            Logger<AUSPlugin>.Error("RpcSurvivor_Notify - Invalid Survivor");
            return;
        }

        if (player.AmOwner())
        {
            MiscUtils.ShowNotification(Info(), Color.white, AUSAssets.SurvivorRoleCard.LoadAsset());
            MiscUtils.AddFakeChat(player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Survivor, "Survivor Info"), Info());
        }
    }

    public bool isVesting;
}

#region Survivor_Vest
#endregion
public sealed class Survivor_Vest : AmongUsSalemRoleButton<Survivor>
{
    public override string Name => "Vest";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Survivor;
    public override float Cooldown => OptionGroupSingleton<Survivor_Options>.Instance.Cooldown;
    public override int MaxUses => (int)OptionGroupSingleton<Survivor_Options>.Instance.Charges;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Survivor_Vest;

    public override void ClickHandler()
    {
        if (MiscUtils.SuccessfulVisit(Player, Player, false, false))
        {
            base.ClickHandler();
        }
    }

    protected override void OnClick()
    {
        Survivor.RpcSurvivor_Vest(Role.Player);
        MiscUtils.PostSuccessfulVisit(Player, Player, false, false);
    }

    public override bool CanUse()
    {
        return base.CanUse() && !Role.isVesting;
    }
}

#region Survivor_Options
#endregion
public sealed class Survivor_Options : AbstractOptionGroup<Survivor>
{
    public override string GroupName => "Survivor";

    [ModdedNumberOption("<color=#dddd00>Survivor</color> <color=#4a86e8>Vest</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#dddd00>Survivor</color> Max <color=#4a86e8>Vests</color>", 1f, 30f, 1f)]
    public float Charges { get; set; } = 4f;
}