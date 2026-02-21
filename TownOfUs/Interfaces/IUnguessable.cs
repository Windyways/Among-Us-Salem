namespace TownOfUs.Extensions;

public interface IUnguessable
{
    // basically, does the player die when the appearance role is guessed (so yes for traitor, no for pestilence)
    bool IsGuessable { get; }
    RoleBehaviour AppearAs { get; }
}