using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using ObjectWorkshop.Options.Modifiers.Crewmate;

namespace ObjectWorkshop.Modifiers.Game.Crewmate;

public sealed class FrozenModifier : TimedModifier
{
    public override string ModifierName => "Frozen";
    public override bool HideOnUi => true;
    public override float Duration => OptionGroupSingleton<FrostyOptions>.Instance.ChillDuration;

    private float SpeedCache { get; set; }
    private DateTime ApplicationTime { get; set; }

    public override void OnDeath(DeathReason reason)
    {
        Player.RemoveModifier(this);
    }

    public override void OnActivate()
    {
        ApplicationTime = DateTime.UtcNow;
        SpeedCache = Player.MyPhysics.Speed;
        Player.MyPhysics.Speed *= OptionGroupSingleton<FrostyOptions>.Instance.ChillStartSpeed;
    }

    public override void OnDeactivate()
    {
        Player.MyPhysics.Speed = SpeedCache;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        var timeSpan = DateTime.UtcNow - ApplicationTime;
        var duration = Duration * 1000f;
        Player.MyPhysics.Speed = SpeedCache * 1 - (duration - (float)timeSpan.TotalMilliseconds) *
            (1 - OptionGroupSingleton<FrostyOptions>.Instance.ChillStartSpeed) / duration;
    }
}