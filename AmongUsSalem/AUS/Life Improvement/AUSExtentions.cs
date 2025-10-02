using System.Linq;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement;

public static class AUSExtentions
{
    public static string Name(this PlayerControl player)
    {
        return player.GetDefaultAppearance().PlayerName;
    }

    public static string Role(this PlayerControl player)
    {
        if (player.Data.Role is IAUSRole ausRole)
        {
            var roleColor = ausRole.Faction != Faction.Neutral ? MiscUtils.GetFactionColour(player) : MiscUtils.GetRoleColour(ausRole.RoleName);
            return $"<b><color=#" + roleColor.ToHtmlStringRGBA() + $">{ausRole.RoleName}</color></b>";
        }
        return "";
    }
    
    public static string Role(this RoleBehaviour role)
    {
        return $"<b><color=#" + role.TeamColor.ToHtmlStringRGBA() + $">{role.NiceName}</color></b>";
    }

    public static void SetTransparency(this PlayerControl player, float transparency, bool hideName = false)
    {
        var colour = player.cosmetics.currentBodySprite.BodySprite.color;
        var cosmetics = player.cosmetics;

        colour.a = transparency;
        player.cosmetics.currentBodySprite.BodySprite.color = colour;

        if (hideName)
        {
            transparency = 0f;
        }

        cosmetics.nameText.color = cosmetics.nameText.color.SetAlpha(transparency);

        if (DataManager.Settings.Accessibility.ColorBlindMode)
        {
            cosmetics.colorBlindText.color = cosmetics.colorBlindText.color.SetAlpha(transparency);
        }

        player.SetHatAndVisorAlpha(transparency);
        cosmetics.skin.layer.color = cosmetics.skin.layer.color.SetAlpha(transparency);
        if (player.cosmetics.GetLongBoi() != null)
        {
            player.cosmetics.GetLongBoi().headSprite.color = player.cosmetics.GetLongBoi().headSprite.color.SetAlpha(transparency);
            player.cosmetics.GetLongBoi().neckSprite.color = player.cosmetics.GetLongBoi().neckSprite.color.SetAlpha(transparency);
            player.cosmetics.GetLongBoi().foregroundNeckSprite.color = player.cosmetics.GetLongBoi().foregroundNeckSprite.color.SetAlpha(transparency);
        }

        if (player.cosmetics.currentPet != null)
        {
            foreach (var rend in player.cosmetics.currentPet.renderers)
            {
                rend.color = rend.color.SetAlpha(transparency);
            }

            foreach (var shadow in player.cosmetics.currentPet.shadows)
            {
                shadow.color = shadow.color.SetAlpha(transparency);
            }
        }

        foreach (var animation in player.transform.GetChild(2).GetComponentsInParent<SpriteRenderer>())
        {
            animation.color = animation.color.SetAlpha(transparency);
        }

        foreach (var animation in player.transform.GetChild(2).GetComponentsInChildren<SpriteRenderer>())
        {
            animation.color = animation.color.SetAlpha(transparency);
        }
    }

    public static bool IsTargetable(this PlayerControl player)
    {
        return true;
    }

    public static void Mobilize(this PlayerControl player)
    {
        player.moveable = true;
    }

    public static void Immobilize(this PlayerControl player)
    {
        player.moveable = false;
        player.MyPhysics.SetNormalizedVelocity(UnityEngine.Vector2.zero);
    }

    public static bool IsSameFaction(this PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is IAUSRole ausRole && target.Data.Role is IAUSRole targetAusRole && ausRole.Faction == targetAusRole.Faction) return true;
        if (player.Is(Faction.Town) && target.HasModifier<VampireRecruit>()) return true;
        if (player.HasModifier<VampireRecruit>() && target.Is(Faction.Town)) return true;
        return false;
    }


    public static bool AppearsEvil(this PlayerControl player)
    {
        return player.IsFramed();
    }

    public static bool IsImpureToTown(this PlayerControl player)
    {
        return player.IsTownTraitor() || player.HasModifier<VampireRecruit>() || player.HasModifier<JackalRecruit>();
    }

    public static bool IsHexed(this PlayerControl player)
    {
        foreach (var hexMasters in MiscUtils.GetPlayersWithRole<HexMaster>())
        {
            var hexMaster = hexMasters.GetRole<HexMaster>();
            return hexMaster.HexedPlayers.Contains(player.PlayerId);
        }
        return false;
    }

    public static bool IsDoused(this PlayerControl player)
    {
        foreach (var arsonists in MiscUtils.GetPlayersWithRole<Arsonist>())
        {
            var arsonist = arsonists.GetRole<Arsonist>();
            return arsonist.DousedPlayers.Contains(player.PlayerId);
        }
        return false;
    }

    public static bool IsSearchedAndInno(this PlayerControl player)
    {
        foreach (var sheriffs in MiscUtils.GetPlayersWithRole<Sheriff>())
        {
            var sheriff = sheriffs.GetRole<Sheriff>();
            return sheriff.SearchedPlayers.Contains(player.PlayerId) && !sheriff.SuspiciousPlayers.Contains(player.PlayerId);
        }
        return false;
    }

    public static bool IsNoCrime(this PlayerControl player)
    {
        foreach (var investigators in MiscUtils.GetPlayersWithRole<Investigator>())
        {
            var investigator = investigators.GetRole<Investigator>();
            return investigator.InvestigatedPlayers.Contains(player.PlayerId) && !investigator.TrespassingPlayers.Contains(player.PlayerId) && !investigator.MurderPlayers.Contains(player.PlayerId);
        }
        return false;
    }

    public static bool IsInnocent(this PlayerControl player)
    {
        return player.IsNoCrime() || player.IsSearchedAndInno();
    }

    public static bool IsFramed(this PlayerControl player)
    {
        foreach (var framers in MiscUtils.GetPlayersWithRole<Framer>())
        {
            var framer = framers.GetRole<Framer>();
            return framer.FramedPlayers.Contains(player.PlayerId);
        }
        return false;
    }

    public static bool IsAlerted(this PlayerControl player)
    {
        foreach (var veterans in MiscUtils.GetPlayersWithRole<Veteran>())
        {
            var veteran = veterans.GetRole<Veteran>();
            return veteran.isAlerted && veteran.Player == player;
        }
        return false;
    }

    public static bool IsSelfProtected(this PlayerControl player, bool canKill)
    {
        foreach (var bodyguards in MiscUtils.GetPlayersWithRole<Bodyguard>())
        {
            var bodyguard = bodyguards.GetRole<Bodyguard>();
            return bodyguard.isSelfProtected && bodyguard.Player == player && !canKill;
        }
        return false;
    }

    /*public static bool IsConsortDistracted(this PlayerControl player)
    {
        foreach (var consorts in MiscUtils.GetPlayersWithRole<Consort>())
        {
            var consort = consorts.GetRole<Consort>();
            return consort.DistractedPlayer == player && !player.HasModifier<RBimmune>();
        }
        return false;
    }*/

    public static bool IsDistracted(this PlayerControl player)
    {
        foreach (var escorts in MiscUtils.GetPlayersWithRole<Escort>())
        {
            var escort = escorts.GetRole<Escort>();
            return escort.DistractedPlayer == player && !player.HasModifier<RBimmune>();
        }
        return false;
    }

    public static bool IsFortified(this PlayerControl player)
    {
        foreach (var crusaders in MiscUtils.GetPlayersWithRole<Crusader>())
        {
            var crusader = crusaders.GetRole<Crusader>();
            return crusader.FortifiedPlayer == player;
        }
        return false;
    }

    public static bool IsShrouded(this PlayerControl player)
    {
        foreach (var shrouds in MiscUtils.GetPlayersWithRole<Shroud>())
        {
            var shroud = shrouds.GetRole<Shroud>();
            return shroud.ShroudedPlayer == player;
        }
        return false;
    }

    public static bool IsGuarded(this PlayerControl player)
    {
        foreach (var bodyguards in MiscUtils.GetPlayersWithRole<Bodyguard>())
        {
            var bodyguard = bodyguards.GetRole<Bodyguard>();
            return bodyguard.GuardedPlayer == player;
        }
        return false;
    }

    public static bool IsBarriered(this PlayerControl player)
    {
        foreach (var clerics in MiscUtils.GetPlayersWithRole<Cleric>())
        {
            var cleric = clerics.GetRole<Cleric>();
            return cleric.BarrieredPlayers.Contains(player.PlayerId);
        }
        return false;
    }

    public static bool IsTownTraitor(this PlayerControl player)
    {
        return false;
        //return player.Is(ModifierEnum.ApocTownTraitor) || player.Is(ModifierEnum.MafiaTownTraitor) || player.Is(ModifierEnum.CovenTownTraitor) || player.Is(ModifierEnum.PandoraTownTraitor) || player.Is(ModifierEnum.EgotistTownie);
    }

    public static bool IsIllusioned(this PlayerControl player)
    {
        foreach (var illusionists in MiscUtils.GetPlayersWithRole<Illusionist>())
        {
            var illusionist = illusionists.GetRole<Illusionist>();
            return illusionist.IllusionedPlayer == player;
        }
        return false;
    }

    public static bool IsBlackmailed(this PlayerControl player)
    {
        foreach (var blackmailers in MiscUtils.GetPlayersWithRole<Blackmailer>())
        {
            var blackmailer = blackmailers.GetRole<Blackmailer>();
            return blackmailer.BlackmailedPlayer == player;
        }
        return false;
    }

    public static bool IsSilenced(this PlayerControl player)
    {
        foreach (var voodooMasters in MiscUtils.GetPlayersWithRole<VoodooMaster>())
        {
            var voodooMaster = voodooMasters.GetRole<VoodooMaster>();
            return voodooMaster.SilencedPlayer == player;
        }
        return false;
    }

    public static bool IsJinxed(this PlayerControl player)
    {
        foreach (var jinxes in MiscUtils.GetPlayersWithRole<Jinx>())
        {
            var jinx = jinxes.GetRole<Jinx>();
            return jinx.JinxedPlayer == player;
        }
        return false;
    }

    public static bool IsCleanTarget(this PlayerControl player)
    {
        foreach (var janitors in MiscUtils.GetPlayersWithRole<Janitor>())
        {
            var janitor = janitors.GetRole<Janitor>();
            return janitor.CleanedPlayer == player;
        }
        return false;
    }

    public static bool IsAmbushed(this PlayerControl player)
    {
        foreach (var ambushers in MiscUtils.GetPlayersWithRole<Ambusher>())
        {
            var ambusher = ambushers.GetRole<Ambusher>();
            return ambusher.AmbushedPlayer == player;
        }
        return false;
    }

    public static bool IsVesting(this PlayerControl player, bool canKill)
    {
        foreach (var survivors in MiscUtils.GetPlayersWithRole<Survivor>())
        {
            var survivor = survivors.GetRole<Survivor>();
            return survivor.isVesting && survivor.Player == player && !canKill;
        }
        return false;
    }
}