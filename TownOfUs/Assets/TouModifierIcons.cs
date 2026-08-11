using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace TownOfUs.Assets;

public static class TouModifierIcons
{
    private static readonly string iconPath = "TownOfUs.Resources.ModifierIcons";

    public static LoadableAsset<Sprite> Bait { get; } = new LoadableResourceAsset($"{iconPath}.Bait.png");
    public static LoadableAsset<Sprite> DoubleShot { get; } = new LoadableResourceAsset($"{iconPath}.DoubleShot.png");
    public static LoadableAsset<Sprite> FirstRoundShield { get; } =
        new LoadableResourceAsset($"{iconPath}.FirstRoundShield.png");
}