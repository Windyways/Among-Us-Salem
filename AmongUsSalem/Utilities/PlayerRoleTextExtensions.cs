using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using AmongUsSalem.Modifiers;
using AmongUsSalem.Modifiers.Crewmate;
using AmongUsSalem.Modifiers.Impostor;
using AmongUsSalem.Modules;
using AmongUsSalem.Options;
using AmongUsSalem.Roles;
using AmongUsSalem.Roles.Neutral;
using UnityEngine;

namespace AmongUsSalem.Utilities;

public static class PlayerRoleTextExtensions
{
    public static Color UpdateTargetColor(this Color color, PlayerControl player, bool hidden = false)
    {
        // Town
        if (PlayerControl.LocalPlayer.Data.Role is Bodyguard bodyguard && bodyguard.GuardedPlayer == player)
        {
            color = AUSColors.Town;
        }
        
        if (PlayerControl.LocalPlayer.Data.Role is Sheriff sheriff && sheriff.SuspiciousPlayers.Contains(player.PlayerId))
        {
            color = AUSColors.Mafia;
        }
        else if (PlayerControl.LocalPlayer.Data.Role is Sheriff sheriff2 && sheriff2.SearchedPlayers.Contains(player.PlayerId))
        {
            color = AUSColors.Town;
        }

        if (PlayerControl.LocalPlayer.Data.Role is Investigator investigator && investigator.MurderPlayers.Contains(player.PlayerId))
        {
            color = AUSColors.Mafia;
        }
        if (PlayerControl.LocalPlayer.Data.Role is Investigator investigator2 && investigator2.TrespassingPlayers.Contains(player.PlayerId))
        {
            color = AUSColors.OrangeShade;
        }
        if (PlayerControl.LocalPlayer.Data.Role is Investigator investigator3 && !investigator3.TrespassingPlayers.Contains(player.PlayerId) && !investigator3.MurderPlayers.Contains(player.PlayerId) && investigator3.InvestigatedPlayers.Contains(player.PlayerId))
        {
            color = AUSColors.Town;
        }

        if (PlayerControl.LocalPlayer.Data.Role is Crusader crusader && crusader.FortifiedPlayer == player)
        {
            color = AUSColors.Town;
        }
        
        if (PlayerControl.LocalPlayer.Data.Role is Escort escort && escort.DistractedPlayer == player)
        {
            color = AUSColors.Town;
        }

        // Neutral
        if (PlayerControl.LocalPlayer.Data.Role is Arsonist arsonist && arsonist.DousedPlayers.Contains(player.PlayerId))
        {
            color = AUSColors.Arsonist;
        }
        
        if (PlayerControl.LocalPlayer.Data.Role is Shroud shroud && shroud.ShroudedPlayer == player)
        {
            color = AUSColors.Shroud;
        }
        
        if (PlayerControl.LocalPlayer.Data.Role is Jackal && player.HasModifier<JackalRecruit>() ||
            PlayerControl.LocalPlayer.HasModifier<JackalRecruit>() && player.HasModifier<JackalRecruit>())
        {
            color = AUSColors.Neutral;
        }
        
        // Mafia
        if ((PlayerControl.LocalPlayer.Data.Role is Framer framer && framer.FramedPlayers.Contains(player.PlayerId))
            || (player.IsFramed() && PlayerControl.LocalPlayer.Is(Faction.Mafia)))
        {
            color = AUSColors.Mafia;
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is Blackmailer blackmailer && blackmailer.BlackmailedPlayer == player)
            || (player.IsBlackmailed() && PlayerControl.LocalPlayer.Is(Faction.Mafia)))
        {
            color = AUSColors.Mafia;
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is Ambusher ambusher && ambusher.AmbushedPlayer == player)
            || (player.IsAmbushed() && PlayerControl.LocalPlayer.Is(Faction.Mafia)))
        {
            color = AUSColors.Mafia;
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is Janitor janitor && janitor.CleanedPlayer == player)
            || (player.IsCleanTarget() && PlayerControl.LocalPlayer.Is(Faction.Mafia)))
        {
            color = AUSColors.Mafia;
        }

        // Coven
        if ((PlayerControl.LocalPlayer.Data.Role is HexMaster hexMaster && hexMaster.HexedPlayers.Contains(player.PlayerId))
            || (player.IsHexed() && PlayerControl.LocalPlayer.Is(Faction.Coven)))
        {
            color = AUSColors.Coven;
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is Illusionist illusionist && illusionist.IllusionedPlayer == player)
            || (player.IsIllusioned() && PlayerControl.LocalPlayer.Is(Faction.Coven)))
        {
            color = AUSColors.Coven;
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is VoodooMaster voodooMaster && voodooMaster.SilencedPlayer == player)
            || (player.IsSilenced() && PlayerControl.LocalPlayer.Is(Faction.Coven)))
        {
            color = AUSColors.Coven;
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Jinx jinx && jinx.JinxedPlayer == player)
            || (player.IsJinxed() && PlayerControl.LocalPlayer.Is(Faction.Coven)))
        {
            color = AUSColors.Coven;
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

    public static string UpdateAllianceSymbols(this string name, PlayerControl player, bool hidden = false)
    {
        return name;
    }

    public static string UpdateStatusSymbols(this string name, PlayerControl player, bool hidden = false)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;

        // Town
        if ((PlayerControl.LocalPlayer.Data.Role is Bodyguard bodyguard && bodyguard.GuardedPlayer == player)
            || (player.IsGuarded() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#06E00C> Ⓖ</color>";
        }

        if (PlayerControl.LocalPlayer.Data.Role is Sheriff sheriff && sheriff.SuspiciousPlayers.Contains(player.PlayerId))
        {
            name += "<color=#DD0000> Ⓢ</color>";
        }
        else if (PlayerControl.LocalPlayer.Data.Role is Sheriff sheriff2 && sheriff2.SearchedPlayers.Contains(player.PlayerId))
        {
            name += "<color=#06E00C> Ⓢ</color>";
        }

        if (PlayerControl.LocalPlayer.Data.Role is Investigator investigator2 && investigator2.TrespassingPlayers.Contains(player.PlayerId))
        {
            name += "<color=#ff9900> Ⓣ</color>"; // Orange
        }
        if (PlayerControl.LocalPlayer.Data.Role is Investigator investigator && investigator.MurderPlayers.Contains(player.PlayerId))
        {
            name += "<color=#DD0000> Ⓜ</color>";
        }
        if (PlayerControl.LocalPlayer.Data.Role is Investigator investigator3 && !investigator3.TrespassingPlayers.Contains(player.PlayerId) && !investigator3.MurderPlayers.Contains(player.PlayerId) && investigator3.InvestigatedPlayers.Contains(player.PlayerId))
        {
            name += "<color=#06E00C> ⓃⒸ</color>";
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is Crusader crusader && crusader.FortifiedPlayer == player)
            || (player.IsFortified() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#06E00C> Ⓕ</color>";
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is Escort escort && escort.DistractedPlayer == player)
            || (player.IsDistracted() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#06E00C> Ⓓ</color>";
        }

        if (PlayerControl.LocalPlayer.Data.Role is Cleric cleric && cleric.BarrieredPlayers.Contains(player.PlayerId))
        {
            name += "<color=#06E00C> Ⓑ</color>";
        }

        if (PlayerControl.LocalPlayer.Data.Role is Coroner coroner && coroner.ExaminedPlayers.Contains(player.PlayerId))
        {
            name += "<color=#06E00C> Ⓔ</color>";
        }

        // Neutral
        if ((PlayerControl.LocalPlayer.Data.Role is Arsonist arsonist && arsonist.DousedPlayers.Contains(player.PlayerId))
            || (player.IsDoused() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#ee7600> Ⓓ</color>";
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is Shroud shroud && shroud.ShroudedPlayer == player)
            || (player.IsShrouded() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#6699ff> Ⓢ</color>";
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is Jackal && player.HasModifier<JackalRecruit>()) ||
            (PlayerControl.LocalPlayer.HasModifier<JackalRecruit>() && player.HasModifier<JackalRecruit>()) ||
            (player.HasModifier<JackalRecruit>() && player.HasDied())
            || (player.HasModifier<JackalRecruit>() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#404040> ☯</color>";
        }

        // Mafia -------------------------------------------------------------------------------------------------------
        if ((PlayerControl.LocalPlayer.Data.Role is Framer framer && framer.FramedPlayers.Contains(player.PlayerId))
            || (player.IsFramed() && PlayerControl.LocalPlayer.Is(Faction.Mafia))
            || (player.IsFramed() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#DD0000> Ⓕ</color>";
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is Blackmailer blackmailer && blackmailer.BlackmailedPlayer == player)
            || (player.IsBlackmailed() && PlayerControl.LocalPlayer.Is(Faction.Mafia))
            || (player.IsBlackmailed() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#DD0000> Ⓑ</color>";
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is Ambusher ambusher && ambusher.AmbushedPlayer == player)
            || (player.IsAmbushed() && PlayerControl.LocalPlayer.Is(Faction.Mafia))
            || (player.IsAmbushed() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#DD0000> Ⓐ</color>";
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is Janitor janitor && janitor.CleanedPlayer == player)
            || (player.IsCleanTarget() && PlayerControl.LocalPlayer.Is(Faction.Mafia))
            || (player.IsCleanTarget() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#DD0000> Ⓙ</color>";
        }
        
        // Coven
        if ((PlayerControl.LocalPlayer.Data.Role is Illusionist illusionist && illusionist.IllusionedPlayer == player)
            || (player.IsIllusioned() && PlayerControl.LocalPlayer.Is(Faction.Coven))
            || (player.IsIllusioned() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#B545FF> Ⓘ</color>";
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is HexMaster hexMaster && hexMaster.HexedPlayers.Contains(player.PlayerId))
            || (player.IsHexed() && PlayerControl.LocalPlayer.Is(Faction.Coven))
            || (player.IsHexed() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#B545FF> Ⓗ</color>";
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is VoodooMaster voodooMaster && voodooMaster.SilencedPlayer == player)
            || (player.IsSilenced() && PlayerControl.LocalPlayer.Is(Faction.Coven))
            || (player.IsSilenced() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#B545FF> Ⓢ</color>";
        }
        
        if ((PlayerControl.LocalPlayer.Data.Role is Jinx jinx && jinx.JinxedPlayer == player)
            || (player.IsJinxed() && PlayerControl.LocalPlayer.Is(Faction.Coven))
            || (player.IsJinxed() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#B545FF> Ⓙ</color>";
        }
        
        if ((player.HasModifier<CovenVIP>() && PlayerControl.LocalPlayer.Is(Faction.Coven))
            || (player.HasModifier<CovenVIP>() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#B545FF> [VIP]</color>";
        }
        
        if ((player.Data.Role is ICovenRole coven && coven.Necronomicon && PlayerControl.LocalPlayer.Is(Faction.Coven))
            || (player.Data.Role is ICovenRole coven2 && coven2.Necronomicon && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#B545FF> [BOOK]</color>";
        }

        return name;
    }
}

/* Status Alphabet

Ⓐ
Ⓑ
Ⓒ
Ⓓ
Ⓔ
Ⓕ
Ⓖ
Ⓗ
Ⓘ
Ⓙ
Ⓚ
Ⓛ
Ⓜ
Ⓝ
Ⓞ
Ⓟ
Ⓠ
Ⓡ
Ⓢ
Ⓣ
Ⓤ
Ⓥ
Ⓦ
Ⓧ
Ⓨ
Ⓩ

*/