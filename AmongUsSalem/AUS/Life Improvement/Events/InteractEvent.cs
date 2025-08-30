using UnityEngine;
using Random = UnityEngine.Random;

namespace ObjectWorkshop.LifeImprovement.Events;

public static class InteractEvent
{
    [RegisterEvent]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        if (!IsVisitSuccessful(source, target, true, !NotAnInteraction(source) && source != target))
        {
            @event.Cancel();
            ResetButtonTimer(source, null, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
        }
    }

    public static bool NotAnInteraction(PlayerControl source)
    {
        // Specter/Culverin once added!
        return source.IsRole<Aimsman>() || source.IsRole<Pyre>() || source.IsRole<Duelist>();
    }
    
    public static bool IsVisitSuccessful(PlayerControl player, PlayerControl target, bool attacking, bool isInteracting)
    {
        // Missing attacks always has the highest priority!
        foreach (var sandstorm in Sandstorm.AllSandstorms)
        {
            var num = Random.Range(0, 100);
            if (sandstorm != null && attacking && isInteracting && num <= OptionGroupSingleton<Oasis_Options>.Instance.MissChance) return Oasis.RpcOasisNotif(player, target, sandstorm.Owner, 2);
        }

        // Target is protected priorities.
        foreach (var sanctuary in Sanctuary.AllSanctuarys)
        {
            if (sanctuary.IsInRange(target) && attacking && isInteracting) return Oasis.RpcOasisNotif(player, target, sanctuary.Owner, 1);
        }
        return true;
    }

    private static void ResetButtonTimer(PlayerControl source, CustomActionButton<PlayerControl>? button = null, float cooldown = 0f)
    {
        button?.SetTimer(cooldown);

        // Reset impostor kill cooldown if they attack a shielded player
        if (!source.AmOwner || !source.IsImpostor())
        {
            return;
        }

        source.SetKillTimer(cooldown);
    }
}