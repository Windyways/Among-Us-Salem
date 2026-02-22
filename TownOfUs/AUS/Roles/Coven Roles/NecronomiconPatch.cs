using MiraAPI.Events.Vanilla.Meeting.Voting;

namespace AmongUsSalem.CovenRoles;

public static class NecronomiconPatch
{
    public static void GrantCovenNecroPassing()
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.Is(Faction.Coven) && OptionGroupSingleton<CovenOptions>.Instance.EnableNecroPassing)
                player.RpcAddModifier<NecroPassing>();
        }
    }

    [RegisterEvent]
    public static void VotingCompleteEvent(VotingCompleteEvent @event)
    {
        ApplyNecronomicon();
    }

    [RegisterEvent]
    public static void RoundStartEvent(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            ApplyNecronomicon();
        }
        else
        {
            foreach (var modifier in ModifierUtils.GetActiveModifiers<Necronomicon>())
            {
                modifier.RefreshAttack();
            }
        }
    }

    [RegisterEvent]
    public static void AfterMurderEvent(AfterMurderEvent @event)
    {
        if (@event.Target.HasModifier<Necronomicon>())
        {
            ApplyNecronomicon();
        }
    }

    public static void ApplyNecronomicon()
    {
        var coven = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Coven) && !x.HasDied())
            .OrderByDescending(x => x.IsRole<HexMaster>()) // Coven Leader
            //.ThenBy(x => x.IsRole<Covenite>()) // Conjurer
            //.ThenBy(x => x.IsRole<Covenite>()) // Medusa
            //.ThenBy(x => x.IsRole<Covenite>()) // Poisoner
            //.ThenBy(x => x.IsRole<Covenite>()) // Witch
            //.ThenBy(x => x.IsRole<Covenite>()) // Wildling
            //.ThenBy(x => x.IsRole<Covenite>()) // Dreamweaver
            //.ThenBy(x => x.IsRole<Covenite>()) // Enchanter
            //.ThenBy(x => x.IsRole<Covenite>()) // Voodoo Master
            //.ThenBy(x => x.IsRole<Covenite>()) // Necromancer
            //.ThenBy(x => x.IsRole<Covenite>()) // Potion Master
            .ThenByDescending(x => x.IsRole<HexMaster>())
            .ThenByDescending(x => x.IsRole<Illusionist>())
            //.ThenBy(x => x.IsRole<Covenite>()) // Ritualist
            //.ThenBy(x => x.IsRole<Covenite>()) // Jinx
            //.ThenBy(x => x.IsRole<Covenite>()) // Cultist
            .ThenByDescending(x => x.IsRole<Covenite>())
            .ThenByDescending(x => x.IsRole<Covenite>()) // Indocrinated
            .ToList();

        if (coven.Count > 0)
        {
            var player = coven.FirstOrDefault();
            player.RpcAddModifier<Necronomicon>();
        }
    }
}