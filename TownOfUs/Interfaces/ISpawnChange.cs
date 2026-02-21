namespace TownOfUs.Extensions;

public interface ISpawnChange
{
    // If enabled, then the role will not be able to spawn at the start of the game, but will be able to spawn later. (Like Traitor)
    // If disabled, then the role will prevent other roles of the same faction from spawning (only applicable to impostor roles).
    bool NoSpawn { get; }
}