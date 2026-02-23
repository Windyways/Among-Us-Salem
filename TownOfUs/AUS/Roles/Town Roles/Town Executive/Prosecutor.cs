using Il2CppInterop.Runtime.Attributes;
using System.Collections;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Prosecutor(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Prosecutor";
    public string revealText => "will stop at nothing to see justice served.";
    public string RoleDescription => "";
    public string RoleLongDescription => "You are a powerful member of the court who ensures that justice prevails.";
    public Color RoleColor { get; set; } = RoleColors.Town;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Alignment Alignment => Alignment.TownExecutive;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.ProsecutorRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can instantly lynch players of their choice, which is very threatening to evils since being in the majority wouldn't matter.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- You cannot Prosecute Day 1.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Prosecute",
            "You can Prosecute a player during the Day.\n" +
            "Your role will be revealed to all and you will immediately lynch your target. If your target was a Town member, you will lose all remaining charges of Prosecute. This does not end the Day.",
            AUSAssets.Prosecutor_Prosecute),
    ];

    [MethodRpc((uint)AUSRpc.Prosecutor_Prosecute)]
    public static void RpcProsecutor_Prosecute(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Prosecutor)
        {
            Logger<AUSPlugin>.Error("RpcProsecutor_Prosecute - Invalid Prosecutor");
            return;
        }

        var prosecutor = player.GetRole<Prosecutor>();
        prosecutor.Player.AddModifier<GlobalReveal>();

        Coroutines.Start(ProsecuteCoroutine(MeetingHud.Instance, target, prosecutor));

        player.RemoveModifier<IncriminatingEvidence>();
        player.RemoveModifier<SeenKill>();
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (Player.AmOwner())
        {
            meetingMenu = new MeetingMenu(
                this,
                ClickGuess,
                MeetingAbilityType.Click,
                AUSAssets.Prosecutor_Prosecute,
                null!,
                IsExempt)
            {
                Position = new Vector3(-0.40f, 0f, -3f)
            };
        }
    }

    public override void OnMeetingStart()
    {
        AUSPlugin.DebugLogMessage("Prosecutor OnMeetingStart called!");
        SmartProsecutor.Start();

        if (Player.AmOwner)
        {
            Coroutines.Start(GenButtons());
        }
    }

    public IEnumerator GenButtons()
    {
        yield return new WaitForSeconds(3f);
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
        var target = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId).Object;

        Charges--;
        RpcProsecutor_Prosecute(Player, target);

        if (Player.AmOwner)
        {
            meetingMenu?.HideButtons();
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId == Player.PlayerId || Player.Data.IsDead || voteArea!.AmDead;
    }

    public static IEnumerator ProsecuteCoroutine(MeetingHud __instance, PlayerControl VotedPlayer, Prosecutor prosecutor)
    {
        SmartProsecutor.IsActive = true;
        ConsoleJoystick.SetMode_Task();

        yield return DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.clear, Color.black, 1f, false);

        ExileController exileController = UnityEngine.Object.Instantiate<ExileController>(ShipStatus.Instance.ExileCutscenePrefab);
        exileController.transform.SetParent(DestroyableSingleton<HudManager>.Instance.transform, false);
        exileController.transform.localPosition = new Vector3(0f, 0f, -60f);
        MeetingHud.Instance.gameObject.SetActive(false);
        HudManager.Instance.Chat.gameObject.SetActive(false);
        exileController.BeginForGameplay(VotedPlayer.Data, false);

        yield return new WaitForSeconds(5f);

        __instance.DespawnOnDestroy = false;
        if (MapBehaviour.Instance) MapBehaviour.Instance.Close();
        MeetingHud.Instance.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.5f);
        HudManager.Instance.Chat.gameObject.SetActive(true);

        if (VotedPlayer.Is(Faction.Town)) prosecutor.Charges = 0;

        SmartProsecutor.IsActive = false;
    }

    public MeetingMenu meetingMenu;
    public int Charges = (int)OptionGroupSingleton<Prosecutor_Options>.Instance.Charges;
}

public sealed class Prosecutor_Options : AbstractOptionGroup<Prosecutor>
{
    public override string GroupName => "Prosecutor";

    [ModdedNumberOption("Prosecutor Max Prosecutes", 0f, 30f, 1f, MiraNumberSuffixes.None, zeroInfinity: true)]
    public float Charges { get; set; } = 2;
}