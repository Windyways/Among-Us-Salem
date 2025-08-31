namespace AmongUsSalem.Roles;

public interface ITouCrewRole : IAUSRole
{
    bool IsPowerCrew { get; }
}

public interface IContinueGame
{
    bool continueGame { get; }
}