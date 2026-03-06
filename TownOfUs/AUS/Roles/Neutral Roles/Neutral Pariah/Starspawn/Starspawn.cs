using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Collections;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Starspawn(IntPtr cppPtr) : CovenRole(cppPtr), ICustomAURole, IWikiDiscoverable, INotThreatable
{
    public string RoleName { get; set; } = RoleColors.StarspawnNameInGradient;
    public string revealText => "radiates cosmic energy.";
    public string RoleDescription => "Better Town Of Salem 2";
    public string RoleLongDescription => "You are a cosmic being who has the power to stop anything at will.";
    public Color RoleColor { get; set; } = RoleColors.Starspawn;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Neutral;
    public Alignment Alignment => Alignment.NeutralPariah;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.Powerful;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.Powerful;
    public CustomRoleConfiguration Configuration => new(this)
    {
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.StarspawnRoleCard
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
            $"Ethereal Defense: {EtherealDefense}\n" +
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can block Town roles from visiting a player and prevent players from using Day abilities, hindering the Town effectively. You will leave town once all Town roles perish.\n" +
            "Eliminate all Town members." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- You are RoleBlock immune.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Isolate",
            "You can Isolate a player at Night.\n" +
            "You will cause all visiting Town abilities to be blocked by an Unknown Obstacle. You will learn which roles visit your target.",
            AUSAssets.Starspawn_Isolate),

        new("Daybreak",
            "You can perform a Daybreak during the Day.\n" +
            "All Day abilities will be unusable until the next Day. All players are notified.",
            AUSAssets.Starspawn_Daybreak),
    ];

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return
            gameOverReason != GameOverReason.CrewmatesByVote &&
            gameOverReason != GameOverReason.CrewmateDisconnect &&
            gameOverReason != GameOverReason.CrewmatesByTask &&
            gameOverReason != GameOverReason.HideAndSeek_CrewmatesByTimer;
    }

    public static string Info(NotificationType type, string text)
    {
        if (type == NotificationType.Starspawn_Daybreak) return $"The {RoleColors.StarspawnNameInGradient} has called Daybreak, casting a shadow over the town. All day abilities are now temporarily suspended!";
        if (type == NotificationType.Starspawn_IsolateButImmune) return $"A {RoleColors.StarspawnNameInGradient} attempted to Starbound you, but you are immune!";
        return $"You have Starbound a {text}.";
    }

    [MethodRpc((uint)AUSRpc.RpcNotifyStarspawn)]
    public static void RpcNotify(PlayerControl player, int notifyType, PlayerControl visitor)
    {
        if (player.AmOwner())
        {
            var notify = (NotificationType)notifyType;
            switch (notify)
            {
                case NotificationType.Starspawn_Isolate:
                    var roleWhenAlive = visitor.GetRoleWhenAlive();
                    player.Notify(Info((NotificationType)notifyType, roleWhenAlive.NiceName), NotifyMode.OnlyMeeting, sprite: AUSAssets.StarspawnRoleCard.LoadAsset());
                    break;
                case NotificationType.Starspawn_IsolateButImmune:
                    player.Notify(Info((NotificationType)notifyType, string.Empty), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.StarspawnRoleCard.LoadAsset());
                    break;
                case NotificationType.Starspawn_Daybreak: // Here but unused lol.
                    player.Notify(Info((NotificationType)notifyType, string.Empty), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.StarspawnRoleCard.LoadAsset());
                    break;
            }
        }
    }

    [MethodRpc((uint)AUSRpc.RpcDaybreak)]
    public static void RpcDaybreak(PlayerControl player)
    {
        if (player.Data.Role is not Starspawn)
        {
            AUSPlugin.DebugLogMessage("RpcDaybreak - Invalid Starspawn", AUSPlugin.MsgType.Error);
            return;
        }

        ClearDayButtons(PlayerControl.LocalPlayer, false);
        PlayerControl.LocalPlayer.Notify(Info(NotificationType.Starspawn_Daybreak, string.Empty), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.StarspawnRoleCard.LoadAsset());
    }

    public static void ClearDayButtons(PlayerControl player, bool includeNecroPassing = true)
    {
        // --- NECRO PASSING ---
        if (player.TryGetModifier<NecroPassing>(out var necroPassing) && includeNecroPassing)
        {
            if (player.AmOwner)
            {
                necroPassing.meetingMenu.HideButtons();
            }
        }

        // --- DEPUTY ---
        if (player.Data.Role is Deputy deputy)
        {
            if (player.AmOwner)
            {
                deputy.meetingMenu.HideButtons();
            }
        }

        // --- MAYOR ---
        if (player.Data.Role is Mayor mayor)
        {
            if (player.AmOwner)
            {
                mayor.meetingMenu.HideButtons();
            }
        }

        // --- PROSECUTOR ---
        if (player.Data.Role is Prosecutor prosecutor)
        {
            if (player.AmOwner)
            {
                prosecutor.meetingMenu.HideButtons();
            }
        }

        // --- STARSPAWN ---
        if (player.Data.Role is Starspawn starspawn)
        {
            if (player.AmOwner)
            {
                starspawn.meetingMenu.HideButtons();
            }
        }
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (Player.AmOwner())
        {
            Player.RpcAddModifier<RoleBlockImmune>(true);
            meetingMenu = new MeetingMenu(
                this,
                ClickGuess,
                MeetingAbilityType.Click,
                AUSAssets.Starspawn_Daybreak,
                null!,
                IsExempt)
            {
                Position = new Vector3(-0.40f, 0f, -3f)
            };
        }
    }

    public void Role_OnMeetingStart()
    {
        if (Player.HasDied())
            return;

        SmartStarspawn.Start();
        if (Player.AmOwner)
        {
            Coroutines.Start(GenButtons());
        }
    }

    public IEnumerator GenButtons(float delay = 3f)
    {
        yield return new WaitForSeconds(delay);
        meetingMenu.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && Charges > 0 && DayNightMechanic.DayCount >= 2);
    }

    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);

        if (Player.AmOwner)
        {
            meetingMenu.HideButtons();
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
            meetingMenu = null!;
        }
    }

    public void ClickGuess(PlayerVoteArea voteArea, MeetingHud __)
    {
        Charges--;
        RpcDaybreak(Player);

        if (Player.AmOwner)
        {
            meetingMenu?.HideButtons();
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId != Player.PlayerId || Player.Data.IsDead || voteArea!.AmDead;
    }


    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not Starspawn || Player.HasDied() || !Player.AmOwner())
        {
            return;
        }

        var allTown = PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Town) && !x.HasDied());
        if (allTown == 0) Player.LeaveTown();
    }

    public MeetingMenu meetingMenu;
    public int Charges = (int)OptionGroupSingleton<Starspawn_Options>.Instance.Charges;
}

public sealed class Starspawn_Isolate : TownOfUsRoleButton<Starspawn, PlayerControl>
{
    public override string Name => "Isolate";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Starspawn;
    public override float Cooldown => OptionGroupSingleton<Starspawn_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Starspawn_Isolate;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, false, true)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        if (Target == null)
            return;

        Target.RpcAddModifier<IsolatedModifier>(Player);
        CustomButtonSingleton<Starspawn_SelfIsolate>.Instance.ResetCooldownAndOrEffect();
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !x.HasModifier<IsolatedModifier>(x => x.Caster == Player));
    }
}

public sealed class Starspawn_SelfIsolate : TownOfUsRoleButton<Starspawn>
{
    public override string Name => "Self Isolate";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => RoleColors.Starspawn;
    public override float Cooldown => OptionGroupSingleton<Starspawn_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Starspawn_Isolate;
    public override ButtonLocation Location => ButtonLocation.BottomLeft;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Player, false, false)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        Player.RpcAddModifier<IsolatedModifier>(Player);
        CustomButtonSingleton<Starspawn_Isolate>.Instance.ResetCooldownAndOrEffect();
    }

    public override bool CanUse()
    {
        if (Player.HasModifier<IsolatedModifier>(x => x.Caster == Player)) return false;
        return base.CanUse();
    }
}

public sealed class Starspawn_Options : AbstractOptionGroup<Starspawn>
{
    public override string GroupName => "Starspawn";

    [ModdedNumberOption("Starspawn Isolate Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("Starspawn Max Daybreaks", 0f, 30f, 1f, MiraNumberSuffixes.None, zeroInfinity: true)]
    public float Charges { get; set; } = 1;
}