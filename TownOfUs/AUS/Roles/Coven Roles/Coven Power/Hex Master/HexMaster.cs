using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class HexMaster(IntPtr cppPtr) : CovenRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Hex Master";
    public string revealText => "is versed in the ways of hexes.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are a powerful spellcaster preparing to disintegrate the town.";
    public Color RoleColor { get; set; } = RoleColors.Coven;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Coven;
    public Alignment Alignment => Alignment.CovenPower;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        // CanUseSabotage = OptionGroupSingleton<CovenOptions>.Instance.CanSabotage,
        CanUseVent = OptionGroupSingleton<CovenOptions>.Instance.CanVent,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.HexMasterRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return 
            $"Attack: {Attack}\n" +
            $"Defense: {Defense}\n" +
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can Hex players, and kill them all once everyone is Hexed. This role is good at rewarding slow gameplay.\n" +
            "Kill all who would oppose the Coven." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- With the Necronomicon, you will also deal a Basic Attack to your targets.\n" +
            $"- Your attacks are Astral while you hold the Necronomicon.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Hex",
            "You can Hex a player at Night.\n" +
            "If all alive non-Coven members are Hexed, perform a Hex-Bomb to kill all opposing players.",
            AUSAssets.HexMaster_Hex),
    ];

    public bool WinConditionMet() => CovenGameOver.WinConditionMet(this);
    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet() || CovenGameOver.AnyCovenWon(gameOverReason);
    }

    public void Role_OnMeetingStart()
    {
        CheckForHexBomb();
    }

    public void Role_AfterMurder(PlayerControl victim)
    {
        if (MeetingHud.Instance) CheckForHexBomb();
    }

    private void CheckForHexBomb()
    {
        if (!Player.HasDied())
        {
            var nonHexed = PlayerControl.AllPlayerControls.ToArray().Where(x =>
                !x.HasDied() && !x.Is(Faction.Coven) && !x.HasModifier<HexedModifier>(x => x.Caster == Player)).ToList();

            var nonCoven = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && !x.Is(Faction.Coven)).ToList();
            if (nonHexed.Count <= 0)
            {
                foreach (var player in nonCoven)
                {
                    Player.RpcCustomMurder(player);
                    VisitingMechanic.RpcAddDeathReason(player, (int)DeathReasonShow.DisintegratedByAHexMaster);
                }
            }
        }
    }

    public void Function(PlayerControl target, int Button)
    {
        if (Button == 1)
        {
            target.RpcAddModifier<HexedModifier>(Player);
            if (Player.HasNecronomicon())
            {
                if (Player.CanKill(target))
                {
                    Player.RpcCustomMurder(target);
                    VisitingMechanic.RpcAddDeathReason(target, (int)DeathReasonShow.KilledByTheCoven);
                }
                else Player.Notify(Feedback.TooMuchDefense(Player, target), NotifyMode.InstantlyAndMeeting);
            }
        }
    }
}

public sealed class HexMaster_Hex : TownOfUsRoleButton<HexMaster, PlayerControl>
{
    public override string Name => "Hex";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Coven;
    public override float Cooldown => Player.HasNecronomicon() ? OptionGroupSingleton<CovenOptions>.Instance.Cooldown : 
        OptionGroupSingleton<HexMaster_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.HexMaster_Hex;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 1, Player.HasNecronomicon(), true);
    public override PlayerControl? GetTarget()
    {
        if (Player.HasNecronomicon())
            return Player.GetClosestLivingPlayer(true, Distance, predicate: x => 
                !x.Is(Faction.Coven));

        return Player.GetClosestLivingPlayer(true, Distance, predicate: x => 
            !x.Is(Faction.Coven) && !x.HasModifier<HexedModifier>(x => x.Caster == Player));
    }
}

public sealed class HexMaster_Options : AbstractOptionGroup<HexMaster>
{
    public override string GroupName => "Hex Master";

    [ModdedNumberOption("Hex Master Hex Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}