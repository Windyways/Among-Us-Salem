using System.Text;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Modifiers.Crewmate;
using ObjectWorkshop.Modifiers.Impostor;
using ObjectWorkshop.Options.Roles.Crewmate;
using ObjectWorkshop.Roles;
using ObjectWorkshop.Roles.Crewmate;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Buttons.Crewmate;

public sealed class SeerRevealButton : ObjectWorkshopRoleButton<SeerRole, PlayerControl>
{
    public override string Name => "Reveal";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Seer;
    public override float Cooldown => OptionGroupSingleton<SeerOptions>.Instance.SeerCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.SeerSprite;

    public override bool IsTargetValid(PlayerControl? target)
    {
        return base.IsTargetValid(target) && !target!.HasModifier<SeerGoodRevealModifier>() &&
               !target!.HasModifier<SeerEvilRevealModifier>();
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        RevealAlliance(Target);
        TouAudio.PlaySound(TouAudio.QuestionSound);

        Target?.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(OWColors.Seer));
    }

    public static void RevealAlliance(PlayerControl target)
    {
        var options = OptionGroupSingleton<SeerOptions>.Instance;
        var possibleAlignment = new StringBuilder();

        if (IsEvil(target))
        {
            target.AddModifier<SeerEvilRevealModifier>();
            var possiblyGood = options.ShowCrewmateKillingAsRed ? "possibly" : string.Empty;
            if (options.ShowNeutralBenignAsRed)
            {
                possiblyGood = "possibly";
            }

            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{OWColors.Infiltrator.ToTextColor()}You have revealed that {target.Data.PlayerName} is {possiblyGood} evil!</color></b>",
                Color.white, spr: TouRoleIcons.Seer.LoadAsset());
            notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);

            if (options.ShowCrewmateKillingAsRed)
            {
                possibleAlignment.Append("Crew Killer, ");
            }

            if (options.ShowNeutralBenignAsRed)
            {
                possibleAlignment.Append("Neutral Benign, ");
            }

            if (options.ShowNeutralEvilAsRed)
            {
                possibleAlignment.Append("Neutral Evil, ");
            }

            if (options.ShowNeutralKillingAsRed)
            {
                possibleAlignment.Append("Neutral Killer, ");
            }

            if (options.SwapTraitorColors)
            {
                possibleAlignment.Append("Traitor, ");
            }

            if (possibleAlignment.Length > 3)
            {
                possibleAlignment = possibleAlignment.Remove(possibleAlignment.Length - 2, 2);
            }

            var impString = possibleAlignment.Length > 1 ? ", or Impostor!" : "Impostor!";
            possibleAlignment.Append(impString);

            Helpers.CreateAndShowNotification($"They must be a {possibleAlignment}", OWColors.Infiltrator);
        }
        else
        {
            target.AddModifier<SeerGoodRevealModifier>();
            var possiblyGood = !options.ShowNeutralBenignAsRed ? "likely" : string.Empty;
            if (!options.ShowNeutralEvilAsRed)
            {
                possiblyGood = "probably";
            }

            if (!options.ShowNeutralKillingAsRed)
            {
                possiblyGood = "possibly";
            }

            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{Palette.CrewmateBlue.ToTextColor()}You have revealed that {target.Data.PlayerName} is {possiblyGood} good!</color></b>",
                Color.white, spr: TouRoleIcons.Seer.LoadAsset());
            notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);

            if (!options.ShowNeutralBenignAsRed)
            {
                possibleAlignment.Append("Neutral Benign, ");
            }

            if (!options.ShowNeutralEvilAsRed)
            {
                possibleAlignment.Append("Neutral Evil, ");
            }

            if (!options.ShowNeutralKillingAsRed)
            {
                possibleAlignment.Append("Neutral Killer, ");
            }

            if (possibleAlignment.Length > 3)
            {
                possibleAlignment = possibleAlignment.Remove(possibleAlignment.Length - 2, 2);
            }

            var impString = possibleAlignment.Length > 1 ? ", or Crewmate!" : "Crewmate!";
            possibleAlignment.Append(impString);
            var notif2 =
                Helpers.CreateAndShowNotification($"<b>They must be a {possibleAlignment}</b>", Palette.CrewmateBlue);
            notif2.Text.SetOutlineThickness(0.35f);
            notif2.transform.localPosition = new Vector3(0f, 1f, -20f);
        }
    }

    public static bool IsEvil(PlayerControl target)
    {
        var options = OptionGroupSingleton<SeerOptions>.Instance;
        return !target.HasModifier<ImitatorCacheModifier>() &&
               ((target.Is(RoleAlignment.CrewmateKilling) && options.ShowCrewmateKillingAsRed) ||
                (target.Is(RoleAlignment.NeutralAssociative) && options.ShowNeutralBenignAsRed) ||
                (target.Is(RoleAlignment.NeutralEvil) && options.ShowNeutralEvilAsRed) ||
                (target.Is(RoleAlignment.NeutralPredator) && options.ShowNeutralKillingAsRed) ||
                (target.IsImpostor() && !target.HasModifier<TraitorCacheModifier>()) ||
                (target.HasModifier<TraitorCacheModifier>() && options.SwapTraitorColors));
    }
}