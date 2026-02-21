namespace AmongUsSalem.CovenRoles;

public static class NecronomiconPatch
{
    [RegisterEvent]
    public static void RoundStartEvent(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            ApplyNecronomicon();
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
            .OrderBy(x => x.IsRole<Covenite>()) // Coven Leader
            .ThenBy(x => x.IsRole<Covenite>()) // Conjurer
            .ThenBy(x => x.IsRole<Covenite>()) // Medusa
            .ThenBy(x => x.IsRole<Covenite>()) // Poisoner
            .ThenBy(x => x.IsRole<Covenite>()) // Witch
            .ThenBy(x => x.IsRole<Covenite>()) // Wildling
            .ThenBy(x => x.IsRole<Covenite>()) // Dreamweaver
            .ThenBy(x => x.IsRole<Covenite>()) // Enchanter
            .ThenBy(x => x.IsRole<Covenite>()) // Voodoo Master
            .ThenBy(x => x.IsRole<Covenite>()) // Necromancer
            .ThenBy(x => x.IsRole<Covenite>()) // Potion Master
            .ThenBy(x => x.IsRole<Covenite>()) // Hex Master
            .ThenBy(x => x.IsRole<Covenite>()) // Illusionist
            .ThenBy(x => x.IsRole<Covenite>()) // Ritualist
            .ThenBy(x => x.IsRole<Covenite>()) // Jinx
            .ThenBy(x => x.IsRole<Covenite>()) // Cultist
            .ThenBy(x => x.IsRole<Covenite>())
            .ThenBy(x => x.IsRole<Covenite>()) // Indocrinated
            .ToList();

        if (coven.Count > 0)
        {
            var player = coven.FirstOrDefault();
            player.RpcAddModifier<Necronomicon>();
        }
    }
}