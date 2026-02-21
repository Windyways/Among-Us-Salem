using UnityEngine;
using AmongUs.GameOptions;

namespace AmongUsSalem.Patches;

[HarmonyPatch(typeof(IntroCutscene._ShowTeam_d__38), "MoveNext")]
public static class IntroCutscene_ShowTeam__d_MoveNext
{
    public static AudioClip GetIntroSound(RoleTypes roleType)
    {
        return (from role in DestroyableSingleton<RoleManager>.Instance.AllRoles.ToArray()
                where role.Role == roleType
                select role).FirstOrDefault<RoleBehaviour>().IntroSound;
    }

    public static void Postfix(IntroCutscene._ShowRole_d__41 __instance)
    {
        if (PlayerControl.LocalPlayer.Data.Role is ICustomAURole ausRole)
        {
            if (ausRole.Faction == Faction.Coven)
            {
                __instance.__4__this.TeamTitle.text = "Coven";
                __instance.__4__this.TeamTitle.color = RoleColors.Coven;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
            }
            else if (ausRole.Alignment == Alignment.NeutralApocalypse)
            {
                __instance.__4__this.TeamTitle.text = "Apocalypse";
                __instance.__4__this.TeamTitle.color = RoleColors.Apocalypse;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
            }

            __instance.__4__this.BackgroundBar.material.color = ausRole.RoleColor;
            __instance.__4__this.RoleText.text = ausRole.RoleName;
            __instance.__4__this.RoleText.color = ausRole.RoleColor;
            __instance.__4__this.YouAreText.color = ausRole.RoleColor;
            __instance.__4__this.RoleBlurbText.color = ausRole.RoleColor;
            __instance.__4__this.RoleBlurbText.text = "My";
        }
    }
}

[HarmonyPatch(typeof(IntroCutscene._ShowRole_d__41), "MoveNext")]
public static class IntroCutscene_ShowRole_d__24
{
    public static void Postfix(IntroCutscene._ShowRole_d__41 __instance)
    {
        if (PlayerControl.LocalPlayer.Data.Role is ICustomAURole ausRole)
        {
            if (ausRole.Faction == Faction.Coven)
            {
                __instance.__4__this.TeamTitle.text = "Coven";
                __instance.__4__this.TeamTitle.color = RoleColors.Coven;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = IntroCutscene_ShowTeam__d_MoveNext.GetIntroSound(RoleTypes.Shapeshifter);
            }
            else if (ausRole.Alignment == Alignment.NeutralApocalypse)
            {
                __instance.__4__this.TeamTitle.text = "Apocalypse";
                __instance.__4__this.TeamTitle.color = RoleColors.Apocalypse;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = IntroCutscene_ShowTeam__d_MoveNext.GetIntroSound(RoleTypes.Shapeshifter);
            }


            __instance.__4__this.BackgroundBar.material.color = ausRole.RoleColor;
            __instance.__4__this.RoleText.text = ausRole.RoleName;
            __instance.__4__this.RoleText.color = ausRole.RoleColor;
            __instance.__4__this.YouAreText.color = ausRole.RoleColor;
            __instance.__4__this.RoleBlurbText.color = ausRole.RoleColor;
            __instance.__4__this.RoleBlurbText.text = "My";
        }
    }
}

[HarmonyPatch(typeof(IntroCutscene._CoBegin_d__35), "MoveNext")]
public static class IntroCutscene_CoBegin_d__29
{
    public static void Postfix(IntroCutscene._CoBegin_d__35 __instance)
    {
        if (PlayerControl.LocalPlayer.Data.Role is ICustomAURole ausRole)
        {
            if (ausRole.Faction == Faction.Coven)
            {
                __instance.__4__this.TeamTitle.text = "Coven";
                __instance.__4__this.TeamTitle.color = RoleColors.Coven;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = IntroCutscene_ShowTeam__d_MoveNext.GetIntroSound(RoleTypes.Shapeshifter);
            }
            else if (ausRole.Alignment == Alignment.NeutralApocalypse)
            {
                __instance.__4__this.TeamTitle.text = "Apocalypse";
                __instance.__4__this.TeamTitle.color = RoleColors.Apocalypse;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = IntroCutscene_ShowTeam__d_MoveNext.GetIntroSound(RoleTypes.Shapeshifter);
            }


            __instance.__4__this.BackgroundBar.material.color = ausRole.RoleColor;
            __instance.__4__this.RoleText.text = ausRole.RoleName;
            __instance.__4__this.RoleText.color = ausRole.RoleColor;
            __instance.__4__this.YouAreText.color = ausRole.RoleColor;
            __instance.__4__this.RoleBlurbText.color = ausRole.RoleColor;
            __instance.__4__this.RoleBlurbText.text = "My";
        }
    }
}