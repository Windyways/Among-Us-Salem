using System.Text;
using UnityEngine;
using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace ObjectWorkshop.LifeImprovement.Roles;

#region GiftWeaver
#endregion
public sealed class GiftWeaver(IntPtr cppPtr) 
    : CrewmateRole(cppPtr), IOWRole, IWikiDiscoverable, IVisualAppearance
{
    public bool attractsMetal => true;
    public string RoleName => TouLocale.Get(TouNames.GiftWeaver, "Gift Weaver");
    public string revealText => "is a hard working builder.";
    public string RoleDescription => "Build crates to support the crew!";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => OWColors.Crewmate;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = OWAssets.GiftWeaver,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Support role that can build and create Crates which other players can open, which can be good or bad depending on their team." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Build",
            $"You can Build a crate during the round. It will take {OptionGroupSingleton<GiftWeaver_Options>.Instance.Duration}s to Build the crate. After 5s, gain a charge of Package.",
            OWAssets.GiftWeaver_Build),

        new("Package",
            $"You can place down a Package during the round. You will drop a Crate, visible to everyone. If a Crewmate opens the Crate, they will complete a random task and you will learn their identity. If an Infiltrator opens the Crate, you will get a message at the start of the meeting saying an Infiltrator opened a Crate.",
            OWAssets.GiftWeaver_Package)
    ];

    #region RpcPackage
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.Package, SendImmediately = true)]
    public static void RpcPackage(PlayerControl player)
    {
        if (player.Data.Role is not GiftWeaver)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcPackage - Invalid Gift Weaver");
            return;
        }

        var giftWeaver = player.GetRole<GiftWeaver>();
		Crate.Begin(giftWeaver.Player);
    }

    #region RpcOpenCrate
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.OpenCrate, SendImmediately = true)]
    public static void RpcOpenCrate(PlayerControl player, PlayerControl openClient)
    {
        if (player.Data.Role is not GiftWeaver)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcOpenCrate - Invalid Gift Weaver");
            return;
        }

        var giftWeaver = player.GetRole<GiftWeaver>();
        var crate = Crate.GetObjectByOwner(giftWeaver.Player);
        
        if (player == null) Destroy(crate.gameObject);
        else
        {
            if (crate.gameObject != null)
            {
                if (openClient == player && openClient.AmOwner/* && !role.Player.IsRole<Claylamity>()*/)
                {
                    MiscUtils.CompleteRandomTask(openClient);
                }
                else
                {
                    if (player.IsFaction(Faction.Crewmate, true))
                    {
                        MiscUtils.CompleteRandomTask(openClient);
                        player.AddToRevealedPlayers(openClient);
                    }
                    else
                    {
                        if (openClient.IsFaction(Faction.Infiltrator, true))
                        {
                            if (openClient.AmOwner) AudioSource.PlayClipAtPoint(HudManager.Instance.TaskUpdateSound, openClient.transform.position);
                            if (player.AmOwner) MiscUtils.ShowNotification("An <color=#ff5050>Infiltrator</color> opened one of your Crates!", Color.white);
                        }
                        else if (openClient.AmOwner)
                        {
                            AudioSource.PlayClipAtPoint(HudManager.Instance.TaskUpdateSound, openClient.transform.position);
                        }
                    }
                }

                Destroy(crate.gameObject);
            }
        }
    }

    public VisualAppearance GetVisualAppearance()
    {
        var appearance = Player.GetDefaultAppearance();
        if (!isBuilding) return appearance;

        appearance.Speed = 0;
        return appearance;
    }

    public void LobbyStart()
    {
        Crate.CleanUp();
    }

    public static void OnRoundStart()
    {
        CustomButtonSingleton<GiftWeaver_Package>.Instance.Usable = false;
    }

    public bool isBuilding;
}

#region GiftWeaver_Build
#endregion
public sealed class GiftWeaver_Build : ObjectWorkshopRoleButton<GiftWeaver>
{
    public override string Name => "Build";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Crewmate;
    public override float Cooldown => OptionGroupSingleton<GiftWeaver_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => OWAssets.GiftWeaver_Build;
    public override float EffectDuration => OptionGroupSingleton<GiftWeaver_Options>.Instance.Duration;

    protected override void OnClick()
    {
        Role.isBuilding = true;
        TouAudio.PlaySound(OWAssets.Build_SFX);
    }

    public override void OnEffectEnd()
    {
        Role.isBuilding = false;

        var button = CustomButtonSingleton<GiftWeaver_Package>.Instance;
        button.IncreaseUses();
        button.Usable = true;
    }
}


#region GiftWeaver_Package
#endregion
public sealed class GiftWeaver_Package : ObjectWorkshopRoleButton<GiftWeaver>
{
    public bool Usable;
    public override string Name => "Package";
    public override Color TextOutlineColor => OWColors.Crewmate;
    public override float Cooldown => 1f;
    public override LoadableAsset<Sprite> Sprite => OWAssets.GiftWeaver_Package;
    public override int MaxUses => 0;

    public override bool CanUse()
    {
        return base.CanUse() && Usable;
    }

    protected override void OnClick()
    {
        GiftWeaver.RpcPackage(PlayerControl.LocalPlayer);

        DecreaseUses();
        if (UsesLeft == 0) Usable = false;
    }
}

#region GiftWeaver_Options
#endregion
public sealed class GiftWeaver_Options : AbstractOptionGroup<GiftWeaver>
{
    public override string GroupName => TouLocale.Get(TouNames.GiftWeaver, "GiftWeaver");

    [ModdedNumberOption("<color=#b3ffff>Gift Weaver</color> <color=#4a86e8>Build</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#b3ffff>Gift Weaver</color> <color=#4a86e8>Build</color> Duration", 0.25f, 60f, 0.25f, MiraNumberSuffixes.Seconds, "0.00")]
    public float Duration { get; set; } = 5f;
}