using System.Text;
using Il2CppInterop.Runtime.Attributes;
using Color = UnityEngine.Color;
using UnityEngine;
using System.Collections;

namespace AmongUsSalem.Roles;

#region Coroner
#endregion
public sealed class Coroner(IntPtr cppPtr) 
    : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable, IContinueGame
{
    public bool continueGame => true;
    public string RoleName { get; set; } = "Coroner";
    public string revealText => "is a skilled surgeon.";
    public string RoleDescription => "Find roles of the dead.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Color RoleColor { get; set; } = AUSColors.Town;
    public Alignment Alignment => Alignment.TownInvestigative;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.CoronerRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var info = ICustomAURole.SetNewTabText(this);

        // Only show info if we have data
        if (Information.Count > 0)
        {
            info.AppendLine();

            foreach (var kvp in Information)
            {
                string roleName = kvp.Key.Item1;
                var (examinedPlayers, isKiller) = kvp.Value;

                // Show the killer role name (color-coded)
                info.AppendLine($"<b><color=#{kvp.Key.Item2.ToHtmlStringRGBA()}>{roleName}</color></b>"); // red role name for now, or replace with your color logic

                if (examinedPlayers.Count == 0)
                {
                    info.AppendLine("   No examined players yet.");
                    continue;
                }

                // Show all examined players and whether they matched the killer
                foreach (var player in examinedPlayers)
                {
                    string color = isKiller ? "#00FF00" : "#FF5555"; // green if matched, red if not
                    string result = isKiller ? "Killer" : "Not Killer";

                    info.AppendLine($"   <color={color}>{player.Data.PlayerName}</color> — {result}");
                }

                info.AppendLine(); // spacing between roles
            }
        }
        else
        {
            info.AppendLine();
            info.AppendLine("No autopsy results yet.");
        }

        return info;
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#06E00C>Coroner</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#06E00C>Town</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#06E00C>Town</color> <color=#1e45d4>Investigative</color>" +
            "\n<color=#fdbc00>Goal:</color> Hang every criminal and evildoer." +
            $"\n\nAttributes:" +
            "\nNone." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Autopsy",
            "You can Autopsy a dead player during the Day. When the Night begins, you will know the role that killed them.",
            AUSAssets.Coroner_Autopsy),
            
        new("Examine",
            "You can Examine a player at Night. You will see if they are any of the roles you Autopsied. If so, learn their exact role.",
            AUSAssets.Coroner_Examine)
    ];

    public enum Type { Killer, NotKiller, Autopsy }
    public static string Info(Type type, PlayerControl player)
    {
        if (type == Type.Autopsy) return $"You have added a {player.Role()} to your list of autopsy results. You can now find them at night";
        if (type == Type.Killer) return $"You found evidence that shows {player.Name()} is a killer and their role is a {player.Role()}";
        return $"You couldn't find any evidence that {player.Name()} is a killer.";
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (Player.AmOwner())
        {
            meetingMenu = new MeetingMenu(
                this,
                ClickGuess,
                MeetingAbilityType.Click,
                AUSAssets.Coroner_Autopsy,
                null!,
                IsExempt)
            {
                Position = new Vector3(-0.40f, 0f, -3f)
            };
        }
    }

    public override void OnMeetingStart()
    {
        AUSPlugin.DebugLogMessage("Coroner OnMeetingStart called!");
        SmartCoroner.Start();

        if (Player.AmOwner)
        {
            Coroutines.Start(GenButtons());
        }
    }

    public IEnumerator GenButtons()
    {
        yield return new WaitForSeconds(3f);
        meetingMenu.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied());
    }

    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);

        if (Player.AmOwner)
        {
            meetingMenu.HideButtons();
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
            meetingMenu = null!;
        }
    }

    public void ClickGuess(PlayerVoteArea voteArea, MeetingHud __)
    {
        var target = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId).Object;

        if (target.TryGetModifier<DeathHandlerModifier>(out var deathMod))
        {
            var killer = deathMod.KillerPlayer;
            if (killer.Data.Role is ICustomAURole ausRole && !AutopsiedPlayers.Contains(killer.PlayerId))
            {
                if (killer.HasDied())
                {
                    var deadAusRole = killer.GetRoleWhenAlive();
                    AutopsiedPlayers.Add(killer.PlayerId);
                    AutopsiedRoles.Add(deadAusRole.NiceName, deadAusRole.TeamColor);
                    recentlyAutopsied = killer;
                }
                else
                {
                    AutopsiedPlayers.Add(killer.PlayerId);
                    AutopsiedRoles.Add(ausRole.RoleName, ausRole.RoleColor);
                    recentlyAutopsied = killer;
                }
                
                AUSPlugin.DebugLogMessage("Coroner Autopsy - " + ausRole.RoleName);
            }
        }

        autopsiedTonight = true;
        if (Player.AmOwner)
        {
            meetingMenu?.HideButtons();
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return
            voteArea?.TargetPlayerId == Player.PlayerId || Player.Data.IsDead || !voteArea!.AmDead ||
            (MiscUtils.PlayerById(voteArea.TargetPlayerId).TryGetModifier<DeathHandlerModifier>(out var deathMod) &&
                (AutopsiedPlayers.Contains(deathMod.KillerPlayer.PlayerId) ||
                deathMod.CauseOfDeath == DeathReasonShow.Lynched ||
                deathMod.CauseOfDeath == DeathReasonShow.ARecruitOfTheJackalAndHaveFailedTheirTeammate ||
                deathMod.CauseOfDeath == DeathReasonShow.LeftTown ||
                deathMod.CauseOfDeath == DeathReasonShow.DishonoredTheTown ||
                deathMod.CauseOfDeath == DeathReasonShow.None ||
                deathMod.CauseOfDeath == DeathReasonShow.KilledByTheCovenVIP ||
                deathMod.CauseOfDeath == DeathReasonShow.KilledByTheTownVIP ||
                deathMod.CauseOfDeath == DeathReasonShow.KilledByThePerfectTown
                ));
    }

    public MeetingMenu meetingMenu;
    public List<byte> AutopsiedPlayers = new List<byte>();
    public Dictionary<string, Color> AutopsiedRoles = new Dictionary<string, Color>();
    public bool autopsiedTonight;
    public PlayerControl recentlyAutopsied;

    public List<byte> ExaminedPlayers = new List<byte>();

    public Dictionary<(string, Color), (List<PlayerControl>, bool)> Information = new Dictionary<(string, Color), (List<PlayerControl>, bool)>();
}

#region Coroner_Examine
#endregion
public sealed class Coroner_Examine : AmongUsSalemRoleButton<Coroner, PlayerControl>
{
    public override string Name => "Examine";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Town;
    public override float Cooldown => OptionGroupSingleton<Coroner_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Coroner_Examine;

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

        foreach (var key in Role.Information.Keys.ToList()) // ToList() prevents collection modification errors
        {
            var (players, isKiller) = Role.Information[key];

            players.Add(Target);
            if (Target.Data.Role is ICustomAURole customRole) isKiller = key.Item1 == customRole.RoleName;

            // Update or remove based on result
            if (isKiller)
            {
                Role.Information.Remove(key);
                Target.RpcAddModifier<RoleLearn>(Player);
                MiscUtils.ShowNotification(Coroner.Info(Coroner.Type.Killer, Target), Color.white, AUSAssets.CoronerRoleCard.LoadAsset());
                MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Coroner Info"), Coroner.Info(Coroner.Type.Killer, Target));

                if (Debugger.IsDebuggerActive)
                {
                    if (!CalculatedVoting.QueueEvidenceAgainst.ContainsValue(Target) && !Target.IsImpureToTown() && !Target.Is(Faction.Town))
                    {
                        CalculatedVoting.QueueEvidenceAgainst.Add(Player, Target);
                    }
                }

                Role.ExaminedPlayers.Clear();
            }
            else
            {
                //  Not the killer, keep it updated
                Role.Information[key] = (players, isKiller);

                MiscUtils.ShowNotification(Coroner.Info(Coroner.Type.NotKiller, Target), Color.white, AUSAssets.CoronerRoleCard.LoadAsset());
                MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Coroner Info"), Coroner.Info(Coroner.Type.NotKiller, Target));


                if (!Role.ExaminedPlayers.Contains(Target.PlayerId)) Role.ExaminedPlayers.Add(Target.PlayerId);
            }
        }

        MiscUtils.PostSuccessfulVisit(Player, Target, false, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role.AutopsiedPlayers.Count > 0;
    }
}

#region Coroner_Events
#endregion
public static class Coroner_Events
{
    [RegisterEvent()]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return; // Only run when round starts.
        }

        foreach (var coroners in MiscUtils.GetPlayersWithRole<Coroner>())
        {
            var coroner = coroners.GetRole<Coroner>();
            if (coroner.autopsiedTonight)
            {
                if (coroner.recentlyAutopsied.Data.Role is ICustomAURole customRole)
                {
                    coroner.Information.Add((customRole.RoleName, customRole.RoleColor), (new List<PlayerControl>(), false));
                }

                MiscUtils.ShowNotification(Coroner.Info(Coroner.Type.Autopsy, coroner.recentlyAutopsied), Color.white, AUSAssets.CoronerRoleCard.LoadAsset());
                MiscUtils.AddFakeChat(coroner.Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Coroner Info"), Coroner.Info(Coroner.Type.Autopsy, coroner.recentlyAutopsied));

                coroner.autopsiedTonight = false;
                coroner.recentlyAutopsied = null;
            }
        }
    }
}

#region Coroner_Options
#endregion
public sealed class Coroner_Options : AbstractOptionGroup<Coroner>
{
    public override string GroupName => "Coroner";

    [ModdedNumberOption("<color=#06E00C>Coroner</color> <color=#4a86e8>Examine</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}