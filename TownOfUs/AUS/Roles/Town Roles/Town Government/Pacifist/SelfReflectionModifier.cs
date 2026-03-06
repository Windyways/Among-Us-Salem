using AmongUs.GameOptions;

namespace AmongUsSalem.Modifiers;

public sealed class SelfReflectionModifier(ushort r) : BaseModifier
{
    public override string ModifierName => "Self Reflection";
    public override bool Unique => false;
    public override bool HideOnUi => true;

    public ushort roleUSHORT => r;
    public override void OnActivate()
    {
        var role = GetRole();

        var player = ModifierUtils.GetPlayersWithModifier<SelfReflectionModifier>(x => x.Player == Player && x != this).FirstOrDefault();
        player?.RpcRemoveModifier<SelfReflectionModifier>();

        if (Player.Data.Role is Pacifist pacifist)
            pacifist.reflectedRole = role;
    }

    public RoleBehaviour GetRole() => RoleManager.Instance.GetRole((RoleTypes)roleUSHORT);
    public ICustomAURole GetICustom()
    {
        if (GetRole() is ICustomAURole customRole) return customRole;
        return null;
    }
}