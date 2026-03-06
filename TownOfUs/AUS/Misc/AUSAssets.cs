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
    public static LoadableAsset<Sprite> SurvivorRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.SurvivorRoleCard.png");
    public static LoadableAsset<Sprite> CrusaderRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.CrusaderRoleCard.png");
    public static LoadableAsset<Sprite> GodfatherRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.GodfatherRoleCard.png");
    public static LoadableAsset<Sprite> JesterRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.JesterRoleCard.png");
    public static LoadableAsset<Sprite> BerserkerRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.BerserkerRoleCard.png");
    public static LoadableAsset<Sprite> WarRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.WarRoleCard.png");
    public static LoadableAsset<Sprite> SerialKillerRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.SerialKillerRoleCard.png");
    public static LoadableAsset<Sprite> StarspawnRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.StarspawnRoleCard.png");
    public static LoadableAsset<Sprite> LookoutRoleCard_TOS2 { get; } = new LoadableResourceAsset($"{RoleCard}.LookoutRoleCard.png");
    public static LoadableAsset<Sprite> TrackerRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.TrackerRoleCard.png");
    public static LoadableAsset<Sprite> AgentRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.AgentRoleCard.png");
    public static LoadableAsset<Sprite> WildlingRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.WildlingRoleCard.png");
    public static LoadableAsset<Sprite> WerewolfRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.WerewolfRoleCard.png");
    public static LoadableAsset<Sprite> PlaguebearerRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.PlaguebearerRoleCard.png");
    public static LoadableAsset<Sprite> PestilenceRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.PestilenceRoleCard.png");
    public static LoadableAsset<Sprite> PacifistRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.PacifistRoleCard.png");
    public static LoadableAsset<Sprite> WarlockRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.WarlockRoleCard.png");
    public static LoadableAsset<Sprite> DeathRoleCard { get; } = new LoadableResourceAsset($"{RoleCard}.DeathRoleCard.png");
    public static LoadableAsset<Sprite> LookoutRoleCard_TOS1 { get; } = new LoadableResourceAsset($"{RoleCard}.LookoutRoleCard_TOS1.png");

    // --- ROLE CARDS: MULTIPLE ---
    public static LoadableAsset<Sprite> LookoutRoleCard
    {
        get
        {
            if (OptionGroupSingleton<Lookout_Options>.Instance.Mode == LookoutMode.TOS1) return LookoutRoleCard_TOS1;
            return LookoutRoleCard_TOS2;
        }
    }

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
    public static LoadableAsset<Sprite> Survivor_Vest { get; } = new LoadableResourceAsset($"{Abilities}.Survivor_Vest.png");
    public static LoadableAsset<Sprite> Crusader_Fortify { get; } = new LoadableResourceAsset($"{Abilities}.Crusader_Fortify.png");
    public static LoadableAsset<Sprite> Godfather_Order { get; } = new LoadableResourceAsset($"{Abilities}.Godfather_Order.png");
    public static LoadableAsset<Sprite> Jester_Haunt { get; } = new LoadableResourceAsset($"{Abilities}.Jester_Haunt.png");
    public static LoadableAsset<Sprite> Berserker_Attack { get; } = new LoadableResourceAsset($"{Abilities}.Berserker_Attack.png");
    public static LoadableAsset<Sprite> War_Attack { get; } = new LoadableResourceAsset($"{Abilities}.War_Attack.png");
    public static LoadableAsset<Sprite> SerialKiller_Attack { get; } = new LoadableResourceAsset($"{Abilities}.SerialKiller_Attack.png");
    public static LoadableAsset<Sprite> SerialKiller_Cautious { get; } = new LoadableResourceAsset($"{Abilities}.SerialKiller_Cautious.png");
    public static LoadableAsset<Sprite> Starspawn_Isolate { get; } = new LoadableResourceAsset($"{Abilities}.Starspawn_Isolate.png");
    public static LoadableAsset<Sprite> Starspawn_Daybreak { get; } = new LoadableResourceAsset($"{Abilities}.Starspawn_Daybreak.png");
    public static LoadableAsset<Sprite> Lookout_Watch_TOS2 { get; } = new LoadableResourceAsset($"{Abilities}.Lookout_Watch.png");
    public static LoadableAsset<Sprite> Lookout_Watch_TOS1 { get; } = new LoadableResourceAsset($"{Abilities}.Lookout_Watch_TOS1.png");
    public static LoadableAsset<Sprite> Tracker_Track { get; } = new LoadableResourceAsset($"{Abilities}.Tracker_Track.png");
    public static LoadableAsset<Sprite> Agent_Stalk { get; } = new LoadableResourceAsset($"{Abilities}.Agent_Stalk.png");
    public static LoadableAsset<Sprite> Wildling_Sense { get; } = new LoadableResourceAsset($"{Abilities}.Wildling_Sense.png");
    public static LoadableAsset<Sprite> Werewolf_Maul { get; } = new LoadableResourceAsset($"{Abilities}.Werewolf_Maul.png");
    public static LoadableAsset<Sprite> Werewolf_TrackScent { get; } = new LoadableResourceAsset($"{Abilities}.Werewolf_TrackScent.png");
    public static LoadableAsset<Sprite> Plaguebearer_Infect { get; } = new LoadableResourceAsset($"{Abilities}.Plaguebearer_Infect.png");
    public static LoadableAsset<Sprite> Pestilence_SpreadPestilence { get; } = new LoadableResourceAsset($"{Abilities}.Pestilence_SpreadPestilence.png");
    public static LoadableAsset<Sprite> Pacifist_Rally { get; } = new LoadableResourceAsset($"{Abilities}.Pacifist_Rally.png");
    public static LoadableAsset<Sprite> Pacifist_SelfReflection { get; } = new LoadableResourceAsset($"{Abilities}.Pacifist_SelfReflection.png");
    public static LoadableAsset<Sprite> Warlock_Curse { get; } = new LoadableResourceAsset($"{Abilities}.Warlock_Curse.png");
    public static LoadableAsset<Sprite> Death_Armageddon { get; } = new LoadableResourceAsset($"{Abilities}.Death_Armageddon.png");

    // --- ABILITIES: MULTIPLE ---
    public static LoadableAsset<Sprite> Lookout_Watch
    {
        get
        {
            if (OptionGroupSingleton<Lookout_Options>.Instance.Mode == LookoutMode.TOS1) return Lookout_Watch_TOS1;
            return Lookout_Watch_TOS2;
        }
    }

    // Audio
    public static LoadableAsset<AudioClip> Mayor_Reveal_SFX { get; } = new LoadableAudioResourceAsset($"{Audio}.Mayor_Reveal_SFX.wav");
    public static LoadableAsset<AudioClip> Deputy_HighNoon_SFX { get; } = new LoadableAudioResourceAsset($"{Audio}.Deputy_Shoot_SFX.wav");

    // OTHER
    public static LoadableAsset<Sprite> NecronomiconIcon { get; } = new LoadableResourceAsset($"{Other}.NecronomiconNotif.png");
}