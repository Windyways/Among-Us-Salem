using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement;

public static class AUSAssets
{
    private const string ShortPath = "AmongUsSalem.Resources";
    private const string ButtonPath = $"{ShortPath}.CrewButtons";

    private const string Ability = $"AmongUsSalem.Resources.Sprites.Abilities";
    private const string RoleCard = $"AmongUsSalem.Resources.Sprites.RoleCards";
    private const string Other = $"AmongUsSalem.Resources.Sprites.Other";
    private const string Audio = $"AmongUsSalem.Resources.Sprites.Sfx";

    // Abilities
    public static LoadableAsset<Sprite> NecronomiconButton { get; } = new LoadableResourceAsset($"{Ability}.NecronomiconButton.png");

    public static LoadableAsset<Sprite> Mafioso_Attack { get; } = new LoadableResourceAsset($"{Ability}.Mafioso_Attack.png");
    public static LoadableAsset<Sprite> Sheriff_Search { get; } = new LoadableResourceAsset($"{Ability}.Sheriff_Search.png");
    public static LoadableAsset<Sprite> Veteran_Alert { get; } = new LoadableResourceAsset($"{Ability}.Veteran_Alert.png");
    public static LoadableAsset<Sprite> Framer_Frame { get; } = new LoadableResourceAsset($"{Ability}.Framer_Frame.png");
    public static LoadableAsset<Sprite> Conjurer_Conjure { get; } = new LoadableResourceAsset($"{Ability}.Conjurer_Conjure.png");
    public static LoadableAsset<Sprite> Bodyguard_Guard { get; } = new LoadableResourceAsset($"{Ability}.Bodyguard_Guard.png");
    public static LoadableAsset<Sprite> Bodyguard_SelfProtect { get; } = new LoadableResourceAsset($"{Ability}.Bodyguard_SelfProtect.png");
    public static LoadableAsset<Sprite> Mayor_Reveal { get; } = new LoadableResourceAsset($"{Ability}.Mayor_Reveal.png");
    public static LoadableAsset<Sprite> Illusionist_Illusion { get; } = new LoadableResourceAsset($"{Ability}.Illusionist_Illusion.png");

    // Role Cards
    public static LoadableAsset<Sprite> PilgrimRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.PilgrimRoleCard.png");
    public static LoadableAsset<Sprite> MafiosoRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.MafiosoRoleCard.png");
    public static LoadableAsset<Sprite> SheriffRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.SheriffRoleCard.png");
    public static LoadableAsset<Sprite> VeteranRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.VeteranRoleCard.png");
    public static LoadableAsset<Sprite> CoveniteRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.CoveniteRoleCard.png");
    public static LoadableAsset<Sprite> FramerRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.FramerRoleCard.png");
    public static LoadableAsset<Sprite> ConjurerRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.ConjurerRoleCard.png");
    public static LoadableAsset<Sprite> BodyguardRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.BodyguardRoleCard.png");
    public static LoadableAsset<Sprite> AmnesiacRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.AmnesiacRoleCard.png");
    public static LoadableAsset<Sprite> MayorRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.MayorRoleCard.png");
    public static LoadableAsset<Sprite> IllusionistRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.IllusionistRoleCard.png");

    // Audio
    public static LoadableAsset<AudioClip> DuelBegin_SFX { get; } = new LoadableAudioResourceAsset($"{Audio}.DuelBegin_SFX.wav"); // Placeholder, reference for audio in the future!
    

    // Other
    public static LoadableAsset<Sprite> Banner { get; } = new LoadableResourceAsset($"{Other}.AmongUsSalemBanner.png");
    public static LoadableAsset<Sprite> Attributes { get; } = new LoadableResourceAsset($"{Other}.Attributes.png");
    public static LoadableAsset<Sprite> CovenAttributes { get; } = new LoadableResourceAsset($"{Other}.CovenAttributes.png");
    public static LoadableAsset<Sprite> Necronomicon { get; } = new LoadableResourceAsset($"{Other}.Necronomicon.png");



    public static LoadableAsset<Sprite> Placeholder { get; } = new LoadableResourceAsset($"AmongUsSalem.Resources.RoleIcons.RandomAny.png");
    public static LoadableAsset<Sprite> KillSprite { get; } = new LoadableResourceAsset($"{ShortPath}.KillButton.png");
}