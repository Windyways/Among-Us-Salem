using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using Reactor.Utilities.Extensions;

namespace AmongUsSalem.MCI;

public class SeenKill : BaseModifier
{
    public override string ModifierName => "Seen Kill";
    public override bool HideOnUi => !Debugger.IsDebuggerActive;
    public override string GetDescription()
    {
        return $"You have seen {killer.Name()} kill!";
    }

    public PlayerControl killer; // The killer that the witness saw.
    public int voteChance = 50; // The chance of others voting the killer. If this fails, the witness will get the vote.
    public override void OnActivate()
    {
        if (Player.HasModifier<ConfirmedEvil>()) voteChance = -100;
        else if (Player.HasModifier<IncriminatingEvidence>()) voteChance -= 40;
        else if (Player.HasModifier<TI>()) voteChance += 20;
    }

    public static SeenKill GetAll()
    {
        var modifiers = new List<SeenKill>();
        foreach (var modifier in ModifierUtils.GetActiveModifiers<SeenKill>(x => !x.Player.HasDied() &&
            !x.killer.HasDied())) modifiers.Add(modifier);

        if (modifiers.Count == 0) return null;
        return modifiers.Random();
    }

    public static int GetSuspicion(PlayerControl p)
    {
        int suspicion = 0;

        if (p.HasModifier<ConfirmedEvil>()) suspicion += 200;
        if (p.HasModifier<IncriminatingEvidence>()) suspicion += 100;
        if (p.HasModifier<SeenKill>()) suspicion += 50;

        if (p.HasModifier<TI>()) suspicion -= 40;
        if (p.HasModifier<SoftCleared>()) suspicion -= 60;
        if (p.HasModifier<Confirmed>()) suspicion -= 100;

        if (p.Is(Faction.Town) && p.HasModifier<GlobalReveal>()) suspicion -= 200;

        return suspicion;
    }
}

public static class SeenKillEvents
{
    [RegisterEvent]
    public static void ReportBodyEvent(ReportBodyEvent @event)
    {
        if (@event.Reporter.TryGetModifier<SeenKill>(out var modifier)) modifier.voteChance += 20;
    }

    [RegisterEvent]
    public static void EjectionEvent(EjectionEvent @event)
    {
        NetworkedPlayerInfo exiled = @event.ExileController.initData.networkedPlayer;
        if (exiled != null)
        {
            PlayerControl player = exiled.Object;
            foreach (var modifier in ModifierUtils.GetActiveModifiers<SeenKill>())
            {
                if (modifier.killer == player) modifier.Player.RemoveModifier(modifier);
            }

            if (player.TryGetModifier<SeenKill>(out var seenKill) && player.Is(Faction.Town)) 
                seenKill.killer.AddModifier<ConfirmedEvil>();
        }
    }

    [RegisterEvent]
    public static void AfterMurderEvent(AfterMurderEvent @event)
    {
        var target = @event.Target;
        foreach (var modifier in ModifierUtils.GetActiveModifiers<SeenKill>())
        {
            if (modifier.killer == target) modifier.Player.RemoveModifier(modifier);
        }
    }
}