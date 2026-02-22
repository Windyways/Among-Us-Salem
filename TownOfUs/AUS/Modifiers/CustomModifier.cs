using MiraAPI.PluginLoading;

namespace AmongUsSalem.Modifiers;

public abstract class CustomModifier : BaseModifier
{
    public PlayerControl Caster; // manually assigned.

    public virtual bool RemoveOnMeetingStart => false;
    public virtual bool OnlyOneActive => false;
    public virtual bool RemoveOnDeath => false;

    public override void OnActivate()
    {
        if (OnlyOneActive)
        {
            var player = ModifierUtils.GetPlayersWithModifier<CustomModifier>(x => x.Caster == Caster).FirstOrDefault();
            player?.RpcRemoveModifier<CustomModifier>();
        }
    }

    public override void OnMeetingStart()
    {
        if (RemoveOnMeetingStart) Player.RpcRemoveModifier<CustomModifier>();
    }

    public override void OnDeath(DeathReason reason)
    {
        if (RemoveOnDeath) Player.RpcRemoveModifier<CustomModifier>();
    }
}