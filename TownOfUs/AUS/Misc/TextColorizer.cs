using System.Text.RegularExpressions;

namespace AmongUsSalem.Misc;

public static class TextColorizer
{
    // Define the words and their colors
    private static readonly Dictionary<string, string> colorMap = new Dictionary<string, string>
    {
        // Town
        { "Town", "#06e00c" },
        { "Pilgrim", "#06e00c" },
        { "Sheriff", "#06e00c" }, { "Sheriffs", "#06e00c" },
        { "Bodyguard", "#06e00c" },
        { "Catalyst", "#06e00c" },
        { "Seer", "#06e00c" },
        { "Cleric", "#06e00c" },
        { "Mayor", "#06e00c" },
        { "Deputy", "#06e00c" },
        { "Amnesiac", "#06e00c" },
        { "Prosecutor", "#06e00c" },
        { "Crusader", "#06e00c" },

        // Mafia
        { "Mafioso", "#dd0000" }, { "Mafiosos", "#dd0000" },
        { "Godfather", "#dd0000" },
        { "Mafia", "#dd0000" },
        { "Framers", "#dd0000" }, { "Framer", "#dd0000" },
        { "Consigliere", "#dd0000" },

        // Coven
        { "Covenite", "#ab42ef" },
        { "Coven", "#ab42ef" },
        { "Enchanters", "#ab42ef" },
        { "Illusionist", "#ab42ef" }, { "Illusionists", "#ab42ef" },
        { "Hex Master", "#ab42ef" },
        { "Jinx", "#ab42ef" },
        { "Potion Master", "#ab42ef" },
        { "Ritualist", "#ab42ef" },

        // Neutral
        { "Neutral", "#a9a9a9" },
        { "Arsonist", "#db7601" },
        { "Vampires", "#a22929" },
        { "Survivor", "#dddd00" },
        { "Jester", "#f5a6d4" },
        { "Serial Killer", "#1d4dfc" },

        // Apocalypse
        { "Apocalypse", "#ff004e" },
        { "Soul Collectors", "#ff004e" },
        { "Berserker", "#ff004e" },
        { "War", "#ff004e" },

        // Alignments
        { "Investigative", "#4a86e8" },
        { "Evils", "#4a86e8" }, { "Evil", "#4a86e8" },
        { "Killing", "#4a86e8" },
        { "Outlier", "#4a86e8" },
        { "Deception", "#4a86e8" },
        { "Power", "#4a86e8" },
        { "Support", "#4a86e8" },
        { "Protective", "#4a86e8" },
        { "Utility", "#4a86e8" },
        { "Government", "#4a86e8" },
        { "Executive", "#4a86e8" },
        { "Benign", "#4a86e8" },
        { "Pariah", "#4a86e8" },

        // Keywords
        { "Search", "#4a86e8" },
        { "Necronomicon", "#4a86e8" },
        { "Kill", "#4a86e8" },
        { "Hex-Bomb", "#4a86e8" }, { "Hexed", "#4a86e8" }, { "Hex", "#4a86e8" },
        { "Frame", "#4a86e8" },
        { "Cast", "#4a86e8" },
        { "Size Up", "#4a86e8" },
        { "Doused", "#4a86e8" },
        { "Guard", "#4a86e8" },
        { "Shrouded", "#4a86e8" },
        { "Self Protect", "#4a86e8" }, { "Self Protects", "#4a86e8" },
        { "Night", "#922058" }, { "Nights", "#922058" },
        { "Day", "#ffff00" },
        { "Overcharge", "#4a86e8" }, { "Overcharged", "#4a86e8" },
        { "Intuit", "#4a86e8" },
        { "Gaze", "#4a86e8" },
        { "Harmful", "#4a86e8" },
        { "Barrier", "#4a86e8" },
        { "Reveal", "#4a86e8" }, { "Revealed", "#4a86e8" },
        { "Illuminated", "#4a86e8" },
        { "Self Barrier", "#4a86e8" },
        { "Poison", "#4a86e8" },
        { "Blood Ritual", "#4a86e8" }, { "Blood Rituals", "#4a86e8" },
        { "TT Hunt", "#4a86e8" },
        { "Shoot", "#4a86e8" },
        { "High Noon", "#4a86e8" }, { "High Noons", "#4a86e8" },
        { "Remember", "#4a86e8" }, { "Remembered", "#4a86e8" }, { "Remembering", "#4a86e8" },
        { "Graveyard", "#4a64e8" },
        { "Prosecute", "#4a86e8" }, { "Prosecutes", "#4a86e8" },
        { "Vest", "#4a86e8" }, { "Vests", "#4a86e8" },
        { "Fortify", "#4a86e8" },
        { "Order", "#4a86e8" },
        { "Haunt", "#4a86e8" },
        { "Chatterbox", "#4a86e8" },
        { "Rampage", "#4a86e8" },
        { "Bloodlust", "#4a86e8" },
        { "RoleBlocked", "#f1c232" }, { "RoleBlockers", "#f1c232" },
        { "Cautious", "#4a86e8" },
        { "Isolate", "#4a86e8" },
        { "Daybreak", "#4a86e8" }, { "Daybreaks", "#4a86e8" },
        { "Unknown Obstacle", "#4a86e8" },

        // Attack
        { "Basic Attack", "#e70052" },
        { "Powerful Attack", "#e70052" }, { "Powerful Attacks", "#e70052" },
        { "Unstoppable Attack", "#e70052" }, { "Unstoppable Attacks", "#e70052" },

        { "Attack: None", "#e70052" },
        { "Attack: Basic", "#e70052" },
        { "Attack: Powerful", "#e70052" },
        { "Attack: Unstoppable", "#e70052" },

        // Defense
        { "Defense: None", "#0000ff" },
        { "Defense: Basic", "#0000ff" },
        
        // Ethereal Defense
        { "Ethereal Defense: Powerful", "#a1a1ff" },

        { "Basic Defense", "#0000ff" },
        { "Powerful Defense", "#0000ff" },
        { "Invincible Defense", "#0000ff" },
    };

    // Build a single regex that matches any keyword. Longer keys are listed first to prefer them when overlapping.
    private static readonly Regex KeywordRegex = new Regex(
        string.Join("|", colorMap.Keys.OrderByDescending(k => k.Length).Select(Regex.Escape)),
        RegexOptions.Compiled
    );

    public static string ApplyKeywords(this string input)
    {
        // Replace in one pass using the original input so replacements cannot be re-matched inside inserted markup.
        return KeywordRegex.Replace(
            input,
            match =>
            {
                var word = match.Value;
                var color = colorMap[word];
                return $"<b><color={color}>{word}</color></b>";
            }
        );
    }
}