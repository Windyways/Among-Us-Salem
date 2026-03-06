using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Berserker(IntPtr cppPtr) : CovenRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Berserker";
    public string revealText => "gets more powerful with each kill.";
    public string RoleDescription => "";
    public string RoleLongDescription => "You are an acolyte of War, embodying nothing but raw power.";
    public Color RoleColor { get; set; } = RoleColors.Apocalypse;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Neutral;
    public Alignment Alignment => Alignment.NeutralApocalypse;

    public Attack Attack { get; set; } = Attack.Powerful;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Powerful;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseSabotage = OptionGroupSingleton<ApocOptions>.Instance.CanSabotage,
        CanUseVent = OptionGroupSingleton<Berserker_Options>.Instance.CanVent,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.BerserkerRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can attack others, unlike its teammates. After a certain amount of kills, it can kill all nearby players, making it a very dangerous and top-priority threat later in the game.\n" +
            "Bring forth the Apocalypse, kill everyone in the town. You will win with other Neutral Apocalypse members." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Attack",
            "You can Attack a player at Night.\n" +
            "You will deal a Powerful Attack to your target.\n" +
            "If you have 0 kills, you can only Attack on Full Moon Nights.\n" +
            "If you have 1 kill, you can Attack every Night.\n" +
            "If you have 2 kills, you will Rampage your target and attack surrounding players.\n" +
            "Once you obtain 3 kills, you will transform into War, horseman of apocalypse.",
            AUSAssets.Berserker_Attack),
    ];

    public bool WinConditionMet() => ApocGameOver.WinConditionMet(this);
    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet() || ApocGameOver.AnyApocWon(gameOverReason);
    }

    public static string Info(int kills)
    {
        if (kills == 1) return "Blood empowers you! You may now attack every night.";
        if (kills == 2) return "Blood empowers you! You will now Rampage at your target's house, passively attacking any nearby players.";
        return "Blood empowers you! You deal Unstoppable Attacks when you Rampage now! You have gained Invincibile Defense.";
    }

    public void Role_OnMeetingStart()
    {
        if (Kills >= 3) Player.RpcChangeRole(RoleId.Get<War>());
    }

    public int Kills;
}

public sealed class Berserker_Attack : TownOfUsRoleButton<Berserker, PlayerControl>
{
    public override string Name => "Attack";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Apocalypse;
    public override float Cooldown => OptionGroupSingleton<Berserker_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Berserker_Attack;

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);
        if (KeybindIcon != null)
        {
            KeybindIcon.transform.localPosition = new Vector3(0.4f, 0.45f, -9f);
        }
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        if (playerControl.IsRole<Berserker>())
        {
            var role = playerControl.GetRole<Berserker>();
            Button?.usesRemainingText.gameObject.SetActive(true);
            Button?.usesRemainingSprite.gameObject.SetActive(true);
            Button!.usesRemainingText.text = role.Kills.ToString();
        }

        base.FixedUpdate(playerControl);
    }

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, true, true)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        if (Target == null)
            return;

        if (Role.Kills >= 2)
        {
            Role.Kills += Player.Rampage(Target, DeathReasonShow.DestroyedByABeserker);
        }
        else if (Player.CanKill(Target))
        {
            Role.Kills++;
            Player.RpcCustomMurder(Target);
            VisitingMechanic.RpcAddDeathReason(Target, (int)DeathReasonShow.DestroyedByABeserker);

            Player.Notify(Berserker.Info(Role.Kills), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.BerserkerRoleCard.LoadAsset());
        }
        else Player.Notify(Feedback.TooMuchDefense(Player, Target), NotifyMode.InstantlyAndMeeting);
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !x.Is(Alignment.NeutralApocalypse));
    }

    public override bool CanUse()
    {
        if (Role.Kills == 0 && !DayNightMechanic.FullMoon()) return false;
        return base.CanUse();
    }
}

public sealed class Berserker_Options : AbstractOptionGroup<Berserker>
{
    public override string GroupName => "Berserker";

    [ModdedNumberOption("Berserker Attack Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedToggleOption("Berserker Can Vent")]
    public bool CanVent { get; set; } = true;
}