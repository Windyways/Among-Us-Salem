using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ObjectWorkshop.LifeImprovement.Roles;

#region UndeadReaper
#endregion
public sealed class UndeadReaper(IntPtr cppPtr)
    : NeutralRole(cppPtr), IOWRole, IWikiDiscoverable, IVisualAppearance
{
    public bool attractsMetal => true;
    public string RoleName => TouLocale.Get(TouNames.UndeadReaper, "Undead Reaper");
    public string revealText => "wants to bring the living to the dead.";
    public string RoleDescription => "Bring all of them down with you.";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => OWColors.UndeadReaper;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralPredator;

    public CustomRoleConfiguration Configuration => new(this)
    {
        MaxRoleCount = 0,
        CanModifyChance = false,
        //Icon = OWAssets.Placeholder,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Neutral Predator role that can kill others, but only spawns from Reaper, which can kill players while dead, although perishes once all Reapers are dead."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Attack",
            "You can Attack a player during the round. You will kill your target.",
            OWAssets.Reaper_Attack)
    ];

    public bool WinConditionMet()
    {
        return false;
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        Player?.ResetAppearance(fullReset: true);
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
        return otherReapersWon;
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

    public static bool ReapersAlive()
    {
        var reapersAlive = PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsRole<Reaper>() && !x.HasDied());
        return reapersAlive.Any();
    }

    #region RpcFlipReaperVisual
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.FlipReaperVisual, SendImmediately = true)]
    public static void RpcFlipReaperVisual(PlayerControl player)
    {
        if (player.Data.Role is not UndeadReaper)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcFlipReaperVisual - Invalid reaper");
            return;
        }

        var undeadReaper = player.GetRole<UndeadReaper>();
        if (undeadReaper.Player.AmOwner)
        {
            var reaperVisual = ReaperVisual.GetObjectByOwner(undeadReaper.Player);

            var sr = reaperVisual.gameObject.GetComponent<SpriteRenderer>();
            float horizontal = Input.GetAxisRaw("Horizontal");

            if (horizontal < 0) sr.flipX = true;  // facing left
            else if (horizontal > 0) sr.flipX = false; // facing right
        }

    }

    public VisualAppearance GetVisualAppearance()
    {
        var appearance = Player.GetDefaultAppearance();
        if (!ReapersAlive()) return appearance;
        
        appearance.Speed = OptionGroupSingleton<UndeadReaper_Options>.Instance.Speed;
        return appearance;
    }
}

#region UndeadReaper_Attack
#endregion
public sealed class UndeadReaper_Attack : ObjectWorkshopRoleButton<UndeadReaper, PlayerControl>
{
    public override string Name => "Attack";
    public override Color TextOutlineColor => OWColors.UndeadReaper;
    public override float Cooldown => OptionGroupSingleton<UndeadReaper_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Reaper_Attack;
    public override bool UsableInDeath => UndeadReaper.ReapersAlive();

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (target == null) return base.IsTargetValid(target);
        return target.IsTargetable() && !target.IsRole<Reaper>();
    }

    public override bool CanUse()
    {
        return base.CanUse() && UndeadReaper.ReapersAlive();
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Reactor.Utilities.Logger<ObjectWorkshopPlugin>.Error("Undead Reaper Ability: Target is null");
            return;
        }

        Role.Player.RpcCustomMurder(Target);
    }
}

#region UndeadReaper_Options
#endregion
public sealed class UndeadReaper_Options : AbstractOptionGroup<UndeadReaper>
{
    public override string GroupName => TouLocale.Get(TouNames.UndeadReaper, "UndeadReaper");

    [ModdedNumberOption("<color=#5a8129>Undead Reaper</color> <color=#4a86e8>Attack</color> Cooldown", 0, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25;

    [ModdedNumberOption("<color=#5a8129>Undead Reaper</color> Speed", 0.25f, 5f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float Speed { get; set; } = 0.25f;
}