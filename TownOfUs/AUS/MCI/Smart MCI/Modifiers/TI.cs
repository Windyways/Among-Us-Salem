namespace AmongUsSalem.MCI;

public class TI : BaseModifier
{
    public override string ModifierName => "TI";
    public override bool HideOnUi => !Debugger.IsDebuggerActive;
    public override string GetDescription()
    {
        return $"You have gotten the Towns trust by getting info!";
    }

    public override void OnActivate()
    {
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        if (alivePlayers.Count <= 7)
            Player.GetModifiers<TI>().Do(x => Player.RemoveModifier(x));
    }

    public override void OnMeetingStart()
    {
        var alivePlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied()).ToList();
        if (alivePlayers.Count <= 7)
            Player.GetModifiers<TI>().Do(x => Player.RemoveModifier(x));
    }
}