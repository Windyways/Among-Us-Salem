using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Mafioso(IntPtr cppPtr) : ImpostorRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Mafioso";
    public string revealText => "does the Godfather's dirty work.";
    public string RoleDescription => "Town Of Salem";
    public string RoleLongDescription => "You are a member of organized crime, trying to work your way to the top.";
    public Color RoleColor { get; set; } = RoleColors.Mafia;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public Faction Faction { get; set; } = Faction.Mafia;
    public Alignment Alignment => Alignment.MafiaKilling;

    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        Icon = AUSAssets.MafiosoRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that performs kills for the Godfather or on its own.\n" +
            "Kill anyone that will not submit to the Mafia." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Kill",
            "You can Kill a player at Night.\n" +
            "You will deal a Basic Attack to your target. You may not Kill if the Godfather has given you orders.",
            AUSAssets.Mafioso_Kill),
    ];

    public string GetAttributes()
    {
        return
            $"- If the Godfather dies, you will be promoted to Godfather.";
    }

    public static string Info()
    {
        return "You were promoted to a Mafioso!";
    }

    [MethodRpc((uint)AUSRpc.RpcResetGodfatherCooldown)]
    public static void RpcResetGodfatherCooldown(PlayerControl player)
    {
        if (player.AmOwner())
        {
            CustomButtonSingleton<Godfather_Kill>.Instance.ResetCooldownAndOrEffect();
            CustomButtonSingleton<Godfather_Order>.Instance.ResetCooldownAndOrEffect();
        }
    }
}

public sealed class Mafioso_Kill : TownOfUsRoleButton<Mafioso, PlayerControl>
{
    public override string Name => "Kill";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Mafioso_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Mafioso_Kill;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, true, true)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        if (Target == null)
            return;

        foreach (var godfather in MiscUtils.GetRoles<Godfather>()) Mafioso.RpcResetGodfatherCooldown(godfather.Player);
        if (Player.CanKill(Target))
        {
            Player.RpcCustomMurder(Target);
            VisitingMechanic.RpcAddDeathReason(Target, (int)DeathReasonShow.KilledByAMemberOfTheMafia);
        }
        else Player.Notify(Feedback.TooMuchDefense(Player, Target), NotifyMode.InstantlyAndMeeting);
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(false, Distance);
    }

    public override bool CanUse()
    {
        return base.CanUse() && !Player.HasModifier<OrderedModifier>();
    }
}

public sealed class Mafioso_Options : AbstractOptionGroup<Mafioso>
{
    public override string GroupName => "Mafioso";

    [ModdedNumberOption("Mafioso Kill Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}