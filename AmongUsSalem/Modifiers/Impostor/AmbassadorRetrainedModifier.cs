using AmongUs.GameOptions;
using MiraAPI.Modifiers;

namespace AmongUsSalem.Modifiers.Impostor;

public sealed class AmbassadorRetrainedModifier(ushort role) : BaseModifier
{
    public override string ModifierName => "Retrained";
    public override bool HideOnUi => true;
    public RoleBehaviour PreviousRole => RoleManager.Instance.GetRole((RoleTypes)role);
}