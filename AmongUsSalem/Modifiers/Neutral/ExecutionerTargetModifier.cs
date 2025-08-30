using MiraAPI.Modifiers;
using ObjectWorkshop.Modifiers.Impostor;

namespace ObjectWorkshop.Modifiers.Neutral;

public sealed class ExecutionerTargetModifier(byte exeId) : PlayerTargetModifier(exeId)
{
    public override string ModifierName => "Executioner Target";
    public override void OnActivate()
    {
        base.OnActivate();
        if (Player.HasModifier<TraitorCacheModifier>())
        {
            Player.RemoveModifier<TraitorCacheModifier>();
        }
    }
}