using System.Collections.Generic;
using System.IO;
using System.Linq;
using AmongUsSalem.Patches;
using HarmonyLib;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement
{
    public class FactionStats
    {
        public string FactionName { get; set; }
        public int Wins { get; set; }
        public int GamesPlayed { get; set; }
        public Color Color { get; set; }

        public float WinRate
        {
            get
            {
                if (GamesPlayed == 0) return 0;
                return (float)Wins / GamesPlayed;
            }
        }

        public FactionStats(string factionName, Color color)
        {
            FactionName = factionName;
            Wins = 0;
            GamesPlayed = 0;
            Color = color;
        }
    }

    public static class FactionReferences
    {
        public static Dictionary<string, FactionStats> factionStats = new Dictionary<string, FactionStats>();
        public static string filePath = "FactionWinData.txt";

        public static void Initialize()
        {  
            factionStats.Add("Town", new FactionStats("Town", AUSColors.Town)); 
            factionStats.Add("Coven", new FactionStats("Coven", AUSColors.Coven)); 
            factionStats.Add("Mafia", new FactionStats("Mafia", AUSColors.Mafia)); 
            factionStats.Add("Traitor", new FactionStats("Traitor", AUSColors.Traitor)); 
            factionStats.Add("Neutral", new FactionStats("Neutral", AUSColors.Neutral)); 

            LoadFactionStats(filePath);
        }

        public static void UpdateFactionResult(string factionName, bool won)
        {
            if (!RoleReferences.CountRoundToLeaderboard)
            {
                AUSPlugin.DebugLogMessage("CountRoundToLeaderboard is false, wins and loses do not count this game.");
                return;
            }

            if (factionStats.TryGetValue(factionName, out FactionStats? stats))
            {
                stats.GamesPlayed++;
                if (won) stats.Wins++;
            }
            SaveFactionStats(filePath);
        }

        public static void SaveFactionStats(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var faction in factionStats.Values)
                {
                    //                     TryParse 0     TryParse 1     TryParse 2  
                    writer.WriteLine($"{faction.FactionName},{faction.Wins},{faction.GamesPlayed}");
                }
            }
        }

        public static void LoadFactionStats(string filePath)
        {
            if (!File.Exists(filePath))
            {
                SaveFactionStats(filePath); // Save the current factionStats (even if empty/default)
                AUSPlugin.DebugLogMessage(".txt file not found, creating a new one.", AUSPlugin.MsgType.Error);
                return;
            }

            foreach (var line in File.ReadLines(filePath))
            {
                var parts = line.Split(',');
                if (parts.Length != 3) continue; // Add this by 1 for each saved stat!

                string factionName = parts[0];
                Color color = Color.white;
                if (int.TryParse(parts[1], out int wins) && int.TryParse(parts[2], out int gamesPlayed))
                {
                    if (factionStats.TryGetValue(factionName, out FactionStats? value))
                    {
                        value.Wins = wins;
                        value.GamesPlayed = gamesPlayed;
                    }
                    else
                    {
                        // Optionally add new factions if missing
                        factionStats.Add(factionName, new FactionStats(factionName, color)
                        {
                            Wins = wins,
                            GamesPlayed = gamesPlayed,
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

            // Optional: Reset in-memory faction stats as well (you could leave it as-is)
            foreach (var faction in FactionReferences.factionStats.Values)
            {
                faction.Wins = 0;
                faction.GamesPlayed = 0;
            }

            // Optionally save the empty stats back to the file
            SaveFactionStats(filePath);
        }
    }
    
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
    public static class FactionRateCommand
    {
        public static string WinRate()
        {
            string rates = "";

            var sortedFactions = FactionReferences.factionStats.Values
                                        .Where(r => r.GamesPlayed > 0) 
                                        .OrderByDescending(r => r.WinRate)  // Sort by WinRate first
                                        .ThenByDescending(r => r.Wins - r.GamesPlayed) // Then by number of losses 
                                        .ToList();

            foreach (var faction in sortedFactions)
            {
                string hexColor = ColorUtility.ToHtmlStringRGB(faction.Color);
                string coloredFactionName = $"<b><color=#{hexColor}>{faction.FactionName}</color></b>";
                
                rates += $"{coloredFactionName} | <b><color=#ff0000>{faction.GamesPlayed - faction.Wins}</color></b> | <b><color=#00ff00>{faction.Wins}</color></b> | {faction.WinRate * 100:F2}% |\n";
            }

            if (rates == "") rates = "There are no data logged on this slot.";
            return "<size=60%>" + rates + "</size>";

        }
    }
}