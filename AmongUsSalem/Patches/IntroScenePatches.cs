using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.ModifierDisplay;
using MiraAPI.Modifiers.Types;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;
using TMPro;
using ObjectWorkshop.Buttons;
using ObjectWorkshop.Modifiers.Game;
using ObjectWorkshop.Options;
using ObjectWorkshop.Roles;
using ObjectWorkshop.Utilities;
using UnityEngine;
using Object = Il2CppSystem.Object;

namespace ObjectWorkshop.Patches;

[HarmonyPatch]
public static class IntroScenePatches
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    [HarmonyPrefix]
    public static bool ImpostorBeginPatch(IntroCutscene __instance)
    {
        if ( /* OptionGroupSingleton<GeneralOptions>.Instance.ImpsKnowRoles &&  */
            !OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode)
        {
            return true;
        }

        __instance.TeamTitle.text =
            DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Impostor, Array.Empty<Object>());
        __instance.TeamTitle.color = OWColors.Infiltrator;

        var player = __instance.CreatePlayer(0, 1, PlayerControl.LocalPlayer.Data, true);
        __instance.ourCrewmate = player;

        return false;
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.OnDestroy))]
    [HarmonyPrefix]
    public static void IntroCutsceneOnDestroyPatch()
    {
        HudManager.Instance.SetHudActive(false);
        HudManager.Instance.SetHudActive(true);

        foreach (var button in CustomButtonManager.Buttons.Where(x => x.Enabled(PlayerControl.LocalPlayer.Data.Role)))
        {
            if (button is FakeVentButton)
            {
                continue;
            }

            button.SetTimer(OptionGroupSingleton<GeneralOptions>.Instance.GameStartCd);
        }

        if (PlayerControl.LocalPlayer.IsImpostor())
        {
            PlayerControl.LocalPlayer.SetKillTimer(OptionGroupSingleton<GeneralOptions>.Instance.GameStartCd);
        }

        var modsTab = ModifierDisplayComponent.Instance;
        if (modsTab != null && !modsTab.IsOpen && PlayerControl.LocalPlayer.GetModifiers<GameModifier>()
                .Any(x => !x.HideOnUi && x.GetDescription() != string.Empty))
        {
            modsTab.ToggleTab();
        }

        var panelThing = HudManager.Instance.TaskStuff.transform.FindChild("RolePanel");
        if (panelThing != null)
        {
            var panel = panelThing.gameObject.GetComponent<TaskPanelBehaviour>();
            var role = PlayerControl.LocalPlayer.Data.Role as ICustomRole;
            if (role == null)
            {
                return;
            }

            panel.open = true;

            var tabText = panel.tab.gameObject.GetComponentInChildren<TextMeshPro>();
            var ogPanel = HudManager.Instance.TaskStuff.transform.FindChild("TaskPanel").gameObject
                .GetComponent<TaskPanelBehaviour>();
            if (tabText.text != role.RoleName)
            {
                tabText.text = role.RoleName;
            }

            var y = ogPanel.taskText.textBounds.size.y + 1;
            panel.closedPosition = new Vector3(ogPanel.closedPosition.x, ogPanel.open ? y + 0.2f : 2f,
                ogPanel.closedPosition.z);
            panel.openPosition = new Vector3(ogPanel.openPosition.x, ogPanel.open ? y : 2f, ogPanel.openPosition.z);

            panel.SetTaskText(role.SetTabText().ToString());
        }
    }

    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Close))]
    [HarmonyPrefix]
    public static void SpawnInMinigameClosePatch()
    {
        IntroCutsceneOnDestroyPatch();
    }
}

public static class ModifierIntroPatch
{
    private static TextMeshPro ModifierText;

    public static void RunModChecks()
    {
        ModifierText.text = string.Empty;

        /*var option = OptionGroupSingleton<GeneralOptions>.Instance.ModifierReveal;
        var modifier = PlayerControl.LocalPlayer.GetModifiers<AllianceGameModifier>().FirstOrDefault();
        var uniModifier = PlayerControl.LocalPlayer.GetModifiers<UniversalGameModifier>().FirstOrDefault();

        if (modifier != null && option is ModReveal.Alliance)
        {
            ModifierText.text = $"<size={modifier.IntroSize}>{modifier.IntroInfo}</size>";

            ModifierText.color = MiscUtils.GetRoleColour(modifier.ModifierName.Replace(" ", string.Empty));
            if (modifier is IColoredModifier colorMod)
            {
                ModifierText.color = colorMod.ModifierColor;
            }
        }
        else if (uniModifier != null && option is ModReveal.Universal)
        {
            ModifierText.text = $"<size=4><color=#FFFFFF>Modifier: </color>{uniModifier.ModifierName}</size>";

            ModifierText.color = MiscUtils.GetRoleColour(uniModifier.ModifierName.Replace(" ", string.Empty));
            if (uniModifier is IColoredModifier colorMod)
            {
                ModifierText.color = colorMod.ModifierColor;
            }
        }
        else
        {
            ModifierText.text = string.Empty;
        }*/
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
    public static class IntroCutscene_BeginCrewmate
    {
        public static void Postfix(IntroCutscene __instance)
        {
            ModifierText =
                UnityEngine.Object.Instantiate(__instance.RoleText, __instance.RoleText.transform.parent, false);
            SetHiddenImpostors(__instance);
        }
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    public static class IntroCutscene_BeginImpostor
    {
        public static void Postfix(IntroCutscene __instance)
        {
            ModifierText =
                UnityEngine.Object.Instantiate(__instance.RoleText, __instance.RoleText.transform.parent, false);
        }
    }

    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__35), nameof(IntroCutscene._CoBegin_d__35.MoveNext))]
    public static class ShowModifierPatch_CoBegin
    {
        public static void Postfix(IntroCutscene._ShowRole_d__41 __instance)
        {
            HudManagerPatches.ResetZoom();
            if (PlayerControl.LocalPlayer.Data.Role is IOWRole custom)
            {
                __instance.__4__this.RoleText.text = custom.RoleName;
                if (__instance.__4__this.YouAreText.transform.TryGetComponent<TextTranslatorTMP>(out var tmp))
                {
                    tmp.defaultStr = custom.YouAreText;
                    tmp.TargetText = StringNames.None;
                    tmp.ResetText();
                }

                __instance.__4__this.RoleBlurbText.text = custom.RoleDescription;
            }

            if (ModifierText == null)
            {
                return;
            }

            RunModChecks();

            ModifierText.transform.position =
                __instance.__4__this.transform.position - new Vector3(0f, 1.6f, -10f);
            ModifierText.gameObject.SetActive(true);
            ModifierText.color.SetAlpha(0.8f);
        }
    }

    [HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__38), nameof(IntroCutscene._ShowTeam_d__38.MoveNext))]
    public static class ShowModifierPatch_MoveNext
    {
        public static void Postfix(IntroCutscene._ShowRole_d__41 __instance)
        {
            if (PlayerControl.LocalPlayer.Data.Role is IOWRole custom)
            {
                __instance.__4__this.RoleText.text = custom.RoleName;
                if (__instance.__4__this.YouAreText.transform.TryGetComponent<TextTranslatorTMP>(out var tmp))
                {
                    tmp.defaultStr = custom.YouAreText;
                    tmp.TargetText = StringNames.None;
                    tmp.ResetText();
                }

                __instance.__4__this.RoleBlurbText.text = custom.RoleDescription;
            }

            if (ModifierText == null)
            {
                return;
            }

            RunModChecks();

            ModifierText.transform.position =
                __instance.__4__this.transform.position - new Vector3(0f, 1.6f, -10f);
            ModifierText.gameObject.SetActive(true);
            ModifierText.color.SetAlpha(0.8f);
        }
    }

    [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__41), nameof(IntroCutscene._ShowRole_d__41.MoveNext))]
    [HarmonyPriority(Priority.Last)]
    public static class ShowModifierPatch_Role
    {
        public static void Postfix(IntroCutscene._ShowRole_d__41 __instance)
        {
            if (PlayerControl.LocalPlayer.Data.Role is IOWRole custom)
            {
                __instance.__4__this.RoleText.text = custom.RoleName;
                __instance.__4__this.YouAreText.text = custom.YouAreText;
                __instance.__4__this.RoleBlurbText.text = custom.RoleDescription;
            }

            var teamModifier = PlayerControl.LocalPlayer.GetModifiers<TouGameModifier>().FirstOrDefault();
            if (teamModifier != null && OptionGroupSingleton<GeneralOptions>.Instance.TeamModifierReveal)
            {
                var color = MiscUtils.GetRoleColour(teamModifier.ModifierName.Replace(" ", string.Empty));
                if (teamModifier is IColoredModifier colorMod)
                {
                    ModifierText.color = colorMod.ModifierColor;
                }

                __instance.__4__this.RoleBlurbText.text =
                    $"<size={teamModifier.IntroSize}>\n</size>{__instance.__4__this.RoleBlurbText.text}\n<size={teamModifier.IntroSize}><color=#{color.ToHtmlStringRGBA()}>{teamModifier.IntroInfo}</color></size>";
            }

            if (ModifierText == null)
            {
                return;
            }

            RunModChecks();

            ModifierText.transform.position =
                __instance.__4__this.transform.position - new Vector3(0f, 1.6f, -10f);
            ModifierText.gameObject.SetActive(true);
            ModifierText.color.SetAlpha(0.8f);
        }
    }

    public static void SetHiddenImpostors(IntroCutscene __instance)
    {
        var amount = Helpers.GetAlivePlayers().Count(x => x.IsImpostor());
        if (amount == 1) __instance.ImpostorText.text = $"There is {amount} Infiltrator among us.";
        else if (amount > 0) __instance.ImpostorText.text = $"There are {amount} Infiltrators among us.";

        /*__instance.ImpostorText.text = DestroyableSingleton<TranslationController>.Instance.GetString(amount == 1 ? StringNames.NumImpostorsS : StringNames.NumImpostorsP, amount);
        __instance.ImpostorText.text = __instance.ImpostorText.text.Replace("[FF1919FF]", "<color=#FF1919FF>");
        __instance.ImpostorText.text = __instance.ImpostorText.text.Replace("[]", "</color>");*/
        
        if (!OptionGroupSingleton<RoleOptions>.Instance.RoleListEnabled) return;

        var players = GameData.Instance.PlayerCount;

        if (players < 7)
        {
            return;
        }

        var list = OptionGroupSingleton<RoleOptions>.Instance;

        int maxSlots = players < 15 ? players : 15;

        List<RoleListOption> buckets = [];
        if (list.RoleListEnabled)
        {
            for (int i = 0; i < maxSlots; i++)
            {
                int slotValue = i switch
                {
                    0 => list.Slot1,
                    1 => list.Slot2,
                    2 => list.Slot3,
                    3 => list.Slot4,
                    4 => list.Slot5,
                    5 => list.Slot6,
                    6 => list.Slot7,
                    7 => list.Slot8,
                    8 => list.Slot9,
                    9 => list.Slot10,
                    10 => list.Slot11,
                    11 => list.Slot12,
                    12 => list.Slot13,
                    13 => list.Slot14,
                    14 => list.Slot15,
                    _ => -1
                };

                buckets.Add((RoleListOption)slotValue);
            }
        }

        if (!buckets.Any(x => x is RoleListOption.Any)) return;


        __instance.ImpostorText.text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.NumImpostorsP, 256);
        __instance.ImpostorText.text = __instance.ImpostorText.text.Replace("[FF1919FF]", "<color=#FF1919FF>");
        __instance.ImpostorText.text = __instance.ImpostorText.text.Replace("[]", "</color>");
        __instance.ImpostorText.text = __instance.ImpostorText.text.Replace("256", "???");
    }
}