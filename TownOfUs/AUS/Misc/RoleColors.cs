using System.Text.RegularExpressions;
using UnityEngine;

namespace AmongUsSalem.Misc;

public static class RoleColors
{
    public static Color Town => new Color32(6, 224, 12, 255);
    public static Color Neutral => new Color32(169, 169, 169, 255);
    public static Color Mafia => new Color32(221, 0, 0, 255);
    public static Color Coven => new Color32(171, 66, 239, 255);

    public static Color Apocalypse => new Color32(255, 0, 78, 255);

    public static Color Survivor => new Color32(221, 221, 0, 255);
    public static Color Jester => new Color32(245, 166, 212, 255);
    public static Color SerialKiller => new Color32(29, 77, 252, 255);
    public static Color Starspawn => new Color32(164, 164, 244, 255);
    public static Color Auditor => new Color32(174, 186, 135, 255);
    public static Color Werewolf => new Color32(170, 109, 6, 255);

    // Other Colors
    public static Color Keyword => new Color32(74, 134, 232, 255);


    #region Gradients
    private static Color HexToColor(string hex)
    {
#pragma warning disable S1854 // Unused assignments should be removed
        Color color = new();
#pragma warning restore S1854 // Unused assignments should be removed
        _ = ColorUtility.TryParseHtmlString("#" + hex, out color);
        return color;
    }

    private static string ColorToHex(Color color)
    {
        Color32 color32 = (Color32)color;
        return $"{color32.r:X2}{color32.g:X2}{color32.b:X2}{color32.a:X2}";
    }

    public static bool CheckGradientCode(string ColorCode)
    {
        Regex regex = new Regex(@"^[0-9A-Fa-f]{6}\s[0-9A-Fa-f]{6}$");
        if (!regex.IsMatch(ColorCode)) return false;
        return true;
    }

    public static string GradientColorText(string startColorHex, string endColorHex, string text)
    {
        if (startColorHex.Length != 6 || endColorHex.Length != 6)
        {
            //throw new ArgumentException("Invalid color hex code. Hex code should be 6 characters long (e.g., FFFFFF).");
            return text;
        }

        Color startColor = HexToColor(startColorHex);
        Color endColor = HexToColor(endColorHex);

        int textLength = text.Length;
        float stepR = (endColor.r - startColor.r) / (float)textLength;
        float stepG = (endColor.g - startColor.g) / (float)textLength;
        float stepB = (endColor.b - startColor.b) / (float)textLength;
        float stepA = (endColor.a - startColor.a) / (float)textLength;

        string gradientText = "";

        for (int i = 0; i < textLength; i++)
        {
            float r = startColor.r + (stepR * i);
            float g = startColor.g + (stepG * i);
            float b = startColor.b + (stepB * i);
            float a = startColor.a + (stepA * i);


            string colorHex = ColorToHex(new Color(r, g, b, a));
            //Logger.Msg(colorHex, "color");
            gradientText += $"<color=#{colorHex}>{text[i]}</color>";
        }

        return gradientText;
    }
    #endregion
    public static string StarspawnNameInGradient => GradientColorText("fce79a", "a4a4f4", "Starspawn");
}