using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Modules;

public record PlayerEvent(byte PlayerId, float Unix, Vector3 Position);

public record DeadPlayer(byte KillerId, byte VictimId, DateTime KillTime, RoleBehaviour killerRole);

public sealed class PlayerStats(byte playerId)
{
    public byte PlayerId { get; set; } = playerId;
    public RoleBehaviour killerRole { get; set; }
    public int CorrectKills { get; set; }
    public int IncorrectKills { get; set; }
    public int CorrectAssassinKills { get; set; }
    public int IncorrectAssassinKills { get; set; }
}

// body report class for when medic/detective reports a body
public sealed class BodyReport
{
    public PlayerControl? Killer { get; set; }
    public PlayerControl? Reporter { get; set; }
    public PlayerControl? Body { get; set; }
    public float KillAge { get; set; }
}

public static class GameHistory
{
    public static readonly Dictionary<byte, RoleBehaviour> RoleDictionary = [];
    public static readonly List<KeyValuePair<byte, RoleBehaviour>> RoleHistory = [];
    public static readonly Dictionary<byte, RoleBehaviour> RoleWhenAlive = [];

    // Unused for now
    public static readonly List<PlayerEvent> PlayerEvents = []; //local player events
    public static readonly List<DeadPlayer> KilledPlayers = [];
    public static readonly List<(byte, DeathReason)> DeathHistory = [];
    public static readonly Dictionary<byte, PlayerStats> PlayerStats = [];
    public static string EndGameSummary = string.Empty;
    public static string WinningFaction = string.Empty;
    public static IEnumerable<RoleBehaviour> AllRoles => [.. RoleDictionary.Values];

    public static void RegisterRole(PlayerControl player, RoleBehaviour role, bool clean = false)
    {
        //Logger<AUSPlugin>.Message($"RegisterRole - player: '{player.Data.PlayerName}', role: '{role.NiceName}'");

        if (clean)
        {
            RoleHistory.RemoveAll(x => x.Key == player.PlayerId);
        }

        RoleDictionary.Remove(player.PlayerId);
        RoleDictionary.Add(player.PlayerId, role);

        RoleHistory.Add(KeyValuePair.Create(player.PlayerId, role));

        if (!PlayerStats.TryGetValue(player.PlayerId, out _))
        {
            PlayerStats.Add(player.PlayerId, new PlayerStats(player.PlayerId));
        }

        if (!role.IsDead)
        {
            RoleWhenAlive.Remove(player.PlayerId);
            RoleWhenAlive.Add(player.PlayerId, role);
        }
    }

    public static void AddMurder(PlayerControl killer, PlayerControl victim)
    {
        var deadBody = new DeadPlayer(killer.PlayerId, victim.PlayerId, DateTime.UtcNow, killer.GetRoleWhenAlive());

        KilledPlayers.Add(deadBody);
    }

    public static void ClearMurder(PlayerControl player)
    {
        var instance = KilledPlayers.FirstOrDefault(x => x.VictimId == player.PlayerId);

        if (instance == null)
        {
            return;
        }

        KilledPlayers.Remove(instance);
    }

    public static void ClearAll()
    {
        RoleDictionary.Do(x =>
        {
            if (x.Value != null && x.Value.gameObject != null)
            {
                Object.Destroy(x.Value.gameObject);
            }
        });

        RoleDictionary.Clear();

        RoleHistory.Do(x =>
        {
            if (x.Value != null && x.Value.gameObject != null)
            {
                Object.Destroy(x.Value.gameObject);
            }
        });

        RoleHistory.Clear();

        RoleWhenAlive.Do(x =>
        {
            if (x.Value != null && x.Value.gameObject != null)
            {
                Object.Destroy(x.Value.gameObject);
            }
        });

        RoleWhenAlive.Clear();

        KilledPlayers.Clear();
        DeathHistory.Clear();
        PlayerStats.Clear();
        PlayerEvents.Clear();
    }

    public static RoleBehaviour GetRoleWhenAlive(this PlayerControl player)
    {
        //var role = RoleHistory.LastOrDefault(x => x.Key == player.PlayerId && !x.Value.IsDead);
        //return role.Value != null ? role.Value : null;

        if (RoleWhenAlive.TryGetValue(player.PlayerId, out var role))
        {
            return role;
        }

        if (!player.Data.IsDead)
        {
            return player.Data.Role;
        }

        var role2 = player.Data.RoleWhenAlive;

        if (role2.HasValue)
        {
            return RoleManager.Instance.GetRole(role2.Value);
        }

        return player.Data.Role;
    }

    public static int RoleCount<T>() where T : RoleBehaviour
    {
        return RoleWhenAlive.Count(x => x.Value is T);
    }
}