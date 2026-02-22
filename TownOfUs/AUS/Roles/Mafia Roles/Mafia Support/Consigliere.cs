using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Consigliere(IntPtr cppPtr) : ImpostorRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Consigliere";
    public string revealText => "gathers information for the Mafia.";
    public string RoleDescription => "";
    public string RoleLongDescription => "";
    public Color RoleColor { get; set; } = RoleColors.Mafia;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public Faction Faction { get; set; } = Faction.Mafia;
    public Alignment Alignment => Alignment.MafiaSupport;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        Icon = AUSAssets.ConsigliereRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can help the Mafia learn the identity of dangerous Town roles or to help avoid other evils.\n" +
            "Kill anyone that will not submit to the Mafia." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- You have access to Mafia chat.\n" +
            $"- If there are no capable Mafia Killing roles, you will be promoted to Mafioso.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Size Up",
            "You can Size Up a player at Night.\n" +
            "You and your Mafia members will know your target's role.\n" +
            (OptionGroupSingleton<Consigliere_Options>.Instance.DousedAsArsonist ? "Doused players appear to be an Arsonist.\n" : string.Empty) +
            "Hexed players appear to be a Hex Master.",
            AUSAssets.Consigliere_SizeUp),
    ];
}

public sealed class Consigliere_SizeUp : TownOfUsRoleButton<Consigliere, PlayerControl>
{
    public override string Name => "Size Up";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Consigliere_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Consigliere_SizeUp;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, false, true)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        if (Target == null)
            return;

        if (Target.HasModifier<HexedModifier>())
        {
            // Since all Mafia member see revealed players, we have to make sure they get Deepfaked too (Includes this player too).
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Mafia) && player.AmOwner()) 
                    Target.RpcAddModifier<DeepfakeRole>(player, "Hex Master", RoleColors.Coven);
            }
        }
        Target.RpcAddModifier<RoleLearn>(Player, true);
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(false, Distance, predicate: x =>
            !x.HasModifier<RoleLearn>(x => x.Visitor == Player));
    }
}

public sealed class Consigliere_Options : AbstractOptionGroup<Consigliere>
{
    public override string GroupName => "Consigliere";

    [ModdedNumberOption("Consigliere Size Up Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedToggleOption("Doused Players Show As Arsonist")]
    public bool DousedAsArsonist { get; set; } = true;
}