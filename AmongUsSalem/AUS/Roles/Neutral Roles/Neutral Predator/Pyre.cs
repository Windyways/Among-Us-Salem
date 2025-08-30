using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ObjectWorkshop.LifeImprovement.Roles;

#region Pyre
#endregion
public sealed class Pyre(IntPtr cppPtr)
    : NeutralRole(cppPtr), IOWRole, IWikiDiscoverable
{
    public bool attractsMetal => true;
    public string RoleName => TouLocale.Get(TouNames.Pyre, "Pyre");
    public string revealText => "is a master at spreading fire.";
    public string RoleDescription => "Spread fire to kill them all!";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => OWColors.Pyre;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralPredator;
    public float visionValue => OptionGroupSingleton<Pyre_Options>.Instance.Vision;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<Pyre_Options>.Instance.CanVent,
        Icon = OWAssets.Pyre,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Neutral Predator role that can light someone on fire after a delay, and any players going into contact to your target will also go on fire. People on fire die after a while."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Ignite",
            "You can Ignite a player with fire during the round. After a delay, your target will go on fire, appearing to be on fire to other players. After a certain amount of time, your target will die. Any players the target gets too close to will get caught on fire immediately.",
            OWAssets.Pyre_Ignite)
    ];

    #region RpcIgnite
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.Ignite, SendImmediately = true)]
    public static void RpcIgnite(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Pyre)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcIgnite - Invalid pyre");
            return;
        }

        var pyre = player.GetRole<Pyre>();
		if (pyre != null) Flames.Begin(pyre.Player, target);
    }
    
    public bool WinConditionMet()
    {
        if (Player.HasDied())
        {
            return false;
        }

        var result = Helpers.GetAlivePlayers().Count <= 2 && MiscUtils.KillersAliveCount() == 1;
        return result;
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(OWColors.Pyre);
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
        bool otherPyresWon = false;
        var pyres = PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsRole<Pyre>() && x != Player);
        if (pyres.Any())
        {
            foreach (var pyre in pyres)
            {
                if (pyre.Data.Role.DidWin(gameOverReason)) otherPyresWon = true;
            }
        }
        return WinConditionMet() || otherPyresWon;
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
        Flames.CleanUp();
    }

    public PlayerControl ignitedPlayer;
}

#region Pyre_Ignite
#endregion
public sealed class Pyre_Ignite : ObjectWorkshopRoleButton<Pyre, PlayerControl>
{
    public override string Name => "Ignite";
    public override Color TextOutlineColor => OWColors.Pyre;
    public override float Cooldown => OptionGroupSingleton<Pyre_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Pyre_Ignite;
    public override float EffectDuration => OptionGroupSingleton<Pyre_Options>.Instance.Delay;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (target == null) return base.IsTargetValid(target);
        return !target.IsOnFire();
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Reactor.Utilities.Logger<ObjectWorkshopPlugin>.Error("Pyre Ability: Target is null");
            return;
        }

        Role.ignitedPlayer = Target;
    }

    public override void OnEffectEnd()
    {
        if (Role.ignitedPlayer == null)
            return;

        Pyre.RpcIgnite(Role.Player, Role.ignitedPlayer);
    }
}

#region Pyre_Options
#endregion
public sealed class Pyre_Options : AbstractOptionGroup<Pyre>
{
    public override string GroupName => TouLocale.Get(TouNames.Pyre, "Pyre");

    [ModdedNumberOption("<color=#ff3771>Pyre</color> <color=#4a86e8>Ignite</color> Cooldown", 0, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25;

    [ModdedNumberOption("<color=#ff3771>Pyre</color> <color=#4a86e8>Ignite</color> Duration", 0.5f, 60f, 0.1f, MiraNumberSuffixes.Seconds, "0.0")]
    public float Duration { get; set; } = 5f;

    [ModdedNumberOption("<color=#ff3771>Pyre</color> <color=#4a86e8>Ignite</color> Delay", 0f, 30f, 1f, MiraNumberSuffixes.Seconds)]
    public float Delay { get; set; } = 3f;

    [ModdedNumberOption("<color=#ff3771>Pyre</color> Vision", 0.25f, 60f, 0.25f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float Vision { get; set; } = 1f;

    [ModdedToggleOption("<color=#ff3771>Pyre</color> Can Vent")]
    public bool CanVent { get; set; } = true;
}