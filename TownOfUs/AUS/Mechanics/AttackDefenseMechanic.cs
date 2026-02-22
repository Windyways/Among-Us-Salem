namespace AmongUsSalem.Mechanics;

public static class AttackDefenseMechanic
{
    [MethodRpc((uint)AUSRpc.RpcApplyDefense)]
    public static void RpcApplyDefense(PlayerControl player, Defense defense, bool perma = false, bool overrideValue = false)
    {
        if (player.Data.Role is ICustomAURole customRole) customRole.ApplyDefense(defense, perma, overrideValue);
    }
}