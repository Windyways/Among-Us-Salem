using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Framer
#endregion
public sealed class Framer(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IWikiDiscoverable, IAUSRole
{
    public string RoleName => TouLocale.Get(TouNames.Framer, "Framer");
    public string revealText => "has a desire or deceive.";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public Faction RoleFaction => Faction.Mafia;
    public Color RoleColor => AUSColors.Mafia;
    public Alignment Alignment => Alignment.MafiaDeception;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        Icon = AUSAssets.FramerRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#dd0000>Framer</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color> <color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#dd0000>Mafia</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#dd0000>Mafia</color> <color=#1e45d4>Deception</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill anyone that will not submit to the Mafia." +
            $"\n\nAttributes:" +
            "\nTBD." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Frame",
            "You can Frame a player during the round. You will kill your target.",
            AUSAssets.Framer_Frame)
    ];

    [MethodRpc((uint)AUSRpc.Framer_Frame, SendImmediately = true)]
    public static void RpcFramer_Frame(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Framer)
        {
            Logger<AUSPlugin>.Error("RpcFramer_Frame - Invalid Framer");
            return;
        }

        var framer = player.GetRole<Framer>();
        framer.FramedPlayers.Add(target.PlayerId);
    }

    [MethodRpc((uint)AUSRpc.Framer_RemoveFrame, SendImmediately = true)]
    public static bool RpcFramer_RemoveFrame(PlayerControl target)
    {
        foreach (var framers in MiscUtils.GetPlayersWithRole<Framer>())
        {
            var framer = framers.GetRole<Framer>();
            framer.FramedPlayers.Remove(target.PlayerId);
        }

        return true;
    }

    public List<byte> FramedPlayers = new List<byte>();
}

#region Framer_Frame
#endregion
public sealed class Framer_Frame : AmongUsSalemRoleButton<Framer, PlayerControl>
{
    public override string Name => "Frame";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Framer_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Framer_Frame;

    public override void ClickHandler()
    {
        if (Target != null)
        {
            if (MiscUtils.SuccessfulVisit(Role.Player, Target, false, true))
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

        Framer.RpcFramer_Frame(Role.Player, Target);
        MiscUtils.PostSuccessfulVisit(Role.Player, Target, false, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(false, Distance);
    }
}

#region Framer_Options
#endregion
public sealed class Framer_Options : AbstractOptionGroup<Framer>
{
    public override string GroupName => TouLocale.Get(TouNames.Framer, "Framer");

    [ModdedNumberOption("<color=#dd0000>Framer</color> <color=#4a86e8>Frame</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}