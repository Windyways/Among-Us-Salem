using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class War(IntPtr cppPtr) : NeutralRole(cppPtr), ICustomAURole, IWikiDiscoverable, ISpawnChange
{
    public string RoleName { get; set; } = "War";
    public string revealText => "fills you with hate towards everyone.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "";
    public Color RoleColor { get => FlexibleFactions.GetNewFaction(OptionGroupSingleton<Berserker_Options>.Instance.faction.Value).Item2; set { } }
    public ModdedRoleTeams Team => FlexibleFactions.GetNewFaction(OptionGroupSingleton<Berserker_Options>.Instance.faction.Value).Item3;

    public Faction Faction { get; set; } = FlexibleFactions.GetNewFaction(OptionGroupSingleton<Berserker_Options>.Instance.faction.Value).Item1;
    public Alignment Alignment => Alignment.NeutralApocalypse;

    public Attack Attack { get; set; } = Attack.Unstoppable;
    public Defense Defense { get; set; } = Defense.Invincible;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Unstoppable;
    public Defense ogDefense { get; set; } = Defense.Invincible;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public bool NoSpawn => true;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanModifyChance = false,
        MaxRoleCount = 0,

        DefaultChance = 0,
        DefaultRoleCount = 0,

        CanUseSabotage = OptionGroupSingleton<ApocOptions>.Instance.CanSabotage,
        CanUseVent = OptionGroupSingleton<War_Options>.Instance.CanVent,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.WarRoleCard
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
            $"{RoleName} is a {Alignment.ToSpacedString()} role that can attack multiple players, as well as killing all nearby players, making it almost unstoppable once it’s transformed.\n" +
            "Bring forth the Apocalypse, kill everyone in the town. You will win with other Neutral Apocalypse members." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Attack",
            "You can Attack a player at Night.\n" +
            "You will deal an Unstoppable Attack to your target and Rampage your target, attacking surrounding players.",
            AUSAssets.War_Attack),

        new("Attack",
            "You can Attack a player at Night.\n" +
            "You will deal an Unstoppable Attack to your target and Rampage your target, attacking surrounding players.",
            AUSAssets.War_Attack),
    ];

    [MethodRpc((uint)AUSRpc.RpcNotifyWar)]
    public static void RpcNotify(PlayerControl player, int notifyType)
    {
        if (player.AmOwner())
        {
            var notify = (NotificationType)notifyType;
            switch (notify)
            {
                case NotificationType.War_Reveal:
                    player.Notify(Info(), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.WarRoleCard.LoadAsset());
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
        RpcNotify(PlayerControl.LocalPlayer, (int)NotificationType.War_Reveal);
    }

    public static string Info()
    {
        return $"The Berserker has transformed into War, Horseman of the Apocalypse! Cry 'Havoc!', and let slip the dogs of war.";
    }
}

public sealed class War_Attack : TownOfUsRoleButton<War, PlayerControl>, IButtonClick
{
    public override string Name => "Attack";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Apocalypse;
    public override float Cooldown => OptionGroupSingleton<War_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.War_Attack;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, true, true)) base.ClickHandler();
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x => 
            !x.Is(Faction.Apocalypse));
    }

    protected override void OnClick() => Click(Player, Target);
    public void Click(PlayerControl player, PlayerControl Target = null)
    {
        if (Target == null)
            return;

        Player.Rampage(Target, DeathReasonShow.DestroyedByWarHorsemanOfTheApocalypse);
    }
}

public sealed class War_Attack2 : TownOfUsRoleButton<War, PlayerControl>, IButtonClick
{
    public override string Name => "Attack";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => RoleColors.Apocalypse;
    public override float Cooldown => OptionGroupSingleton<War_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.War_Attack;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, true, true)) base.ClickHandler();
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !x.Is(Faction.Apocalypse));
    }

    protected override void OnClick() => Click(Player, Target);
    public void Click(PlayerControl player, PlayerControl Target = null)
    {
        if (Target == null)
            return;

        Player.Rampage(Target, DeathReasonShow.DestroyedByWarHorsemanOfTheApocalypse);
    }
}

public sealed class War_Options : AbstractOptionGroup<War>
{
    public override string GroupName => "War";

    [ModdedNumberOption("War Attack Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedToggleOption("War Can Vent")]
    public bool CanVent { get; set; } = true;
}