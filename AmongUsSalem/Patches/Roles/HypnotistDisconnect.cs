

using HarmonyLib;
using MiraAPI.Modifiers;
using ObjectWorkshop.Modifiers.Impostor;
using ObjectWorkshop.Roles.Impostor;

namespace ObjectWorkshop.Patches.Roles;

[HarmonyPatch(typeof(GameData))]
public static class DisconnectHandler
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameData.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
    public static void Prefix([HarmonyArgument(0)] PlayerControl player)
    {
        if (player.Data.Role is HypnotistRole hypno)
        {
            ModifierUtils.GetPlayersWithModifier<HypnotisedModifier>(x => x.Hypnotist == hypno)
                         .Do(x => x.RemoveModifier<HypnotisedModifier>());
        }   
    }
}

