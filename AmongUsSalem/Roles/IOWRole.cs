using System.Globalization;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using ObjectWorkshop.Utilities;

namespace ObjectWorkshop.Roles;

public interface IOWRole : ICustomRole
{
    RoleAlignment RoleAlignment { get; }
    string revealText { get; }
    float visionValue => GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
    bool attractsMetal => false;

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
            if (RoleAlignment == RoleAlignment.CrewmateInvestigative) return TouRoleGroups.CrewmateInvestigative;
            if (RoleAlignment == RoleAlignment.CrewmateKilling) return TouRoleGroups.CrewmateKilling;
            if (RoleAlignment == RoleAlignment.CrewmateProtective) return TouRoleGroups.CrewmateProtective;
            if (RoleAlignment == RoleAlignment.CrewmateSupport) return TouRoleGroups.CrewmateSupport;
            
            if (RoleAlignment == RoleAlignment.NeutralAssociative) return TouRoleGroups.NeutralAssociative;
            if (RoleAlignment == RoleAlignment.NeutralEvil) return TouRoleGroups.NeutralEvil;
            if (RoleAlignment == RoleAlignment.NeutralPredator) return TouRoleGroups.NeutralPredator;
            
            if (RoleAlignment == RoleAlignment.InfiltratorDisruption) return TouRoleGroups.InfiltratorDisruption;
            if (RoleAlignment == RoleAlignment.InfiltratorEvacuative) return TouRoleGroups.InfiltratorEvacuative;
            if (RoleAlignment == RoleAlignment.InfiltratorGunsman) return TouRoleGroups.InfiltratorGunsman;
            if (RoleAlignment == RoleAlignment.InfiltratorSupport) return TouRoleGroups.InfiltratorSupport;

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
        var alignment = role is IOWRole touRole
            ? touRole.RoleAlignment.ToDisplayString()
            : "Custom";

        if (alignment.Contains("Crewmate"))
        {
            alignment = alignment.Replace("Crewmate", "<color=#68ACF4>Crewmate");
        }
        else if (alignment.Contains("Infiltrator"))
        {
            alignment = alignment.Replace("Infiltrator", "<color=#D63F42>Impostor");
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
        var alignment = role is IOWRole touRole
            ? touRole.RoleAlignment.ToDisplayString()
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

public enum RoleAlignment
{
    None,
    CrewmateInvestigative,
    CrewmateKilling,
    CrewmateProtective,
    CrewmateSupport,

    NeutralAssociative,
    NeutralEvil,
    NeutralPredator,

    InfiltratorDisruption,
    InfiltratorGunsman,
    InfiltratorEvacuative,
    InfiltratorSupport
}