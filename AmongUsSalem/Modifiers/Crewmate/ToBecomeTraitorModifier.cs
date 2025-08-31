using AmongUs.GameOptions;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using AmongUsSalem.Modifiers.Game.Alliance;
using AmongUsSalem.Modifiers.Game.Impostor;
using AmongUsSalem.Modifiers.Neutral;
using AmongUsSalem.Options;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Roles.Impostor;
using AmongUsSalem.Utilities;
using UnityEngine;
using Random = System.Random;

namespace AmongUsSalem.Modifiers.Crewmate;

public sealed class ToBecomeTraitorModifier : ExcludedGameModifier, IAssignableTargets
{
    public override string ModifierName => "Possible Traitor";
    public override bool HideOnUi => true;

    public int Priority { get; set; } = 3;
    public override Color FreeplayFileColor => new Color32(255, 25, 25, 255);

    public void AssignTargets()
    {
        if (GameOptionsManager.Instance.CurrentGameOptions.RoleOptions
            .GetNumPerGame((RoleTypes)RoleId.Get<TraitorRole>()) == 0)
        {
            return;
        }

        Random rnd = new();
        var chance = rnd.Next(1, 101);

        if (chance <=
            GameOptionsManager.Instance.CurrentGameOptions.RoleOptions.GetChancePerGame(
                (RoleTypes)RoleId.Get<TraitorRole>()))
        {
            var filtered = PlayerControl.AllPlayerControls.ToArray()
                .Where(x => x.Is(ModdedRoleTeams.Crewmate) &&
                            !x.Data.IsDead &&
                            !x.Data.Disconnected &&
                            !x.HasModifier<ExecutionerTargetModifier>() &&
                            !x.HasModifier<EgotistModifier>() &&
                            x.Data.Role is not MayorRole).ToList();

            if (filtered.Count == 0)
            {
                return;
            }

            var randomTarget = filtered[rnd.Next(0, filtered.Count)];

            randomTarget.RpcAddModifier<ToBecomeTraitorModifier>();
        }
    }

    public override int GetAmountPerGame()
    {
        return 0;
    }

    public override int GetAssignmentChance()
    {
        return 0;
    }

    public void Clear()
    {
        AssignTargets();
        ModifierComponent?.RemoveModifier(this);
    }

    [MethodRpc((uint)AUSRpc.SetTraitor, SendImmediately = true)]
    public static void RpcSetTraitor(PlayerControl player)
    {
        if (!player.HasModifier<ToBecomeTraitorModifier>())
        {
            return;
        }

        player.ChangeRole(RoleId.Get<TraitorRole>());
        player.RemoveModifier<ToBecomeTraitorModifier>();

        if (OptionGroupSingleton<AssassinOptions>.Instance.TraitorCanAssassin)
        {
            player.AddModifier<ImpostorAssassinModifier>();
        }

        CustomRoleUtils.GetActiveRolesOfType<SnitchRole>().ToList()
            .ForEach(snitch => snitch.AddSnitchTraitorArrows());
    }
}