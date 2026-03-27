using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace TownOfUs.Assets;

public static class TouChatAssets
{
    private const string ChatPath = "TownOfUs.Resources.Chat";

    public static LoadableAsset<Sprite> ImpBubble { get; } = new LoadableResourceAsset($"{ChatPath}.ChatImpBubble.png");
    public static LoadableAsset<Sprite> JailBubble { get; } = new LoadableResourceAsset($"{ChatPath}.ChatJailBubble.png");
    public static LoadableAsset<Sprite> VampBubble { get; } = new LoadableResourceAsset($"{ChatPath}.ChatVampBubble.png");
    public static LoadableAsset<Sprite> TeamChatIdle { get; } = new LoadableResourceAsset($"{ChatPath}.TeamChatIdle.png", 101f);
    public static LoadableAsset<Sprite> TeamChatHover { get; } = new LoadableResourceAsset($"{ChatPath}.TeamChatHover.png", 101f);
    public static LoadableAsset<Sprite> TeamChatOpen { get; } = new LoadableResourceAsset($"{ChatPath}.TeamChatOpen.png", 101f);
    
    public static LoadableAsset<Sprite> NormalBubble { get; } = new LoadableResourceAsset($"{ChatPath}.ChatBubble.png");
    public static LoadableAsset<Sprite> NormalChatIdle { get; } = new LoadableResourceAsset($"{ChatPath}.NormalChatIdle.png", 101f);
    public static LoadableAsset<Sprite> NormalChatHover { get; } = new LoadableResourceAsset($"{ChatPath}.NormalChatHover.png", 101f);
    public static LoadableAsset<Sprite> NormalChatOpen { get; } = new LoadableResourceAsset($"{ChatPath}.NormalChatOpen.png", 101f);

    // --- COVEN ---
    public static LoadableAsset<Sprite> CovenBubble { get; } = new LoadableResourceAsset($"{ChatPath}.ChatCovenBubble.png");
    public static LoadableAsset<Sprite> CovenChatIdle { get; } = new LoadableResourceAsset($"{ChatPath}.CovenChatIdle.png", 101f);
    public static LoadableAsset<Sprite> CovenChatHover { get; } = new LoadableResourceAsset($"{ChatPath}.CovenChatHover.png", 101f);
    public static LoadableAsset<Sprite> CovenChatOpen { get; } = new LoadableResourceAsset($"{ChatPath}.CovenChatOpen.png", 101f);
}
