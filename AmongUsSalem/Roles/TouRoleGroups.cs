using MiraAPI.Roles;
using UnityEngine;

namespace ObjectWorkshop.Roles;

public static class TouRoleGroups
{
    public static RoleOptionsGroup CrewmateInvestigative { get; } = new("Crewmate Investigative Roles", OWColors.Crewmate);
    public static RoleOptionsGroup CrewmateKilling { get; } = new("Crewmate Killing Roles", OWColors.Crewmate);
    public static RoleOptionsGroup CrewmateProtective { get; } = new("Crewmate Protective Roles", OWColors.Crewmate);
    public static RoleOptionsGroup CrewmateSupport { get; } = new("Crewmate Support Roles", OWColors.Crewmate);
    
    public static RoleOptionsGroup NeutralAssociative { get; } = new("Neutral Associative Roles", OWColors.Neutral);
    public static RoleOptionsGroup NeutralEvil { get; } = new("Neutral Evil Roles", OWColors.Neutral);
    public static RoleOptionsGroup NeutralPredator { get; } = new("Neutral Predator Roles", OWColors.Neutral);

    public static RoleOptionsGroup InfiltratorDisruption { get; } = new("Infiltrator Disruption Roles", OWColors.Infiltrator);
    public static RoleOptionsGroup InfiltratorEvacuative { get; } = new("Infiltrator Evacuative Roles", OWColors.Infiltrator);
    public static RoleOptionsGroup InfiltratorGunsman { get; } = new("Infiltrator Gunsman Roles", OWColors.Infiltrator);
    public static RoleOptionsGroup InfiltratorSupport { get; } = new("Infiltrator Support Roles", OWColors.Infiltrator);
    public static RoleOptionsGroup None { get; } = new("XFILLER", Color.white);
}