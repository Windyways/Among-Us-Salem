using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace ObjectWorkshop.LifeImprovement;

public static class OWAssets
{
    private const string ShortPath = "ObjectWorkshop.Resources";
    private const string ButtonPath = $"{ShortPath}.CrewButtons";

    private const string Ability = $"ObjectWorkshop.Resources.Sprites.Abilities";
    private const string RoleCard = $"ObjectWorkshop.Resources.Sprites.RoleCards";
    private const string Object = $"ObjectWorkshop.Resources.Sprites.Objects";
    private const string Other = $"ObjectWorkshop.Resources.Sprites.Other";
    private const string Audio = $"ObjectWorkshop.Resources.Sprites.Sfx";

    // Abilities
    public static LoadableAsset<Sprite> Alarum_Activate { get; } = new LoadableResourceAsset($"{Ability}.Alarum_Activate.png");
    public static LoadableAsset<Sprite> Duelist_Duel { get; } = new LoadableResourceAsset($"{Ability}.Duelist_Duel.png");
    public static LoadableAsset<Sprite> Duelist_Sharpen { get; } = new LoadableResourceAsset($"{Ability}.Duelist_Sharpen.png");
    public static LoadableAsset<Sprite> UFO_Destination { get; } = new LoadableResourceAsset($"{Ability}.UFO_Destination.png");
    public static LoadableAsset<Sprite> UFO_Abduct { get; } = new LoadableResourceAsset($"{Ability}.UFO_Abduct.png");
    public static LoadableAsset<Sprite> Totemist_Install { get; } = new LoadableResourceAsset($"{Ability}.Totemist_Install.png");
    public static LoadableAsset<Sprite> Totemist_Watch { get; } = new LoadableResourceAsset($"{Ability}.Totemist_Watch.png");
    public static LoadableAsset<Sprite> Totemist_Cycle { get; } = new LoadableResourceAsset($"{Ability}.Totemist_Cycle.png");
    public static LoadableAsset<Sprite> Canopy_Cast { get; } = new LoadableResourceAsset($"{Ability}.Canopy_Cast.png");
    public static LoadableAsset<Sprite> Enticer_Prepare { get; } = new LoadableResourceAsset($"{Ability}.Enticer_Prepare.png");
    public static LoadableAsset<Sprite> Pyre_Ignite { get; } = new LoadableResourceAsset($"{Ability}.Pyre_Ignite.png");
    public static LoadableAsset<Sprite> Reaper_Attack { get; } = new LoadableResourceAsset($"{Ability}.Reaper_Attack.png");
    public static LoadableAsset<Sprite> Reaper_Catastrophe { get; } = new LoadableResourceAsset($"{Ability}.Reaper_Catastrophe.png");
    public static LoadableAsset<Sprite> Obstructor_Barricade { get; } = new LoadableResourceAsset($"{Ability}.Obstructor_Barricade.png");
    public static LoadableAsset<Sprite> Aimsman_Aim { get; } = new LoadableResourceAsset($"{Ability}.Aimsman_Aim.png");
    public static LoadableAsset<Sprite> Aimsman_Fire { get; } = new LoadableResourceAsset($"{Ability}.Aimsman_Fire.png");
    public static LoadableAsset<Sprite> GiftWeaver_Build { get; } = new LoadableResourceAsset($"{Ability}.GiftWeaver_Build.png");
    public static LoadableAsset<Sprite> GiftWeaver_Package { get; } = new LoadableResourceAsset($"{Ability}.GiftWeaver_Package.png");
    public static LoadableAsset<Sprite> Oasis_Sanctify { get; } = new LoadableResourceAsset($"{Ability}.Oasis_Sanctify.png");
    public static LoadableAsset<Sprite> Oasis_Sandstorm { get; } = new LoadableResourceAsset($"{Ability}.Oasis_Sandstorm.png");
    public static LoadableAsset<Sprite> Peacock_Declare { get; } = new LoadableResourceAsset($"{Ability}.Peacock_Declare.png");
    public static LoadableAsset<Sprite> Peacock_Bloom { get; } = new LoadableResourceAsset($"{Ability}.Peacock_Bloom.png");

    // Role Cards
    public static LoadableAsset<Sprite> Duelist { get; } = new LoadableResourceAsset($"{RoleCard}.Duelist.png");
    public static LoadableAsset<Sprite> UFO { get; } = new LoadableResourceAsset($"{RoleCard}.UFO.png");
    public static LoadableAsset<Sprite> Crewmate { get; } = new LoadableResourceAsset($"{RoleCard}.Crewmate.png");
    public static LoadableAsset<Sprite> Obstructor { get; } = new LoadableResourceAsset($"{RoleCard}.Obstructor.png");
    public static LoadableAsset<Sprite> Canopy { get; } = new LoadableResourceAsset($"{RoleCard}.Canopy.png");
    public static LoadableAsset<Sprite> Reaper { get; } = new LoadableResourceAsset($"{RoleCard}.Reaper.png");
    public static LoadableAsset<Sprite> Aimsman { get; } = new LoadableResourceAsset($"{RoleCard}.Aimsman.png");
    public static LoadableAsset<Sprite> Pyre { get; } = new LoadableResourceAsset($"{RoleCard}.Pyre.png");
    public static LoadableAsset<Sprite> Totemist { get; } = new LoadableResourceAsset($"{RoleCard}.Totemist.png");
    public static LoadableAsset<Sprite> Infiltrator { get; } = new LoadableResourceAsset($"{RoleCard}.Infiltrator.png");
    public static LoadableAsset<Sprite> GiftWeaver { get; } = new LoadableResourceAsset($"{RoleCard}.GiftWeaver.png");
    public static LoadableAsset<Sprite> Oasis { get; } = new LoadableResourceAsset($"{RoleCard}.Oasis.png");
    public static LoadableAsset<Sprite> Prognosticator { get; } = new LoadableResourceAsset($"{RoleCard}.Prognosticator.png");
    public static LoadableAsset<Sprite> Alarum { get; } = new LoadableResourceAsset($"{RoleCard}.Alarum.png");
    public static LoadableAsset<Sprite> Luminescence { get; } = new LoadableResourceAsset($"{RoleCard}.Luminescence.png");

    // Objects
    public static LoadableAsset<Sprite> Bubble { get; } = new LoadableResourceAsset($"{Object}.Bubble.png");
    public static LoadableAsset<Sprite> FillerCircle { get; } = new LoadableResourceAsset($"{Object}.FillerCircle.png");

    public static LoadableAsset<Sprite> AlarmClock { get; } = new LoadableResourceAsset($"{Object}.Alarum_Object_AlarmClock.png");
    public static LoadableAsset<Sprite> SwordSprite { get; } = new LoadableResourceAsset($"{Object}.Duelist_Object_Sword.png");
    public static LoadableAsset<Sprite> MoonSprite { get; } = new LoadableResourceAsset($"{Object}.UFO_Object_Moon.png");
    public static LoadableAsset<Sprite> TotemSprite { get; } = new LoadableResourceAsset($"{Object}.Totemist_Object_Totem.png");
    public static LoadableAsset<Sprite> DarkCloudSprite { get; } = new LoadableResourceAsset($"{Object}.Canopy_Object_DarkCloud.png");
    public static LoadableAsset<Sprite> FlamesSprite { get; } = new LoadableResourceAsset($"{Object}.Pyre_Object_FireSprite.png");
    public static LoadableAsset<Sprite> ReaperSprite { get; } = new LoadableResourceAsset($"{Object}.UndeadReaper_Object_ReaperIcon.png");
    public static LoadableAsset<Sprite> BarricadeSprite { get; } = new LoadableResourceAsset($"{Object}.Obstructor_Object_Barricade.png");
    public static LoadableAsset<Sprite> SnipeCrosshair { get; } = new LoadableResourceAsset($"{Object}.Aimsman_Object_Crosshair.png");
    public static LoadableAsset<Sprite> CrateSprite { get; } = new LoadableResourceAsset($"{Object}.GiftWeaver_Object_Crate.png");
    public static LoadableAsset<Sprite> GooPool { get; } = new LoadableResourceAsset($"{Object}.Claylim_Object_GooPool.png");
    public static LoadableAsset<Sprite> PeacockVisual { get; } = new LoadableResourceAsset($"{Object}.Peacock_Object_PeacockVisual.png");
    
    public static LoadableAsset<Sprite> SandOverlay { get; } = new LoadableResourceAsset($"{Object}.Oasis_Object_SandOverlay.png");
    public static LoadableAsset<Sprite> SandParticle1 { get; } = new LoadableResourceAsset($"{Object}.Oasis_Object_SandParticle1.png");
    public static LoadableAsset<Sprite> SandParticle2 { get; } = new LoadableResourceAsset($"{Object}.Oasis_Object_SandParticle2.png");

    // Audio
    public static LoadableAsset<AudioClip> DuelBegin_SFX { get; } = new LoadableAudioResourceAsset($"{Audio}.DuelBegin_SFX.wav");
    public static LoadableAsset<AudioClip> DuelKill_SFX { get; } = new LoadableAudioResourceAsset($"{Audio}.DuelKill_SFX.wav");
    public static LoadableAsset<AudioClip> Duelist_Sharpen_SFX { get; } = new LoadableAudioResourceAsset($"{Audio}.Duelist_Sharpen_SFX.wav");
    public static LoadableAsset<AudioClip> Abduct_SFX { get; } = new LoadableAudioResourceAsset($"{Audio}.Abduct_SFX.wav");
    public static LoadableAsset<AudioClip> Build_SFX { get; } = new LoadableAudioResourceAsset($"{Audio}.Build_SFX.wav");
    

    // Other
    public static LoadableAsset<Sprite> Banner { get; } = new LoadableResourceAsset($"{Other}.ObjectWorkshopBanner.png");
    public static LoadableAsset<Sprite> Placeholder { get; } = new LoadableResourceAsset($"{ButtonPath}.MediateButton.png");
    public static LoadableAsset<Sprite> KillSprite { get; } = new LoadableResourceAsset($"{ShortPath}.KillButton.png");
}