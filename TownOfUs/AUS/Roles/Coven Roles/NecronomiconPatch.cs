namespace AmongUsSalem.CovenRoles;

public static class NecronomiconPatch
{
    public static void GrantCovenNecroPassing()
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.Is(Faction.Coven) && OptionGroupSingleton<CovenOptions>.Instance.EnableNecroPassing && !player.HasModifier<NecroPassing>())
                player.RpcAddModifier<NecroPassing>();
        }
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

    [RegisterEvent]
    public static void ReportBodyEvent(ReportBodyEvent @event)
    {
        SmartClientSwapping.SwapToCoven();
    }

    [RegisterEvent]
    public static void EjectionEvent(EjectionEvent @event)
    {
        NetworkedPlayerInfo exiled = @event.ExileController.initData.networkedPlayer;
        if (exiled != null)
        {
            PlayerControl player = exiled.Object;
            if (player.HasNecronomicon()) ApplyNecronomicon(player);
        }
    }

    public static void ApplyNecronomicon(PlayerControl exclude = null)
    {
        var coven = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Coven) && !x.HasDied() && x != exclude)
            .OrderByDescending(x => x.IsRole<Wildling>()) // Coven Leader
            //.ThenByDescending(x => x.IsRole<Covenite>()) // Conjurer
            //.ThenByDescending(x => x.IsRole<Covenite>()) // Medusa
            //.ThenBy(x => x.IsRole<Covenite>()) // Poisoner
            //.ThenBy(x => x.IsRole<Covenite>()) // Witch
            .ThenByDescending(x => x.IsRole<Wildling>()) // Wildling
            //.ThenBy(x => x.IsRole<Covenite>()) // Dreamweaver
            //.ThenBy(x => x.IsRole<Covenite>()) // Enchanter
            //.ThenBy(x => x.IsRole<Covenite>()) // Voodoo Master
            //.ThenBy(x => x.IsRole<Covenite>()) // Necromancer
            .ThenByDescending(x => x.IsRole<PotionMaster>())
            .ThenByDescending(x => x.IsRole<HexMaster>())
            .ThenByDescending(x => x.IsRole<Illusionist>())
            .ThenByDescending(x => x.IsRole<Ritualist>())
            .ThenByDescending(x => x.IsRole<Jinx>()) 
            //.ThenBy(x => x.IsRole<Covenite>()) // Cultist
            .ThenByDescending(x => x.IsRole<Covenite>())
            //.ThenByDescending(x => x.IsRole<Covenite>()) // Indocrinated
            .ThenBy(x => UnityEngine.Random.value)
            .ToList();

        if (coven.Count > 0)
        {
            var player = coven.FirstOrDefault();
            player.RpcAddModifier<Necronomicon>();
        }
    }
}