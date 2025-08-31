using MiraAPI.Events;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using AmongUsSalem.Events.TouEvents;
using AmongUsSalem.Utilities;
using AmongUsSalem.Utilities.Appearances;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AmongUsSalem.Modifiers.Impostor;

public sealed class HypnotisedModifier(PlayerControl hypnotist) : BaseModifier
{
    private List<PlayerControl> players = [];
    public override string ModifierName => "Hypnotised";
    public override bool HideOnUi => true;
    public PlayerControl Hypnotist { get; } = hypnotist;

    public bool HysteriaActive { get; set; }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnActivate()
    {
        base.OnActivate();
        var touAbilityEvent = new TouAbilityEvent(AbilityType.HypnotistHypno, Hypnotist, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
    }

    public override void OnDeactivate()
    {
        UnHysteria();

        players.Clear();
    }

    public void Hysteria()
    {
        if (Player.HasDied())
        {
            return;
        }

        var touAbilityEvent = new TouAbilityEvent(AbilityType.HypnotistHysteria, Hypnotist, Player);
        MiraEventManager.InvokeEvent(touAbilityEvent);
        if (!Player.AmOwner)
        {
            return;
        }

        if (HysteriaActive)
        {
            return;
        }

        // Logger<AUSPlugin>.Message($"HypnotisedModifier.Hysteria - {Player.Data.PlayerName}");
        players = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x != Player).ToList();

        foreach (var player in players)
        {
            var hidden = Random.RandomRangeInt(0, 3);
            if (hidden == 0)
            {
                var morph = new VisualAppearance(Player.GetDefaultModifiedAppearance(), AmongUsSalemAppearances.Morph);

                player?.RawSetAppearance(morph);
            }
            else if (hidden == 1)
            {
                player.SetCamouflage();
            }
            else
            {
                var swoop = new VisualAppearance(player.GetDefaultModifiedAppearance(), AmongUsSalemAppearances.Swooper)
                {
                    HatId = string.Empty,
                    SkinId = string.Empty,
                    VisorId = string.Empty,
                    PlayerName = string.Empty,
                    PetId = string.Empty,
                    RendererColor = Color.clear,
                    NameColor = Color.clear,
                    ColorBlindTextColor = Color.clear
                };

                player.RawSetAppearance(swoop);
            }

            player?.cosmetics.ToggleNameVisible(false);
        }

        if (Player.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{AUSColors.Mafia.ToTextColor()}You are under a Mass Hysteria!</color></b>", Color.white,
                spr: TouRoleIcons.Hypnotist.LoadAsset());

            notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
        }

        HysteriaActive = true;
    }

    public void UnHysteria()
    {
        if (!Player.AmOwner)
        {
            return;
        }

        if (!HysteriaActive)
        {
            return;
        }

        // Logger<AUSPlugin>.Message($"HypnotisedModifier.UnHysteria - {Player.Data.PlayerName}");
        foreach (var player in players)
        {
            Player.ResetAppearance();
            player?.cosmetics.ToggleNameVisible(true);
        }

        HysteriaActive = false;
    }
}