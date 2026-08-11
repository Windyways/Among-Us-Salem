using UnityEngine;

namespace TownOfUs.Utilities.Appearances;

public sealed class VisualAppearance : NetworkedPlayerInfo.PlayerOutfit
{
    public VisualAppearance(TownOfUsAppearances appearanceType)
    {
        AppearanceType = appearanceType;
    }

    public VisualAppearance(NetworkedPlayerInfo.PlayerOutfit outfit, TownOfUsAppearances appearanceType)
    {
        ColorId = outfit.ColorId;
        HatId = outfit.HatId;
        SkinId = outfit.SkinId;
        VisorId = outfit.VisorId;
        PlayerName = outfit.PlayerName;
        PetId = outfit.PetId;

        AppearanceType = appearanceType;
    }

    public VisualAppearance(VisualAppearance outfit, TownOfUsAppearances appearanceType)
    {
        ColorId = outfit.ColorId;
        HatId = outfit.HatId;
        SkinId = outfit.SkinId;
        VisorId = outfit.VisorId;
        PlayerName = outfit.PlayerName;
        PetId = outfit.PetId;

        RendererColor = outfit.RendererColor;
        PlayerMaterialColor = outfit.PlayerMaterialColor;
        PlayerMaterialBackColor = outfit.PlayerMaterialBackColor;
        PlayerMaterialVisorColor = outfit.PlayerMaterialVisorColor;
        NameColor = outfit.NameColor;
        ColorBlindTextColor = outfit.ColorBlindTextColor;

        Speed = outfit.Speed;
        Size = outfit.Size;
        NameVisible = outfit.NameVisible;

        AppearanceType = appearanceType;
    }

    public Color RendererColor { get; set; } = Color.white;
    public Color? PlayerMaterialColor { get; set; }
    public Color? PlayerMaterialBackColor { get; set; }
    public Color? PlayerMaterialVisorColor { get; set; }
    public Color? NameColor { get; set; }
    public Color ColorBlindTextColor { get; set; } = Color.white;
    public bool NameVisible { get; set; } = true;

    public float Speed { get; set; } = 1f;
    public Vector3 Size { get; set; } = new(0.7f, 0.7f, 1f);

    public TownOfUsAppearances AppearanceType { get; set; }
}