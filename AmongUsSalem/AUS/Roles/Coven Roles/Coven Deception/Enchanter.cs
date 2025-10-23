using System.Text;
using AmongUsSalem.Modules.Components;
using AmongUsSalem.Utilities;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Enchanter
#endregion
public sealed class Enchanter(IntPtr cppPtr)
    : CovenRole(cppPtr), IWikiDiscoverable, ICustomAURole, ICovenRole
{
    public string RoleName { get; set; } = "Enchanter";
    public string revealText => "is trained in enchantments.";
    public string RoleDescription => "Enchant others, change how players see ones role.";
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

    public NecronomiconPriority NecronomiconPriority => NecronomiconPriority.Enchanter;
    public bool Necronomicon { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.EnchanterRoleCard,
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
            "<color=#B545FF>Enchanter</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#B545FF>Coven</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#B545FF>Coven</color> <color=#1e45d4>Deception</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill all who would oppose the Coven." +
            $"\n\nAttributes:" +
            "\nWith the Necronomicon, you will deal a Basic Attack your targets." +
            "\nYou will obtain the Necronomicon 8th, after the <color=#B545FF>Dreamweaver</color>." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Cast",
            "You can Cast an illusion on a Coven member at Night.\n\n".ApplyKeywords() +
            "You will make your target appear as a member of the Town for the Night.\n\n".ApplyKeywords() +
            "This is an Astral visit.".ApplyKeywords(),
            AUSAssets.Enchanter_Enchant)
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

    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);
        
        if (Necronomicon) CovenNecronomiconMechanic.AdjustButtons(Player);
    }

    [MethodRpc((uint)AUSRpc.RpcEnchant, SendImmediately = true)]
    public static void RpcEnchant(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Enchanter)
        {
            Logger<AUSPlugin>.Error("RpcEnchant - Invalid Enchanter");
            return;
        }

        var enchanter = player.GetRole<Enchanter>();
        enchanter.EnchantedPlayers.Add(target.PlayerId);
    }

    [MethodRpc((uint)AUSRpc.RpcRemoveEnchant, SendImmediately = true)]
    public static void RpcRemoveEnchant(PlayerControl target)
    {
        foreach (var enchanters in MiscUtils.GetPlayersWithRole<Enchanter>())
        {
            var enchanter = enchanters.GetRole<Enchanter>();
            enchanter.EnchantedPlayers.Remove(target.PlayerId);
        }
    }

    [MethodRpc((uint)AUSRpc.RpcDoAlterate, SendImmediately = true)]
    public static void RpcDoAlterate(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Enchanter)
        {
            Logger<AUSPlugin>.Error("RpcDoAlterate - Invalid Enchanter");
            return;
        }

        var enchanter = player.GetRole<Enchanter>();
        if (player.AmOwner())
        {
            enchanter.Charges--;

            var button = CustomButtonSingleton<Enchanter_AlteratePlayer>.Instance;
            button.UsesLeft = enchanter.Charges;

            var button2 = CustomButtonSingleton<Enchanter_AlterateBody>.Instance;
            button2.UsesLeft = enchanter.Charges;

            if (enchanter.alteratingRole is ICustomAURole customRole)
            {
                target.RpcAddModifier<DeepfakeRole>(customRole.RoleName, customRole.RoleColor);
            }
        }
    }

    [MethodRpc((uint)AUSRpc.RpcAlterate, SendImmediately = true)]
    public static void RpcAlterate(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Enchanter)
        {
            Logger<AUSPlugin>.Error("RpcAlterate - Invalid Enchanter");
            return;
        }

        var forger = player.GetRole<Enchanter>();
        forger.AlteratedPlayer = target;
    }

    public override void OnMeetingStart()
    {
        AlteratedPlayer = null;
    }

    public void OnTargetDeath(PlayerControl target, DeathReason? reason)
    {
        if (target == AlteratedPlayer)
        {
            RpcDoAlterate(Player, target);
            AlteratedPlayer = null;
        }
    }

    public void AlterateMenu()
    {
        if (Minigame.Instance != null)
        {
            return;
        }

        var shapeMenu = GuesserMenu.Create();
        shapeMenu.Begin(IsRoleValid, ClickRoleHandle);

        void ClickRoleHandle(RoleBehaviour role)
        {
            alteratingRole = role;
            shapeMenu.Close();
        }
    }

    private bool IsRoleValid(RoleBehaviour role)
    {
        if (role.IsDead) return false;
        if (role is IGhostRole) return false;
        return true;
    }

    public List<byte> EnchantedPlayers = new List<byte>();
    public int Charges = (int)OptionGroupSingleton<Enchanter_Options>.Instance.Charges;
    public PlayerControl AlteratedPlayer;
    public RoleBehaviour alteratingRole;
}

#region Enchanter_Enchant
#endregion
public sealed class Enchanter_Enchant : AmongUsSalemRoleButton<Enchanter, PlayerControl>
{
    public override string Name => "Enchant";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Coven;
    public override float Cooldown => Role.Necronomicon ? OptionGroupSingleton<CovenOptions>.Instance.Cooldown : OptionGroupSingleton<Enchanter_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Enchanter_Enchant;

    public override void ClickHandler()
    {
        if (Target != null && Timer <= 0)
        {
            if (MiscUtils.SuccessfulVisit(Player, Target, Role.Necronomicon, true))
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

        Enchanter.RpcEnchant(Player, Target);
        if (Role.Necronomicon)
        {
            if (Player.CanKill(Target)) MiscUtils.RpcApplyDeathReason(Player, Target, DeathReasonShow.KilledByTheCoven);
            else
            {
                MiscUtils.ShowNotification(MessageTexts.TooMuchDefense(Player, Target), Color.white);
                MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Neutral, "General Info"), MessageTexts.TooMuchDefense(Player, Target));
            }
        }

        MiscUtils.PostSuccessfulVisit(Player, Target, Role.Necronomicon, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(false, Distance, predicate: x =>
            !Role.EnchantedPlayers.Contains(x.PlayerId) &&
            !(x.Data.Role is ICustomAURole customRole && customRole.Faction != Role.Faction && !Role.Necronomicon));
    }
}

#region Enchanter_AlteratePlayer
#endregion
public sealed class Enchanter_PickAlterate : AmongUsSalemRoleButton<Enchanter>
{
    public override string Name => "Alterating Role";
    public override BaseKeybind Keybind => Keybinds.TertiaryAction;
    public override Color TextOutlineColor => AUSColors.Coven;
    public override float Cooldown => 0.1f;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Enchanter_Alterate;
    public override ButtonLocation Location => ButtonLocation.BottomLeft;

    public override void ClickHandler()
    {
        if (Timer <= 0)
        {
            if (MiscUtils.SuccessfulVisit(Player, Player, false, false))
            {
                base.ClickHandler();
            }
        }
    }

    protected override void OnClick()
    {
        Role.AlterateMenu();

        MiscUtils.PostSuccessfulVisit(Player, Player, false, false, false);
    }
}

#region Enchanter_AlteratePlayer
#endregion
public sealed class Enchanter_AlteratePlayer : AmongUsSalemRoleButton<Enchanter, PlayerControl>
{
    public override string Name => "Alterate";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => AUSColors.Coven;
    public override float Cooldown => Role.Necronomicon ? OptionGroupSingleton<CovenOptions>.Instance.Cooldown : OptionGroupSingleton<Enchanter_Options>.Instance.Cooldown;
    public override int MaxUses => (int)OptionGroupSingleton<Enchanter_Options>.Instance.Charges;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Enchanter_Alterate;

    public override void ClickHandler()
    {
        if (Target != null && Timer <= 0)
        {
            if (MiscUtils.SuccessfulVisit(Player, Target, Role.Necronomicon, true))
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

        if (Player.AmOwner)
        {
            var button = CustomButtonSingleton<Enchanter_AlterateBody>.Instance;
            button.ResetCooldownAndOrEffect();
            button.UsesLeft = Role.Charges;
        }

        Enchanter.RpcAlterate(Player, Target);
        if (Role.Necronomicon)
        {
            if (Player.CanKill(Target)) MiscUtils.RpcApplyDeathReason(Player, Target, DeathReasonShow.KilledByTheCoven);
            else
            {
                MiscUtils.ShowNotification(MessageTexts.TooMuchDefense(Player, Target), Color.white);
                MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Neutral, "General Info"), MessageTexts.TooMuchDefense(Player, Target));
            }
        }

        MiscUtils.PostSuccessfulVisit(Player, Target, Role.Necronomicon, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, predicate: x =>
            x != Role.AlteratedPlayer &&
            !(x.Data.Role is ICustomAURole customRole && customRole.Faction != Role.Faction && !Role.Necronomicon));
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role.alteratingRole != null;
    }
}

#region Enchanter_AlterateBody
#endregion
public sealed class Enchanter_AlterateBody : AmongUsSalemRoleButton<Enchanter, DeadBody>
{
    public override string Name => "Alterate (Body)";
    public override BaseKeybind Keybind => Keybinds.ModifierAction;
    public override Color TextOutlineColor => AUSColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Enchanter_Options>.Instance.Cooldown;
    public override int MaxUses => (int)OptionGroupSingleton<Enchanter_Options>.Instance.Charges;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Enchanter_Alterate;

    public override void ClickHandler()
    {
        if (Target != null && Timer <= 0)
        {
            var targetId = Target.ParentId;
            var targetPlayer = MiscUtils.PlayerById(targetId);

            if (MiscUtils.SuccessfulVisit(Player, targetPlayer, false, true))
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

        var targetId = Target.ParentId;
        var targetPlayer = MiscUtils.PlayerById(targetId);

        if (targetPlayer == null)
        {
            return; // Someone may have left mid game or something and gc just vacuumed, but idk. better safe than sorry ig.
        }

        if (Player.AmOwner)
        {
            var button = CustomButtonSingleton<Enchanter_AlteratePlayer>.Instance;
            button.ResetCooldownAndOrEffect();
            button.DecreaseUses();
        }

        Enchanter.RpcDoAlterate(Player, targetPlayer);
        MiscUtils.PostSuccessfulVisit(Player, targetPlayer, false, true);
    }

    public override DeadBody? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetNearestDeadBody(Distance);
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role.alteratingRole != null;
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Target != null;
    }
}

#region Enchanter_Options
#endregion
public sealed class Enchanter_Options : AbstractOptionGroup<Enchanter>
{
    public override string GroupName => "Enchanter";

    [ModdedNumberOption("<color=#B545FF>Enchanter</color> <color=#4a86e8>Enchant</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#B545FF>Enchanter</color> <color=#4a86e8>Alterate</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float AlterateCD { get; set; } = 25f;

    [ModdedNumberOption("<color=#B545FF>Enchanter</color> Max <color=#4a86e8>Alterates</color>", 1f, 30f, 1f)]
    public float Charges { get; set; } = 2f;
}