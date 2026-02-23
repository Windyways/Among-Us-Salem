using UnityEngine;

namespace AmongUsSalem.Misc;

public static class AUSAssets
{
    private const string RoleCard = "TownOfUs.Resources.AUS.Sprites.RoleCards";
    private const string Abilities = "TownOfUs.Resources.AUS.Sprites.Abilities";
    private const string Other = "TownOfUs.Resources.AUS.Sprites.Other";
    private const string Audio = "TownOfUs.Resources.AUS.Audio";


    public static void PlaySound(LoadableAsset<AudioClip> clip, float vol = 1f)
    {
        if (Constants.ShouldPlaySfx())
        {
            SoundManager.Instance.PlaySound(clip.LoadAsset(), false, vol);
        }
    }

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
    public static LoadableAsset<Sprite> JinxRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.JinxRoleCard.png");
    public static LoadableAsset<Sprite> PotionMasterRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.PotionMasterRoleCard.png");
    public static LoadableAsset<Sprite> ClericRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.ClericRoleCard.png");
    public static LoadableAsset<Sprite> RitualistRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.RitualistRoleCard.png");
    public static LoadableAsset<Sprite> MayorRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.MayorRoleCard.png");
    public static LoadableAsset<Sprite> DeputyRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.DeputyRoleCard.png");
    public static LoadableAsset<Sprite> AmnesiacRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.AmnesiacRoleCard.png");
    public static LoadableAsset<Sprite> ProsecutorRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.ProsecutorRoleCard.png");

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
    public static LoadableAsset<Sprite> Jinx_Jinx { get; } = new LoadableResourceAsset($"{Abilities}.Jinx_Jinx.png");
    public static LoadableAsset<Sprite> PotionMaster_Harmful { get; } = new LoadableResourceAsset($"{Abilities}.PotionMaster_Harmful.png");
    public static LoadableAsset<Sprite> PotionMaster_Barrier { get; } = new LoadableResourceAsset($"{Abilities}.PotionMaster_Barrier.png");
    public static LoadableAsset<Sprite> PotionMaster_Reveal { get; } = new LoadableResourceAsset($"{Abilities}.PotionMaster_Reveal.png");
    public static LoadableAsset<Sprite> Cleric_Barrier { get; } = new LoadableResourceAsset($"{Abilities}.Cleric_Barrier.png");
    public static LoadableAsset<Sprite> Cleric_SelfBarrier { get; } = new LoadableResourceAsset($"{Abilities}.Cleric_SelfBarrier.png");
    public static LoadableAsset<Sprite> Ritualist_BloodRitual { get; } = new LoadableResourceAsset($"{Abilities}.Ritualist_BloodRitual.png");
    public static LoadableAsset<Sprite> Mayor_Reveal { get; } = new LoadableResourceAsset($"{Abilities}.Mayor_Reveal.png");
    public static LoadableAsset<Sprite> Deputy_HighNoon { get; } = new LoadableResourceAsset($"{Abilities}.Deputy_HighNoon.png");
    public static LoadableAsset<Sprite> Amnesiac_Remember { get; } = new LoadableResourceAsset($"{Abilities}.Amnesiac_Remember.png");
    public static LoadableAsset<Sprite> Prosecutor_Prosecute { get; } = new LoadableResourceAsset($"{Abilities}.Prosecutor_Prosecute.png");

    // Audio
    public static LoadableAsset<AudioClip> Mayor_Reveal_SFX { get; } = new LoadableAudioResourceAsset($"{Audio}.Mayor_Reveal_SFX.wav");
    public static LoadableAsset<AudioClip> Deputy_HighNoon_SFX { get; } = new LoadableAudioResourceAsset($"{Audio}.Deputy_Shoot_SFX.wav");

    // OTHER
    public static LoadableAsset<Sprite> NecronomiconIcon { get; } = new LoadableResourceAsset($"{Other}.NecronomiconNotif.png");
}