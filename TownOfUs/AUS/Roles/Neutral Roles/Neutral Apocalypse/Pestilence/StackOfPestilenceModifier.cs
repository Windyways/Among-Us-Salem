using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Modifiers;

public sealed class StackOfPestilenceModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Stack Of Pestilence";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public int stack = 1;
    public PlayerControl Caster => c;
    public int PerformInteraction(PlayerControl visitor, PlayerControl target)
    {
        if (target.IsRole<Pestilence>() && !visitor.Is(Alignment.NeutralApocalypse))
        {
            if (visitor.TryGetModifier<StackOfPestilenceModifier>(out var vstackOP)) vstackOP.AddStack();
        }

        if (target.Is(Alignment.NeutralApocalypse))
            return 0;

        if (target.TryGetModifier<StackOfPestilenceModifier>(out var stackOP)) stackOP.AddStack();
        return 0; // Do not Block visit.
    }

    public void AddStack(int stacks = 1)
    {
        stack += stacks;
        if (stack >= 3 && !Caster.HasDied())
        {
            Caster.RpcCustomMurder(Player, teleportMurderer: false);
            VisitingMechanic.RpcAddDeathReason(Player, (int)DeathReasonShow.SuccumbedToAPestilence);
        }
    }
}