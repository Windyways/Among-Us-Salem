using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Modules;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class MirrorcasterMagicMirrorButton : ObjectWorkshopRoleButton<MirrorcasterRole>, IAftermathableButton
{
    public override string Name => "Magic Mirror";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Mirrorcaster;
    public override float Cooldown => OptionGroupSingleton<MirrorcasterOptions>.Instance.MirrorCooldown.Value + MapCooldown + 0.001f;
    public override float EffectDuration => OptionGroupSingleton<MirrorcasterOptions>.Instance.MirrorDuration.Value;
    public override int MaxUses => (int)OptionGroupSingleton<MirrorcasterOptions>.Instance.MaxMirrors;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.MagicMirrorSprite;
    public override ButtonLocation Location => ButtonLocation.BottomRight;
    public override bool ShouldPauseInVent => false;
    public bool TargetWasValid { get; set; }

    public override bool CanUse()
    {
        return base.CanUse() && Role is { Protected: null } && (OptionGroupSingleton<MirrorcasterOptions>.Instance.MultiUnleash || Role.UnleashesAvailable <= 0) && !EffectActive;
    }
    public override void ClickHandler()
    {
        if (!CanUse())
        {
            return;
        }

        OnClick();
    }
    
    protected override void OnClick()
    {
        /*if (!OptionGroupSingleton<GlitchOptions>.Instance.MoveWithMenu)
        {
            PlayerControl.LocalPlayer.NetTransform.Halt();
        }*/

        var playerMenu = CustomPlayerMenu.Create();
        playerMenu.transform.FindChild("PhoneUI").GetChild(0).GetComponent<SpriteRenderer>().material =
            PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
        playerMenu.transform.FindChild("PhoneUI").GetChild(1).GetComponent<SpriteRenderer>().material =
            PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
        playerMenu.Begin(
            plr => (!plr.HasDied() ||
                    Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == plr.PlayerId) ||
                    FakePlayer.FakePlayers.FirstOrDefault(x => x?.body?.name == $"Fake {plr.gameObject.name}")
                        ?.body) && plr != PlayerControl.LocalPlayer,
            plr =>
            {
                playerMenu.ForceClose();

                if (plr != null)
                {
                    MirrorcasterRole.RpcMagicMirror(PlayerControl.LocalPlayer, plr);

                    EffectActive = true;
                    Timer = EffectDuration;
                    OverrideName("Protecting");
                    TargetWasValid = !plr.HasDied();
                }
                else
                {
                    EffectActive = false;
                    Timer = 0.01f;
                }
            });
        foreach (var panel in playerMenu.potentialVictims)
        {
            panel.PlayerIcon.cosmetics.SetPhantomRoleAlpha(1f);
            if (panel.NameText.text != PlayerControl.LocalPlayer.Data.PlayerName)
            {
                panel.NameText.color = Color.white;
            }
        }
    }

    public override void OnEffectEnd()
    {
        if (TargetWasValid)
        {
            DecreaseUses();
        }
        else
        {
            var notif1 = Helpers.CreateAndShowNotification($"<b>The player you tried to protect was already dead!</b>", Color.white, new Vector3(0f, 1f, -20f), spr: TouRoleIcons.Mirrorcaster.LoadAsset());
            notif1.Text.SetOutlineThickness(0.35f);
        }

        if (Role.Protected != null && Role.Protected.HasDied())
        {
            var notif1 = Helpers.CreateAndShowNotification($"<b>{Role.Protected.Data.PlayerName} died to an indirect attack, which your mirrors couldn't protect from.</b>", Color.white, new Vector3(0f, 1f, -20f), spr: TouRoleIcons.Mirrorcaster.LoadAsset());
            notif1.Text.SetOutlineThickness(0.35f);
        }
        else if (Role.Protected != null && !Role.Protected.HasDied())
        {
            var notif1 = Helpers.CreateAndShowNotification($"<b>{Role.Protected.Data.PlayerName} was not attacked.</b>", Color.white, new Vector3(0f, 1f, -20f), spr: TouRoleIcons.Mirrorcaster.LoadAsset());
            notif1.Text.SetOutlineThickness(0.35f);
        }

        TargetWasValid = false;
        MirrorcasterRole.RpcClearMagicMirror(PlayerControl.LocalPlayer);
        OverrideName("Magic Mirror");
    }
}