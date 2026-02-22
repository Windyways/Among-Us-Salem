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
        { "Sheriff", "#06e00c" },
        { "Bodyguard", "#06e00c" },
        { "Catalyst", "#06e00c" },
        { "Seer", "#06e00c" },

        // Mafia
        { "Mafioso", "#dd0000" },
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

        // Neutral
        { "Neutral", "#a9a9a9" },
        { "Arsonist", "#db7601" },
        { "Vampires", "#a22929" },

        // Apocalypse
        { "Apocalypse", "#ff004e" },
        { "Soul Collectors", "#ff004e" },

        // Alignments
        { "Investigative", "#4a86e8" },
        { "Evils", "#4a86e8" }, { "Evil", "#4a86e8" },
        { "Killing", "#4a86e8" },
        { "Outlier", "#4a86e8" },
        { "Deception", "#4a86e8" },
        { "Power", "#4a86e8" },
        { "Support", "#4a86e8" },
        { "Protective", "#4a86e8" },

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
        { "Night", "#922058" },
        { "Day", "#ffff00" },
        { "Overcharge", "#4a86e8" }, { "Overcharged", "#4a86e8" },
        { "Intuit", "#4a86e8" },
        { "Gaze", "#4a86e8" },

        // Attack
        { "Basic Attack", "#e70052" },
        { "Powerful Attack", "#e70052" },

        { "Attack: None", "#e70052" },
        { "Attack: Basic", "#e70052" },
        { "Attack: Powerful", "#e70052" },

        // Defense
        { "Defense: None", "#0000ff" },

        { "Basic Defense", "#0000ff" },
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