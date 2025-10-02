using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace AmongUsSalem.Assets;

public static class TouModifierIcons
{
    private static readonly string iconPath = "AmongUsSalem.Resources.ModifierIcons";

    public static LoadableAsset<Sprite> Bait { get; } = new LoadableResourceAsset($"{iconPath}.Bait.png");
}