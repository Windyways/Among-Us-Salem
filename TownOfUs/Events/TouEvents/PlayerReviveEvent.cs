using MiraAPI.Events;

namespace TownOfUs.Events.TouEvents;

/// <summary>
///     Event that is invoked after a player is revived. This event is not cancelable.
/// </summary>
public class PlayerReviveEvent : MiraEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PlayerReviveEvent" /> class.
    /// </summary>
    /// <param name="player">The player who was revived.</param>
    public PlayerReviveEvent(PlayerControl player)
    {
        Player = player;
    }

    /// <summary>
    ///     Gets the player who was revived.
    /// </summary>
    public PlayerControl Player { get; }
}