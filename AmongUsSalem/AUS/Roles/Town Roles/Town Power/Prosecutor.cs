using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using Color = UnityEngine.Color;
using UnityEngine;
using System.Collections;

namespace AmongUsSalem.Roles;

#region Prosecutor
#endregion
public sealed class Prosecutor(IntPtr cppPtr) 
    : CrewmateRole(cppPtr), IAUSRole, IRevealable, IWikiDiscoverable, IContinueGame
{
    public bool continueGame => true;
    public string RoleName { get; set; } = "Prosecutor";
    public string revealText => "???";
    public string RoleDescription => "";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Color RoleColor { get; set; } = AUSColors.Town;
    public Alignment Alignment => Alignment.TownPower;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public bool IsRevealed { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.ProsecutorRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#06E00C>Prosecutor</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#06E00C>Town</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#06E00C>Town</color> <color=#1e45d4>Power</color>" +
            "\n<color=#fdbc00>Goal:</color> Hang every criminal and evildoer." +
            $"\n\nAttributes:" +
            "\nN/A" +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Reveal",
            "N/A.",
            AUSAssets.Prosecutor_Prosecute)
    ];


    [MethodRpc((uint)AUSRpc.Prosecutor_Prosecute, SendImmediately = true)]
    public static void RpcProsecutor_Prosecute(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Prosecutor)
        {
            Logger<AUSPlugin>.Error("RpcProsecutor_Prosecute - Invalid Prosecutor");
            return;
        }

        var prosecutor = player.GetRole<Prosecutor>();
        prosecutor.IsRevealed = true;
        
        Coroutines.Start(ProsecuteCoroutine(MeetingHud.Instance, target));
    }

    public static IEnumerator ProsecuteCoroutine(MeetingHud __instance, PlayerControl VotedPlayer)
    {
        ConsoleJoystick.SetMode_Task();

        yield return DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.clear, Color.black, 1f, false);

        ExileController exileController = UnityEngine.Object.Instantiate<ExileController>(ShipStatus.Instance.ExileCutscenePrefab);
        exileController.transform.SetParent(DestroyableSingleton<HudManager>.Instance.transform, false);
        exileController.transform.localPosition = new Vector3(0f, 0f, -60f);
        MeetingHud.Instance.gameObject.SetActive(false);
        exileController.BeginForGameplay(VotedPlayer.Data, false);

        yield return new WaitForSeconds(5);

        __instance.DespawnOnDestroy = false;
        if (MapBehaviour.Instance) MapBehaviour.Instance.Close();
        MeetingHud.Instance.gameObject.SetActive(true);
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

        RpcProsecutor_Prosecute(Player, target);
        Charges--;

        if (Player.AmOwner)
        {
            meetingMenu?.HideButtons();
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId == Player.PlayerId || Player.Data.IsDead || voteArea!.AmDead;
    }

    public MeetingMenu meetingMenu;
    public int Charges = (int)OptionGroupSingleton<Prosecutor_Options>.Instance.Charges;
}

#region Prosecutor_Options
#endregion
public sealed class Prosecutor_Options : AbstractOptionGroup<Prosecutor>
{
    public override string GroupName => "Prosecutor";

    [ModdedNumberOption("<color=#06E00C>Prosecutor</color> Max <color=#4a86e8>Prosecutions</color>", 1f, 30f, 1f)]
    public float Charges { get; set; } = 2f;
}