using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Survivor(IntPtr cppPtr) : CovenRole(cppPtr), ICustomAURole, IWikiDiscoverable, INotThreatable
{
    public string RoleName { get; set; } = "Survivor";
    public string revealText => "simply wants to live.";
    public string RoleDescription => "Town Of Salem";
    public string RoleLongDescription => "You are a neutral character who just wants to live.";
    public Color RoleColor { get; set; } = RoleColors.Survivor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Neutral;
    public Alignment Alignment => Alignment.NeutralBenign;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.SurvivorRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can protect itself by granting itself defense, winning if it gets to the end of the game alive.\n" +
            "Live until the end of the game." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- You will know if you were attacked.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Vest",
            "You can Vest yourself at Night.\n" +
            "You will grant yourself Basic Defense.",
            AUSAssets.Survivor_Vest),
    ];

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return !Player.HasDied();
    }

    public static string Info()
    {
        return "You were attacked, but your bulletproof Vest saved you!";
    }

    [MethodRpc((uint)AUSRpc.RpcNotifySurvivor)]
    public static void RpcNotify(PlayerControl player, int notifyType)
    {
        if (player.AmOwner())
        {
            var notify = (NotificationType)notifyType;
            switch (notify)
            {
                case NotificationType.Survivor_AttackedAndVested:
                    player.Notify(Info(), NotifyMode.OnlyMeeting, sprite: AUSAssets.SurvivorRoleCard.LoadAsset());
                    break;
            }
        }
    }
}

public sealed class Survivor_Vest : TownOfUsRoleButton<Survivor>
{
    public override string Name => "Vest";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Survivor;
    public override float Cooldown => OptionGroupSingleton<Survivor_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Survivor_Vest;
    public override int MaxUses => (int)OptionGroupSingleton<Survivor_Options>.Instance.Charges;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Player, false, false)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        Player.RpcAddModifier<VestedModifier>(Player);
        //Player.RpcAddModifier<OverrideDefense>((int)Defense.Basic);
        AttackDefenseMechanic.RpcApplyDefense(Player, Defense.Basic, visualize: true);
    }
}

public sealed class Survivor_Options : AbstractOptionGroup<Survivor>
{
    public override string GroupName => "Survivor";

    [ModdedNumberOption("Survivor Vest Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("Survivor Max Vests", 0f, 30f, 1f, MiraNumberSuffixes.None, zeroInfinity: true)]
    public float Charges { get; set; } = 4;
}