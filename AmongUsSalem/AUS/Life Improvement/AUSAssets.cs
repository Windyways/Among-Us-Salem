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
    private const string Icon = $"AmongUsSalem.Resources.Sprites.Icons";

    // Abilities
    public static LoadableAsset<Sprite> NecronomiconButton { get; } = new LoadableResourceAsset($"{Ability}.NecronomiconButton.png");
    public static LoadableAsset<Sprite> NecroPassing_PassNecronomicon { get; } = new LoadableResourceAsset($"{Ability}.NecroPassing_PassNecronomicon.png");

    public static LoadableAsset<Sprite> Mafioso_Attack { get; } = new LoadableResourceAsset($"{Ability}.Mafioso_Attack.png");
    public static LoadableAsset<Sprite> Sheriff_Search { get; } = new LoadableResourceAsset($"{Ability}.Sheriff_Search.png");
    public static LoadableAsset<Sprite> Veteran_Alert { get; } = new LoadableResourceAsset($"{Ability}.Veteran_Alert.png");
    public static LoadableAsset<Sprite> Framer_Frame { get; } = new LoadableResourceAsset($"{Ability}.Framer_Frame.png");
    public static LoadableAsset<Sprite> Conjurer_Conjure { get; } = new LoadableResourceAsset($"{Ability}.Conjurer_Conjure.png");
    public static LoadableAsset<Sprite> Bodyguard_Guard { get; } = new LoadableResourceAsset($"{Ability}.Bodyguard_Guard.png");
    public static LoadableAsset<Sprite> Bodyguard_SelfProtect { get; } = new LoadableResourceAsset($"{Ability}.Bodyguard_SelfProtect.png");
    public static LoadableAsset<Sprite> Mayor_Reveal { get; } = new LoadableResourceAsset($"{Ability}.Mayor_Reveal.png");
    public static LoadableAsset<Sprite> Illusionist_Cast { get; } = new LoadableResourceAsset($"{Ability}.Illusionist_Cast.png");
    public static LoadableAsset<Sprite> HexMaster_Hex { get; } = new LoadableResourceAsset($"{Ability}.HexMaster_Hex.png");
    public static LoadableAsset<Sprite> Blackmailer_Blackmail { get; } = new LoadableResourceAsset($"{Ability}.Blackmailer_Blackmail.png");
    public static LoadableAsset<Sprite> Godfather_Attack { get; } = new LoadableResourceAsset($"{Ability}.Godfather_Attack.png");
    public static LoadableAsset<Sprite> VoodooMaster_Voodoo { get; } = new LoadableResourceAsset($"{Ability}.VoodooMaster_Voodoo.png");
    public static LoadableAsset<Sprite> Amnesiac_Remember { get; } = new LoadableResourceAsset($"{Ability}.Amnesiac_Remember.png");
    public static LoadableAsset<Sprite> Arsonist_Douse { get; } = new LoadableResourceAsset($"{Ability}.Arsonist_Douse.png");
    public static LoadableAsset<Sprite> Arsonist_Ignite { get; } = new LoadableResourceAsset($"{Ability}.Arsonist_Ignite.png");
    public static LoadableAsset<Sprite> Shroud_Attack { get; } = new LoadableResourceAsset($"{Ability}.Shroud_Attack.png");
    public static LoadableAsset<Sprite> Shroud_Shroud { get; } = new LoadableResourceAsset($"{Ability}.Shroud_Shroud.png");
    public static LoadableAsset<Sprite> Investigator_Investigate { get; } = new LoadableResourceAsset($"{Ability}.Investigator_Investigate.png");
    public static LoadableAsset<Sprite> Vampire_Drain { get; } = new LoadableResourceAsset($"{Ability}.Vampire_Drain.png");
    public static LoadableAsset<Sprite> Vampire_Convert { get; } = new LoadableResourceAsset($"{Ability}.Vampire_Convert.png");
    public static LoadableAsset<Sprite> Deputy_Shoot { get; } = new LoadableResourceAsset($"{Ability}.Deputy_HighNoon.png");
    public static LoadableAsset<Sprite> Crusader_Fortify { get; } = new LoadableResourceAsset($"{Ability}.Crusader_Fortify.png");
    public static LoadableAsset<Sprite> Ambusher_Ambush { get; } = new LoadableResourceAsset($"{Ability}.Ambusher_Ambush.png");
    public static LoadableAsset<Sprite> Jackal_Assassinate { get; } = new LoadableResourceAsset($"{Ability}.Jackal_Assassinate.png");
    public static LoadableAsset<Sprite> Survivor_Vest { get; } = new LoadableResourceAsset($"{Ability}.Survivor_Vest.png");
    public static LoadableAsset<Sprite> Jinx_Jinx { get; } = new LoadableResourceAsset($"{Ability}.Jinx_Jinx.png");
    public static LoadableAsset<Sprite> Vigilante_Shoot { get; } = new LoadableResourceAsset($"{Ability}.Vigilante_Shoot.png");
    public static LoadableAsset<Sprite> Escort_Distract { get; } = new LoadableResourceAsset($"{Ability}.Escort_Distract.png");
    public static LoadableAsset<Sprite> Cleric_Barrier { get; } = new LoadableResourceAsset($"{Ability}.Cleric_Barrier.png");
    public static LoadableAsset<Sprite> Cleric_SelfBarrier { get; } = new LoadableResourceAsset($"{Ability}.Cleric_SelfBarrier.png");
    public static LoadableAsset<Sprite> Coroner_Autopsy { get; } = new LoadableResourceAsset($"{Ability}.Coroner_Autopsy.png");
    public static LoadableAsset<Sprite> Coroner_Examine { get; } = new LoadableResourceAsset($"{Ability}.Coroner_Examine.png");
    public static LoadableAsset<Sprite> Prosecutor_Prosecute { get; } = new LoadableResourceAsset($"{Ability}.Prosecutor_Prosecute.png");
    public static LoadableAsset<Sprite> Janitor_Clean { get; } = new LoadableResourceAsset($"{Ability}.Janitor_Clean.png");

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
    public static LoadableAsset<Sprite> HexMasterRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.HexMasterRoleCard.png");
    public static LoadableAsset<Sprite> BlackmailerRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.BlackmailerRoleCard.png");
    public static LoadableAsset<Sprite> GodfatherRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.GodfatherRoleCard.png");
    public static LoadableAsset<Sprite> VoodooMasterRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.VoodooMasterRoleCard.png");
    public static LoadableAsset<Sprite> ArsonistRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.ArsonistRoleCard.png");
    public static LoadableAsset<Sprite> ShroudRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.ShroudRoleCard.png");
    public static LoadableAsset<Sprite> InvestigatorRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.InvestigatorRoleCard.png");
    public static LoadableAsset<Sprite> VampireRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.VampireRoleCard.png");
    public static LoadableAsset<Sprite> DeputyRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.DeputyRoleCard.png");
    public static LoadableAsset<Sprite> CrusaderRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.CrusaderRoleCard.png");
    public static LoadableAsset<Sprite> AmbusherRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.AmbusherRoleCard.png");
    public static LoadableAsset<Sprite> JackalRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.JackalRoleCard.png");
    public static LoadableAsset<Sprite> SurvivorRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.SurvivorRoleCard.png");
    public static LoadableAsset<Sprite> JinxRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.JinxRoleCard.png");
    public static LoadableAsset<Sprite> VigilanteRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.VigilanteRoleCard.png");
    public static LoadableAsset<Sprite> EscortRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.EscortRoleCard.png");
    public static LoadableAsset<Sprite> ClericRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.ClericRoleCard.png");
    public static LoadableAsset<Sprite> CoronerRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.CoronerRoleCard.png");
    public static LoadableAsset<Sprite> ProsecutorRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.ProsecutorRoleCard.png");
    public static LoadableAsset<Sprite> JanitorRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.JanitorRoleCard.png");

    // Icons
    public static LoadableAsset<Sprite> CovenVIPIcon { get; } = new LoadableResourceAsset($"{Icon}.CovenVIPIcon.png");

    // Audio
    public static LoadableAsset<AudioClip> Mayor_Reveal_SFX { get; } = new LoadableAudioResourceAsset($"{Audio}.Mayor_Reveal_SFX.wav");
    public static LoadableAsset<AudioClip> TownWin_SFX { get; } = new LoadableAudioResourceAsset($"{Audio}.TownWin_SFX.wav");
    public static LoadableAsset<AudioClip> CovenWin_SFX { get; } = new LoadableAudioResourceAsset($"{Audio}.CovenWin_SFX.wav");
    public static LoadableAsset<AudioClip> ArsonistWin_SFX { get; } = new LoadableAudioResourceAsset($"{Audio}.ArsonistWin_SFX.wav");
    public static LoadableAsset<AudioClip> ShroudWin_SFX { get; } = new LoadableAudioResourceAsset($"{Audio}.ArsonistWin_SFX.wav");
    public static LoadableAsset<AudioClip> Deputy_Shoot_SFX { get; } = new LoadableAudioResourceAsset($"{Audio}.Deputy_Shoot_SFX.wav");


    // Other
    public static LoadableAsset<Sprite> Banner { get; } = new LoadableResourceAsset($"{Other}.AmongUsSalemBanner.png");
    public static LoadableAsset<Sprite> Attributes { get; } = new LoadableResourceAsset($"{Other}.Attributes.png");
    public static LoadableAsset<Sprite> CovenAttributes { get; } = new LoadableResourceAsset($"{Other}.CovenAttributes.png");
    public static LoadableAsset<Sprite> Necronomicon { get; } = new LoadableResourceAsset($"{Other}.Necronomicon.png");



    public static LoadableAsset<Sprite> Placeholder { get; } = new LoadableResourceAsset($"AmongUsSalem.Resources.RoleIcons.RandomAny.png");
    public static LoadableAsset<Sprite> KillSprite { get; } = new LoadableResourceAsset($"{ShortPath}.KillButton.png");
    
    public static void PlaySound(LoadableAsset<AudioClip> clip, float vol = 1f)
    {
        if (Constants.ShouldPlaySfx())
        {
            SoundManager.Instance.PlaySound(clip.LoadAsset(), false, vol);
        }
    }
}