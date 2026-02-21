namespace AmongUsSalem.CovenRoles;

public class Necronomicon : BaseModifier
{
    public override string ModifierName => "Necronomicon";
    public override bool HideOnUi => false;
    public override string GetDescription()
    {
        return "You obtain the Necronomicon, dealing a Basic Attack to your targets.";
    }

    public override void OnActivate()
    {
        // Ensures there's only always 1 Necronomicon.
        foreach (var modifier in ModifierUtils.GetActiveModifiers<Necronomicon>())
        {
            if (modifier != this) modifier.Player.RemoveModifier(modifier);
        }

        RpcNotifyCoven(Player);
    }

    [MethodRpc((uint)AUSRpc.RpcNotifyCoven)]
    public static void RpcNotifyCoven(PlayerControl bookHolder)
    {
        if (PlayerControl.LocalPlayer.Is(Faction.Coven))
        {
            PlayerControl.LocalPlayer.Notify(Info(bookHolder), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.NecronomiconIcon.LoadAsset());
        }
    }

    public static string Info(PlayerControl player)
    {
        return $"{player.Name()} possesses the Necronomicon. Their powers are enhanced!";
    }
}