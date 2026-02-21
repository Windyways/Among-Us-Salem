using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using Reactor.Utilities.Extensions;

namespace AmongUsSalem.MCI;

public class ConfirmedEvil : BaseModifier
{
    public override string ModifierName => "ConfirmedEvil";
    public override bool HideOnUi => !Debugger.IsDebuggerActive;
    public override string GetDescription()
    {
        return "You are confirmed evil to the Town.";
    }

    public static ConfirmedEvil GetAll()
    {
        var modifiers = new List<ConfirmedEvil>();
        foreach (var modifier in ModifierUtils.GetActiveModifiers<ConfirmedEvil>(x => !x.Player.HasDied())) modifiers.Add(modifier);

        if (modifiers.Count == 0) return null;
        return modifiers.Random();
    }
}