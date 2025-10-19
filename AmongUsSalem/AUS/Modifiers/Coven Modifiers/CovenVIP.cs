using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using AmongUsSalem.Modules;
using AmongUsSalem.Options.Modifiers;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Modifiers;

public sealed class CovenVIP : TouGameModifier,
    IWikiDiscoverable
{
    public override string ModifierName => "Coven VIP";
    public override LoadableAsset<Sprite>? ModifierIcon => AUSAssets.CovenVIPIcon;

    public override ModifierFaction FactionType => ModifierFaction.Coven;
    public override Color FreeplayFileColor => AUSColors.Coven;


    public string GetAdvancedDescription()
    {
        return "You are started with Invincible Defense. All your Coven members know that you're Coven VIP. If you die, all your Coven members will also die.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "Invincible Defense, just don't get lynched.";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<CovenVIP_Options>.Instance.Chance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<CovenVIP_Options>.Instance.Maximum;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.Player.Is(Faction.Coven);
    }

    public override void OnActivate()
    {
        if (Player.Data.Role is ICustomAURole ausRole)
        {
            ausRole.ApplyDefense(Defense.Invincible, true);
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.Data.Role is ICovenRole && !player.HasDied())
            {
                MiscUtils.RpcApplyDeathReason(player, player, DeathReasonShow.KilledByTheCovenVIP);
            }
        }
    }
}


#region CovenVIP_Options
#endregion
public sealed class CovenVIP_Options : AbstractOptionGroup<CovenVIP>
{
    public override string GroupName => "Coven VIP";

    [ModdedNumberOption("<color=#B545FF>Coven VIP</color> Chance", 0f, 100f, 10f, MiraNumberSuffixes.Percent)]
    public float Chance { get; set; } = 0f;

    [ModdedNumberOption("<color=#B545FF>Coven VIP</color> Maximum", 1f, 15f, 1f)]
    public float Maximum { get; set; } = 1f;
}