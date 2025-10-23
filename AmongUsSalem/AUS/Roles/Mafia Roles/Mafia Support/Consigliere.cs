using System.Text;
using AmongUsSalem.Utilities;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using MiraAPI.Voting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace AmongUsSalem.LifeImprovement.Roles;

#region Consigliere
#endregion
public sealed class Consigliere(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IWikiDiscoverable, ICustomAURole
{
    public string RoleName { get; set; } = "Consigliere";
    public string revealText => "gathers information for the Mafia.";
    public string RoleDescription => "Reveal the roles of players.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public Faction Faction { get; set; } = Faction.Mafia;
    public Color RoleColor { get; set; } = AUSColors.Mafia;
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
        Icon = AUSAssets.ConsigliereRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#DD0000>Consigliere</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#DD0000>Mafia</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#DD0000>Mafia</color> <color=#1e45d4>Deception</color>" +
            "\n<color=#fdbc00>Goal:</color> Kill anyone that will not submit to the Mafia." +
            $"\n\nAttributes:" +
            "\nIf all Mafia Killing roles are dead, you will be promoted to Mafioso." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Check",
            "You can Check a player at Night. You will learn your targets role." +
            "\nIf your target is Doused, they will appear to be an Arsonist." +
            "\nIf your target is Hexed, they will appear to be a Hex Master.",
            AUSAssets.Consigliere_Check)
    ];
}

#region Consigliere_Check
#endregion
public sealed class Consigliere_Check : AmongUsSalemRoleButton<Consigliere, PlayerControl>
{
    public override string Name => "Check";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Consigliere_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Consigliere_Check;

    public override void ClickHandler()
    {
        if (Target != null && Timer <= 0)
        {
            if (MiscUtils.SuccessfulVisit(Player, Target, false, true))
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

        if (Player.AmOwner())
        {
            Target.AddModifier<RoleLearn>(Player, true);
            if (Target.IsDoused() && OptionGroupSingleton<Consigliere_Options>.Instance.ShowArso)
            {
                Target.AddModifier<DeepfakeRole>("Arsonist", AUSColors.Arsonist);
            }
            else if (Target.IsHexed()) Target.AddModifier<DeepfakeRole>("Hex Master", AUSColors.Coven);
        }

        MiscUtils.PostSuccessfulVisit(Player, Target, false, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(false, Distance);
    }
}

#region Consigliere_Options
#endregion
public sealed class Consigliere_Options : AbstractOptionGroup<Consigliere>
{
    public override string GroupName => "Consigliere";

    [ModdedNumberOption("<color=#DD0000>Consigliere</color> <color=#4a86e8>Check</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedToggleOption("Doused Players Appear To Be An <color=#ee7600>Arsonist</color>")]
    public bool ShowArso { get; set; } = true;
}