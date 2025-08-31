namespace AmongUsSalem.Roles;

public interface ITouCrewRole : ITOURole
{
    bool IsPowerCrew { get; }
}

public interface IContinueGame
{
    bool continueGame { get; }
}