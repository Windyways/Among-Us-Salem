using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.PluginLoading;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities;
using AmongUsSalem.Events;
using AmongUsSalem.Modifiers.Crewmate;
using AmongUsSalem.Modules;
using AmongUsSalem.Modules.Components;
using AmongUsSalem.Options;
using AmongUsSalem.Roles;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Roles.Impostor;
using AmongUsSalem.Roles.Neutral;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Modifiers.Game;

[MiraIgnore]
public abstract class AssassinModifier : ExcludedGameModifier
{
    public int maxKills;

    public MeetingMenu meetingMenu;
    public override string ModifierName => "Assassin";
    public string LastGuessedItem { get; set; }
    public PlayerControl? LastAttemptedVictim { get; set; }

    public override bool HideOnUi => true;

    public override int GetAssignmentChance()
    {
        return 100;
    }

    public override int GetAmountPerGame()
    {
        return 0;
    }

    public override int Priority()
    {
        return 0;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return false;
    }

    public override void OnActivate()
    {
        base.OnActivate();

        maxKills = 15;

        //Logger<AUSPlugin>.Error($"AssassinModifier.OnActivate maxKills: {maxKills}");
        if (Player.AmOwner)
        {
            meetingMenu = new MeetingMenu(
                Player.Data.Role,
                ClickGuess,
                MeetingAbilityType.Click,
                TouAssets.Guess,
                null!,
                IsExempt);
        }
    }

    public override void OnMeetingStart()
    {
        //Logger<AUSPlugin>.Error($"AssassinModifier.OnMeetingStart maxKills: {maxKills}");
        if (Player.AmOwner)
        {
            //meetingMenu.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && maxKills > 0);
            meetingMenu.GenButtons(MeetingHud.Instance, !Player.HasDied() && maxKills > 0);
        }
    }

    public void OnVotingComplete()
    {
        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
        }
    }

    public override void OnDeactivate()
    {
        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
            meetingMenu = null!;
        }
    }

    public void ClickGuess(PlayerVoteArea voteArea, MeetingHud meetingHud)
    {
        if (meetingHud.state == MeetingHud.VoteStates.Discussion)
        {
            return;
        }
        
        if (Minigame.Instance != null)
        {
            return;
        }

        var player = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId).Object;

        var shapeMenu = GuesserMenu.Create();
        shapeMenu.Begin(IsRoleValid, ClickRoleHandle, IsModifierValid, ClickModifierHandle);

        void ClickRoleHandle(RoleBehaviour role)
        {
            var realRole = player.Data.Role;

            var cachedMod = player.GetModifiers<BaseModifier>().FirstOrDefault(x => x is ICachedRole) as ICachedRole;
            if (cachedMod != null)
            {
                realRole = cachedMod.CachedRole;
            }

            var pickVictim = role.Role == realRole.Role;
            var victim = pickVictim ? player : Player;

            ClickHandler(victim);
            LastAttemptedVictim = player;
            LastGuessedItem = $"{role.TeamColor.ToTextColor()}{role.NiceName}</color>";
        }

        void ClickModifierHandle(BaseModifier modifier)
        {
            var pickVictim = player.HasModifier(modifier.TypeId);
            var victim = pickVictim ? player : Player;

            ClickHandler(victim);
            LastAttemptedVictim = player;
            LastGuessedItem =
                $"{MiscUtils.GetRoleColour(modifier.ModifierName.Replace(" ", string.Empty)).ToTextColor()}{modifier.ModifierName}</color>";
        }

        void ClickHandler(PlayerControl victim)
        {
            if (victim.TryGetModifier<OracleBlessedModifier>(out var oracleMod))
            {
                OracleRole.RpcOracleBlessNotify(oracleMod.Oracle, PlayerControl.LocalPlayer, victim);

                MeetingMenu.Instances.Do(x => x.HideSingle(victim.PlayerId));

                shapeMenu.Close();
                LastGuessedItem = string.Empty;
                LastAttemptedVictim = null;

                return;
            }

            if (victim == Player && Player.TryGetModifier<DoubleShotModifier>(out var modifier) && !modifier.Used)
            {
                modifier!.Used = true;

                Coroutines.Start(MiscUtils.CoFlash(AUSColors.Mafia));

                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{AUSColors.Mafia.ToTextColor()}Your Double Shot has prevented you from dying this meeting!</color></b>",
                    Color.white, spr: TouModifierIcons.DoubleShot.LoadAsset());

                notif1.Text.SetOutlineThickness(0.35f);
                notif1.transform.localPosition = new Vector3(0f, 1f, -20f);

                shapeMenu.Close();
                LastGuessedItem = string.Empty;
                LastAttemptedVictim = null;

                return;
            }

            Player.RpcCustomMurder(victim, createDeadBody: false, teleportMurderer: false, showKillAnim: false,
                playKillSound: false);

            if (victim != Player)
            {
                LastGuessedItem = string.Empty;
                LastAttemptedVictim = null;
                MeetingMenu.Instances.Do(x => x.HideSingle(victim.PlayerId));
                DeathHandlerModifier.RpcUpdateDeathHandler(victim, "Guessed", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse, $"By {Player.Data.PlayerName}", lockInfo: DeathHandlerOverride.SetTrue);
            }
            else
            {
                DeathHandlerModifier.RpcUpdateDeathHandler(victim, "Misguessed", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse, lockInfo: DeathHandlerOverride.SetTrue);
            }

            maxKills--;

            if (maxKills == 0 || victim == Player)
            {
                meetingMenu?.HideButtons();
            }

            shapeMenu.Close();
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        var votePlayer = voteArea.GetPlayer();
        return voteArea?.TargetPlayerId == Player.PlayerId ||
               Player.Data.IsDead ||
               voteArea!.AmDead ||
               (Player.IsImpostor() && votePlayer?.IsImpostor() == true) ||
               (Player.Data.Role is VampireRole && votePlayer?.Data.Role is VampireRole) ||
               (votePlayer?.Data.Role is MayorRole mayor && mayor.Revealed) ||
               (votePlayer?.GetModifiers<RevealModifier>().Any(x => x.Visible && x.RevealRole) == true) ||
               (Player.IsLover() && votePlayer?.IsLover() == true) ||
               votePlayer?.HasModifier<JailedModifier>() == true;
    }

    private bool IsRoleValid(RoleBehaviour role)
    {
        if (role.IsDead)
        {
            return false;
        }

        var touRole = role as IAUSRole;
        var unguessableRole = role as IUnguessable;

        if (touRole is IGhostRole)
        {
            return false;
        }

        if (unguessableRole != null && !unguessableRole.IsGuessable)
        {
            return false;
        }

        if (touRole?.Alignment == Alignment.TownInvestigative)
        {
            return false;
        }

        if (role.IsCrewmate() && role is ICustomRole)
        {
            return true;
        }

        return false;
    }

    private static bool IsModifierValid(BaseModifier modifier)
    {
        var isValid = true;
        // This will remove modifiers that alter their chance/amount
        if ((modifier is TouGameModifier touMod && (touMod.CustomAmount <= 0 || touMod.CustomChance <= 0)) ||
            (modifier is AllianceGameModifier allyMod && (allyMod.CustomAmount <= 0 || allyMod.CustomChance <= 0)) ||
            (modifier is UniversalGameModifier uniMod && (uniMod.CustomAmount <= 0 || uniMod.CustomChance <= 0)))
        {
            isValid = false;
        }

        if (!isValid)
        {
            return false;
        }

        var crewMod = modifier as TouGameModifier;
        if (crewMod != null && crewMod.FactionType.ToDisplayString().Contains("Crew") &&
            !crewMod.FactionType.ToDisplayString().Contains("Non"))
        {
            return true;
        }

        return false;
    }
}