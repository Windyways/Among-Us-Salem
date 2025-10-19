using System.Globalization;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace AmongUsSalem.Roles;

public interface ICustomAURole : ICustomRole
{ 
    string RoleName { get; set; }
    Color RoleColor { get; set; }

    Faction Faction { get; set; }
    Alignment Alignment { get; }

    string revealText => "";
    float visionValue => GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;


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







    void OnDeath(DeathReason? reason)
    {
    }

    void OnTargetDeath(PlayerControl target, DeathReason? reason)
    {
    }

    public virtual bool MetWinCon => false;

    public virtual string YouAreText
    {
        get
        {
            var prefix = " a";
            if (RoleName.StartsWithVowel())
            {
                prefix = " an";
            }

            if (Configuration.MaxRoleCount is 0 or 1)
            {
                prefix = " the";
            }

            if (RoleName.StartsWith("the", StringComparison.OrdinalIgnoreCase))
            {
                prefix = "";
            }

            return $"You are{prefix}";
        }
    }

    RoleOptionsGroup ICustomRole.RoleOptionsGroup
    {
        get
        {
            if (Alignment == Alignment.TownInvestigative) return TouRoleGroups.TownInvestigative;
            if (Alignment == Alignment.TownKilling) return TouRoleGroups.TownKilling;
            if (Alignment == Alignment.TownOutlier) return TouRoleGroups.TownOutlier;
            if (Alignment == Alignment.TownPower) return TouRoleGroups.TownPower;
            if (Alignment == Alignment.TownProtective) return TouRoleGroups.TownProtective;
            if (Alignment == Alignment.TownSupport) return TouRoleGroups.TownSupport;
            if (Alignment == Alignment.TownUtility) return TouRoleGroups.TownUtility;

            if (Alignment == Alignment.NeutralApocalypse) return TouRoleGroups.NeutralApocalypse;
            if (Alignment == Alignment.NeutralBenign) return TouRoleGroups.NeutralBenign;
            if (Alignment == Alignment.NeutralChaos) return TouRoleGroups.NeutralChaos;
            if (Alignment == Alignment.NeutralEvil) return TouRoleGroups.NeutralEvil;
            if (Alignment == Alignment.NeutralKilling) return TouRoleGroups.NeutralKilling;
            if (Alignment == Alignment.NeutralOutlier) return TouRoleGroups.NeutralOutlier;
            if (Alignment == Alignment.NeutralPariah) return TouRoleGroups.NeutralPariah;

            if (Alignment == Alignment.MafiaDeception) return TouRoleGroups.MafiaDeception;
            if (Alignment == Alignment.MafiaKilling) return TouRoleGroups.MafiaKilling;
            if (Alignment == Alignment.MafiaSupport) return TouRoleGroups.MafiaSupport;

            if (Alignment == Alignment.CovenDeception) return TouRoleGroups.CovenDeception;
            if (Alignment == Alignment.CovenKilling) return TouRoleGroups.CovenKilling;
            if (Alignment == Alignment.CovenOutlier) return TouRoleGroups.CovenOutlier;
            if (Alignment == Alignment.CovenPower) return TouRoleGroups.CovenPower;
            if (Alignment == Alignment.CovenUtility) return TouRoleGroups.CovenUtility;

            if (Alignment == Alignment.TraitorDeceptive) return TouRoleGroups.TraitorDeceptive;
            if (Alignment == Alignment.TraitorPower) return TouRoleGroups.TraitorPower;
            if (Alignment == Alignment.TraitorUtility) return TouRoleGroups.TraitorUtility;

            return TouRoleGroups.None;
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
        var alignment = role is ICustomAURole touRole
            ? touRole.Alignment.ToDisplayString()
            : "Custom";

        if (alignment.Contains("Town"))
        {
            alignment = alignment.Replace("Town", "<color=#06E00C>Town");
        }
        else if (alignment.Contains("Mafia"))
        {
            alignment = alignment.Replace("Mafia", "<color=#DD0000>Mafia");
        }
        else if (alignment.Contains("Neutral"))
        {
            alignment = alignment.Replace("Neutral", "<color=#a9a9a9>Neutral");
        }
        else if (alignment.Contains("Coven"))
        {
            alignment = alignment.Replace("Coven", "<color=#B545FF>Coven");
        }
        else if (alignment.Contains("Traitor"))
        {
            alignment = alignment.Replace("Traitor", "<color=#ce36fa>Traitor");
        }

        var prefix = " a";
        if (role.RoleName.StartsWithVowel())
        {
            prefix = " an";
        }

        if (role.Configuration.MaxRoleCount is 0 or 1)
        {
            prefix = " the";
        }

        if (role.RoleName.StartsWith("the", StringComparison.OrdinalIgnoreCase))
        {
            prefix = "";
        }

        var stringB = new StringBuilder();
        stringB.AppendLine(CultureInfo.InvariantCulture,
            $"{role.RoleColor.ToTextColor()}You are{prefix}<b> {role.RoleName}.</b></color>");
        stringB.AppendLine(CultureInfo.InvariantCulture, $"<size=60%>Alignment: <b>{alignment}</color></b></size>");
        stringB.Append("<size=70%>");
        stringB.AppendLine(CultureInfo.InvariantCulture, $"{role.RoleLongDescription}");

        if (role is ICustomAURole ausRole)
        {
            stringB.AppendLine(CultureInfo.InvariantCulture, $"<color=#e70052>Attack: {ausRole.Attack}</color>");
            stringB.AppendLine(CultureInfo.InvariantCulture, $"<color=#0000ff>Defense: {ausRole.Defense}</color>");
            if (ausRole.ogEtherealDefense > EtherealDefense.None) stringB.AppendLine(CultureInfo.InvariantCulture, $"<color=#a1a1ff>Ethereal Defense: {ausRole.EtherealDefense}</color>");
        }

        return stringB;
    }

    public static StringBuilder SetDeadTabText(ICustomRole role)
    {
        var alignment = role is ICustomAURole touRole
            ? touRole.Alignment.ToDisplayString()
            : "Custom";

        if (alignment.Contains("Town"))
        {
            alignment = alignment.Replace("Town", "<color=#06E00C>Town");
        }
        else if (alignment.Contains("Mafia"))
        {
            alignment = alignment.Replace("Mafia", "<color=#DD0000>Mafia");
        }
        else if (alignment.Contains("Neutral"))
        {
            alignment = alignment.Replace("Neutral", "<color=#a9a9a9>Neutral");
        }
        else if (alignment.Contains("Coven"))
        {
            alignment = alignment.Replace("Coven", "<color=#B545FF>Coven");
        }
        else if (alignment.Contains("Traitor"))
        {
            alignment = alignment.Replace("Traitor", "<color=#ce36fa>Traitor");
        }

        var prefix = " a";
        if (role.RoleName.StartsWithVowel())
        {
            prefix = " an";
        }

        if (role.Configuration.MaxRoleCount is 0 or 1)
        {
            prefix = " the";
        }

        if (role.RoleName.StartsWith("the", StringComparison.OrdinalIgnoreCase))
        {
            prefix = "";
        }

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