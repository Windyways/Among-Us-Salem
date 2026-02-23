using Reactor.Utilities.Extensions;

namespace AmongUsSalem.MCI;

public class IncriminatingEvidence : BaseModifier
{
    public override string ModifierName => "Incriminating Evidence";
    public override bool HideOnUi => !Debugger.IsDebuggerActive;
    public override string GetDescription()
    {
        return $"You have Incriminating Evidence!";
    }

    public static IncriminatingEvidence GetRandom()
    {
        var modifiers = new List<IncriminatingEvidence>();
        foreach (var modifier in ModifierUtils.GetActiveModifiers<IncriminatingEvidence>(x => !x.Player.HasDied())) modifiers.Add(modifier);

        if (modifiers.Count == 0) return null;
        return modifiers.Random();
    }

    public static List<IncriminatingEvidence> GetAll()
    {
        var modifiers = new List<IncriminatingEvidence>();
        foreach (var modifier in ModifierUtils.GetActiveModifiers<IncriminatingEvidence>(x => !x.Player.HasDied())) modifiers.Add(modifier);

        if (modifiers.Count == 0) return null;
        return modifiers;
    }
}