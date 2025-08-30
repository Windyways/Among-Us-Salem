using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ObjectWorkshop.LifeImprovement.Roles;

#region Reaper
#endregion
public sealed class Reaper(IntPtr cppPtr)
    : NeutralRole(cppPtr), IOWRole, IWikiDiscoverable
{
    public bool attractsMetal => true;
    public string RoleName => TouLocale.Get(TouNames.Reaper, "Reaper");
    public string revealText => "wants to kill everyone with their partner.";
    public string RoleDescription => "Bring all of them down with you.";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => OWColors.Reaper;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralPredator;
    public float visionValue => OptionGroupSingleton<Reaper_Options>.Instance.Vision;

    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<Reaper_Options>.Instance.CanVent,
        Icon = OWAssets.Reaper,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Neutral Predator role that can kill others, but can enable their catastrophe ability to make their target become an Undead Reaper once per game."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Attack",
            "You can Attack a player during the round. You will kill your target.",
            OWAssets.Reaper_Attack),
            
        new("Catastrophe",
            "You can cause a Catastrophe during the round. The next player you kill will become the Undead Reaper.",
            OWAssets.Reaper_Catastrophe)
    ];

    #region RpcAttack
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.ReaperAttack, SendImmediately = true)]
    public static void RpcReaperAttack(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Reaper)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcReaperAttack - Invalid reaper");
            return;
        }

        var reaper = player.GetRole<Reaper>();
        if (reaper.causingCatastrophe)
        {
            player.AddToRevealedPlayers(target, false);
            target.AddToRevealedPlayers(player);

            reaper.causingCatastrophe = false;

            target.RpcChangeRole(RoleId.Get<UndeadReaper>());
            ReaperVisual.Begin(reaper.Player, target);

            if (player.AmOwner || Debugger.IsDebuggerActive)
            {
                var button = CustomButtonSingleton<Reaper_Catastrophe>.Instance;
                button.DecreaseUses();
                button.OverrideName("Catastrophe");
            }
        }
    }

    #region RpcCatastrophe
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.Catastrophe, SendImmediately = true)]
    public static void RpcCatastrophe(PlayerControl player)
    {
        if (player.Data.Role is not Reaper)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcCatastrophe - Invalid reaper");
            return;
        }

        var reaper = player.GetRole<Reaper>();
        reaper.causingCatastrophe = !reaper.causingCatastrophe;
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
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(OWColors.Reaper);
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
        bool otherReapersWon = false;
        var reapers = PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsRole<Reaper>() && x != Player);
        if (reapers.Any())
        {
            foreach (var reaper in reapers)
            {
                if (reaper.Data.Role.DidWin(gameOverReason)) otherReapersWon = true;
            }
        }
        return WinConditionMet() || otherReapersWon;
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

    public override void OnMeetingStart()
    {
        if (causingCatastrophe)
        {
            causingCatastrophe = false;

            var button = CustomButtonSingleton<Reaper_Catastrophe>.Instance;
            button.OverrideName("Catastrophe");
        }
    }

    public void LobbyStart()
    {
        ReaperVisual.CleanUp();
    }

    public bool causingCatastrophe;
    public List<byte> RevealedPlayers { get; set; } = [];
    public static bool VisibilityFlag(PlayerControl player)
    {
        if (PlayerControl.LocalPlayer.IsRole<Reaper>())
        {
            var mod = PlayerControl.LocalPlayer.GetRole<Reaper>()!;
            return mod.RevealedPlayers.Contains(player.PlayerId);
        }
        return false;
    }
}

#region Reaper_Attack
#endregion
public sealed class Reaper_Attack : ObjectWorkshopRoleButton<Reaper, PlayerControl>
{
    public override string Name => "Attack";
    public override Color TextOutlineColor => OWColors.Reaper;
    public override float Cooldown => OptionGroupSingleton<Reaper_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Reaper_Attack;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (target == null) return base.IsTargetValid(target);
        return target.IsTargetable();
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Reactor.Utilities.Logger<ObjectWorkshopPlugin>.Error("Reaper Ability: Target is null");
            return;
        }

        Role.Player.RpcCustomMurder(Target);
        Reaper.RpcReaperAttack(Role.Player, Target);
    }
}

#region Reaper_Catastrophe
#endregion
public sealed class Reaper_Catastrophe : ObjectWorkshopRoleButton<Reaper>
{
    public override string Name => "Catastrophe";
    public override Color TextOutlineColor => OWColors.Reaper;
    public override float Cooldown => 1f;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Reaper_Catastrophe;
    public override int MaxUses => (int)OptionGroupSingleton<Reaper_Options>.Instance.Charges;

    protected override void OnClick()
    {
        var reaper = PlayerControl.LocalPlayer.GetRole<Reaper>();
        if (reaper == null)
            return;

        Reaper.RpcCatastrophe(reaper.Player);

        if (reaper.causingCatastrophe) OverrideName("ENABLED");
        else OverrideName("Catastrophe");
        
        var button = CustomButtonSingleton<Reaper_Catastrophe>.Instance;
        button.IncreaseUses();
    }
}

#region Reaper_Options
#endregion
public sealed class Reaper_Options : AbstractOptionGroup<Reaper>
{
    public override string GroupName => TouLocale.Get(TouNames.Reaper, "Reaper");

    [ModdedNumberOption("<color=#5a8129>Reaper</color> <color=#4a86e8>Attack</color> Cooldown", 0, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25;

    [ModdedNumberOption("<color=#5a8129>Reaper</color> Max <color=#4a86e8>Catastrophies</color>", 1f, 15f, 1f)]
    public float Charges { get; set; } = 1f;

    [ModdedNumberOption("<color=#5a8129>Reaper</color> Vision", 0.25f, 60f, 0.25f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float Vision { get; set; } = 1f;

    [ModdedToggleOption("<color=#5a8129>Reaper</color> Can Vent")]
    public bool CanVent { get; set; } = true;
}