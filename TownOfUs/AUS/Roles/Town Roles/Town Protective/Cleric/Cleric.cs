using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.Roles;

public sealed class Cleric(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Cleric";
    public string revealText => "is skilled in protective magic.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are a healer providing protection to the town.";
    public Color RoleColor { get; set; } = RoleColors.Town;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Alignment Alignment => Alignment.TownProtective;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.ClericRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can protect valuable Town roles, or just Town members in general and keep them alive for longer.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- You know if your target is attacked.\n" +
            $"- Your target knows they were attacked.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Barrier",
            "You can Barrier a player at Night.\n" +
            "You will grant your target Powerful Defense and clear off Poison.",
            AUSAssets.Cleric_Barrier),

        new("Self Barrier",
            "You can Self Barrier yourself at Night.\n" +
            "You will grant yourself Powerful Defense and clear off Poison.",
            AUSAssets.Cleric_SelfBarrier),
    ];

    public int Charges = (int)OptionGroupSingleton<Cleric_Options>.Instance.Charges;
    public void Function(PlayerControl target, int Button)
    {
        if (Button is 1)
        {
            target.RpcAddModifier<BarrieredModifier>(Player);
            AttackDefenseMechanic.RpcApplyDefense(target, Defense.Powerful);
        }
        else if (Button is 2)
        {
            Charges--;
            Player.RpcAddModifier<BarrieredModifier>(Player);
            AttackDefenseMechanic.RpcApplyDefense(Player, Defense.Powerful, visualize: true);
        }
    }
}

public sealed class Cleric_Barrier : TownOfUsRoleButton<Cleric, PlayerControl>
{
    public override string Name => "Barrier";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => OptionGroupSingleton<Cleric_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Cleric_Barrier;

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Target, 1, false, true);
    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x => 
            !x.HasModifier<BarrieredModifier>(x => x.Caster == Player));
    }
}

public sealed class Cleric_SelfBarrier : TownOfUsRoleButton<Cleric>
{
    public override string Name => "Self Barrier";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => OptionGroupSingleton<Cleric_Options>.Instance.SelfBarrierCD;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Cleric_SelfBarrier;

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
        if (playerControl == Player)
        {
            Button?.usesRemainingText.gameObject.SetActive(true);
            Button?.usesRemainingSprite.gameObject.SetActive(true);

            Button!.usesRemainingText.text = Role.Charges.ToString() + $"";
        }

        base.FixedUpdate(playerControl);
    }

    public override bool CanUse()
    {
        return base.CanUse() && Role.Charges > 0;
    }

    protected override void OnClick() => VisitingMechanic.CheckVisit(Player, Player, 2, false, false);
}

public sealed class Cleric_Options : AbstractOptionGroup<Cleric>
{
    public override string GroupName => "Cleric";

    [ModdedNumberOption("Cleric Barrier Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("Cleric Self Barrier Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float SelfBarrierCD { get; set; } = 25f;

    [ModdedNumberOption("Cleric Max Self Barriers", 0f, 30f, 1f, MiraNumberSuffixes.None, zeroInfinity: true)]
    public float Charges { get; set; } = 1;
}