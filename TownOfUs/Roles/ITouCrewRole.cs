namespace TownOfUs.Roles;

public interface ITouCrewRole : ITownOfUsRole
{
    bool IsPowerCrew { get; }
}