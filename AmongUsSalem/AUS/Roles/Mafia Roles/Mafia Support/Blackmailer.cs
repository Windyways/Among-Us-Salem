using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Blackmailer
#endregion
public sealed class Blackmailer(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IWikiDiscoverable, ICustomAURole
{
    public string RoleName { get; set; } = "Blackmailer";
    public string revealText => "has a desire or deceive.";
    public string RoleDescription => "Make players stop talking.";
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

    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        Icon = AUSAssets.BlackmailerRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#DD0000>Blackmailer</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#DD0000>Mafia</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#DD0000>Mafia</color> <color=#1e45d4>Deception</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill anyone that will not submit to the Mafia." +
            $"\n\nAttributes:" +
            "\nIf all Mafia Killing roles are dead, you will be promoted to Mafioso." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Blackmail",
            "You can Blackmail a player at Night" +
            "\n\nYour target cannot talk the following day.",
            AUSAssets.Blackmailer_Blackmail)
    ];

    [MethodRpc((uint)AUSRpc.Blackmailer_Blackmail, SendImmediately = true)]
    public static void RpcBlackmailer_Blackmail(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Blackmailer)
        {
            Logger<AUSPlugin>.Error("RpcBlackmailer_Blackmail - Invalid Blackmailer");
            return;
        }

        var blackmailer = player.GetRole<Blackmailer>();
        blackmailer.BlackmailedPlayer = target;
    }

    public PlayerControl BlackmailedPlayer;
}

#region Blackmailer_Blackmail
#endregion
public sealed class Blackmailer_Blackmail : AmongUsSalemRoleButton<Blackmailer, PlayerControl>
{
    public override string Name => "Blackmail";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Blackmailer_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Blackmailer_Blackmail;

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

        Blackmailer.RpcBlackmailer_Blackmail(Player, Target);
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
            Role.BlackmailedPlayer != target;
    }
}

#region Blackmailer_Events
#endregion
public static class Blackmailer_Events
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return; // Only run when round starts.
        }

        foreach (var blackmailers in MiscUtils.GetPlayersWithRole<Blackmailer>())
        {
            var blackmailer = blackmailers.GetRole<Blackmailer>();
            blackmailer.BlackmailedPlayer = null;
        }
    }
}

#region Blackmailer_Options
#endregion
public sealed class Blackmailer_Options : AbstractOptionGroup<Blackmailer>
{
    public override string GroupName => "Blackmailer";

    [ModdedNumberOption("<color=#DD0000>Blackmailer</color> <color=#4a86e8>Blackmail</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedToggleOption("<color=#DD0000>Blackmailer</color> Sees Whispers>")]
    public bool SeeWhispers { get; set; } = true;
}