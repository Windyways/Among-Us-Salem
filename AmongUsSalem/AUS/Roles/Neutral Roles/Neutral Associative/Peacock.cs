using System.Text;
using ObjectWorkshop;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ObjectWorkshop.LifeImprovement.Roles;

#region Peacock
#endregion
public sealed class Peacock(IntPtr cppPtr)
    : NeutralRole(cppPtr), IOWRole, IWikiDiscoverable
{
    public bool attractsMetal => true;
    public string RoleName => TouLocale.Get(TouNames.Peacock, "Peacock");
    public string revealText => "is a beautiful bird.";
    public string RoleDescription => "Paralyze players, win with your Associate!";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => OWColors.Peacock;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralAssociative;
    public float visionValue => OptionGroupSingleton<Peacock_Options>.Instance.Vision;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<Peacock_Options>.Instance.CanVent,
        // Icon = OWAssets.Peacock,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Neutral Associative role that can Declare someone as their associate to win with them, and can turn into a Peacock to paralyze players to prevent them from moving."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Declare",
            "You can Declare a player during the round. You will select your target as your Associate. You will learn their role. Your target will learn your identity. If you do not pick an Associate during the first round, one will be randomly assigned. You will die if your Associate dies, unless they become an Undead Reaper.",
            OWAssets.Peacock_Declare),
            
        new("Bloom",
            "You can Bloom into your true form during the round. You will turn into an anonymous Peacock. Any players that you see in your vision will go visibly pale and be completely immobile. If there are 3 or more people that see you, the first two will gain confidence and become immune to you until the next round. Using this ability again will turn you back to normal. Pale players cannot use any abilities. If a non-Crewmate is turned pale, you will notify your Associate.",
            OWAssets.Peacock_Bloom),
    ];

    #region RpcDeclare
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.Declare, SendImmediately = true)]
    public static void RpcDeclare(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Peacock)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcDeclare - Invalid peacock");
            return;
        }

        var peacock = player.GetRole<Peacock>();
        if (peacock != null)
        {
            peacock.Associate = target;
            player.AddToRevealedPlayers(target);
            target.AddToRevealedPlayers(player);
        }
    }

    #region RpcBloom
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.Bloom, SendImmediately = true)]
    public static void RpcBloom(PlayerControl player)
    {
        if (player.Data.Role is not Peacock)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcBloom - Invalid peacock");
            return;
        }

		WitnessKillEvent.WitnessSuspiciousActivity(player, player, false, false, true, true);

        var peacock = player.GetRole<Peacock>();
        if (peacock != null)
        {
            if (peacock.isBloomed)
            {
                peacock.isBloomed = false;
                player.SetTransparency(1, false);
                
                var peacockVisual = PeacockVisual.GetObjectByOwner(player);
                Destroy(peacockVisual.gameObject);
            }
            else
            {
                peacock.isBloomed = true;
                player.SetTransparency(0, true);

                PeacockVisual.Begin(peacock);
            }
        }
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(OWColors.Peacock);
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(OWColors.Infiltrator);
        }
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        if (Associate == null) return false;
        return Associate.Data.Role.DidWin(gameOverReason);
    }

    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
        {
            return false;
        }

        var console = usable.TryCast<Console>()!;
        return console == null || console.AllowImpostor;
    }

    public void LobbyStart()
    {
        PeacockVisual.CleanUp();
    }

    public override void OnMeetingStart()
    {
        if (isBloomed) RpcBloom(Player);
        else if (Associate == null)
        {
            List<PlayerControl> potentialTargets = new List<PlayerControl>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player != Player && !player.HasDied())
                {
                    potentialTargets.Add(player);
                }
            }

            potentialTargets.Shuffle();
            RpcDeclare(Player, potentialTargets[0]);
        }
    }

    public void OnTargetDeath(PlayerControl target, DeathReason? reason)
    {
        if (Associate != null)
        {
            if (Associate == target && Associate.HasDied())
            {
                if (Associate.IsRole<UndeadReaper>())
                {
                    if (!UndeadReaper.ReapersAlive()) Player.RpcCustomMurder(Player);
                }
                else
                {
                    Player.RpcCustomMurder(Player);
                }
            }
        }
    }

    public PlayerControl Associate;
    public bool isBloomed;
}

#region Peacock_Declare
#endregion
public sealed class Peacock_Declare : ObjectWorkshopRoleButton<Peacock, PlayerControl>
{
    public override string Name => "Declare";
    public override Color TextOutlineColor => OWColors.Peacock;
    public override float Cooldown => 1f;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Peacock_Declare;
    public override int MaxUses => 1;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Reactor.Utilities.Logger<ObjectWorkshopPlugin>.Error("Peacock Ability: Target is null");
            return;
        }

        Peacock.RpcDeclare(Role.Player, Target);
    }
}

#region Peacock_Bloom
#endregion
public sealed class Peacock_Bloom : ObjectWorkshopRoleButton<Peacock>
{
    public override string Name => "Bloom";
    public override Color TextOutlineColor => OWColors.Infiltrator;
    public override float Cooldown => OptionGroupSingleton<Peacock_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Peacock_Bloom;

    public override bool CanUse()
    {
        return base.CanUse() && Role.Associate != null;
    }

    protected override void OnClick()
    {
        Peacock.RpcBloom(Role.Player);
    }
}

#region Peacock_Options
#endregion
public sealed class Peacock_Options : AbstractOptionGroup<Peacock>
{
    public override string GroupName => TouLocale.Get(TouNames.Peacock, "Peacock");

    [ModdedNumberOption("<color=#d8a0ff>Peacock</color> <color=#4a86e8>Bloom</color> Cooldown", 0, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25;

    [ModdedNumberOption("<color=#d8a0ff>Peacock</color> Vision", 0.25f, 60f, 0.25f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float Vision { get; set; } = 0.5f;

    [ModdedToggleOption("<color=#d8a0ff>Peacock</color> Can Vent")]
    public bool CanVent { get; set; } = false;
}