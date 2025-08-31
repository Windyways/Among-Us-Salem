using System.Linq;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement;

public static class OWExtentions
{
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


    public static bool AppearsEvil(this PlayerControl player)
    {
        return player.IsFramed();
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

    public static bool IsTownTraitor(this PlayerControl player)
    {
        return false;
        //return player.Is(ModifierEnum.ApocTownTraitor) || player.Is(ModifierEnum.MafiaTownTraitor) || player.Is(ModifierEnum.CovenTownTraitor) || player.Is(ModifierEnum.PandoraTownTraitor) || player.Is(ModifierEnum.EgotistTownie);
    }

    public static bool IsIllusioned(this PlayerControl player)
    {
        return false;
        /*return Role.GetRoles(RoleEnum.Illusionist).Any(delegate(Role role)
        {
            PlayerControl illusionedTarget = ((Illusionist)role).IllusionedPlayer;
            return illusionedTarget != null && player.PlayerId == illusionedTarget.PlayerId;
        });*/
    }


    /*public static bool IsPeacockAssociate(this PlayerControl player)
    {
        return MiscUtils.GetRoles("Peacock").Any(role =>
        {
            var target = ((Peacock)role).Associate;
            return target != null && player.PlayerId == target.PlayerId;
        });
    }*/

    /*public static bool IsMorphed(this PlayerControl player)
    {
        return MiscUtils.GetRoles("Identity Thief").Any(role =>
        {
            var identityThief = (IdentityThief)role;
            return identityThief != null && player.PlayerId == identityThief.Player.PlayerId && identityThief.ImpersonatingPlayer != null;
        });
    }
    
    public static bool IsPure(this PlayerControl player)
    {
        return MiscUtils.GetRoles("Inquisitor").Any(role =>
        {
            var targets = ((Inquisitor)role).InvestigatedPlayers;
            var inList = ((Inquisitor)role).InvestigatedPlayers.ContainsKey(player);
            var isPure = ((Inquisitor)role).InvestigatedPlayers.TryGetValue(player, out var pure) && pure;
            return targets != null && inList && isPure;
        });
    }*/
}