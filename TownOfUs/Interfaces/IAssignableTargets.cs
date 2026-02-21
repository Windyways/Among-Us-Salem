namespace TownOfUs.Extensions;

public interface IAssignableTargets
{
    // Used to assign targets to a player, like GA or Lovers
    // Called in the SelectRolesPatch in RoleManagerPatches.cs
    int Priority { get; set; }
    void AssignTargets();
}