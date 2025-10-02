using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace AmongUsSalem.Assets;

public static class TouRoleIcons
{
    private static readonly string iconPath = "AmongUsSalem.Resources.RoleIcons";
    private const string DAG = $"AmongUsSalem.Resources.Sprites.RoleCards";

    // THIS FILE SHOULD ONLY HOLD ROLE ICONS


    public static LoadableAsset<Sprite> Detective { get; } = new LoadableResourceAsset($"{iconPath}.Detective.png");


    public static LoadableAsset<Sprite> Warlock { get; } = new LoadableResourceAsset($"{iconPath}.Warlock.png");
    
    public static LoadableAsset<Sprite> RandomAny { get; } = new LoadableResourceAsset($"{iconPath}.RandomAny.png");
    public static LoadableAsset<Sprite> RandomCrew { get; } = new LoadableResourceAsset($"{iconPath}.RandomCrew.png");
    public static LoadableAsset<Sprite> RandomNeut { get; } = new LoadableResourceAsset($"{iconPath}.RandomNeut.png");
    public static LoadableAsset<Sprite> RandomImp { get; } = new LoadableResourceAsset($"{iconPath}.RandomImp.png");
}