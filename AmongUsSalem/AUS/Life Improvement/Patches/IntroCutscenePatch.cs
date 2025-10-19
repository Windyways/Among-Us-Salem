using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using AmongUsSalem.Modules;
using AmongUs.GameOptions;

namespace AmongUsSalem.LifeImprovement;

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
            if (PlayerControl.LocalPlayer.HasModifier<JackalRecruit>())
            {
                __instance.__4__this.TeamTitle.text = $"{AUSColors.GradientColorText("404040", "b8b8b8", "Recruit")}";
                __instance.__4__this.TeamTitle.color = AUSColors.Neutral;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
            }
            else if (ausRole.Faction == Faction.Coven)
            {
                __instance.__4__this.TeamTitle.text = "Coven";
                __instance.__4__this.TeamTitle.color = AUSColors.Coven;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
            }
            else if (ausRole.Faction == Faction.Traitor)
            {
                __instance.__4__this.TeamTitle.text = "Traitor";
                __instance.__4__this.TeamTitle.color = AUSColors.Traitor;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = GetIntroSound(RoleTypes.Shapeshifter);
            }
            else if (ausRole.Alignment == Alignment.NeutralApocalypse)
            {
                __instance.__4__this.TeamTitle.text = "Apocalypse";
                __instance.__4__this.TeamTitle.color = AUSColors.Apocalypse;
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
            if (PlayerControl.LocalPlayer.HasModifier<JackalRecruit>())
            {
                __instance.__4__this.TeamTitle.text = $"{AUSColors.GradientColorText("404040", "b8b8b8", "Recruit")}";
                __instance.__4__this.TeamTitle.color = AUSColors.Neutral;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = IntroCutscene_ShowTeam__d_MoveNext.GetIntroSound(RoleTypes.Shapeshifter);
            }
            else if (ausRole.Faction == Faction.Coven)
            {
                __instance.__4__this.TeamTitle.text = "Coven";
                __instance.__4__this.TeamTitle.color = AUSColors.Coven;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = IntroCutscene_ShowTeam__d_MoveNext.GetIntroSound(RoleTypes.Shapeshifter);
            }
            else if (ausRole.Faction == Faction.Traitor)
            {
                __instance.__4__this.TeamTitle.text = "Traitor";
                __instance.__4__this.TeamTitle.color = AUSColors.Traitor;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = IntroCutscene_ShowTeam__d_MoveNext.GetIntroSound(RoleTypes.Shapeshifter);
            }
            else if (ausRole.Alignment == Alignment.NeutralApocalypse)
            {
                __instance.__4__this.TeamTitle.text = "Apocalypse";
                __instance.__4__this.TeamTitle.color = AUSColors.Apocalypse;
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
            if (PlayerControl.LocalPlayer.HasModifier<JackalRecruit>())
            {
                __instance.__4__this.TeamTitle.text = $"{AUSColors.GradientColorText("404040", "b8b8b8", "Recruit")}";
                __instance.__4__this.TeamTitle.color = AUSColors.Neutral;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = IntroCutscene_ShowTeam__d_MoveNext.GetIntroSound(RoleTypes.Shapeshifter);
            }
            else if (ausRole.Faction == Faction.Coven)
            {
                __instance.__4__this.TeamTitle.text = "Coven";
                __instance.__4__this.TeamTitle.color = AUSColors.Coven;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = IntroCutscene_ShowTeam__d_MoveNext.GetIntroSound(RoleTypes.Shapeshifter);
            }
            else if (ausRole.Faction == Faction.Traitor)
            {
                __instance.__4__this.TeamTitle.text = "Traitor";
                __instance.__4__this.TeamTitle.color = AUSColors.Traitor;
                PlayerControl.LocalPlayer.Data.Role.IntroSound = IntroCutscene_ShowTeam__d_MoveNext.GetIntroSound(RoleTypes.Shapeshifter);
            }
            else if (ausRole.Alignment == Alignment.NeutralApocalypse)
            {
                __instance.__4__this.TeamTitle.text = "Apocalypse";
                __instance.__4__this.TeamTitle.color = AUSColors.Apocalypse;
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