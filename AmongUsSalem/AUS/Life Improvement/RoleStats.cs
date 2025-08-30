using System.Collections.Generic;
using System.IO;
using System.Linq;
using ObjectWorkshop.Patches;
using ObjectWorkshop.Roles;
using HarmonyLib;
using UnityEngine;

namespace ObjectWorkshop.LifeImprovement
{
    public class RoleStats
    {
        public string RoleName { get; set; }

        public int Wins { get; set; }
        public int GamesPlayed { get; set; }
        public Color Color { get; set; }
        
        public int Kills { get; set; }
        
        public int ThrallWins { get; set; }

        public float WinRate
        {
            get
            {
                if (GamesPlayed == 0) return 0;
                return (float)(Wins + ThrallWins) / GamesPlayed;
            }
        }

        public RoleStats(string roleName, Color color)
        {
            RoleName = roleName;
            Wins = 0;
            ThrallWins = 0;
            GamesPlayed = 0;
            Kills = 0;
            Color = color;
        }
    }

    public static class RoleReferences
    {
        public static Dictionary<string, RoleStats> roleStats = new Dictionary<string, RoleStats>();
        public static string filePath = "WinData.txt";
        public static bool CountRoundToLeaderboard = true;

        public static void Initialize()
        {
            #region Crewmate
            roleStats.Add("Crewmate", new RoleStats("Crewmate", OWColors.Crewmate));
            roleStats.Add("Alarum", new RoleStats("Alarum", OWColors.Crewmate));
            roleStats.Add("Totemist", new RoleStats("Totemist", OWColors.Crewmate));
            roleStats.Add("Duelist", new RoleStats("Duelist", OWColors.Crewmate));
            roleStats.Add("UFO", new RoleStats("UFO", OWColors.Crewmate));
            roleStats.Add("Gift Weaver", new RoleStats("Gift Weaver", OWColors.Crewmate));
            #endregion
            #region Neutral
            roleStats.Add("Pyre", new RoleStats("Pyre", OWColors.Pyre));
            roleStats.Add("Reaper", new RoleStats("Reaper", OWColors.Reaper));
            roleStats.Add("Undead Reaper", new RoleStats("Undead Reaper", OWColors.UndeadReaper));
            #endregion
            #region Infiltrator
            roleStats.Add("Infiltrator", new RoleStats("Infiltrator", OWColors.Infiltrator));
            roleStats.Add("Canopy", new RoleStats("Canopy", OWColors.Infiltrator));
            roleStats.Add("Obstructor", new RoleStats("Obstructor", OWColors.Infiltrator));
            roleStats.Add("Aimsman", new RoleStats("Aimsman", OWColors.Infiltrator));
            roleStats.Add("Claylim", new RoleStats("Claylim", OWColors.Claylim));
            #endregion

            LoadRoleStats(filePath);
        }

        public static void UpdateRoleResult(RoleBehaviour roleBehaviour, int kills, bool won, bool wonAsNewTeam)
        {
            var role = roleBehaviour.Player.GetOWRole();
            if (role == null)
                return;

            string roleName = role.RoleName;
            if (!CountRoundToLeaderboard)
            {
                ObjectWorkshopPlugin.DebugLogMessage("CountRoundToLeaderboard is false, wins and loses do not count this game.");
                return;
            }
            
            if (roleStats.TryGetValue(roleName, out RoleStats? stats))
            {
                stats.GamesPlayed++;
                stats.Kills += kills;
                if (wonAsNewTeam && won) stats.ThrallWins++;
                else if (won) stats.Wins++;
            }
            SaveRoleStats(filePath);
        }

        public static void SaveRoleStats(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var role in roleStats.Values)
                {
                    //                    TryParse 0    TryParse 1     TryParse 2        TryParse 3          TryParse 4
                    writer.WriteLine($"{role.RoleName},{role.Wins},{role.GamesPlayed},{role.ThrallWins},{role.Kills}");
                }
            }
        }

        public static void LoadRoleStats(string filePath)
        {
            if (!File.Exists(filePath))
            {
                SaveRoleStats(filePath); // Save the current roleStats (even if empty/default)
                ObjectWorkshopPlugin.DebugLogMessage(".txt file not found, creating a new one.", ObjectWorkshopPlugin.MsgType.Error);
                return;
            }

            foreach (var line in File.ReadLines(filePath))
            {
                var parts = line.Split(',');
                if (parts.Length != 5) continue; // skip broken lines
                // Add this by 1 for each saved stat i want.

                string roleName = parts[0];
                Color color = Color.white;
                if (int.TryParse(parts[1], out int wins) && int.TryParse(parts[2], out int gamesPlayed) && int.TryParse(parts[3], out int ThrallWins) && int.TryParse(parts[4], out int kills))
                {
                    if (roleStats.TryGetValue(roleName, out RoleStats? value))
                    {
                        value.Wins = wins;
                        value.GamesPlayed = gamesPlayed;
                        value.ThrallWins = ThrallWins;
                        value.Kills = kills;
                    }
                    else
                    {
                        // Optionally add new roles if missing
                        roleStats.Add(roleName, new RoleStats(roleName, color)
                        {
                            Wins = wins,
                            GamesPlayed = gamesPlayed,
                            ThrallWins = ThrallWins,
                            Kills = kills,
                            Color = color
                        });
                    }
                }
            }
        }

        public static void ResetLeaderboard(string filePath)
        {
            // Clear the file (or you can delete it)
            if (File.Exists(filePath))
            {
                File.WriteAllText(filePath, string.Empty);  // This just empties the file
                ObjectWorkshopPlugin.DebugLogMessage("Leaderboard has been reset!");
            }

            // Optional: Reset in-memory role stats as well (you could leave it as-is)
            foreach (var role in roleStats.Values)
            {
                role.Wins = 0;
                role.GamesPlayed = 0;
                role.ThrallWins = 0;
                role.Kills = 0;
            }

            // Optionally save the empty stats back to the file
            SaveRoleStats(filePath);  // This will save an empty leaderboard
        }
    }
    
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
    public static class WinRateCommand
    {
        public static bool Prefix(ChatController __instance)
        {
            if (__instance.freeChatField.Text.ToLower(CultureInfo.CurrentCulture).Contains("/lb", StringComparison.CurrentCultureIgnoreCase))
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!string.IsNullOrWhiteSpace(WinRate()) && player == PlayerControl.LocalPlayer)  DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, WinRate());
                    if (!string.IsNullOrWhiteSpace(FactionRateCommand.WinRate()) && player == PlayerControl.LocalPlayer)  DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, FactionRateCommand.WinRate());
                }
                return true;
            }

            if (__instance.freeChatField.Text.ToLower(CultureInfo.CurrentCulture).Contains("/resetlb", StringComparison.CurrentCultureIgnoreCase))
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    var playerResults = ResetLeaderboard();
                    RoleReferences.ResetLeaderboard(RoleReferences.filePath);
                    FactionReferences.ResetLeaderboard(FactionReferences.filePath);
                    
                    if (!string.IsNullOrWhiteSpace(playerResults) && player == PlayerControl.LocalPlayer)
                    {
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, playerResults);
                    }
                }
                return true;
            }
            return true;
        }

        public static string WinRate()
        {
            string rates = "";

            var sortedRoles = RoleReferences.roleStats.Values
                                        .Where(r => r.GamesPlayed > 0) 
                                        .OrderByDescending(r => r.WinRate)  // Sort by WinRate first
                                        .ThenByDescending(r => r.Wins + r.ThrallWins - r.GamesPlayed) // Then by number of losses 
                                        .ThenByDescending(r => r.Kills) // Then by number of kills 
                                        .ToList();

            foreach (var role in sortedRoles)
            {
                string hexColor = ColorUtility.ToHtmlStringRGB(role.Color);
                string coloredRoleName = $"<b><color=#{hexColor}>{role.RoleName}</color></b>";

                string thrallWinsMsg = role.ThrallWins > 0 ? $" <b><color=#0000ff>+ {role.ThrallWins}</color></b>" : "";
                string KillsMSG = role.Kills > 0 ? $" <b><color=#ff5050>{role.Kills} kills</color></b> |" : "";
                rates += $"{coloredRoleName} | <b><color=#ff0000>{role.GamesPlayed - role.Wins - role.ThrallWins}</color></b> | <b><color=#00ff00>{role.Wins}</color></b>{thrallWinsMsg} |{KillsMSG} {role.WinRate * 100:F2}% |\n";
            }

            if (rates == "") rates = "There are no data logged on this slot.";
            return rates;
        }

        public static string ResetLeaderboard()
        {
            return "The leaderboard has been reset successfully.";
        }
    }
}