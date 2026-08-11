using System.Collections;
using UnityEngine;

namespace AmongUsSalem.Modifiers;

public sealed class HauntableModifier(PlayerControl c) : BaseModifier
{
    public override string ModifierName => "Hauntable";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public PlayerControl Caster => c;
    public override void OnMeetingStart()
    {
        Coroutines.Start(DelayRemoveHaunt());
    }

    private IEnumerator DelayRemoveHaunt()
    {
        yield return new WaitForSeconds(3f);
        Player.RpcRemoveModifier<HauntableModifier>();
    }
}