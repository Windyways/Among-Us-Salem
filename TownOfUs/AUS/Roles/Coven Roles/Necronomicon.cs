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

        if (PlayerControl.LocalPlayer.Is(Faction.Coven))
        {
            PlayerControl.LocalPlayer.Notify(Info(Player), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.NecronomiconIcon.LoadAsset());
        }

        if (Player.Data.Role is ICustomAURole customRole)
        {
            customRole.Attack = Attack.Basic;

            if (Player.AmOwner())
            {
                if (Player.IsRole<HexMaster>())
                {
                    var button = CustomButtonSingleton<HexMaster_Hex>.Instance;
                    button.OverrideSprite(AUSAssets.Necronomicon.LoadAsset());
                    button.OverrideName("Attack & Hex");
                }
                else if (Player.IsRole<Illusionist>())
                {
                    var button = CustomButtonSingleton<Illusionist_Cast>.Instance;
                    button.OverrideSprite(AUSAssets.Necronomicon.LoadAsset());
                    button.OverrideName("Attack / Cast");
                }
                else if (Player.IsRole<Jinx>())
                {
                    var button = CustomButtonSingleton<Jinx_Jinx>.Instance;
                    button.OverrideSprite(AUSAssets.Necronomicon.LoadAsset());
                    button.OverrideName("Attack & Jinx");
                }
                else if (Player.IsRole<Wildling>())
                {
                    var button = CustomButtonSingleton<Wildling_Sense>.Instance;
                    button.OverrideSprite(AUSAssets.Necronomicon.LoadAsset());
                    button.OverrideName("Attack & Sense");
                }
            }
        }
    }

    public override void OnDeactivate()
    {
        if (Player.Data.Role is ICustomAURole customRole)
        {
            if (customRole.ogAttack == Attack.None)
            {
                customRole.Attack = Attack.None;
                customRole.ogAttack = Attack.None;
            }

            if (Player.AmOwner())
            {
                if (Player.IsRole<HexMaster>())
                {
                    var button = CustomButtonSingleton<HexMaster_Hex>.Instance;
                    button.OverrideSprite(AUSAssets.HexMaster_Hex.LoadAsset());
                    button.OverrideName("Hex");
                }
                else if (Player.IsRole<Illusionist>())
                {
                    var button = CustomButtonSingleton<Illusionist_Cast>.Instance;
                    button.OverrideSprite(AUSAssets.Illusionist_Cast.LoadAsset());
                    button.OverrideName("Cast");
                }
                else if (Player.IsRole<Jinx>())
                {
                    var button = CustomButtonSingleton<Jinx_Jinx>.Instance;
                    button.OverrideSprite(AUSAssets.Jinx_Jinx.LoadAsset());
                    button.OverrideName("Jinx");
                }
                else if (Player.IsRole<Wildling>())
                {
                    var button = CustomButtonSingleton<Wildling_Sense>.Instance;
                    button.OverrideSprite(AUSAssets.Wildling_Sense.LoadAsset());
                    button.OverrideName("Sense");
                }
            }
        }
    }

    public static string Info(PlayerControl player)
    {
        return $"{player.Name()} possesses the Necronomicon. Their powers are enhanced!";
    }

    public void RefreshAttack()
    {
        if (Player.Data.Role is ICustomAURole customRole)
        {
            customRole.Attack = Attack.Basic;
        }
    }
}