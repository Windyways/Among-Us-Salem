using HarmonyLib;
using Rewired;
using Rewired.Data;
using TownOfUs.Utilities;

namespace TownOfUs.Patches;

// original patch taken from TheOtherRolesAU/TheOtherRoles/pull/347 by dadoum
[HarmonyPatch(typeof(InputManager_Base), nameof(InputManager_Base.Awake))]
public static class Keybinds
{
    [HarmonyPrefix]
    private static void Prefix(InputManager_Base __instance)
    {
        // change the text shown on the screen for the keybinds menu
        try
        {
            __instance.userData.GetAction("ActionSecondary").descriptiveName = "Kill / Secondary Ability";
            __instance.userData.GetAction("ActionQuaternary").descriptiveName = "Primary Ability";
            __instance.userData.RegisterBind("tou.ActionCustom", "Tertiary Ability (Hack Ability)");
            __instance.userData.RegisterBind("tou.ActionCustom2", "Modifier Ability");
        }
        catch
        {
            // Logger<AUSPlugin>.Error($"Error applying names for custom keybinds: {e}");
        }
    }

    private static int RegisterBind(this UserData self, string name, string description, int elementIdentifierId = -1,
        int category = 0, InputActionType type = InputActionType.Button)
    {
        self.AddAction(category);
        var action = self.GetAction(self.actions.Count - 1)!;

        action.name = name;
        action.descriptiveName = description;
        action.categoryId = category;
        action.type = type;
        action.userAssignable = true;

        var map = new ActionElementMap
        {
            _elementIdentifierId = elementIdentifierId,
            _actionId = action.id,
            _elementType = ControllerElementType.Button,
            _axisContribution = Pole.Positive,
            _modifierKey1 = ModifierKey.None,
            _modifierKey2 = ModifierKey.None,
            _modifierKey3 = ModifierKey.None
        };
        self.keyboardMaps[0].actionElementMaps.Add(map);
        self.joystickMaps[0].actionElementMaps.Add(map);

        return action.id;
    }
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class KillVentBinds
{
    public static void Postfix(HudManager __instance)
    {
        if (PlayerControl.LocalPlayer == null)
        {
            return;
        }

        if (PlayerControl.LocalPlayer.Data == null)
        {
            return;
        }

        if (PlayerControl.LocalPlayer.Data.IsDead)
        {
            return;
        }

        if (PlayerControl.LocalPlayer.IsImpostor())
        {
            return;
        }

        // for neutrals

        var button = __instance.KillButton;
        var vent = __instance.ImpostorVentButton;

        if (button.isActiveAndEnabled)
        {
            var killKey = ReInput.players.GetPlayer(0).GetButtonDown("ActionSecondary");
            var controllerKill = ConsoleJoystick.player.GetButtonDown(8);
            if (killKey || controllerKill)
            {
                button.DoClick();
            }
        }

        if (vent.isActiveAndEnabled)
        {
            var ventKey = ReInput.players.GetPlayer(0).GetButtonDown("UseVent");
            var controllerVent = ConsoleJoystick.player.GetButtonDown(50);
            if (ventKey || controllerVent)
            {
                vent.DoClick();
            }
        }
    }
}