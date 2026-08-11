using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class Plaguebearer(IntPtr cppPtr) : NeutralRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Plaguebearer";
    public string revealText => "is a carrier of disease.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are an acolyte of Pestilence, infecting the town with a deadly disease.";
    public Color RoleColor { get; set; } = RoleColors.Apocalypse;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Apocalypse;
    public Alignment Alignment => Alignment.NeutralApocalypse;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        //CanUseSabotage = OptionGroupSingleton<ApocOptions>.Instance.CanSabotage,
        CanUseVent = OptionGroupSingleton<Plaguebearer_Options>.Instance.CanVent,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.PlaguebearerRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that slowly infects the whole town, becoming Pestilence upon success.\n" +
            "Bring forth the Apocalypse, kill everyone in the town. You will win with other Neutral Apocalypse members." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Infect",
            "You can Infect a player at Night.\n" +
            "You will infect your target with Plague.\n" +
            "Infections spread when Infected players visit another player.\n" +
            "Infections spread when players visit an Infected player.\n" +
            "If all non-Apocalypse members are Infected, you will transform into Pestilence, horseman of apocalypse.",
            AUSAssets.Plaguebearer_Infect),
    ];

    public bool WinConditionMet() => ApocGameOver.WinConditionMet(this);
    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet() || ApocGameOver.AnyApocWon(gameOverReason);
    }

    public static string Info(PlayerControl player, PlayerControl target)
    {
        return $"{player.Name()} has infected {target.Name()} with the Plague.";
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        Player.RpcAddModifier<InfectedModifier>(Player);
    }

    [MethodRpc((uint)AUSRpc.RpcNotifyPlaguebearer)]
    public static void RpcNotify(PlayerControl PB, PlayerControl player, PlayerControl target, int notifyType)
    {
        if (PB.AmOwner() && !PB.HasDied())
        {
            var notify = (NotificationType)notifyType;
            switch (notify)
            {
                case NotificationType.Plaguebearer_SpreadPlague:
                    PB.Notify(Info(player, target), NotifyMode.OnlyMeeting, sprite: AUSAssets.PlaguebearerRoleCard.LoadAsset());
                    break;
            }
        }
    }

    public void Role_OnMeetingStart()
    {
        CheckForPestilence();
    }

    public void Role_AfterMurder(PlayerControl victim)
    {
        if (MeetingHud.Instance) CheckForPestilence();
    }

    private void CheckForPestilence()
    {
        if (!Player.HasDied())
        {
            var nonInfected = PlayerControl.AllPlayerControls.ToArray().Where(x =>
                !x.HasDied() && !x.IsSameFaction(Player) && !x.HasModifier<InfectedModifier>(x => x.Caster == Player)).ToList();

            if (nonInfected.Count <= 0)
            {
                foreach (var infected in ModifierUtils.GetActiveModifiers<InfectedModifier>()) infected.Player.RpcRemoveModifier<InfectedModifier>();
                Player.RpcChangeRole(RoleId.Get<Pestilence>());
            }
        }
    }

    public void Function(PlayerControl target, int Button)
    {
        if (Button is 1)
        {
            target.RpcAddModifier<InfectedModifier>(Player);
        }
    }
}

public sealed class Plaguebearer_Infect : TownOfUsRoleButton<Plaguebearer, PlayerControl>
{
    public override string Name => "Infect";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Apocalypse;
    public override float Cooldown => OptionGroupSingleton<Plaguebearer_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Plaguebearer_Infect;

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
        if (playerControl.IsRole<Plaguebearer>())
        {
            var nonInfected = PlayerControl.AllPlayerControls.ToArray().Where(x =>
                !x.HasDied() && !x.Is(Faction.Apocalypse) && !x.HasModifier<InfectedModifier>(x => x.Caster == Player)).ToList();

            Button?.usesRemainingText.gameObject.SetActive(true);
            Button?.usesRemainingSprite.gameObject.SetActive(true);
            Button!.usesRemainingText.text = nonInfected.Count.ToString();
        }

        base.FixedUpdate(playerControl);
    }

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 1, false, true);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !x.Is(Faction.Apocalypse) && !x.HasModifier<InfectedModifier>(x => x.Caster == Player));
    }

}

public sealed class Plaguebearer_Options : AbstractOptionGroup<Plaguebearer>
{
    public override string GroupName => "Plaguebearer";

    [ModdedNumberOption("Plaguebearer Infect Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedToggleOption("Plaguebearer Can Vent")]
    public bool CanVent { get; set; } = false;
}