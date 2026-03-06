using MiraAPI.Modifiers.ModifierDisplay;
using MiraAPI.Modifiers.Types;
using TMPro;
using TownOfUs.Modifiers.Game;
using TownOfUs.Options;
using TownOfUs.Patches;
using UnityEngine;

namespace AmongUsSalem.Patches;

[HarmonyPatch]
public static class IntroScenePatches
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
    [HarmonyPrefix]
    public static bool ImpostorBeginPatch(IntroCutscene __instance)
    {
        __instance.TeamTitle.text = "Mafia";
        __instance.TeamTitle.color = RoleColors.Mafia;

        var yourTeam = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Mafia)).ToList();
        GenerateYourTeam(__instance, yourTeam, true);

        return false;
    }

    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
    [HarmonyPrefix]
    public static bool BeginCrewmatePatch(IntroCutscene __instance)
    {
        if (PlayerControl.LocalPlayer.Is(Faction.Town))
        {
            __instance.TeamTitle.text = "Town";
            __instance.TeamTitle.color = RoleColors.Town;

            var yourTeam = PlayerControl.AllPlayerControls.ToArray().ToList();
            GenerateYourTeam(__instance, yourTeam);
        }
        else if (PlayerControl.LocalPlayer.Is(Faction.Coven))
        {
            __instance.TeamTitle.text = "Coven";
            __instance.TeamTitle.color = RoleColors.Coven;

            var yourTeam = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Coven)).ToList();
            GenerateYourTeam(__instance, yourTeam);
        }
        else if (PlayerControl.LocalPlayer.Is(Alignment.NeutralApocalypse))
        {
            __instance.TeamTitle.text = "Apocalypse";
            __instance.TeamTitle.color = RoleColors.Apocalypse;

            var yourTeam = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Alignment.NeutralApocalypse)).ToList();
            GenerateYourTeam(__instance, yourTeam);
        }
        else
        {
            var player = __instance.CreatePlayer(0, 1, PlayerControl.LocalPlayer.Data, false);
            __instance.ourCrewmate = player;
        }

        return false;
    }

    public static void GenerateYourTeam(IntroCutscene __instance, List<PlayerControl> yourTeam, bool mafia = false)
    {
        //var yourTeam = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Mafia)).ToList();
        for (int i = 0; i < yourTeam.Count; i++)
        {
            PlayerControl playerControl = yourTeam[i];
            if (playerControl)
            {
                NetworkedPlayerInfo data = playerControl.Data;
                if (!(data == null))
                {
                    PoolablePlayer poolablePlayer = __instance.CreatePlayer(i, 1, data, mafia);
                    if (i == 0 && data.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                    {
                        __instance.ourCrewmate = poolablePlayer;
                    }
                }
            }
        }
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
            if (PlayerControl.LocalPlayer.Data.Role is ICustomAURole custom)
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
            if (PlayerControl.LocalPlayer.Data.Role is ICustomAURole custom)
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
            if (PlayerControl.LocalPlayer.Data.Role is ICustomAURole custom)
            {
                __instance.__4__this.RoleText.text = custom.RoleName;
                __instance.__4__this.YouAreText.text = custom.YouAreText;
                __instance.__4__this.RoleBlurbText.text = custom.RoleDescription;
            }

            var teamModifier = PlayerControl.LocalPlayer.GetModifiers<TouGameModifier>().FirstOrDefault();
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
        var mafiaAmount = Helpers.GetAlivePlayers().Count(x => x.IsImpostor());
        if (mafiaAmount == 1) __instance.ImpostorText.text = $"There is {mafiaAmount} <color=#DD0000>Mafia</color> among us.";
        else if (mafiaAmount > 0) __instance.ImpostorText.text = $"There are {mafiaAmount} <color=#DD0000>Mafias</color> among us.";

        var covenAmount = Helpers.GetAlivePlayers().Count(x => x.Is(Faction.Coven));
        if (covenAmount == 1) __instance.ImpostorText.text += $"\nThere is {covenAmount} <color=#B545FF>Coven</color> among us.";
        else if (covenAmount > 0) __instance.ImpostorText.text += $"\nThere are {covenAmount} <color=#B545FF>Covens</color> among us.";

        var players = GameData.Instance.PlayerCount;

        if (players < 7)
        {
            return;
        }

        var list = OptionGroupSingleton<RoleOptions>.Instance;

        int maxSlots = players < 15 ? players : 15;

        List<RoleListOption> buckets = [];
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

        if (!buckets.Any(x => x is RoleListOption.Any)) return;

        __instance.ImpostorText.text = $"There is ??? <color=#DD0000>Mafias</color> among us.";
        __instance.ImpostorText.text += $"\nThere is ??? <color=#B545FF>Covens</color> among us.";
    }
}