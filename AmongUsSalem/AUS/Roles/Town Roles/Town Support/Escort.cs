using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using Color = UnityEngine.Color;
using UnityEngine;

namespace AmongUsSalem.Roles;

#region Escort
#endregion
public sealed class Escort(IntPtr cppPtr)
    : CrewmateRole(cppPtr), IWikiDiscoverable, ICustomAURole
{
    public string RoleName { get; set; } = "Escort";
    public string revealText => "is a beautiful person working for the town.";
    public string RoleDescription => "Distract to stop abilities.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Color RoleColor { get; set; } = AUSColors.Town;
    public Alignment Alignment => Alignment.TownSupport;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;


    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.EscortRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#06E00C>Escort</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#06E00C>Town</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#06E00C>Town</color> <color=#1e45d4>Support</color>" +
            "\n<color=#fdbc00>Goal:</color> Hang every criminal and evildoer." +
            $"\n\nAttributes:" +
            "\nNone." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Distract",
            "You can Distract a player at Night." +
            "\n\nYou will RoleBlock your target, preventing them from using their abilities.",
            AUSAssets.Escort_Distract)
    ];

    public override void OnMeetingStart()
    {
        DistractedPlayer = null;
    }

    public static string Info()
    {
        return "Someone occupied your <b><color=#922058>Night</color></b>. You were role blocked!";
    }

    [MethodRpc((uint)AUSRpc.Escort_Distract, SendImmediately = true)]
    public static void RpcEscort_Distract(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Escort)
        {
            Logger<AUSPlugin>.Error("RpcEscort_Distract - Invalid Escort");
            return;
        }

        var escort = player.GetRole<Escort>();
        escort.DistractedPlayer = target;
    }

    [MethodRpc((uint)AUSRpc.Escort_Notify, SendImmediately = true)]
    public static bool RpcEscort_Notify(PlayerControl visitor, PlayerControl target)
    {
        foreach (var escorts in MiscUtils.GetPlayersWithRole<Escort>())
        {
            var escort = escorts.GetRole<Escort>();
            if (escort.DistractedPlayer == visitor)
            {
                if (visitor.AmOwner)
                {
                    Coroutines.Start(MiscUtils.CoFlash(AUSColors.Town));
                    MiscUtils.ShowNotification(Info(), Color.white, AUSAssets.EscortRoleCard.LoadAsset());
                    MiscUtils.AddFakeChat(target.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Escort Info"), Info());

                    var buttons = CustomButtonManager.Buttons.Where(x => x.Enabled(visitor.Data.Role) && x.Timer <= 0).ToList();
                    foreach (var button in buttons) button.ResetCooldownAndOrEffect();
                }
            }
        }

        return false;
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        player.RpcAddModifier<RBimmune>();
        if (player.TryGetModifier<RBimmune>(out var rb)) rb.permanent = true;
    }

    public PlayerControl DistractedPlayer;
}

#region Escort_Distract
#endregion
public sealed class Escort_Distract : AmongUsSalemRoleButton<Escort, PlayerControl>
{
    public override string Name => "Distract";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Town;
    public override float Cooldown => OptionGroupSingleton<Escort_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Escort_Distract;

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

        Escort.RpcEscort_Distract(Player, Target);
        MiscUtils.PostSuccessfulVisit(Player, Target, false, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (target == null) return base.IsTargetValid(target);
        return base.IsTargetValid(target) && Role.DistractedPlayer != target;
    }
}

#region Escort_Options
#endregion
public sealed class Escort_Options : AbstractOptionGroup<Escort>
{
    public override string GroupName => "Escort";

    [ModdedNumberOption("<color=#06E00C>Escort</color> <color=#4a86e8>Distract</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}