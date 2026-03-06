using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Bodyguard(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Bodyguard";
    public string revealText => "is a trained protector.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are a former knight dedicated to protecting the town.";
    public Color RoleColor { get; set; } = RoleColors.Town;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Alignment Alignment => Alignment.TownProtective;

    public Attack Attack { get; set; } = Attack.Powerful;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Powerful;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.BodyguardRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can protect potential targets or keep valuable Town roles alive.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- You will deal a Powerful Attack yourself if you Guard while Shrouded.\n" +
            $"- Your target knows they were attacked.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Guard",
            "You can Guard a player at Night.\n" +
            "If your target is attacked, you will deal Powerful Attack to their attacker.\n" +
            "You will not counterattack passive attacks.",
            AUSAssets.Bodyguard_Guard),

        new("Self Protect",
            "You can Self Protect yourself at Night.\n" +
            "You will grant yourself Basic Defense.",
            AUSAssets.Bodyguard_SelfProtect),
    ];

    public static string Info(NotificationType type)
    {
        if (type is NotificationType.Bodyguard_Protect) return "Someone attacked you, but a Bodyguard fought off your attacker!";//return "Someone attacked you, but a Bodyguard protected you!";
        return "Someone attacked you, but your armor protected you!";
    }

    [MethodRpc((uint)AUSRpc.RpcNotifyBodyguard)]
    public static void RpcNotify(PlayerControl player, int notifyType)
    {
        if (player.AmOwner())
        {
            var notify = (NotificationType)notifyType;
            switch (notify)
            {
                case NotificationType.Bodyguard_Protect:
                    player.Notify(Info(notify), NotifyMode.OnlyMeeting, sprite: AUSAssets.BodyguardRoleCard.LoadAsset());
                    break;
                case NotificationType.Bodyguard_SelfProtect:
                    player.Notify(Info(notify), NotifyMode.OnlyMeeting, sprite: AUSAssets.BodyguardRoleCard.LoadAsset());
                    break;
            }
        }
    }
}

public sealed class Bodyguard_Guard : TownOfUsRoleButton<Bodyguard, PlayerControl>
{
    public override string Name => "Guard";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => OptionGroupSingleton<Bodyguard_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Bodyguard_Guard;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, false, true)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        if (Target == null)
            return;

        Target.RpcAddModifier<GuardedModifier>(Player);
        CustomButtonSingleton<Bodyguard_SelfProtect>.Instance.ResetCooldownAndOrEffect();
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x => 
            !x.HasModifier<GuardedModifier>(x => x.Caster == Player));
    }
}

public sealed class Bodyguard_SelfProtect : TownOfUsRoleButton<Bodyguard>
{
    public override string Name => "Self Protect";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => OptionGroupSingleton<Bodyguard_Options>.Instance.SelfProtectCD;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Bodyguard_SelfProtect;
    public override int MaxUses => (int)OptionGroupSingleton<Bodyguard_Options>.Instance.Charges;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Player, false, false)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        Player.RpcAddModifier<SelfProtectedModifier>(Player);
        //Player.RpcAddModifier<OverrideDefense>((int)Defense.Basic);
        AttackDefenseMechanic.RpcApplyDefense(Player, Defense.Basic, visualize: true);
        CustomButtonSingleton<Bodyguard_Guard>.Instance.ResetCooldownAndOrEffect();
    }
}

public sealed class Bodyguard_Options : AbstractOptionGroup<Bodyguard>
{
    public override string GroupName => "Bodyguard";

    [ModdedNumberOption("Bodyguard Guard Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("Bodyguard Self Protect Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float SelfProtectCD { get; set; } = 25f;

    [ModdedNumberOption("Bodyguard Max Self Protects", 0f, 30f, 1f, MiraNumberSuffixes.None, zeroInfinity: true)]
    public float Charges { get; set; } = 2;
}