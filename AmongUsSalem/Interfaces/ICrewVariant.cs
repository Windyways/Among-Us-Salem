namespace AmongUsSalem.Extensions;

public interface ICrewVariant
{
    // Determines the closest crewmate role an Imitator can pick
    RoleBehaviour CrewVariant { get; }
}