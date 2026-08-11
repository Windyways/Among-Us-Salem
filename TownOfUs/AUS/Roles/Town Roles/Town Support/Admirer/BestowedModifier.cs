using System.Collections;
using UnityEngine;

namespace AmongUsSalem.Modifiers;

public sealed class BestowedModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Bestowed";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;

    public bool applied;
    public void ApplyEffects()
    {
        if (applied)
            return;

        applied = true;

        Player.RpcAddModifier<RoleBlockImmune>(true);
        Player.RpcAddModifier<ControlImmune>(true);
        Player.RpcAddModifier<Astral>(true);
    }
}

public static class BestowedModifier_Events
{
    [RegisterEvent]
    public static void RoundStartEvent(RoundStartEvent @event)
    {
        foreach (var bestowed in ModifierUtils.GetActiveModifiers<BestowedModifier>())
            bestowed.ApplyEffects();
    }
}