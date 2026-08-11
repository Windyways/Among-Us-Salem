namespace AmongUsSalem.ApocalypseRoles;

public sealed class SoloApocModifier : BaseModifier
{
    public override string ModifierName => "Solo Apoc";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public override void OnActivate()
    {
        AttackDefenseMechanic.RpcApplyDefense(Player, Defense.Basic, true);
    }
}