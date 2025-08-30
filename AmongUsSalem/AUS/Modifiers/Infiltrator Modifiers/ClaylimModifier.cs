using System.Collections;
using Sentry.Unity.NativeUtils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ObjectWorkshop.LifeImprovement.Modifiers;

public sealed class ClaylimModifier : TouGameModifier
{
    public override string ModifierName => TouLocale.Get(TouNames.Claylim, "Claylim");
    public override string IntroInfo => "";

    public override LoadableAsset<Sprite>? ModifierIcon => OWAssets.Placeholder;
    public override ModifierFaction FactionType => ModifierFaction.Infiltrator;
    public override Color FreeplayFileColor => new Color32(91, 83, 83, 255);

    public string GetAdvancedDescription()
    {
        return
            $"You may choose to change your role into a base Infiltrator that is calculated as a Neutral.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Rebirth",
            "You can Rebirth yourself during the round. You will replace your role with this modifier, being calculated as a Neutral instead of an Infiltrator, although you will have no abilities aside from Attack.",
            OWAssets.Placeholder)
    ];

    public override string GetDescription()
    {
        return "Rebirth into Claylim on command!";
    }

    public override int GetAssignmentChance()
    {
        // return (int)OptionGroupSingleton<Claylim_Options>.Instance.Chance;

        List<ushort> roleList = [(ushort)PlayerControl.LocalPlayer.Data.Role.Role];
        if (PlayerControl.LocalPlayer.Data.IsDead && !roleList.Contains((ushort)PlayerControl.LocalPlayer.GetRoleWhenAlive().Role))
        {
            roleList.Add((ushort)PlayerControl.LocalPlayer.GetRoleWhenAlive().Role);
        }

        var comparer = new RoleComparer(roleList);
        var roles = MiscUtils.AllRoles.OrderBy(x => x, comparer);

        foreach (var role in roles)
        {
            var customRole = role as ICustomRole;
            if (customRole != null)
            {
#pragma warning disable CS8629 // Nullable value type may be null.

                var chance = (int)customRole.GetChance();
#pragma warning restore CS8629 // Nullable value type may be null.

                return chance;
            }
        }

        return 0;
    }

    public override int GetAmountPerGame()
    {
        // return (int)OptionGroupSingleton<Claylim_Options>.Instance.Amount;

        List<ushort> roleList = [(ushort)PlayerControl.LocalPlayer.Data.Role.Role];
        if (PlayerControl.LocalPlayer.Data.IsDead && !roleList.Contains((ushort)PlayerControl.LocalPlayer.GetRoleWhenAlive().Role))
        {
            roleList.Add((ushort)PlayerControl.LocalPlayer.GetRoleWhenAlive().Role);
        }

        var comparer = new RoleComparer(roleList);
        var roles = MiscUtils.AllRoles.OrderBy(x => x, comparer);

        foreach (var role in roles)
        {
            var customRole = role as ICustomRole;
            if (customRole != null)
            {
#pragma warning disable CS8629 // Nullable value type may be null.

                var amount = (int)customRole.GetCount();
#pragma warning restore CS8629 // Nullable value type may be null.
                return amount;
            }
        }

        return 0;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsImpostor();

        /* &&
           !role.Player.GetModifierComponent().HasModifier<SatelliteModifier>(true) &&
           !role.Player.GetModifierComponent().HasModifier<ButtonBarryModifier>(true);
        */
    }
}

public sealed class Claylim_Rebirth : ObjectWorkshopButton
{
    public override string Name => "Rebirth";
    public override string Keybind => Keybinds.ModifierAction;
    public override Color TextOutlineColor => OWColors.Claylim;
    public override float Cooldown => 0.001f;
    public override int MaxUses => 1;
    public override ButtonLocation Location => ButtonLocation.BottomLeft;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Placeholder;

    public override bool Enabled(RoleBehaviour? role)
    {
        return PlayerControl.LocalPlayer != null &&
               PlayerControl.LocalPlayer.HasModifier<ClaylimModifier>() &&
               !PlayerControl.LocalPlayer.Data.IsDead;
    }

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);

        Button!.usesRemainingSprite.sprite = TouAssets.AbilityCounterPlayerSprite.LoadAsset();
    }

    protected override void OnClick()
    {
        if (PlayerControl.LocalPlayer.IsRole<Aimsman>())
        {
            var aimsman = PlayerControl.LocalPlayer.GetRole<Aimsman>();
            if (aimsman.isAiming)
            {
                var crosshair = Crosshair.GetObjectByPlayer(aimsman.Player);
                crosshair.DestroyGameObject();
                aimsman.isAiming = false;
            }
        }

        Sabotages.TLights.Call();
        PlayerControl.LocalPlayer.RpcChangeRole(RoleId.Get<Claylim>());
        PlayerControl.LocalPlayer.RpcRemoveModifier<ClaylimModifier>();
    }
}