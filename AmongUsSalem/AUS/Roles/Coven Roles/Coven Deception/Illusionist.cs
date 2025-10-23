using System.Text;
using AmongUsSalem.Utilities;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Illusionist
#endregion
public sealed class Illusionist(IntPtr cppPtr)
    : CovenRole(cppPtr), IWikiDiscoverable, ICustomAURole, ICovenRole
{
    public string RoleName { get; set; } = "Illusionist";
    public string revealText => "can alter a person's appearance to others.";
    public string RoleDescription => "Disguise the Coven.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Coven;
    public Color RoleColor { get; set; } = AUSColors.Coven;
    public Alignment Alignment => Alignment.CovenDeception;
    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public NecronomiconPriority NecronomiconPriority => NecronomiconPriority.Illusionist;
    public bool Necronomicon { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.IllusionistRoleCard,
        CanUseSabotage = OptionGroupSingleton<CovenOptions>.Instance.CanSabotage,
        CanUseVent = OptionGroupSingleton<CovenOptions>.Instance.CanVent,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
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
            "\nWith the Necronomicon, you can deal a Basic Attack to Non-Coven targets." +
            "\nYou will obtain the Necronomicon 3rd, after the <color=#B545FF>Conjurer</color>." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Cast",
            "You can Cast an illusion on a Coven member at Night.\n\n".ApplyKeywords() +
            "You will make your target appear as a member of the Town for the Night.\n\n".ApplyKeywords() +
            "This is an Astral visit.".ApplyKeywords(),
            AUSAssets.Illusionist_Cast)
    ];

    public bool WinConditionMet()
    {
        var aliveCoven = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.Is(Faction.Coven));
        if (aliveCoven == 0) return false;

        var result = MiscUtils.GetAlivePlayersToEnd().Count <= aliveCoven && MiscUtils.KillersAliveCount() == aliveCoven;
        return result || AmongUsSalem.Patches.LogicGameFlowPatches.EndGameEarlyCheck(this);
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
        if (Necronomicon) CovenNecronomiconMechanic.AdjustButtons(Player);
    }

    public PlayerControl IllusionedPlayer;
}

#region Illusionist_Cast
#endregion
public sealed class Illusionist_Cast : AmongUsSalemRoleButton<Illusionist, PlayerControl>
{
    public override string Name => "Cast";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Coven;
    public override float Cooldown => Role.Necronomicon ? OptionGroupSingleton<CovenOptions>.Instance.Cooldown : OptionGroupSingleton<Illusionist_Options>.Instance.Cooldown;
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
        if (Target != null && Timer <= 0)
        {
            if (MiscUtils.SuccessfulVisit(Player, Target, Role.Necronomicon && !Role.Player.IsSameFaction(Target), Role.Player.IsSameFaction(Target)))
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
            else
            {
                MiscUtils.ShowNotification(MessageTexts.TooMuchDefense(Player, Target), Color.white);
                MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Neutral, "General Info"), MessageTexts.TooMuchDefense(Player, Target));
            }
            MiscUtils.PostSuccessfulVisit(Player, Target, true, true);
        }
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, predicate: x =>
            x != Role.IllusionedPlayer &&
            !(x.Data.Role is ICustomAURole customRole && customRole.Faction == Role.Faction && !Role.Necronomicon));
    }
}

#region Illusionist_Options
#endregion
public sealed class Illusionist_Options : AbstractOptionGroup<Illusionist>
{
    public override string GroupName => "Illusionist";

    [ModdedNumberOption("<color=#B545FF>Illusionist</color> <color=#4a86e8>Cast</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}