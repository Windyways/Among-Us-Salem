using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Illusionist
#endregion
public sealed class Illusionist(IntPtr cppPtr)
    : NeutralRole(cppPtr), IWikiDiscoverable, IAUSRole, ICovenRole
{
    public string RoleName { get; set; } = TouLocale.Get(TouNames.Illusionist, "Illusionist");
    public string revealText => "can alter a person's appearance to others.";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction RoleFaction { get; set; } = Faction.Coven;
    public Color RoleColor { get; set; } = AUSColors.Coven;
    public Alignment Alignment => Alignment.CovenDeception;
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;

    public NecronomiconPriority NecronomiconPriority => NecronomiconPriority.Illusionist;
    public bool Necronomicon { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.IllusionistRoleCard,
        CanUseSabotage = true,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#B545FF>Illusionist</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#B545FF>Coven</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#B545FF>Coven</color> <color=#1e45d4>Deception</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill all who would oppose the Coven." +
            $"\n\nAttributes:" +
            "\nYou have access to Coven chat." +
            "\nWith the Necronomicon, you can deal a Basic Attack to Non-Coven targets." +
            "\nYou will obtain the Necronomicon 3rd, after the <color=#B545FF>Conjurer</color>." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Cast",
            "Your illusioned target will appear innocent to the Sheriff and Investigator." +
            "\n\nYour illusion will make a Psychic see your target as good." +
            "\n\nYour illusion will make Bodyguards and Traps see your target as a non-harmful visitor." +
            "\n\nSeers will see your illusioned target as a Town member." +
            "\n\nCasting an illusion on a Coven member is an Astral visit.",
            AUSAssets.Illusionist_Cast)
    ];

    public bool WinConditionMet()
    {
        var aliveCoven = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.Is(Faction.Coven));
        if (aliveCoven == 0) return false;

        var result = Helpers.GetAlivePlayers().Count <= aliveCoven && MiscUtils.KillersAliveCount() == aliveCoven;
        return result;
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    [MethodRpc((uint)AUSRpc.Illusionist_Cast, SendImmediately = true)]
    public static void RpcIllusionist_Cast(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Illusionist)
        {
            Logger<AUSPlugin>.Error("RpcIllusionist_Illusion - Invalid Illusionist");
            return;
        }

        var illusionist = player.GetRole<Illusionist>();
        illusionist.IllusionedPlayer = target;
    }

    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);
        IllusionedPlayer = null;
    }

    public PlayerControl IllusionedPlayer;
}

#region Illusionist_Cast
#endregion
public sealed class Illusionist_Cast : AmongUsSalemRoleButton<Illusionist, PlayerControl>
{
    public override string Name => "Cast";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Coven;
    public override float Cooldown => OptionGroupSingleton<Illusionist_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Illusionist_Cast;

    public override void FixedUpdateHandler(PlayerControl playerControl)
    {
        base.FixedUpdateHandler(playerControl);
        if (Role.Necronomicon)
        {
            if (Target != null)
            {
                if (Role.Player.IsSameFaction(Target))
                {
                    OverrideSprite(AUSAssets.Illusionist_Cast.LoadAsset());
                    OverrideName("Cast");
                    return;
                }
            }

            OverrideSprite(AUSAssets.NecronomiconButton.LoadAsset());
            OverrideName("Attack");
        }
    }

    public override void ClickHandler()
    {
        if (Target != null)
        {
            if (MiscUtils.SuccessfulVisit(Player, Target, Role.Necronomicon, Role.Player.IsSameFaction(Target)))
            {
                base.ClickHandler();
            }
        }
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        if (Role.Player.IsSameFaction(Target)) // Target is Coven, or whatever Illu's faction is.
        {
            Illusionist.RpcIllusionist_Cast(Player, Target);
            MiscUtils.PostSuccessfulVisit(Player, Target, false, false);
        }
        else
        {
            if (Player.CanKill(Target)) MiscUtils.RpcApplyDeathReason(Player, Target, DeathReasonShow.KilledByTheCoven);
            else MiscUtils.ShowNotification(MessageTexts.TooMuchDefense(Player, Target), Color.white);
            MiscUtils.PostSuccessfulVisit(Player, Target, true, true);
        }
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (target == null) return base.IsTargetValid(target);
        return base.IsTargetValid(target) &&
            !(target.Data.Role is IAUSRole ausrole && ausrole.RoleFaction != Role.RoleFaction && !Role.Necronomicon) &&
            Role.IllusionedPlayer != target;
    }
}

#region Illusionist_Options
#endregion
public sealed class Illusionist_Options : AbstractOptionGroup<Illusionist>
{
    public override string GroupName => TouLocale.Get(TouNames.Illusionist, "Illusionist");

    [ModdedNumberOption("<color=#B545FF>Illusionist</color> <color=#4a86e8>Cast</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}