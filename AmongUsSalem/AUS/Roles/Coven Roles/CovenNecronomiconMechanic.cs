using UnityEngine;

namespace AmongUsSalem.LifeImprovement.GameMechanics;

public static class CovenNecronomiconMechanic
{
    public static List<ICovenRole> GetLivingCovenRoles()
    {
        return PlayerControl.AllPlayerControls.ToArray()
            .Where(x => !x.HasDied())
            .Select(x => x.Data.Role)
            .OfType<ICovenRole>() // only take roles that uses ICovenRole.
            .Where(coven => coven.NecronomiconPriority != NecronomiconPriority.None)
            .ToList();
    }

    [MethodRpc((uint)AUSRpc.AssignNecronomicon, SendImmediately = true)]
    public static void RpcAssignNecronomicon()
    {
        var covenRoles = GetLivingCovenRoles();
        if (covenRoles.Count == 0)
            return;

        // Order by enum order
        var nextHolder = covenRoles.OrderBy(r => r.NecronomiconPriority).FirstOrDefault();
        if (nextHolder != null)
        {
            nextHolder.Necronomicon = true;
            if (nextHolder.Player.Data.Role is ICustomAURole ausRole)
            {
                ausRole.Attack = Attack.Basic;
                ausRole.ogAttack = Attack.Basic;
                AdjustButtons(nextHolder.Player);
            }

            if (PlayerControl.LocalPlayer.Is(Faction.Coven) || nextHolder.Player.AmOwner())
            {
                MiscUtils.ShowNotification(Info(nextHolder.Player), Color.white, AUSAssets.Necronomicon.LoadAsset());
                MiscUtils.AddFakeChat(nextHolder.Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Coven, "Coven Info"), Info(nextHolder.Player));
            }
        }
    }

    public static void AdjustButtons(PlayerControl player)
    {
        if (player.AmOwner())
        {
            if (player.IsRole<HexMaster>())
            {
                var button = CustomButtonSingleton<HexMaster_Hex>.Instance;
                button.OverrideSprite(AUSAssets.NecronomiconButton.LoadAsset());
                button.OverrideName("Attack & Hex");
            }
            else if (player.IsRole<VoodooMaster>())
            {
                var button = CustomButtonSingleton<VoodooMaster_Voodoo>.Instance;
                button.OverrideSprite(AUSAssets.NecronomiconButton.LoadAsset());
                button.OverrideName("Attack & Voodoo");
            }
            else if (player.IsRole<Jinx>())
            {
                var button = CustomButtonSingleton<Jinx_Jinx>.Instance;
                button.OverrideSprite(AUSAssets.NecronomiconButton.LoadAsset());
                button.OverrideName("Attack & Jinx");
            }
            else if (player.IsRole<Enchanter>())
            {
                var button = CustomButtonSingleton<Enchanter_Enchant>.Instance;
                button.OverrideSprite(AUSAssets.NecronomiconButton.LoadAsset());
                button.OverrideName("Attack & Enchant");

                var button2 = CustomButtonSingleton<Enchanter_AlteratePlayer>.Instance;
                button2.OverrideSprite(AUSAssets.NecronomiconButton.LoadAsset());
                button2.OverrideName("Attack & Alterate");
            }
        }
    }

    public static string Info(PlayerControl target)
    {
        return target.GetDefaultAppearance().PlayerName + " possesses the Necronomicon. Their powers are enhanced!";
    }

    public static void OnRoleDeath(ICovenRole coven)
    {
        if (coven.Necronomicon)
        {
            coven.Necronomicon = false;
            RpcAssignNecronomicon(); // pass to next priority
        }
    }
    
    public static void ClearNecronomicon()
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.Data.Role is ICovenRole covenRole)
            {
                covenRole.Necronomicon = false;
                
                if (covenRole.Player.AmOwner())
                {
                    if (covenRole.Player.IsRole<HexMaster>())
                    {
                        var button = CustomButtonSingleton<HexMaster_Hex>.Instance;
                        button.OverrideSprite(AUSAssets.HexMaster_Hex.LoadAsset());
                        button.OverrideName("Hex");
                    }
                    else if (covenRole.Player.IsRole<VoodooMaster>())
                    {
                        var button = CustomButtonSingleton<VoodooMaster_Voodoo>.Instance;
                        button.OverrideSprite(AUSAssets.VoodooMaster_Voodoo.LoadAsset());
                        button.OverrideName("Voodoo");
                    }
                    else if (covenRole.Player.IsRole<Jinx>())
                    {
                        var button = CustomButtonSingleton<Jinx_Jinx>.Instance;
                        button.OverrideSprite(AUSAssets.Jinx_Jinx.LoadAsset());
                        button.OverrideName("Jinx");
                    }
                    else if (covenRole.Player.IsRole<Enchanter>())
                    {
                        var button = CustomButtonSingleton<Enchanter_Enchant>.Instance;
                        button.OverrideSprite(AUSAssets.Enchanter_Enchant.LoadAsset());
                        button.OverrideName("Enchant");

                        var button2 = CustomButtonSingleton<Enchanter_AlteratePlayer>.Instance;
                        button2.OverrideSprite(AUSAssets.Enchanter_Alterate.LoadAsset());
                        button2.OverrideName("Alterate");
                    }
                }
            }
        }
    }

    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (!@event.TriggeredByIntro)
        {
            return; // Only run when game starts.
        }

        RpcAssignNecronomicon();
    }
}

public enum NecronomiconPriority
{
    CovenLeader,
    Conjurer,
    Medusa,
    Poisoner,
    Witch,
    Wildling,
    Dreamweaver,
    Enchanter,
    VoodooMaster,
    Necromancer,
    PotionMaster,
    HexMaster,
    Illusionist,
    Ritualist,
    Jinx,
    Cultist,
    Covenite,
    Indocrinated,

    None
}