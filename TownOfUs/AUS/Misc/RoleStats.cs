using System.Globalization;
using UnityEngine;

namespace AmongUsSalem.Misc
{
    public class RoleStats
    {
        public string RoleName { get; set; }

        public int Wins { get; set; }
        public int GamesPlayed { get; set; }
        public Color Color { get; set; }
        
        public int Kills { get; set; }

        public float WinRate
        {
            get
            {
                if (GamesPlayed == 0) return 0;
                return (float)Wins / GamesPlayed;
            }
        }

        public RoleStats(string roleName, Color color)
        {
            RoleName = roleName;
            Wins = 0;
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
            // --- TOWN ---
            roleStats.Add("Pilgrim", new RoleStats("Pilgrim", RoleColors.Town));
            roleStats.Add("Sheriff", new RoleStats("Sheriff", RoleColors.Town));
            roleStats.Add("Bodyguard", new RoleStats("Bodyguard", RoleColors.Town));
            roleStats.Add("Mayor", new RoleStats("Mayor", RoleColors.Town));
            roleStats.Add("Amnesiac", new RoleStats("Amnesiac", RoleColors.Town));
            roleStats.Add("Deputy", new RoleStats("Deputy", RoleColors.Town));
            roleStats.Add("Cleric", new RoleStats("Cleric", RoleColors.Town));
            roleStats.Add("Seer", new RoleStats("Seer", RoleColors.Town));
            roleStats.Add("Catalyst", new RoleStats("Catalyst", RoleColors.Town));

            // --- MAFIA ---
            roleStats.Add("Mafioso", new RoleStats("Mafioso", RoleColors.Mafia));
            roleStats.Add("Framer", new RoleStats("Framer", RoleColors.Mafia));
            roleStats.Add("Consigliere", new RoleStats("Consigliere", RoleColors.Mafia));

            // --- COVEN ---
            roleStats.Add("Covenite", new RoleStats("Covenite", RoleColors.Coven));
            roleStats.Add("Illusionist", new RoleStats("Illusionist", RoleColors.Coven));
            roleStats.Add("Hex Master", new RoleStats("Hex Master", RoleColors.Coven));
            roleStats.Add("Jinx", new RoleStats("Jinx", RoleColors.Coven));
            roleStats.Add("Potion Master", new RoleStats("Potion Master", RoleColors.Coven));
            roleStats.Add("Ritualist", new RoleStats("Ritualist", RoleColors.Coven));

            LoadRoleStats(filePath);
        }

        public static void UpdateRoleResult(RoleBehaviour roleBehaviour, int kills, bool won, bool wonAsNewTeam)
        {
            string roleName = roleBehaviour.NiceName;
            if (!CountRoundToLeaderboard)
            {
                AUSPlugin.DebugLogMessage("CountRoundToLeaderboard is false, wins and loses do not count this game.");
                return;
            }
            
            if (roleStats.TryGetValue(roleName, out RoleStats? stats))
            {
                stats.GamesPlayed++;
                stats.Kills += kills;
                if (won) stats.Wins++;
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
                    writer.WriteLine($"{role.RoleName},{role.Wins},{role.GamesPlayed},{role.Kills}");
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
                if (parts.Length != 3) continue; // skip broken lines
                // Add this by 1 for each saved stat i want.

                string roleName = parts[0];
                Color color = Color.white;
                if (int.TryParse(parts[1], out int wins) && int.TryParse(parts[2], out int gamesPlayed) && int.TryParse(parts[3], out int kills))
                {
                    if (roleStats.TryGetValue(roleName, out RoleStats? value))
                    {
                        value.Wins = wins;
                        value.GamesPlayed = gamesPlayed;
                        value.Kills = kills;
                    }
                    else
                    {
                        // Optionally add new roles if missing
                        roleStats.Add(roleName, new RoleStats(roleName, color)
                        {
                            Wins = wins,
                            GamesPlayed = gamesPlayed,
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
                }
                return true;
            }

            if (__instance.freeChatField.Text.ToLower(CultureInfo.CurrentCulture).Contains("/resetlb", StringComparison.CurrentCultureIgnoreCase))
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    var playerResults = ResetLeaderboard();
                    RoleReferences.ResetLeaderboard(RoleReferences.filePath);
                    
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
                                        .ThenByDescending(r => r.Wins - r.GamesPlayed) // Then by number of losses 
                                        .ThenByDescending(r => r.Kills) // Then by number of kills 
                                        .ToList();

            foreach (var role in sortedRoles)
            {
                string hexColor = ColorUtility.ToHtmlStringRGB(role.Color);
                string coloredRoleName = $"<b><color=#{hexColor}>{role.RoleName}</color></b>";

                string KillsMSG = role.Kills > 0 ? $" <b><color=#ff5050>{role.Kills} kills</color></b> |" : "";
                rates += $"{coloredRoleName} | <b><color=#ff0000>{role.GamesPlayed - role.Wins}</color></b> | <b><color=#00ff00>{role.Wins}</color></b> |{KillsMSG} {role.WinRate * 100:F2}% |\n";
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