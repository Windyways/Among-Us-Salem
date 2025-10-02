using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmongUsSalem.Patches;
using AmongUsSalem.Roles;
using HarmonyLib;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement
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
            #region Town
            roleStats.Add("Pilgrim", new RoleStats("Pilgrim", AUSColors.Town));
            roleStats.Add("Sheriff", new RoleStats("Sheriff", AUSColors.Town));
            roleStats.Add("Veteran", new RoleStats("Veteran", AUSColors.Town));
            roleStats.Add("Bodyguard", new RoleStats("Bodyguard", AUSColors.Town));
            roleStats.Add("Mayor", new RoleStats("Mayor", AUSColors.Town));
            roleStats.Add("Amnesiac", new RoleStats("Amnesiac", AUSColors.Town));
            roleStats.Add("Investigator", new RoleStats("Investigator", AUSColors.Town));
            roleStats.Add("Deputy", new RoleStats("Deputy", AUSColors.Town));
            roleStats.Add("Crusader", new RoleStats("Crusader", AUSColors.Town));
            roleStats.Add("Escort", new RoleStats("Escort", AUSColors.Town));
            roleStats.Add("Coroner", new RoleStats("Coroner", AUSColors.Town));
            roleStats.Add("Prosecutor", new RoleStats("Prosecutor", AUSColors.Town));
            #endregion
            #region Neutral
            #endregion
            roleStats.Add("Arsonist", new RoleStats("Arsonist", AUSColors.Arsonist));
            roleStats.Add("Shroud", new RoleStats("Shroud", AUSColors.Shroud));
            roleStats.Add("Vampire", new RoleStats("Vampire", AUSColors.Vampire));
            roleStats.Add("Jackal", new RoleStats(AUSColors.GradientColorText("404040", "b8b8b8", "Jackal"), AUSColors.Neutral));
            roleStats.Add("Survivor", new RoleStats("Survivor", AUSColors.Survivor));
            #region Mafia
            roleStats.Add("Mafioso", new RoleStats("Mafioso", AUSColors.Mafia));
            roleStats.Add("Framer", new RoleStats("Framer", AUSColors.Mafia));
            roleStats.Add("Blackmailer", new RoleStats("Blackmailer", AUSColors.Mafia));
            roleStats.Add("Ambusher", new RoleStats("Ambusher", AUSColors.Mafia));
            roleStats.Add("Janitor", new RoleStats("Janitor", AUSColors.Mafia));
            #endregion
            #region Coven
            roleStats.Add("Covenite", new RoleStats("Covenite", AUSColors.Coven));
            roleStats.Add("Conjurer", new RoleStats("Conjurer", AUSColors.Coven));
            roleStats.Add("Illusionist", new RoleStats("Illusionist", AUSColors.Coven));
            roleStats.Add("Hex Master", new RoleStats("Hex Master", AUSColors.Coven));
            roleStats.Add("Voodoo Master", new RoleStats("Voodoo Master", AUSColors.Coven));
            roleStats.Add("Jinx", new RoleStats("Jinx", AUSColors.Coven));
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
                AUSPlugin.DebugLogMessage("CountRoundToLeaderboard is false, wins and loses do not count this game.");
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
                AUSPlugin.DebugLogMessage(".txt file not found, creating a new one.", AUSPlugin.MsgType.Error);
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
                AUSPlugin.DebugLogMessage("Leaderboard has been reset!");
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