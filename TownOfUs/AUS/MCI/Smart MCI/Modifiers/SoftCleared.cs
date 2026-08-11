namespace AmongUsSalem.MCI;

public class SoftCleared : BaseModifier
{
    public override string ModifierName => "Soft Cleared";
    public override bool HideOnUi => !Debugger.IsDebuggerActive;
    public override string GetDescription()
    {
        return $"You have gotten the Towns trust for now!";
    }

    public override void OnMeetingStart()
    {
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        if (alivePlayers.Count <= 6)
            Player.GetModifiers<SoftCleared>().Do(x => Player.RemoveModifier(x));
    }
}