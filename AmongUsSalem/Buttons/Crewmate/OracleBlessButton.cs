using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using AmongUsSalem.Modifiers.Crewmate;
using AmongUsSalem.Options.Roles.Crewmate;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Buttons.Crewmate;

public sealed class OracleBlessButton : AmongUsSalemRoleButton<OracleRole, PlayerControl>
{
    public override string Name => "Bless";
    public override Color TextOutlineColor => AUSColors.Oracle;
    public override string Keybind => Keybinds.SecondaryAction;
    public override float Cooldown => OptionGroupSingleton<OracleOptions>.Instance.BlessCooldown;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.BlessSprite;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.HasModifier<OracleBlessedModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<AUSPlugin>.Error($"{Name}: Target is null");
            return;
        }

        var players = ModifierUtils.GetPlayersWithModifier<OracleBlessedModifier>(x => x.Oracle.AmOwner);
        players.Do(x => x.RpcRemoveModifier<OracleBlessedModifier>());

        Target.RpcAddModifier<OracleBlessedModifier>(PlayerControl.LocalPlayer);
    }
}