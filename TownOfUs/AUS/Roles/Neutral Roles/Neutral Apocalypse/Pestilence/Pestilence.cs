using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

namespace AmongUsSalem.Roles;

public sealed class Pestilence(IntPtr cppPtr) : NeutralRole(cppPtr), ICustomAURole, IWikiDiscoverable, ISpawnChange
{
    public string RoleName { get; set; } = "Pestilence";
    public string revealText => "reeks of disease.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "";
    public Color RoleColor { get; set; } = RoleColors.Apocalypse;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Apocalypse;
    public Alignment Alignment => Alignment.NeutralApocalypse;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.Invincible;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.Invincible;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public bool NoSpawn => true;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanModifyChance = false,
        MaxRoleCount = 0,

        DefaultChance = 0,
        DefaultRoleCount = 0,

        //CanUseSabotage = OptionGroupSingleton<ApocOptions>.Instance.CanSabotage,
        CanUseVent = OptionGroupSingleton<Pestilence_Options>.Instance.CanVent,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.PestilenceRoleCard
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
            $"{RoleName} is a {Alignment.ToSpacedString()} role that punishes players that visit with death, discouraging most Night activity.\n" +
            "Bring forth the Apocalypse, kill everyone in the town. You will win with other Neutral Apocalypse members." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Spread Pestilence",
            "You can Spread Pestilence to a player at Night.\n" +
            "You will add 3 stacks of pestilence to your target.",
            AUSAssets.Pestilence_SpreadPestilence),
    ];

    public string GetAttributes()
    {
        return
            $"- You are RoleBlock immune.\n" +
            $"- Any players visiting you will receive a stack of pestilence.\n" +
            $"- Players that visit will spread a stack of pestilence to their targets.\n" +
            $"- At 3 stacks of pestilence a player will succumb and die.\n" +
            $"- Horsemen cannot have more than 1 stack of pestilence.";
    }

    [MethodRpc((uint)AUSRpc.RpcNotifyPestilence)]
    public static void RpcNotify(PlayerControl player, int notifyType)
    {
        if (player.AmOwner())
        {
            var notify = (NotificationType)notifyType;
            switch (notify)
            {
                case NotificationType.Pestilence_Reveal:
                    player.Notify(Info(), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.PestilenceRoleCard.LoadAsset());
                    break;
            }
        }
    }

    public bool WinConditionMet() => ApocGameOver.WinConditionMet(this);
    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet() || ApocGameOver.AnyApocWon(gameOverReason);
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        foreach (var players in PlayerControl.AllPlayerControls) players.RpcAddModifier<StackOfPestilenceModifier>(Player);

        RpcNotify(PlayerControl.LocalPlayer, (int)NotificationType.Pestilence_Reveal);
        Player.RpcAddModifier<RoleBlockImmune>(true);
    }

    public static string Info()
    {
        return $"A Plague has consumed the Town, transforming the Plaguebearer into Pestilence, Horseman of the Apocalypse!";
    }

    public void Function(PlayerControl target, int Button)
    {
        if (Button == 1)
        {
            if (target.TryGetModifier<StackOfPestilenceModifier>(out var stackOP)) stackOP.AddStack(3);
        }
    }
}

public sealed class Pestilence_SpreadPestilence : TownOfUsRoleButton<Pestilence, PlayerControl>
{
    public override string Name => "Spread Pestilence";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Apocalypse;
    public override float Cooldown => OptionGroupSingleton<Pestilence_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Pestilence_SpreadPestilence;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 1, false, true);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x => 
            !x.Is(Faction.Apocalypse));
    }
}

public sealed class Pestilence_Options : AbstractOptionGroup<Pestilence>
{
    public override string GroupName => "Pestilence";

    [ModdedNumberOption("Pestilence Spread Pestilence Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedToggleOption("Pestilence Can Vent")]
    public bool CanVent { get; set; } = true;
}