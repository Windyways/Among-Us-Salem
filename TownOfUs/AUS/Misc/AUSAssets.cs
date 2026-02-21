using UnityEngine;

namespace AmongUsSalem.Misc;

public static class AUSAssets
{
    private const string RoleCard = "TownOfUs.Resources.AUS.Sprites.RoleCards";
    private const string Abilities = "TownOfUs.Resources.AUS.Sprites.Abilities";
    private const string Other = "TownOfUs.Resources.AUS.Sprites.Other";

    public static LoadableAsset<Sprite> MafiosoRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.MafiosoRoleCard.png");
    public static LoadableAsset<Sprite> PilgrimRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.PilgrimRoleCard.png");
    public static LoadableAsset<Sprite> CoveniteRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.CoveniteRoleCard.png");
    public static LoadableAsset<Sprite> SheriffRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.SheriffRoleCard.png");

    // ABILITIES
    public static LoadableAsset<Sprite> Mafioso_Kill { get; } = new LoadableResourceAsset($"{Abilities}.Mafioso_Kill.png");
    public static LoadableAsset<Sprite> Necronomicon { get; } = new LoadableResourceAsset($"{Abilities}.Necronomicon.png");
    public static LoadableAsset<Sprite> Sheriff_Search { get; } = new LoadableResourceAsset($"{Abilities}.Sheriff_Search.png");

    // OTHER
    public static LoadableAsset<Sprite> NecronomiconIcon { get; } = new LoadableResourceAsset($"{Other}.NecronomiconNotif.png");
}