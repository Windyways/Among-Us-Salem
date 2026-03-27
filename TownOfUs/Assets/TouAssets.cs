using UnityEngine;

namespace TownOfUs.Assets;

public static class TouAssets
{
    private const string ShortPath = "TownOfUs.Resources";
    private const string CounterPath = "TownOfUs.Resources.AbilityCounters";

    public static readonly AssetBundle MainBundle = AssetBundleManager.Load("tou-assets");

    public static readonly LoadableAsset<GameObject> RoleSelectionGame =
        new LoadableBundleAsset<GameObject>("SelectRoleGame", MainBundle);
    
    public static readonly LoadableAsset<GameObject> AltRoleSelectionGame =
        new LoadableBundleAsset<GameObject>("AmbassadorRoleGame", MainBundle);
    
    public static readonly LoadableAsset<GameObject> ConfirmMinigame =
        new LoadableBundleAsset<GameObject>("AmbassadorConfirmGame", MainBundle);

    public static LoadableAsset<GameObject> WikiPrefab { get; } =
        new LoadableBundleAsset<GameObject>("IngameWiki", MainBundle);

    public static LoadableAsset<GameObject> FirstRoundShield { get; } =
        new LoadableBundleAsset<GameObject>("FirstRoundShield", MainBundle);
    
    public static LoadableAsset<GameObject> AmbushPrefab { get; } =
        new LoadableBundleAsset<GameObject>("Ambush", MainBundle);

    public static LoadableAsset<GameObject> EscapistMarkPrefab { get; } =
        new LoadableBundleAsset<GameObject>("EscapistMark", MainBundle);

    public static LoadableAsset<GameObject> MeetingDeathPrefab { get; } =
        new LoadableBundleAsset<GameObject>("DeathAnimation", MainBundle);

    public static LoadableAsset<GameObject> MayorRevealPrefab { get; set; } =
        new LoadableBundleAsset<GameObject>("MayorReveal", MainBundle);
    
    public static LoadableAsset<GameObject> MayorPostRevealPrefab { get; set; } =
        new LoadableBundleAsset<GameObject>("MayorPostReveal", MainBundle);

    public static LoadableAsset<AnimationClip> MeetingDeathAnim1 { get; } =
        new LoadableBundleAsset<AnimationClip>("DeathMeetingShotFront", MainBundle);

    public static LoadableAsset<AnimationClip> MeetingDeathBloodAnim1 { get; } =
        new LoadableBundleAsset<AnimationClip>("DeathMeetingShotFrontBlood", MainBundle);

    public static LoadableAsset<AnimationClip> MeetingDeathAnim2 { get; } =
        new LoadableBundleAsset<AnimationClip>("DeathMeetingShotRight", MainBundle);

    public static LoadableAsset<AnimationClip> MeetingDeathBloodAnim2 { get; } =
        new LoadableBundleAsset<AnimationClip>("DeathMeetingShotRightBlood", MainBundle);

    public static LoadableAsset<AnimationClip> MeetingDeathAnim3 { get; } =
        new LoadableBundleAsset<AnimationClip>("DeathMeetingBody", MainBundle);

    public static LoadableAsset<AnimationClip> MeetingDeathBloodAnim3 { get; } =
        new LoadableBundleAsset<AnimationClip>("DeathMeetingBodyBlood", MainBundle);

    public static LoadableAsset<GameObject> ScatterUI { get; } =
        new LoadableBundleAsset<GameObject>("ScatterUI", MainBundle);

    public static LoadableAsset<Sprite> Banner { get; } = new LoadableResourceAsset($"{ShortPath}.Banner.png");

    public static LoadableAsset<Sprite> BlankSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.BlankSprite.png");

    public static LoadableAsset<Sprite> WikiButton { get; } = new LoadableResourceAsset($"{ShortPath}.WikiButton.png");

    public static LoadableAsset<Sprite> WikiButtonActive { get; } =
        new LoadableResourceAsset($"{ShortPath}.WikiButtonActive.png");

    public static LoadableAsset<Sprite> ZoomPlus { get; } = new LoadableResourceAsset($"{ShortPath}.Plus.png");
    public static LoadableAsset<Sprite> ZoomMinus { get; } = new LoadableResourceAsset($"{ShortPath}.Minus.png");

    public static LoadableAsset<Sprite> ZoomPlusActive { get; } =
        new LoadableResourceAsset($"{ShortPath}.PlusActive.png");

    public static LoadableAsset<Sprite> ZoomMinusActive { get; } =
        new LoadableResourceAsset($"{ShortPath}.MinusActive.png");

    public static LoadableAsset<Sprite> TeamChatInactive { get; } =
        new LoadableResourceAsset($"{ShortPath}.TeamChatInactive.png");

    public static LoadableAsset<Sprite> TeamChatActive { get; } =
        new LoadableResourceAsset($"{ShortPath}.TeamChatActive.png");

    public static LoadableAsset<Sprite> TeamChatSelected { get; } =
        new LoadableResourceAsset($"{ShortPath}.TeamChatSelected.png");

    public static LoadableAsset<Sprite> KillSprite { get; } = new LoadableResourceAsset($"{ShortPath}.KillButton.png");
    public static LoadableAsset<Sprite> VentSprite { get; } = new LoadableResourceAsset($"{ShortPath}.VentButton.png");
    public static LoadableAsset<Sprite> RangeSprite { get; } = new LoadableResourceAsset($"{ShortPath}.Range.png");

    public static LoadableAsset<Sprite> FootprintSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.Footprint.png");
    
    public static LoadableAsset<Sprite> Hacked { get; } = new LoadableResourceAsset($"{ShortPath}.Hacked.png");

    public static LoadableAsset<Sprite> AuAvengersSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.AuAvengers.png", 290);

    public static LoadableAsset<Sprite> ArrowSprite
    {
        get
        {
            var sprite = ArrowBasicSprite;
            switch (AUSPlugin.ArrowStyle.Value)
            {
                case 1:
                    sprite = ArrowDarkOutSprite;
                    break;
                case 2:
                    sprite = ArrowLightOutSprite;
                    break;
                case 3:
                    sprite = ArrowLegacySprite;
                    break;
            }
            return sprite;
        }
    }
    public static string ArrowSpriteName
    {
        get
        {
            var name = "Default";
            switch (AUSPlugin.ArrowStyle.Value)
            {
                case 1:
                    name = "Dark Glow";
                    break;
                case 2:
                    name = "Color Glow";
                    break;
                case 3:
                    name = "Legacy";
                    break;
            }
            return name;
        }
    }

    public static LoadableAsset<Sprite> ArrowBasicSprite { get; } = new LoadableResourceAsset($"{ShortPath}.Arrow.png", 110);
    public static LoadableAsset<Sprite> ArrowDarkOutSprite { get; } = new LoadableResourceAsset($"{ShortPath}.ArrowDarkOut.png", 110);
    public static LoadableAsset<Sprite> ArrowLightOutSprite { get; } = new LoadableResourceAsset($"{ShortPath}.ArrowLightOut.png", 110);
    
    public static LoadableAsset<Sprite> ArrowLegacySprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.ArrowLegacy.png");

    public static LoadableAsset<Sprite> CrimeSceneSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.CrimeScene.png");

    public static LoadableAsset<Sprite> ScreenFlash { get; } =
        new LoadableResourceAsset($"{ShortPath}.ScreenFlash.png");

    public static LoadableAsset<Sprite> KillBG { get; } = new LoadableResourceAsset($"{ShortPath}.KillBackground.png");

    public static LoadableAsset<Sprite> RetributionBG { get; } =
        new LoadableResourceAsset($"{ShortPath}.RetributionBackground.png");

    public static LoadableAsset<Sprite> AbilityCounterPlayerSprite { get; } =
        new LoadableResourceAsset($"{CounterPath}.Player.png");

    public static LoadableAsset<Sprite> AbilityCounterVentSprite { get; } =
        new LoadableResourceAsset($"{CounterPath}.Vent.png");

    public static LoadableAsset<Sprite> AbilityCounterBodySprite { get; } =
        new LoadableResourceAsset($"{CounterPath}.Body.png");

    public static LoadableAsset<Sprite> AbilityCounterBasicSprite { get; } =
        new LoadableResourceAsset($"{CounterPath}.Basic.png");

    public static LoadableAsset<Sprite> GameSummarySprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.GameSummaryButton.png");

    public static LoadableAsset<Sprite> MapVentSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.MapVent.png", 350);

    public static LoadableAsset<Sprite> MapBodySprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.MapBody.png", 350);

    public static LoadableAsset<Sprite> WikiBgSprite { get; } = new LoadableResourceAsset($"{ShortPath}.WikiBg.png");

    public static LoadableAsset<Sprite> TimerDrawSprite { get; } =
        new LoadableResourceAsset($"{ShortPath}.TimerDraw.png", 300);

    public static LoadableAsset<Sprite> TeamChatSwitch { get; } =
        new LoadableResourceAsset($"{ShortPath}.TeamChatSwitch.png", 105f);

    public static void Initialize()
    {
        AuAvengersAnims.Initialize();
    }
}