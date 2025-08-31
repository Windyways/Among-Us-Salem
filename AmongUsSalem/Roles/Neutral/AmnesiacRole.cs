using System.Text;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using AmongUsSalem.Events.TouEvents;
using AmongUsSalem.Modifiers.Game.Impostor;
using AmongUsSalem.Modifiers.Game.Neutral;
using AmongUsSalem.Modifiers.Neutral;
using AmongUsSalem.Modules;
using AmongUsSalem.Options;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Roles.Neutral;

public sealed class AmnesiacRole(IntPtr cppPtr)
    : NeutralRole(cppPtr), IAUSRole, IDoomable, ICrewVariant
{
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;
    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<MysticRole>());
    public DoomableType DoomHintType => DoomableType.Death;
    public string RoleName => TouLocale.Get(TouNames.Amnesiac, "Amnesiac");
    public string RoleDescription => "Remember A Role Of A Deceased Player";
    public string RoleLongDescription => "Find a dead body to remember and become their role";
    public Color RoleColor => AUSColors.Amnesiac;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public Alignment Alignment => Alignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.MediumIntroSound,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = TouRoleIcons.Amnesiac
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Neutral Benign role that gains access to a new role from remembering a dead body’s role. Use the role you remember to win the game." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Remember",
            "Remember the role of a dead body. If the dead body's role is a unique role, you will remember the base faction's role instead.",
            TouNeutAssets.RememberButtonSprite)
    ];

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        if (Player.HasModifier<AmnesiacArrowModifier>())
        {
            var mods = Player.GetModifiers<AmnesiacArrowModifier>();

            mods.Do([HideFromIl2Cpp](x) => Player.RemoveModifier(x.UniqueId));
        }
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return false;
    }

    [MethodRpc((uint)AUSRpc.Remember, SendImmediately = true)]
    public static void RpcRemember(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not AmnesiacRole)
        {
            Logger<AUSPlugin>.Error("RpcRemember - Invalid amnesiac");
            return;
        }

        var roleWhenAlive = target.GetRoleWhenAlive();

        if (roleWhenAlive is AmnesiacRole)
        {
            if (player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{target.CachedPlayerData.PlayerName} was an {AUSColors.Amnesiac.ToTextColor()}Amnesiac</color>, so their role cannot be picked up.</b>",
                    Color.white, new Vector3(0f, 1f, -20f), spr: TouRoleIcons.Amnesiac.LoadAsset());
                notif1.Text.SetOutlineThickness(0.35f);
            }

            return;
        }

        var touAbilityEvent = new TouAbilityEvent(AbilityType.AmnesiacPreRemember, player, target);
        MiraEventManager.InvokeEvent(touAbilityEvent);

        player.ChangeRole((ushort)roleWhenAlive.Role);
        if (player.Data.Role is InquisitorRole inquis)
        {
            if (player.HasModifier<InquisitorHereticModifier>())
            {
                player.RemoveModifier<InquisitorHereticModifier>();
            }
            inquis.Targets = ModifierUtils.GetPlayersWithModifier<InquisitorHereticModifier>().ToList();
            inquis.TargetRoles = ModifierUtils.GetActiveModifiers<InquisitorHereticModifier>().Select(x => x.TargetRole)
                .OrderBy(x => x.NiceName).ToList();
        }
        else if (player.Data.Role is PlaguebearerRole || player.Data.Role is PestilenceRole)
        {
            ModifierUtils.GetActiveModifiers<PlaguebearerInfectedModifier>().Do(x => x.ModifierComponent?.RemoveModifier(x));
        }
        else if (player.Data.Role is MayorRole mayor)
        {
            mayor.Revealed = false;
        }
        else if (player.Data.Role is GuardianAngelTouRole ga)
        {
            var gaTarget = ModifierUtils.GetPlayersWithModifier<GuardianAngelTargetModifier>().FirstOrDefault(x => x.PlayerId == target.PlayerId);

            if (gaTarget != null && gaTarget.TryGetModifier<GuardianAngelTargetModifier>(out var gaMod))
            {
                ga.Target = gaTarget;
                gaMod.OwnerId = player.PlayerId;
            }
        }
        else if (player.Data.Role is ExecutionerRole exe)
        {
            var exeTarget = ModifierUtils.GetPlayersWithModifier<ExecutionerTargetModifier>().FirstOrDefault(x => x.PlayerId == target.PlayerId);

            if (exeTarget != null && exeTarget.TryGetModifier<ExecutionerTargetModifier>(out var exeMod))
            {
                exe.Target = exeTarget;
                exeMod.OwnerId = player.PlayerId;
            }
        }
        else if (player.Data.Role is VampireRole)
        {
            if (target.HasModifier<VampireBittenModifier>())
            {
                // Makes the amne stay with the bitten modifier
                player.AddModifier<VampireBittenModifier>();
            }
            else
            {
                // Makes the og vampire a bitten vampire so to speak, yes it makes it more confusing, but that's how it is, deal with it - Atony
                target.AddModifier<VampireBittenModifier>();
            }
        }

        if (player.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>You remembered that you were like {target.Data.PlayerName}, the {player.Data.Role.TeamColor.ToTextColor()}{player.Data.Role.NiceName}</color>.</b>",
                Color.white, new Vector3(0f, 1f, -20f), spr: TouRoleIcons.Amnesiac.LoadAsset());
            notif1.Text.SetOutlineThickness(0.35f);
        }

        if (roleWhenAlive is not VampireRole && (roleWhenAlive.MaxCount <= 1 || (roleWhenAlive.MaxCount <= PlayerControl.AllPlayerControls
                .ToArray().Count(x => x.Data.Role.Role == roleWhenAlive.Role))))
        {
            if (target.IsCrewmate())
            {
                target.ChangeRole((ushort)RoleTypes.Crewmate);
            }
            else if (target.IsImpostor())
            {
                target.ChangeRole((ushort)RoleTypes.Impostor);
            }
            /*else if (target.IsNeutral() && player.Data.Role is IAUSRole touRole)
            {
                switch (touRole.Alignment)
                {
                    default:
                        target.ChangeRole(RoleId.Get<SurvivorRole>());
                        break;
                    case Alignment.NeutralEvil:
                        target.ChangeRole(RoleId.Get<JesterRole>());
                        break;
                    case Alignment.NeutralKilling:
                        target.ChangeRole(RoleId.Get<MercenaryRole>());
                        player.AddModifier<MercenaryBribedModifier>(target)!.alerted = true;
                        break;
                }
            }*/
            else
            {
                target.ChangeRole(RoleId.Get<MercenaryRole>());
                player.AddModifier<MercenaryBribedModifier>(target)!.alerted = true;
            }
        }

        if (player.IsImpostor() && OptionGroupSingleton<AssassinOptions>.Instance.AmneTurnImpAssassin)
        {
            player.AddModifier<ImpostorAssassinModifier>();
        }
        else if (player.IsNeutral() &&
                 OptionGroupSingleton<AssassinOptions>.Instance.AmneTurnNeutAssassin)
        {
            player.AddModifier<NeutralKillerAssassinModifier>();
        }
        
        var touAbilityEvent2 = new TouAbilityEvent(AbilityType.AmnesiacPostRemember, player, target);
        MiraEventManager.InvokeEvent(touAbilityEvent2);
    }
}