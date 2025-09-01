using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using AmongUsSalem.Modifiers;
using AmongUsSalem.Modifiers.Crewmate;
using AmongUsSalem.Modifiers.Game.Alliance;
using AmongUsSalem.Modifiers.Impostor;
using AmongUsSalem.Modifiers.Neutral;
using AmongUsSalem.Modules;
using AmongUsSalem.Options;
using AmongUsSalem.Options.Roles.Neutral;
using AmongUsSalem.Roles;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Roles.Impostor;
using AmongUsSalem.Roles.Neutral;
using UnityEngine;

namespace AmongUsSalem.Utilities;

public static class PlayerRoleTextExtensions
{
    public static Color UpdateTargetColor(this Color color, PlayerControl player, bool hidden = false)
    {
        if ((PlayerControl.LocalPlayer.Data.Role is Framer framer && framer.FramedPlayers.Contains(player.PlayerId))
            || (player.IsFramed() && PlayerControl.LocalPlayer.Is(Faction.Mafia)))
        {
            color = AUSColors.Mafia;
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is Illusionist illusionist && illusionist.IllusionedPlayer == player)
            || (player.IsIllusioned() && PlayerControl.LocalPlayer.Is(Faction.Coven)))
        {
            color = AUSColors.Coven;
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is Bodyguard bodyguard && bodyguard.GuardedPlayer == player))
        {
            color = AUSColors.Town;
        }
        
        if (PlayerControl.LocalPlayer.Data.Role is Sheriff sheriff && sheriff.SuspiciousPlayers.Contains(player.PlayerId))
        {
            color = AUSColors.Town;
        }

        return color;
    }

    public static string UpdateTargetSymbols(this string name, PlayerControl player, bool hidden = false)
    {
        /*var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        if ((player.IsPeacockAssociate() && PlayerControl.LocalPlayer.IsRole<Peacock>()) ||
            (player.IsPeacockAssociate() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#d8a0ff> ↭</color>";
        }*/

        return name;
    }

    public static string UpdateProtectionSymbols(this string name, PlayerControl player, bool hidden = false)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        if ((player.HasModifier<GuardianAngelTargetModifier>(x => x.OwnerId == PlayerControl.LocalPlayer.PlayerId) &&
             PlayerControl.LocalPlayer.IsRole<GuardianAngelTouRole>())
            || (player.HasModifier<GuardianAngelTargetModifier>() &&
                ((PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden)
                 || (player.AmOwner &&
                     OptionGroupSingleton<GuardianAngelOptions>.Instance.GATargetKnows))))
        {
            name += (player.HasModifier<GuardianAngelProtectModifier>() && OptionGroupSingleton<GuardianAngelOptions>.Instance.ShowProtect is not ProtectOptions.GA)
                ? "<color=#FFD900> ★</color>"
                : "<color=#B3FFFF> ★</color>";
        }

        if ((player.HasModifier<MedicShieldModifier>(x => x.Medic.AmOwner) &&
             PlayerControl.LocalPlayer.IsRole<MedicRole>())
            || (player.HasModifier<MedicShieldModifier>() &&
                ((PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden)
                 || (player.AmOwner && player.TryGetModifier<MedicShieldModifier>(out var med) && med.VisibleSymbol))))
        {
            name += "<color=#006600> +</color>";
        }
        
        if ((player.HasModifier<MagicMirrorModifier>(x => x.Mirrorcaster.AmOwner) &&
             PlayerControl.LocalPlayer.IsRole<MirrorcasterRole>())
            || (player.HasModifier<MagicMirrorModifier>() &&
                ((PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden)
                 || (player.AmOwner && player.TryGetModifier<MagicMirrorModifier>(out var mm) && mm.VisibleSymbol))))
        {
            name += "<color=#90A2C3>〚〛</color>";
        }

        if ((player.HasModifier<ClericBarrierModifier>(x => x.Cleric.AmOwner) &&
             PlayerControl.LocalPlayer.IsRole<ClericRole>())
            || (player.HasModifier<ClericBarrierModifier>() &&
                ((PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden)
                 || (player.AmOwner && player.TryGetModifier<ClericBarrierModifier>(out var cleric) &&
                     cleric.VisibleSymbol))))
        {
            name += "<color=#00FFB3> Ω</color>";
        }

        if ((player.HasModifier<WardenFortifiedModifier>(x => x.Warden.AmOwner) &&
             PlayerControl.LocalPlayer.IsRole<WardenRole>())
            || (player.HasModifier<WardenFortifiedModifier>() &&
                ((PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden)
                 || (player.AmOwner && player.TryGetModifier<WardenFortifiedModifier>(out var warden) &&
                     warden.VisibleSymbol))))
        {
            name += "<color=#9900FF> π</color>";
        }

        return name;
    }

    public static string UpdateAllianceSymbols(this string name, PlayerControl player, bool hidden = false)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;

        if (player.HasModifier<LoverModifier>() && (PlayerControl.LocalPlayer.HasModifier<LoverModifier>() ||
                                                    (PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow &&
                                                     !hidden)))
        {
            name += "<color=#FF66CC> ♥</color>";
        }

        if (player.HasModifier<EgotistModifier>() && (player.AmOwner ||
                                                      (EgotistModifier.EgoVisibilityFlag(player) &&
                                                       (player.GetModifiers<RevealModifier>().Any(x => x.Visible && x.RevealRole))) ||
                                                      (PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow &&
                                                       !hidden)))
        {
            name += "<color=#FFFFFF> (<color=#669966>Egotist</color>)</color>";
        }

        return name;
    }

    public static string UpdateStatusSymbols(this string name, PlayerControl player, bool hidden = false)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;

        if ((PlayerControl.LocalPlayer.Data.Role is Framer framer && framer.FramedPlayers.Contains(player.PlayerId))
            || (player.IsFramed() && PlayerControl.LocalPlayer.Is(Faction.Mafia))
            || (player.IsFramed() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#dd0000> Ⓕ</color>";
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is Bodyguard bodyguard && bodyguard.GuardedPlayer == player)
            || (player.IsGuarded() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#06e00c> Ⓖ</color>";
        }

        if (PlayerControl.LocalPlayer.Data.Role is Sheriff sheriff && sheriff.SuspiciousPlayers.Contains(player.PlayerId))
        {
            name += "<color=#dd0000> Ⓢ</color>";
        }
        else if (PlayerControl.LocalPlayer.Data.Role is Sheriff sheriff2 && sheriff2.SearchedPlayers.Contains(player.PlayerId))
        {
            name += "<color=#06e00c> Ⓢ</color>";
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is Illusionist illusionist && illusionist.IllusionedPlayer == player)
            || (player.IsIllusioned() && PlayerControl.LocalPlayer.Is(Faction.Coven))
            || (player.IsIllusioned() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#ab42ef> Ⓘ</color>";
        }

        return name;
    }
}