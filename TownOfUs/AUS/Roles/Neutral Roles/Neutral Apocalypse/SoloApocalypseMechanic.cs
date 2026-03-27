namespace AmongUsSalem.ApocalypseRoles;

public static class SoloApocalypseMechanic
{
    [RegisterEvent()]
    public static void RoundStartEvent(RoundStartEvent @event)
    {
        CheckForSoloApocalypse();
    }

    [RegisterEvent()]
    public static void AfterMurderEvent(AfterMurderEvent @event)
    {
        CheckForSoloApocalypse();
    }

    public static void CheckForSoloApocalypse()
    {
        var allApocalypse = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x.Is(Faction.Apocalypse)).ToList();

        if (allApocalypse.Count <= 1)
        {
            var player = allApocalypse.FirstOrDefault();
            if (player != null) player.RpcAddModifier<SoloApocModifier>();
        }
    }
}