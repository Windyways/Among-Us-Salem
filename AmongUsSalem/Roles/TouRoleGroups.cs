using MiraAPI.Roles;
using UnityEngine;

namespace AmongUsSalem.Roles;

public static class TouRoleGroups
{
    public static RoleOptionsGroup TownInvestigative { get; } = new("Town Investigative Roles", AUSColors.Town);
    public static RoleOptionsGroup TownKilling { get; } = new("Town Killing Roles", AUSColors.Town);
    public static RoleOptionsGroup TownOutlier { get; } = new("Town Outlier Roles", AUSColors.Town);
    public static RoleOptionsGroup TownPower { get; } = new("Town Power Roles", AUSColors.Town);
    public static RoleOptionsGroup TownProtective { get; } = new("Town Protective Roles", AUSColors.Town);
    public static RoleOptionsGroup TownSupport { get; } = new("Town Support Roles", AUSColors.Town);
    public static RoleOptionsGroup TownUtility { get; } = new("Town Utility Roles", AUSColors.Town);
    
    public static RoleOptionsGroup NeutralApocalypse { get; } = new("Neutral Apocalypse Roles", AUSColors.Neutral);
    public static RoleOptionsGroup NeutralBenign { get; } = new("Neutral Benign Roles", AUSColors.Neutral);
    public static RoleOptionsGroup NeutralChaos { get; } = new("Neutral Chaos Roles", AUSColors.Neutral);
    public static RoleOptionsGroup NeutralEvil { get; } = new("Neutral Evil Roles", AUSColors.Neutral);
    public static RoleOptionsGroup NeutralKilling { get; } = new("Neutral Killing Roles", AUSColors.Neutral);
    public static RoleOptionsGroup NeutralOutlier { get; } = new("Neutral Outlier Roles", AUSColors.Neutral);
    public static RoleOptionsGroup NeutralPariah { get; } = new("Neutral Pariah Roles", AUSColors.Neutral);

    public static RoleOptionsGroup MafiaDeception { get; } = new("Mafia Deception Roles", AUSColors.Mafia);
    public static RoleOptionsGroup MafiaKilling { get; } = new("Mafia Killing Roles", AUSColors.Mafia);
    public static RoleOptionsGroup MafiaSupport { get; } = new("Mafia Support Roles", AUSColors.Mafia);
    
    public static RoleOptionsGroup CovenDeception { get; } = new("Coven Deception Roles", AUSColors.Coven);
    public static RoleOptionsGroup CovenKilling { get; } = new("Coven Killing Roles", AUSColors.Coven);
    public static RoleOptionsGroup CovenOutlier { get; } = new("Coven Outlier Roles", AUSColors.Coven);
    public static RoleOptionsGroup CovenPower { get; } = new("Coven Power Roles", AUSColors.Coven);
    public static RoleOptionsGroup CovenUtility { get; } = new("Coven Utility Roles", AUSColors.Coven);
    
    public static RoleOptionsGroup TraitorDeceptive { get; } = new("Traitor Deceptive Roles", AUSColors.Traitor);
    public static RoleOptionsGroup TraitorPower { get; } = new("Traitor Power Roles", AUSColors.Traitor);
    public static RoleOptionsGroup TraitorUtility { get; } = new("Traitor Utility Roles", AUSColors.Traitor);



    public static RoleOptionsGroup None { get; } = new("XFILLER", Color.white);
}