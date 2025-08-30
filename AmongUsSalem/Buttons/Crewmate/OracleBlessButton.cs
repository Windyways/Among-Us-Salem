using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using ObjectWorkshop.Modifiers.Crewmate;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class OracleBlessButton : ObjectWorkshopRoleButton<OracleRole, PlayerControl>
{
    public override string Name => "Bless";
    public override Color TextOutlineColor => OWColors.Oracle;
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
            Logger<ObjectWorkshopPlugin>.Error($"{Name}: Target is null");
            return;
        }

        var players = ModifierUtils.GetPlayersWithModifier<OracleBlessedModifier>(x => x.Oracle.AmOwner);
        players.Do(x => x.RpcRemoveModifier<OracleBlessedModifier>());

        Target.RpcAddModifier<OracleBlessedModifier>(PlayerControl.LocalPlayer);
    }
}