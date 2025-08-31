using System.Globalization;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using AmongUsSalem.Utilities;

namespace AmongUsSalem.Roles;

public interface IAUSRole : ICustomRole
{
    Faction RoleFaction => Faction.None;
    Alignment Alignment { get; }
    string revealText { get; }
    float visionValue => GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;

    
    Attack Attack => Attack.None;
    Defense Defense => Defense.None;
    EtherealDefense EtherealDefense => EtherealDefense.None;
    bool Necronomicon => false;





    void OnDeath(DeathReason? reason)
    {
    }

    void OnTargetDeath(PlayerControl target, DeathReason? reason)
    {
    }

    bool HasImpostorVision => false;
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
        var alignment = role is IAUSRole touRole
            ? touRole.Alignment.ToDisplayString()
            : "Custom";

        if (alignment.Contains("Crewmate"))
        {
            alignment = alignment.Replace("Crewmate", "<color=#68ACF4>Crewmate");
        }
        else if (alignment.Contains("Mafia"))
        {
            alignment = alignment.Replace("Mafia", "<color=#D63F42>Impostor");
        }
        else if (alignment.Contains("Neutral"))
        {
            alignment = alignment.Replace("Neutral", "<color=#8A8A8A>Neutral");
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

        return stringB;
    }

    public static StringBuilder SetDeadTabText(ICustomRole role)
    {
        var alignment = role is IAUSRole touRole
            ? touRole.Alignment.ToDisplayString()
            : "Custom";

        if (alignment.Contains("Crewmate"))
        {
            alignment = alignment.Replace("Crewmate", "<color=#68ACF4>Crewmate");
        }
        else if (alignment.Contains("Impostor"))
        {
            alignment = alignment.Replace("Impostor", "<color=#D63F42>Impostor");
        }
        else if (alignment.Contains("Neutral"))
        {
            alignment = alignment.Replace("Neutral", "<color=#8A8A8A>Neutral");
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

public enum Alignment
{
    None,

    NeutralAssociative,

    MafiaDisruption,
    MafiaGunsman,
    MafiaEvacuative,

    TownInvestigative,
    TownKilling,
    TownProtective,
    TownPower,
    TownOutlier,
    TownSupport,
    TownUtility,

    NeutralApocalypse,
    NeutralBenign,
    NeutralChaos,
    NeutralEvil,
    NeutralKilling,
    NeutralOutlier,
    NeutralPariah,

    MafiaDeception,
    MafiaKilling,
    MafiaSupport,

    CovenDeception,
    CovenKilling,
    CovenOutlier,
    CovenPower,
    CovenUtility,

    TraitorDeceptive,
    TraitorPower,
    TraitorUtility
}