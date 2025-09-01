using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using Color = UnityEngine.Color;
using UnityEngine;

namespace AmongUsSalem.Roles;

#region Bodyguard
#endregion
public sealed class Bodyguard(IntPtr cppPtr)
    : CrewmateRole(cppPtr), IWikiDiscoverable, IAUSRole
{
    public string RoleName => TouLocale.Get(TouNames.Bodyguard, "Bodyguard");
    public string revealText => "is a trained protector.";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction RoleFaction { get; set; } = Faction.Town;
    public Color RoleColor { get; set; } = AUSColors.Town;
    public Alignment Alignment => Alignment.TownProtective;

    public Attack Attack { get; set; } = Attack.Powerful;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Powerful;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.BodyguardRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#06e00c>Bodyguard</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#06e00c>Town</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#06e00c>Town</color> <color=#1e45d4>Investigative</color>" +
            "\n<color=#fdbc00>Goal:</color> Hang every criminal and evildoer." +
            $"\n\nAttributes:" +
            "\nYou cannot counterattack passive attacks." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Guard",
            "If your target is directly attacked or is the victim of a harmful visit, you and the visitor will fight, stopping them from attacking your target." +
            "\n\nYou will deal a Powerful Attack to your foe and yourself.",
            AUSAssets.Bodyguard_Guard),

        new("Self Protect",
            "You may stay at gome and protect yourself to gain Basic Defense.",
            AUSAssets.Bodyguard_SelfProtect)
    ];

    public void OnMeetingStart(MeetingHud __instance)
    {
        GuardedPlayer = null;
        isSelfProtected = false;
    }

    public enum Type { AttackedButProtected, AttackedButSelfProtected }
    public static string Info(Type type)
    {
        if (type is Type.AttackedButProtected) return "Someone attacked you, but a <b><color=#06e00c>Bodyguard</color></b> protected you!";
        return "Someone attacked you, but your armor protected you!";
    }

    [MethodRpc((uint)AUSRpc.Bodyguard_Guard, SendImmediately = true)]
    public static void RpcBodyguard_Guard(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Bodyguard)
        {
            Logger<AUSPlugin>.Error("RpcBodyguard_Guard - Invalid Bodyguard");
            return;
        }

        var bodyguard = player.GetRole<Bodyguard>();
        bodyguard.GuardedPlayer = target;

        if (target.Data.Role is IAUSRole ausRole) ausRole.ApplyDefense(Defense.Powerful);
    }

    [MethodRpc((uint)AUSRpc.Bodyguard_SelfProtect, SendImmediately = true)]
    public static void RpcBodyguard_SelfProtect(PlayerControl player)
    {
        if (player.Data.Role is not Bodyguard)
        {
            Logger<AUSPlugin>.Error("RpcBodyguard_SelfProtect - Invalid Bodyguard");
            return;
        }

        var bodyguard = player.GetRole<Bodyguard>();
        if (bodyguard.Player.Data.Role is IAUSRole ausRole) ausRole.ApplyDefense(Defense.Basic);
    }

    [MethodRpc((uint)AUSRpc.Bodyguard_Notify, SendImmediately = true)]
    public static bool RpcBodyguard_Notify(PlayerControl visitor, PlayerControl target)
    {
        foreach (var bodyguards in MiscUtils.GetPlayersWithRole<Bodyguard>())
        {
            var bodyguard = bodyguards.GetRole<Bodyguard>();
            if (bodyguard.Player == target && bodyguard.Player.AmOwner())
            {
                MiscUtils.ShowNotification(Info(Type.AttackedButSelfProtected), Color.white, AUSAssets.BodyguardRoleCard.LoadAsset());
                MiscUtils.AddFakeChat(target.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Bodyguard Info"), Info(Type.AttackedButSelfProtected));
            }
            else if (bodyguard.GuardedPlayer == target)
            {
                if (Debugger.IsDebuggerActive)
                {
                    if (bodyguard.Player.CanKill(bodyguard.Player)) MiscUtils.RpcApplyDeathReason(bodyguard.Player, bodyguard.Player, DeathReasonShow.DiedWhileDefendingTheirTarget);
                    if (bodyguard.Player.CanKill(visitor)) MiscUtils.RpcApplyDeathReason(bodyguard.Player, visitor, DeathReasonShow.KilledByABodyguard);
                }

                if (visitor.AmOwner())
                {
                    if (bodyguard.Player.CanKill(bodyguard.Player)) MiscUtils.RpcApplyDeathReason(bodyguard.Player, bodyguard.Player, DeathReasonShow.DiedWhileDefendingTheirTarget);
                }
                if (target.AmOwner())
                {
                    MiscUtils.ShowNotification(Info(Type.AttackedButProtected), Color.white, AUSAssets.BodyguardRoleCard.LoadAsset());
                    MiscUtils.AddFakeChat(target.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Bodyguard Info"), Info(Type.AttackedButProtected));
                }
                if (bodyguard.Player.AmOwner())
                {
                    if (bodyguard.Player.CanKill(visitor)) MiscUtils.RpcApplyDeathReason(bodyguard.Player, visitor, DeathReasonShow.KilledByABodyguard);
                }
            }
        }

        return false;
    }

    public PlayerControl GuardedPlayer;
    public bool isSelfProtected;
}

#region Bodyguard_Guard
#endregion
public sealed class Bodyguard_Guard : AmongUsSalemRoleButton<Bodyguard, PlayerControl>
{
    public override string Name => "Guard";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Town;
    public override float Cooldown => OptionGroupSingleton<Bodyguard_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Bodyguard_Guard;

    public override void ClickHandler()
    {
        if (Target != null)
        {
            if (MiscUtils.SuccessfulVisit(Role.Player, Target, false, true))
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

        Bodyguard.RpcBodyguard_Guard(Role.Player, Target);
        MiscUtils.PostSuccessfulVisit(Role.Player, Target, false, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (target == null) return base.IsTargetValid(target);
        return base.IsTargetValid(target) && Role.GuardedPlayer != target;
    }
}

#region Bodyguard_SelfProtect
#endregion
public sealed class Bodyguard_SelfProtect : AmongUsSalemRoleButton<Bodyguard>
{
    public override string Name => "Self Protect";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Town;
    public override float Cooldown => OptionGroupSingleton<Bodyguard_Options>.Instance.Cooldown;
    public override int MaxUses => (int)OptionGroupSingleton<Bodyguard_Options>.Instance.Charges;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Bodyguard_SelfProtect;

    public override void ClickHandler()
    {
        if (MiscUtils.SuccessfulVisit(Role.Player, Role.Player, false, false))
        {
            base.ClickHandler();
        }
    }

    protected override void OnClick()
    {
        Bodyguard.RpcBodyguard_SelfProtect(Role.Player);
        MiscUtils.PostSuccessfulVisit(Role.Player, Role.Player, false, false);
    }
}

#region Bodyguard_Options
#endregion
public sealed class Bodyguard_Options : AbstractOptionGroup<Bodyguard>
{
    public override string GroupName => TouLocale.Get(TouNames.Bodyguard, "Bodyguard");

    [ModdedNumberOption("<color=#06e00c>Bodyguard</color> <color=#4a86e8>Guard</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#06e00c>Bodyguard</color> Max <color=#4a86e8>Self Protects</color>", 1f, 30f, 1f)]
    public float Charges { get; set; } = 1;
}