namespace AmongUsSalem.Modifiers;

public sealed class HauntedModifier(PlayerControl c, bool random) : BaseModifier
{
    public override string ModifierName => "Haunted";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public bool IsRandom => random;
    public override void OnActivate()
    {
        var player = ModifierUtils.GetPlayersWithModifier<HauntedModifier>(x => x.Caster == Caster && x != this).FirstOrDefault();
        player?.RpcRemoveModifier<HauntedModifier>();

        if (IsRandom)
        {
            if (Caster.CanKill(Player))
            {
                Caster.RpcCustomMurder(Player);
                VisitingMechanic.RpcAddDeathReason(Player, (int)DeathReasonShow.HauntedByAJester);
            }

            Player.RpcRemoveModifier<HauntedModifier>();
        }
    }

    public override void OnMeetingStart()
    {
        if (IsRandom)
            return;

        AUSPlugin.DebugLogMessage("Haunted meeting started.");
        if (Caster.CanKill(Player))
        {
            AUSPlugin.DebugLogMessage("Jester could kill!");

            Caster.RpcCustomMurder(Player);
            VisitingMechanic.RpcAddDeathReason(Player, (int)DeathReasonShow.HauntedByAJester);
        }

        Player.RpcRemoveModifier<HauntedModifier>();
    }
}