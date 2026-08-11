namespace TownOfUs.Roles;

public interface IGhostRole
{
    bool Setup { get; set; }
    bool Caught { get; set; }
    bool Faded { get; set; }
    bool CanBeClicked { get; set; }
    bool GhostActive => Setup && !Caught;

    void Spawn();

    void FadeUpdate(HudManager instance);

    void Clicked();

    bool CanCatch();
}