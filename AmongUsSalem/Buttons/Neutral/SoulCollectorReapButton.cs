using MiraAPI.GameOptions;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using ObjectWorkshop.Options.Roles.Neutral;
using ObjectWorkshop.Roles.Neutral;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Neutral;

public sealed class SoulCollectorReapButton : ObjectWorkshopRoleButton<SoulCollectorRole, PlayerControl>, IDiseaseableButton,
    IKillButton
{
    public override string Name => "Reap";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => OWColors.SoulCollector;
    public override float Cooldown => OptionGroupSingleton<SoulCollectorOptions>.Instance.KillCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouNeutAssets.ReapSprite;

    public void SetDiseasedTimer(float multiplier)
    {
        SetTimer(Cooldown * multiplier);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<ObjectWorkshopPlugin>.Error("Soul Collector Reap: Target is null");
            return;
        }

        PlayerControl.LocalPlayer.RpcCustomMurder(Target, createDeadBody: false);

        if (Target.Data.IsDead)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{OWColors.SoulCollector.ToTextColor()}You have taken {Target.Data.PlayerName}'s soul from their body, leaving a soulless player behind.</color></b>",
                Color.white, spr: TouRoleIcons.SoulCollector.LoadAsset());

            notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
        }
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }
}