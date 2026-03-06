using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using Reactor.Utilities.Extensions;
using System.Text;
using TownOfUs.Modifiers;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Jester(IntPtr cppPtr) : CovenRole(cppPtr), ICustomAURole, IWikiDiscoverable, INotThreatable
{
    public string RoleName { get; set; } = "Jester";
    public string revealText => "wants to be hanged.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are a chaotic spirit who's power can only be unleashed from the gallows.";
    public Color RoleColor { get; set; } = RoleColors.Jester;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Neutral;
    public Alignment Alignment => Alignment.NeutralEvil;

    public Attack Attack { get; set; } = Attack.Unstoppable;
    public Defense Defense { get; set; } = Defense.Basic;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Unstoppable;
    public Defense ogDefense { get; set; } = Defense.Basic;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.JesterRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return 
            $"Attack: {Attack}\n" +
            $"Defense: {Defense}\n" +
            $"The {RoleName} is a {Alignment.ToSpacedString()} role whose goal is to be lynched. Upon being lynched, it can Haunt a player to kill them.\n" +
            "Get yourself hanged by any means necessary." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return ShowAttributes();
    }

    private static string ShowAttributes()
    {
        if (!OptionGroupSingleton<Jester_Options>.Instance.EnableChatterbox) return $"- You will lose your Defense on Day 3.";
        return
            $"- At the start of each Day, your Chatterbox requires you to send 1-4 messages.\n" +
            $"- You cannot win until your Chatterbox is completed.\n" +
            $"- You will die of boredom if you do not complete your Chatterbox.\n" +
            $"- You will lose your Defense on Day 3.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Haunt",
            "You can Haunt a player that has voted you or abstained at Night.\n" +
            "You will deal an Unstoppable Attack to your target when the Day begins. You can only Haunt once you’ve been lynched.\n" +
            "If you do not pick a player by the end of the Night, a random player will be chosen.",
            AUSAssets.Jester_Haunt),
    ];

    [MethodRpc((uint)AUSRpc.RpcQuotaLynch)]
    public static void RpcQuotaLynch(PlayerControl player)
    {
        if (player.Data.Role is not Jester)
        {
            Logger<AUSPlugin>.Error("RpcQuotaLynch - Invalid Jester");
            return;
        }

        var jester = player.GetRole<Jester>();
        jester.CompletedQuotaWhenLynched = true;
    }

    public bool Lynched()
    {
        return CompletedQuotaWhenLynched &&
            Player.TryGetModifier<DeathHandlerModifier>(out var deathHandler) && deathHandler.CauseOfDeath == DeathReasonShow.Lynched;
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return Lynched();
    }

    public static string Info()
    {
        return "The Jester will get their revenge from the grave!";
    }

    public void Role_OnMeetingStart()
    {
        AUSPlugin.DebugLogMessage("Jester OnMeetingStart called!");

        if (Lynched() && ModifierUtils.GetActiveModifiers<HauntableModifier>().Any())
        {
            var hauntable = PlayerControl.AllPlayerControls.ToArray().Where(x => x.HasModifier<HauntableModifier>(h => h.Caster == Player)).ToList();
            AUSPlugin.DebugLogMessage("Jester Hauntable targets: " + hauntable.Count);

            if (hauntable.Count > 0) hauntable.Random().RpcAddModifier<HauntedModifier>(Player, true);
            foreach (var haunt in hauntable) haunt.RpcRemoveModifier<HauntableModifier>();
        }

        if (DayNightMechanic.DayCount < 2)
            return;

        if (MessagesSent < ChatterBoxQuota && Player.AmOwner() && !Player.HasDied())
        {
            Player.RpcCustomMurder(Player);
            VisitingMechanic.RpcAddDeathReason(Player, (int)DeathReasonShow.DiedOfBoredom);
        }
        else
        {
            MessagesSent = 0;
            ChatterBoxQuota = UnityEngine.Random.Range(1, 5); // 1-4.
            if (!OptionGroupSingleton<Jester_Options>.Instance.EnableChatterbox) ChatterBoxQuota = 0;
        }

        // Lose Defense D3.
        if (DayNightMechanic.DayCount == 3) AttackDefenseMechanic.RpcApplyDefense(Player, Defense.None, true, true);
    }

    public int MessagesSent;
    public int ChatterBoxQuota;
    public bool CompletedQuotaWhenLynched;
    public List<byte> Voters = new List<byte>();

    public void OnMessageSend(ChatController __instance)
    {
        //if (__instance.timeSinceLastMessage > 0)
        //    return;

        MessagesSent++;
    }
}

public sealed class Jester_Haunt : TownOfUsRoleButton<Jester>
{
    public override string Name => "Haunt";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Jester;
    public override float Cooldown => 1f;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Jester_Haunt;
    public override bool UsableInDeath => true;
    public bool Show { get; set; }

    public override bool Enabled(RoleBehaviour? role)
    {
        return Show && ModifierUtils.GetActiveModifiers<HauntableModifier>().Any();
    }

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Player, false, false)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        var player1Menu = CustomPlayerMenu.Create();
        player1Menu.transform.FindChild("PhoneUI").GetChild(0).GetComponent<SpriteRenderer>().material =
            PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
        player1Menu.transform.FindChild("PhoneUI").GetChild(1).GetComponent<SpriteRenderer>().material =
            PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;

        player1Menu.Begin(
            plr => !plr.HasDied() && plr.HasModifier<HauntableModifier>(x => x.Caster == Player),
            plr =>
            {
                player1Menu.ForceClose();

                if (plr == null)
                {
                    return;
                }

                plr.RpcAddModifier<HauntedModifier>(Player, false);
            }
        );
        foreach (var panel in player1Menu.potentialVictims)
        {
            panel.PlayerIcon.cosmetics.SetPhantomRoleAlpha(1f);
            if (panel.NameText.text != PlayerControl.LocalPlayer.Data.PlayerName)
            {
                panel.NameText.color = Color.white;
            }
        }
    }

    public override bool CanUse()
    {
        return ModifierUtils.GetActiveModifiers<HauntableModifier>().Any();
    }
}

public static class Jester_Events
{
    [RegisterEvent]
    public static void EjectionEvent(EjectionEvent @event)
    {
        NetworkedPlayerInfo exiled = @event.ExileController.initData.networkedPlayer;
        if (exiled != null)
        {
            PlayerControl player = exiled.Object;

            if (player.Data.Role is Jester jester)
            {
                if (jester.MessagesSent >= jester.ChatterBoxQuota)
                {
                    Jester.RpcQuotaLynch(jester.Player);
                    PlayerControl.LocalPlayer.Notify(Jester.Info(), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.JesterRoleCard.LoadAsset());

                    // Abstainers or voters of the Jester can be Haunted.
                    var validVoters = jester.Voters.Where(x => x != player.PlayerId).ToArray();
                    foreach (var vote in validVoters)
                    {
                        MiscUtils.PlayerById(vote).RpcAddModifier<HauntableModifier>(jester.Player);
                    }

                    if (jester.Player.AmOwner())
                    {
                        CustomButtonSingleton<Jester_Haunt>.Instance.SetActive(true, jester);
                        CustomButtonSingleton<Jester_Haunt>.Instance.Show = true;
                    }
                }
            }
        }
    }

    [RegisterEvent]
    public static void StartMeetingEvent(StartMeetingEvent @event)
    {
        var jestButton = CustomButtonSingleton<Jester_Haunt>.Instance;
        jestButton.Show = false;
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        foreach (var jester in MiscUtils.GetRoles<Jester>())
        {
            if (!jester.Lynched())
            {
                jester.Voters.Clear();
            }
        }
    }

    [RegisterEvent]
    public static void HandleVoteEventHandler(HandleVoteEvent @event)
    {
        var votingPlayer = @event.Player;
        var suspectPlayer = @event.TargetPlayerInfo;

        if (@event.TargetId == MeetingHud.Instance.SkipVoteButton.TargetPlayerId)
        {
            foreach (var jester in MiscUtils.GetRoles<Jester>()) jester.Voters.Add(votingPlayer.PlayerId);
            return;
        }

        if (suspectPlayer?.Role is not Jester targetJester)
        {
            return;
        }

        targetJester.Voters.Add(votingPlayer.PlayerId);
    }
}

public sealed class Jester_Options : AbstractOptionGroup<Jester>
{
    public override string GroupName => "Jester";

    [ModdedToggleOption("Enable Jester Chatterbox")]
    public bool EnableChatterbox { get; set; } = true;
}