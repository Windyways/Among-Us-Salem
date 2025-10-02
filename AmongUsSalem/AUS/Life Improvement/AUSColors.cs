using System.Text.RegularExpressions;
using UnityEngine;

namespace AmongUsSalem;

public static class AUSColors
{
    public static Color Town => new Color32(6, 224, 12, 255);

    public static Color Neutral => new Color32(169, 169, 169, 255);
    public static Color Mafia => new Color32(221, 0, 0, 255);
    public static Color Apocalypse => new Color32(255, 0, 78, 255);
    public static Color Coven => new Color32(181, 69, 255, 255);
    public static Color Traitor => new Color32(206, 54, 250, 255);
    
    public static Color Arsonist => new Color32(238, 118, 0, 255);
    public static Color Shroud => new Color32(102, 153, 255, 255);
    public static Color Vampire => new Color32(162, 41, 41, 255);

    public static Color NecroPassing => Coven;
    public static Color CovenVIP => Coven;
    public static Color Sheriff => Town;
    public static Color Veteran => Town;
    public static Color Mayor => Town;
    public static Color Amnesiac => Town;
    
    public static Color OrangeShade => new Color32(255, 153, 0, 255);




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














































































    public static bool UseBasic { get; set; }

    // Crew Colors

    public static Color Aurial => UseBasic ? Palette.CrewmateBlue : new Color32(179, 77, 153, 255);
    public static Color Detective => UseBasic ? Palette.CrewmateBlue : new Color32(77, 77, 255, 255);
    public static Color Haunter => UseBasic ? Palette.CrewmateBlue : new Color32(212, 212, 212, 255);
    public static Color Investigator => UseBasic ? Palette.CrewmateBlue : new Color32(0, 179, 179, 255);
    public static Color Lookout => UseBasic ? Palette.CrewmateBlue : new Color32(51, 255, 102, 255);
    public static Color Mystic => UseBasic ? Palette.CrewmateBlue : new Color32(77, 153, 230, 255);
    public static Color Seer => UseBasic ? Palette.CrewmateBlue : new Color32(255, 204, 128, 255);
    public static Color Snitch => UseBasic ? Palette.CrewmateBlue : new Color32(212, 176, 56, 255);
    public static Color Spy => UseBasic ? Palette.CrewmateBlue : new Color32(204, 163, 204, 255);
    public static Color Tracker => UseBasic ? Palette.CrewmateBlue : new Color32(0, 153, 0, 255);
    public static Color Trapper => UseBasic ? Palette.CrewmateBlue : new Color32(166, 209, 179, 255);
    
    public static Color Deputy => UseBasic ? Palette.CrewmateBlue : new Color32(255, 204, 0, 255);
    public static Color Hunter => UseBasic ? Palette.CrewmateBlue : new Color32(41, 171, 135, 255);
    public static Color Vigilante => UseBasic ? Palette.CrewmateBlue : new Color32(255, 255, 153, 255);
    
    public static Color Jailor => UseBasic ? Palette.CrewmateBlue : new Color32(166, 166, 166, 255);
    public static Color Politician => UseBasic ? Palette.CrewmateBlue : new Color32(102, 0, 153, 255);
    public static Color Prosecutor => UseBasic ? Palette.CrewmateBlue : new Color32(179, 128, 0, 255);
    public static Color Swapper => UseBasic ? Palette.CrewmateBlue : new Color32(102, 230, 102, 255);
    
    public static Color Altruist => UseBasic ? Palette.CrewmateBlue : new Color32(102, 0, 0, 255);
    public static Color Cleric => UseBasic ? Palette.CrewmateBlue : new Color32(0, 255, 179, 255);
    public static Color Medic => UseBasic ? Palette.CrewmateBlue : new Color32(0, 102, 0, 255);
    public static Color Mirrorcaster => UseBasic ? Palette.CrewmateBlue : new Color32(144, 162, 195, 255);
    public static Color Oracle => UseBasic ? Palette.CrewmateBlue : new Color32(191, 0, 191, 255);
    public static Color Warden => UseBasic ? Palette.CrewmateBlue : new Color32(153, 0, 255, 255);
    
    public static Color Engineer => UseBasic ? Palette.CrewmateBlue : new Color32(255, 166, 10, 255);
    public static Color Imitator => UseBasic ? Palette.CrewmateBlue : new Color32(179, 217, 77, 255);
    public static Color Medium => UseBasic ? Palette.CrewmateBlue : new Color32(166, 128, 255, 255);
    public static Color Plumber => UseBasic ? Palette.CrewmateBlue : new Color32(204, 102, 0, 255);
    public static Color Transporter => UseBasic ? Palette.CrewmateBlue : new Color32(0, 237, 255, 255);

    // Neutral Colors
    public static Color GuardianAngel => new Color32(179, 255, 255, 255);
    public static Color Mercenary => new Color32(140, 102, 153, 255);
    public static Color Survivor => new Color32(255, 230, 77, 255);
    
    
    public static Color Doomsayer => new Color32(0, 255, 128, 255);
    public static Color Executioner => new Color32(99, 59, 31, 255);
    public static Color Inquisitor = new Color32(217, 66, 145, 255);
    public static Color Jester => new Color32(255, 191, 204, 255);
    public static Color Phantom => new Color32(102, 41, 97, 255);
    
    public static Color Glitch => Color.green;
    public static Color Juggernaut => new Color32(140, 0, 77, 255);
    public static Color Plaguebearer => new Color32(230, 255, 179, 255);
    public static Color Pestilence => new Color32(77, 77, 77, 255);
    public static Color SoulCollector => new Color32(153, 255, 204, 255);

    // Alliance Modifiers
    public static Color Egotist => new Color32(102, 153, 102, 255);
    public static Color Lover => new Color32(255, 102, 204, 255);
    // Universal Modifiers
    public static Color ButtonBarry => new Color32(179, 51, 204, 255);
    public static Color Flash => new Color32(255, 128, 128, 255);
    public static Color Giant => new Color32(255, 179, 77, 255);
    public static Color Mini => new Color32(204, 255, 230, 255);
    public static Color Satellite => new Color32(0, 153, 204, 255);
    public static Color Shy => new Color32(255, 179, 204, 255);
    public static Color SixthSense => new Color32(217, 255, 140, 255);
    // Crewmate Modifiers
    public static Color Aftermath => new Color32(166, 255, 166, 255);
    public static Color Bait => new Color32(51, 179, 179, 255);
    public static Color Celebrity => new Color32(255, 153, 153, 255);
    public static Color Diseased => Color.grey;
    public static Color Frosty => new Color32(153, 255, 255, 255);
    public static Color Noisemaker => new Color32(232, 105, 158, 255);
    public static Color Operative => new Color32(153, 8, 18, 255);
    public static Color Rotting => new Color32(171, 128, 105, 255);
    public static Color Scientist => new Color32(0, 199, 105, 255);
    public static Color Taskmaster => new Color32(148, 214, 237, 255);

}