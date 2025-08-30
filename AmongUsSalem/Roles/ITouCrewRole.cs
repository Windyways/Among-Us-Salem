namespace ObjectWorkshop.Roles;

public interface ITouCrewRole : IOWRole
{
    bool IsPowerCrew { get; }
}

public interface IContinueGame
{
    bool continueGame { get; }
}