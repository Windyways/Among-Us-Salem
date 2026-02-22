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
    public static LoadableAsset<Sprite> FramerRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.FramerRoleCard.png");
    public static LoadableAsset<Sprite> HexMasterRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.HexMasterRoleCard.png");
    public static LoadableAsset<Sprite> IllusionistRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.IllusionistRoleCard.png");
    public static LoadableAsset<Sprite> ConsigliereRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.ConsigliereRoleCard.png");
    public static LoadableAsset<Sprite> BodyguardRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.BodyguardRoleCard.png");
    public static LoadableAsset<Sprite> CatalystRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.CatalystRoleCard.png");
    public static LoadableAsset<Sprite> SeerRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.SeerRoleCard.png");

    // ABILITIES
    public static LoadableAsset<Sprite> Mafioso_Kill { get; } = new LoadableResourceAsset($"{Abilities}.Mafioso_Kill.png");
    public static LoadableAsset<Sprite> Necronomicon { get; } = new LoadableResourceAsset($"{Abilities}.Necronomicon.png");
    public static LoadableAsset<Sprite> Sheriff_Search { get; } = new LoadableResourceAsset($"{Abilities}.Sheriff_Search.png");
    public static LoadableAsset<Sprite> Framer_Frame { get; } = new LoadableResourceAsset($"{Abilities}.Framer_Frame.png");
    public static LoadableAsset<Sprite> HexMaster_Hex { get; } = new LoadableResourceAsset($"{Abilities}.HexMaster_Hex.png");
    public static LoadableAsset<Sprite> NecroPassing_PassNecronomicon { get; } = new LoadableResourceAsset($"{Abilities}.NecroPassing_PassNecronomicon.png");
    public static LoadableAsset<Sprite> Illusionist_Cast { get; } = new LoadableResourceAsset($"{Abilities}.Illusionist_Cast.png");
    public static LoadableAsset<Sprite> Consigliere_SizeUp { get; } = new LoadableResourceAsset($"{Abilities}.Consigliere_SizeUp.png");
    public static LoadableAsset<Sprite> Bodyguard_Guard { get; } = new LoadableResourceAsset($"{Abilities}.Bodyguard_Guard.png");
    public static LoadableAsset<Sprite> Bodyguard_SelfProtect { get; } = new LoadableResourceAsset($"{Abilities}.Bodyguard_SelfProtect.png");
    public static LoadableAsset<Sprite> Catalyst_Overcharge { get; } = new LoadableResourceAsset($"{Abilities}.Catalyst_Overcharge.png");
    public static LoadableAsset<Sprite> Seer_Intuit { get; } = new LoadableResourceAsset($"{Abilities}.Seer_Intuit.png");
    public static LoadableAsset<Sprite> Seer_Gaze { get; } = new LoadableResourceAsset($"{Abilities}.Seer_Gaze.png");

    // OTHER
    public static LoadableAsset<Sprite> NecronomiconIcon { get; } = new LoadableResourceAsset($"{Other}.NecronomiconNotif.png");
}