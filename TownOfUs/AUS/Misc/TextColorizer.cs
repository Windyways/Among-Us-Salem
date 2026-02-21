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

        // Mafia
        { "Mafioso", "#dd0000" },
        { "Godfather", "#dd0000" },
        { "Mafia", "#dd0000" },

        // Coven
        { "Covenite", "#ab42ef" },
        { "Coven", "#ab42ef" },
        { "Enchanters", "#ab42ef" },
        { "Illusionists", "#ab42ef" },

        // Neutral
        { "Neutral", "#a9a9a9" },

        // Apocalypse
        { "Apocalypse", "#ff004e" },
        { "Soul Collectors", "#ff004e" },

        // Alignments
        { "Investigative", "#4a86e8" },
        { "Evils", "#4a86e8" }, { "Evil", "#4a86e8" },
        { "Killing", "#4a86e8" },
        { "Outlier", "#4a86e8" },

        // Keywords
        { "Search", "#4a86e8" },
        { "Necronomicon", "#4a86e8" },
        { "Kill", "#4a86e8" },

        // Attack
        { "Basic Attack", "#e70052" },

        { "Attack: None", "#e70052" },
        { "Attack: Basic", "#e70052" },

        // Defense
        { "Defense: None", "#0000ff" },
    };

    public static string ApplyKeywords(this string input)
    {
        foreach (var kvp in colorMap)
        {
            string word = kvp.Key;
            string color = kvp.Value;

            // Replace word with colored version
            input = input.Replace(
                word,
                $"<b><color={color}>{word}</color></b>"
            );
        }

        return input;
    }
}