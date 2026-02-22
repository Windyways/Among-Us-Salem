using Il2CppInterop.Runtime.Attributes;
using Reactor.Utilities.Extensions;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Interfaces;

public interface ICustomAURole : ICustomRole
{
    string RoleName { get; set; }
    Color RoleColor { get; set; }
    string revealText { get; }

    Faction Faction { get; set; }
    Alignment Alignment { get; }
    
    Attack Attack { get; set; }
    Defense Defense { get; set; }
    EtherealDefense EtherealDefense { get; set; }

    Attack ogAttack { get; set; }
    Defense ogDefense { get; set; }
    EtherealDefense ogEtherealDefense { get; set; }

    void ApplyDefense(Defense defense, bool perma = false, bool overrideValue = false)
    {
        if (overrideValue)
        {
            Defense = defense;
            if (perma) ogDefense = defense;
        }
        else
        {
            if (Defense < defense) Defense = defense;
            if (perma && ogDefense < defense) ogDefense = defense;
        }
    }


    public virtual bool MetWinCon => false;
    public virtual string YouAreText
    {
        get
        {
            var prefix = " a";
            if (RoleName.StartsWithVowel()) prefix = " an";
            if (Configuration.MaxRoleCount is 0 or 1) prefix = " the";
            if (RoleName.StartsWith("the", StringComparison.OrdinalIgnoreCase)) prefix = "";
            return $"You are{prefix}";
        }
    }

    RoleOptionsGroup ICustomRole.RoleOptionsGroup
    {
        get
        {
            if (Alignment == Alignment.TownInvestigative) return TouRoleGroups.TI;
            if (Alignment == Alignment.TownExecutive) return TouRoleGroups.TE;
            if (Alignment == Alignment.TownGovernment) return TouRoleGroups.TG;
            if (Alignment == Alignment.TownKilling) return TouRoleGroups.TK;
            if (Alignment == Alignment.TownOutlier) return TouRoleGroups.TO;
            if (Alignment == Alignment.TownProtective) return TouRoleGroups.TP;
            if (Alignment == Alignment.TownSupport) return TouRoleGroups.TS;

            if (Alignment == Alignment.NeutralApocalypse) return TouRoleGroups.NA;
            if (Alignment == Alignment.NeutralBenign) return TouRoleGroups.NB;
            if (Alignment == Alignment.NeutralChaos) return TouRoleGroups.NC;
            if (Alignment == Alignment.NeutralEvil) return TouRoleGroups.NE;
            if (Alignment == Alignment.NeutralKilling) return TouRoleGroups.NK;
            if (Alignment == Alignment.NeutralOutlier) return TouRoleGroups.NO;
            if (Alignment == Alignment.NeutralPariah) return TouRoleGroups.NP;

            if (Alignment == Alignment.MafiaDeception) return TouRoleGroups.MD;
            if (Alignment == Alignment.MafiaKilling) return TouRoleGroups.MK;
            if (Alignment == Alignment.MafiaSupport) return TouRoleGroups.MS;

            if (Alignment == Alignment.CovenDeception) return TouRoleGroups.CD;
            // if (Alignment == Alignment.TownInvestigative) return TouRoleGroups.CE;
            if (Alignment == Alignment.CovenKilling) return TouRoleGroups.CK;
            if (Alignment == Alignment.CovenOutlier) return TouRoleGroups.CO;
            if (Alignment == Alignment.CovenPower) return TouRoleGroups.CPow;
            if (Alignment == Alignment.CovenUtility) return TouRoleGroups.CU;

            return Team switch
            {
                ModdedRoleTeams.Crewmate => TouRoleGroups.TS,
                ModdedRoleTeams.Impostor => TouRoleGroups.MS,
                _ => TouRoleGroups.NB
            };
        }
    }

    bool WinConditionMet()
    {
        return false;
    }

    /// <summary>
    ///     LobbyStart - Called for each role when a lobby begins.
    /// </summary>
    void LobbyStart()
    {
    }

    public static StringBuilder SetNewTabText(ICustomRole role)
    {
        var alignment = role is ICustomAURole customRole
            ? customRole.Alignment.ToDisplayString().ApplyKeywords()
            : "Custom";

        var prefix = " a";
        if (role.RoleName.StartsWithVowel()) prefix = " an";
        if (role.Configuration.MaxRoleCount is 0 or 1) prefix = " the";
        if (role.RoleName.StartsWith("the", StringComparison.OrdinalIgnoreCase)) prefix = "";

        var stringB = new StringBuilder();
        stringB.AppendLine(CultureInfo.InvariantCulture,
            $"{role.RoleColor.ToTextColor()}You are{prefix}<b> {role.RoleName}.</b></color>");
        stringB.AppendLine(CultureInfo.InvariantCulture, $"<size=60%>Alignment: <b>{alignment}</color></b></size>");
        stringB.Append("<size=70%>");
        stringB.AppendLine(CultureInfo.InvariantCulture, $"{role.RoleLongDescription}");

        if (role is ICustomAURole ausRole)
        {
            string defense = ausRole.Defense.ToString();
            if (PlayerControl.LocalPlayer.TryGetModifier<OverrideDefense>(out var od)) defense = od.defense.ToString();
            else if (PlayerControl.LocalPlayer.HasModifier<HideGainedDefense>()) defense = ausRole.ogDefense.ToString();

            stringB.AppendLine(CultureInfo.InvariantCulture, $"<color=#e70052>Attack: {ausRole.Attack}</color>");
            stringB.AppendLine(CultureInfo.InvariantCulture, $"<color=#0000ff>Defense: {defense}</color>");
            if (ausRole.ogEtherealDefense > EtherealDefense.None) stringB.AppendLine(CultureInfo.InvariantCulture, $"<color=#a1a1ff>Ethereal Defense: {ausRole.EtherealDefense}</color>");
        }

        return stringB;
    }

    public static StringBuilder SetDeadTabText(ICustomRole role)
    {
        var alignment = role is ICustomAURole customRole
            ? "<color=#" + RoleColors.Keyword.ToHtmlStringRGBA() + customRole.Alignment.ToDisplayString()
            : "Custom";

        if (alignment.Contains("Town")) alignment = alignment.Replace("Town", $"<color=#" + RoleColors.Town.ToHtmlStringRGBA() + ">Town</color>");
        if (alignment.Contains("Neutral")) alignment = alignment.Replace("Neutral", $"<color=#" + RoleColors.Neutral.ToHtmlStringRGBA() + ">Neutral</color>");
        if (alignment.Contains("Mafia")) alignment = alignment.Replace("Mafia", $"<color=#" + RoleColors.Mafia.ToHtmlStringRGBA() + ">Mafia</color>");
        if (alignment.Contains("Coven")) alignment = alignment.Replace("Coven", $"<color=#" + RoleColors.Coven.ToHtmlStringRGBA() + ">Coven</color>");

        var prefix = " a";
        if (role.RoleName.StartsWithVowel()) prefix = " an";
        if (role.Configuration.MaxRoleCount is 0 or 1) prefix = " the";
        if (role.RoleName.StartsWith("the", StringComparison.OrdinalIgnoreCase)) prefix = "";

        var stringB = new StringBuilder();
        stringB.AppendLine(CultureInfo.InvariantCulture,
            $"{role.RoleColor.ToTextColor()}You were{prefix}<b> {role.RoleName}.</b></color>");
        stringB.AppendLine(CultureInfo.InvariantCulture, $"<size=60%>Alignment: <b>{alignment}</color></b></size>");
        stringB.Append("<size=70%>");
        stringB.AppendLine(CultureInfo.InvariantCulture, $"{role.RoleLongDescription}");

        return stringB;
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return SetNewTabText(this);
    }
}