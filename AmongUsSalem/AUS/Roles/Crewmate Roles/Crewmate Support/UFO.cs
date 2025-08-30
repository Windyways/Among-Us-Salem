using System.Text;
using UnityEngine;
using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace ObjectWorkshop.LifeImprovement.Roles;

#region UFO
#endregion
public sealed class UFO(IntPtr cppPtr) 
    : CrewmateRole(cppPtr), IOWRole, IWikiDiscoverable
{
    public bool attractsMetal => true;
    public string RoleName => TouLocale.Get(TouNames.UFO, "UFO");
    public string revealText => "owns a mythological ship.";
    public string RoleDescription => "Abduct players to the Moon!";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => OWColors.Crewmate;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = OWAssets.UFO,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Support role that can place down a moon, then can Abduct players from a distance to teleport them to the moon." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Destination",
            "You can set a Destination during the round. You will place down the moon, visible to everyone. Using this ability again overrides any existing moons. Moons can be destroyed by Obstructors.",
            OWAssets.UFO_Destination),

        new("Abduct",
            $"You can Abduct a player during the round. You will select your target via a menu, after {OptionGroupSingleton<UFO_Options>.Instance.Duration}s, your target will be teleported onto the moon you placed beforehand.",
            OWAssets.UFO_Abduct)
    ];

    #region RpcPlaceMoon
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.PlaceMoon, SendImmediately = true)]
    public static void RpcPlaceMoon(PlayerControl player)
    {
        if (player.Data.Role is not UFO)
        {
            Logger<ObjectWorkshopPlugin>.Error("PlaceMoon - Invalid ufo");
            return;
        }

        var ufo = player.GetRole<UFO>();
		Moon.DestroyAll();
		Moon.Begin(ufo.Player);
    }
    
    #region RpcTeleport
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.Teleport, SendImmediately = true)]
    public static void RpcTeleport(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not UFO)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcTeleport - Invalid ufo");
            return;
        }

        var ufo = player.GetRole<UFO>();
        if (ufo != null)
        {
            var moon = Moon.GetMoon();
            Coroutines.Start(moon.PlaySwirlAndTeleport(ufo, target));
        }
    }

    public void LobbyStart()
    {
        Moon.CleanUp();
    }
}

#region UFO_Destination
#endregion
public sealed class UFO_Destination : ObjectWorkshopRoleButton<UFO>
{
    public override string Name => "Destination";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Crewmate;
    public override float Cooldown => OptionGroupSingleton<UFO_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => OWAssets.UFO_Destination;

    public override bool CanUse()
    {
        var moon = Moon.GetMoon();
        if (moon == null) return base.CanUse();
        return base.CanUse() && !Moon.GetMoon().isAbductOccuring;
    }

    protected override void OnClick()
    {
        UFO.RpcPlaceMoon(Role.Player);
    }
}

#region UFO_Abduct
#endregion
public sealed class UFO_Abduct : ObjectWorkshopRoleButton<UFO>
{
    public override string Name => "Abduct";
    public override Color TextOutlineColor => OWColors.Crewmate;
    public override float Cooldown => OptionGroupSingleton<UFO_Options>.Instance.AbductCD;
    public override LoadableAsset<Sprite> Sprite => OWAssets.UFO_Abduct;
    public override float EffectDuration => OptionGroupSingleton<UFO_Options>.Instance.Duration;

    public override bool CanUse()
    {
        var moon = Moon.GetMoon();
        if (moon == null) return false;
        return base.CanUse() && !Moon.GetMoon().isAbductOccuring;
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
        var playerMenu = CustomPlayerMenu.Create();
        playerMenu.transform.FindChild("PhoneUI").GetChild(0).GetComponent<SpriteRenderer>().material = PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
        playerMenu.transform.FindChild("PhoneUI").GetChild(1).GetComponent<SpriteRenderer>().material = PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
        playerMenu.Begin(
            plr => !plr.HasDied() ||
                    Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == plr.PlayerId) ||
                    FakePlayer.FakePlayers.FirstOrDefault(x => x?.body?.name == $"Fake {plr.gameObject.name}") ?.body, plr =>
            {
                playerMenu.ForceClose();

                if (plr != null)
                {
                    ObjectWorkshopPlugin.DebugLogMessage("UFO is Abducting...");
                    UFO.RpcTeleport(Role.Player, plr);
                    
                    EffectActive = true;
                    Timer = EffectDuration;
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
}

#region UFO_Options
#endregion
public sealed class UFO_Options : AbstractOptionGroup<UFO>
{
    public override string GroupName => TouLocale.Get(TouNames.UFO, "UFO");

    [ModdedNumberOption("<color=#b3ffff>UFO</color> <color=#4a86e8>Destination</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#b3ffff>UFO</color> <color=#4a86e8>Abduct</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float AbductCD { get; set; } = 25f;

    [ModdedNumberOption("<color=#b3ffff>UFO</color> <color=#4a86e8>Abduct</color> Duration", 0.25f, 60f, 0.05f, MiraNumberSuffixes.Seconds, "0.00")]
    public float Duration { get; set; } = 1.5f;
}