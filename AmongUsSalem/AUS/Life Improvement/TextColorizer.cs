using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement;

public static class TextColorizer
{
    // Define the words and their colors
    private static readonly Dictionary<string, string> colorMap = new Dictionary<string, string>
    {
        { "Day", "#FFFF00" },
        { "Night", "#922058" },

        { "Town", "#06e00c" },
        { "Neutral", "#a9a9a9" },
        { "Coven", "#B545FF" },
        { "Mafia", "#DD0000" },

        { "Astral", "#ae0c9b" },
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