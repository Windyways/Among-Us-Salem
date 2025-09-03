using HarmonyLib;
using MiraAPI.GameOptions;
using AmongUsSalem.Options;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AmongUsSalem.Patches.Options;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
public static class DeadSeeVoteColorsPatch
{
    public static bool Prefix(MeetingHud __instance, [HarmonyArgument(0)] NetworkedPlayerInfo voterPlayer,
        [HarmonyArgument(1)] int index, [HarmonyArgument(2)] Transform parent)
    {
        var spriteRenderer = Object.Instantiate(__instance.PlayerVotePrefab);
        var player = MiscUtils.PlayerById(voterPlayer.PlayerId);
        if (Debugger.IsDebuggerActive && player != null)
        {
            if (player.Is(Faction.Town)) PlayerMaterial.SetColors(AUSColors.Town, spriteRenderer);
            if (player.Is(Faction.Neutral)) PlayerMaterial.SetColors(AUSColors.Neutral, spriteRenderer);
            if (player.Is(Faction.Mafia)) PlayerMaterial.SetColors(AUSColors.Mafia, spriteRenderer);
            if (player.Is(Faction.Coven)) PlayerMaterial.SetColors(AUSColors.Coven, spriteRenderer);
            if (player.Is(Faction.Traitor)) PlayerMaterial.SetColors(AUSColors.Traitor, spriteRenderer);
        }
        else if (GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes && (!OptionGroupSingleton<GeneralOptions>.Instance.TheDeadKnow || !PlayerControl.LocalPlayer.Data.IsDead))
        {
            PlayerMaterial.SetColors(Palette.DisabledGrey, spriteRenderer);
        }
        else
        {
            PlayerMaterial.SetColors(voterPlayer.DefaultOutfit.ColorId, spriteRenderer);
        }

        spriteRenderer.transform.SetParent(parent);
        spriteRenderer.transform.localScale = Vector3.zero;
        var component = parent.GetComponent<PlayerVoteArea>();
        if (component != null)
        {
            spriteRenderer.material.SetInt(PlayerMaterial.MaskLayer, component.MaskLayer);
        }

        __instance.StartCoroutine(Effects.Bloop(index * 0.3f, spriteRenderer.transform));
        parent.GetComponent<VoteSpreader>().AddVote(spriteRenderer);
        return false;
    }
}