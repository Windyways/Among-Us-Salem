using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class Amnesiac(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Amnesiac";
    public string revealText => "does not remember their role.";
    public string RoleDescription => OptionGroupSingleton<Amnesiac_Options>.Instance.Mode == AmnesiacMode.TOS1 ? "Town Of Salem" : "Town Of Salem 2";
    public string RoleLongDescription => "You do not remember who you are.";
    public Color RoleColor { get => RoleColors.Amnesiac(); set { } }

    public ModdedRoleTeams Team =>
        OptionGroupSingleton<Amnesiac_Options>.Instance.Mode == AmnesiacMode.TOS1 ?
        ModdedRoleTeams.Custom :
        ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } =
        OptionGroupSingleton<Amnesiac_Options>.Instance.Mode == AmnesiacMode.TOS1 ? 
        Faction.Neutral :
        Faction.Town;
    public Alignment Alignment =>
        OptionGroupSingleton<Amnesiac_Options>.Instance.Mode == AmnesiacMode.TOS1 ? 
        Alignment.NeutralBenign :
        Alignment.TownSupport;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.AmnesiacRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        if (OptionGroupSingleton<Amnesiac_Options>.Instance.Mode == AmnesiacMode.TOS1)
        {
            return
                $"Attack: {Attack}\n" +
                $"Defense: {Defense}\n" +
                $"The {RoleName} is a {Alignment.ToSpacedString()} role that can choose a dead player's role to become.\n" +
                "Remember who you were and complete that role’s goal." +
                MiscUtils.AppendOptionsText(GetType());
        }

        return
            $"Attack: {Attack}\n" +
            $"Defense: {Defense}\n" +
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that becomes the role of a dead Town member, which can be very useful if a highly valuable Town role dies very early.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        if (OptionGroupSingleton<Amnesiac_Options>.Instance.Mode == AmnesiacMode.TOS1) return "- None.";
        return $"- You will prioritize Remembering Town Executive, Catalyst & Town Government roles.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities =>
    [
        new("Remember", GetRememberDescription(), OptionGroupSingleton<Amnesiac_Options>.Instance.Mode == AmnesiacMode.TOS1 ?
            AUSAssets.Amnesiac_Remember_TOS1 : AUSAssets.Amnesiac_Remember_TOS2)
    ];

    private static string GetRememberDescription()
    {
        var opt = OptionGroupSingleton<Amnesiac_Options>.Instance;

        if (opt.Mode == AmnesiacMode.TOS1)
        {
            return "You can Remember a dead player at Night.\n" +
            "You will become your target’s role when the Day begins, and all players are notified that an Amnesiac Remembered the role you chose.\n" +
            "Only one Amnesiac can Remember a Unique role. This is Astral.\n" +
            "You cannot Remember Stoned or Cleaned roles.\n" +
            "If you attempt to Remember as a Godfather, you will instead become a Mafioso if all Mafia members are dead or there is a Godfather already alive.";
        }

        return
            "You will automatically attempt to Remember a Town role in the Graveyard during the Day.\n" +
            "You will become a random Town role in the Graveyard.\n" +
            "If a player dies during the Day, they will have a chance to be Remembered along with the people that die during the Night.\n" +
            "If the Teams Modifier is enabled, you may Remember any role.";
    }

    [MethodRpc((uint)AUSRpc.RpcGlobalRemember)]
    public static void RpcGlobalRemember(PlayerControl player, string text)
    {
        if (player.Data.Role is not Amnesiac)
        {
            Logger<AUSPlugin>.Error("RpcGlobalRemember - Invalid Amnesiac");
            return;
        }

        PlayerControl.LocalPlayer.Notify(Info(NotificationType.Amnesiac_GlobalReveal, text), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.AmnesiacRoleCard_TOS1.LoadAsset());
    }

    public override void Initialize(PlayerControl player) // This patches the ability sprite since it's loaded on runtime.
    {
        RoleBehaviourStubs.Initialize(this, player);

        CustomButtonSingleton<Amnesiac_Remember>.Instance.OverrideSprite(AUSAssets.Amnesiac_Remember.LoadAsset());
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        AUSPlugin.DebugLogMessage("Amnesiac OnMeetingStart called!");
        if (Player.HasDied())
            return;

        if (currentTarget != null && !currentTarget.HasModifier<RememberedModifier>()) Remember_TOS(currentTarget);
        else if (OptionGroupSingleton<Amnesiac_Options>.Instance.Mode == AmnesiacMode.TOS2)
        {
            AUSPlugin.DebugLogMessage("Potential Remember Targets: " + RememberablePlayers.Count);
            var rememberTarget = RememberablePlayers.Where(x => !x.HasModifier<RememberedModifier>())
                .OrderBy(x => x.IsTPow())
                .ThenBy(x => x.Is(Alignment.TownOutlier)).FirstOrDefault();
            if (rememberTarget != null)
            {
                var targetRole = rememberTarget.GetRoleWhenAlive();
                if (Player.AmOwner())
                {
                    var roleWhenAlive = rememberTarget.GetRoleWhenAlive();
                    Player.Notify(Info(NotificationType.Amnesiac_RememberIWasLike, roleWhenAlive.NiceName), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.AmnesiacRoleCard.LoadAsset());
                }

                // Prevents other Amnesiacs from remembering the same TPOW.
                if (rememberTarget.IsTPow() || rememberTarget.Is(Alignment.TownOutlier)) rememberTarget.RpcAddModifier<RememberedModifier>();

                Player.RpcChangeRole((ushort)targetRole.Role);
                if (Player.Data.Role is ICustomAURole customRole) customRole.Role_OnMeetingStart();
            }
        }
    }

    public void Remember_TOS(PlayerControl target)
    {
        var targetRole = currentTarget.GetRoleWhenAlive();
        var toBecome = (ushort)targetRole.Role;
        var role = targetRole.Role;

        var aliveGodfathers = PlayerControl.AllPlayerControls.ToArray().Count(x => x.IsRole<Godfather>() && !x.HasDied());
        var aliveMafia = PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Mafia) && !x.HasDied());
        if ((aliveMafia == 0 || aliveGodfathers > 0) && role is Godfather) toBecome = RoleId.Get<Mafioso>();

        RpcGlobalRemember(Player, toBecome.UshortToRole().NiceName);
        if (currentTarget.Is(Faction.Coven)) NecronomiconPatch.GrantCovenNecroPassing();

        Player.RpcChangeRole(toBecome);
        if (Player.Data.Role is ICustomAURole customRole) customRole.Role_OnMeetingStart();

        if (targetRole.IsUnique()) currentTarget.RpcAddModifier<RememberedModifier>();
    }

    public void OnTargetDeath(PlayerControl target)
    {
        var targetRole = target.GetRoleWhenAlive();
        if (targetRole is ICustomAURole customRole && customRole.Faction == Faction.Town && targetRole is not Amnesiac)
        {
            RememberablePlayers.Add(target);
        }
    }

    public static string Info(NotificationType type, string text)
    {
        if (type == NotificationType.Amnesiac_GlobalReveal) return $"An Amnesiac has Remembered that they were like the {text}.";
        if (type == NotificationType.Amnesiac_RememberingRoleReminder) return $"You have chosen to Remember you were like the {text}.";
        return $"You Remembered that you were like the {text}.";
    }

    public PlayerControl currentTarget;

    public List<PlayerControl> RememberablePlayers = new List<PlayerControl>();
}

public sealed class Amnesiac_Remember : TownOfUsRoleButton<Amnesiac>, IButtonClick
{
    public override string Name => "Remember";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Amnesiac(AmnesiacMode.TOS1);
    public override float Cooldown => 1f;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Amnesiac_Remember;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Player, false, false)) base.ClickHandler();
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && OptionGroupSingleton<Amnesiac_Options>.Instance.Mode == AmnesiacMode.TOS1;
    }

    public override bool CanUse()
    {
        var anyDead = PlayerControl.AllPlayerControls.ToArray().Any(x => x.HasDied());
        return base.CanUse() && anyDead;
    }

    protected override void OnClick() => Click(Player);
    public void Click(PlayerControl player, PlayerControl Target = null)
    {
        var player1Menu = CustomPlayerMenu.Create();
        player1Menu.transform.FindChild("PhoneUI").GetChild(0).GetComponent<SpriteRenderer>().material =
            PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
        player1Menu.transform.FindChild("PhoneUI").GetChild(1).GetComponent<SpriteRenderer>().material =
            PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;

        player1Menu.Begin(
            plr => (plr.HasDied()),
            plr =>
            {
                player1Menu.ForceClose();

                if (plr == null)
                {
                    return;
                }

                // Stuff here.
                Role.currentTarget = plr;

                var targetRole = Role.currentTarget.GetRoleWhenAlive();
                if (Player.AmOwner())
                {
                    var roleWhenAlive = Role.currentTarget.GetRoleWhenAlive();
                    Player.Notify(Amnesiac.Info(NotificationType.Amnesiac_RememberingRoleReminder, roleWhenAlive.NiceName), NotifyMode.Instantly, sprite: AUSAssets.AmnesiacRoleCard.LoadAsset());
                }
            }
        );
        foreach (var panel in player1Menu.potentialVictims)
        {
            panel.PlayerIcon.cosmetics.SetPhantomRoleAlpha(1f);
            if (panel.NameText.text != PlayerControl.LocalPlayer.Data.PlayerName)
            {
                panel.NameText.color = Color.white;
            }
        }
    }
}

public sealed class Amnesiac_Options : AbstractOptionGroup<Amnesiac>
{
    public override string GroupName => "Amnesiac";

    [ModdedEnumOption("Amnesiac Mode", typeof(AmnesiacMode), ["TOS", "TOS2"])]
    public AmnesiacMode Mode { get; set; } = AmnesiacMode.TOS2;
}