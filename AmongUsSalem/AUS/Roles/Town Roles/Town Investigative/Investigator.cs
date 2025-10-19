using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using Color = UnityEngine.Color;
using UnityEngine;

namespace AmongUsSalem.Roles;

#region Investigator
#endregion
public sealed class Investigator(IntPtr cppPtr)
    : CrewmateRole(cppPtr), IWikiDiscoverable, ICustomAURole
{
    public string RoleName { get; set; } = "Investigator";
    public string revealText => "gathers information about people.";
    public string RoleDescription => "Look for Murder & Trespassing.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Color RoleColor { get; set; } = AUSColors.Town;
    public Alignment Alignment => Alignment.TownInvestigative;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.InvestigatorRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#06E00C>Investigator</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#06E00C>Town</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#06E00C>Town</color> <color=#1e45d4>Investigative</color>" +
            "\n<color=#fdbc00>Goal:</color> Hang every criminal and evildoer." +
            $"\n\nAttributes:" +
            "\n<color=#B545FF>Enchanters</color> can make their targets look suspicious." +
            "\n<color=#DD0000>Framers</color> can make their targets look suspicious." +
            "\n<color=#B545FF>Illusionists</color> can make their targets look not suspicious." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Investigate",
            "You can Investigate a player at Night." +
            "\nYour target will appear to be Trespassing if they visited an opposing faction at any point during the game." +
            "\nYour target will appear to have Murder if they killed someone the same Night.",
            AUSAssets.Investigator_Investigate)
    ];

    public static bool IsTrespassing(PlayerControl target)
    {
        if (target.AppearsEvil()) return true;
        else if (target.IsIllusioned()) return false;
        else if (Statistics.IsTrespassing.Contains(target)) return true;
        return false;
    }

    public static bool HasMurder(PlayerControl target)
    {
        if (target.AppearsEvil()) return true;
        else if (target.IsIllusioned()) return false;
        else if (Statistics.HasMurder.Contains(target)) return true;
        return false;
    }

    public enum Type { Trespassing, Murder, NoCrime }
    public static string Info(PlayerControl target, Type type)
    {
        if (type == Type.Trespassing && (IsTrespassing(target)) || target.IsFramed())
        {
            return "You discovered evidence that " + target.GetDefaultAppearance().PlayerName + " is <b><color=#4a86e8>Trespassing</color></b>!";
        }
        else if (type == Type.Murder && (HasMurder(target)) || target.IsFramed())
        {
            return "You found evidence of <b><color=#4a86e8>Murder</color></b> in " + target.GetDefaultAppearance().PlayerName + "'s house!";
        }
        else if (type == Type.NoCrime && !IsTrespassing(target) && !HasMurder(target) && !target.IsFramed())
        {
            return "You didn't find any evidence of a <b><color=#4a86e8>Crime</color></b> in " + target.GetDefaultAppearance().PlayerName + "'s house.";
        }

        return "";
    }
    
	public List<byte> TrespassingPlayers = new List<byte>();
	public List<byte> MurderPlayers = new List<byte>();
	public List<byte> InvestigatedPlayers = new List<byte>();
}

#region Investigator_Investigate
#endregion
public sealed class Investigator_Investigate : AmongUsSalemRoleButton<Investigator, PlayerControl>
{
    public override string Name => "Investigate";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Town;
    public override float Cooldown => OptionGroupSingleton<Investigator_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Investigator_Investigate;

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

        Role.InvestigatedPlayers.Add(Target.PlayerId);
        if (Investigator.IsTrespassing(Target))
        {
            Role.TrespassingPlayers.Add(Target.PlayerId);
            if (Debugger.IsDebuggerActive) if (!CalculatedVoting.QueueKillerContagious.ContainsValue(Target)) CalculatedVoting.QueueKillerContagious.Add(Player, Target);
            MiscUtils.ShowNotification(Investigator.Info(Target, Investigator.Type.Trespassing), Color.white, AUSAssets.InvestigatorRoleCard.LoadAsset());
            MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Investigator Info"), Investigator.Info(Target, Investigator.Type.Trespassing));
        }
        if (Investigator.HasMurder(Target))
        {
            Role.MurderPlayers.Add(Target.PlayerId);
            if (Debugger.IsDebuggerActive) if (!CalculatedVoting.QueueEvidenceAgainst.ContainsValue(Target)) CalculatedVoting.QueueEvidenceAgainst.Add(Player, Target);
            MiscUtils.ShowNotification(Investigator.Info(Target, Investigator.Type.Murder), Color.white, AUSAssets.InvestigatorRoleCard.LoadAsset());
            MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Investigator Info"), Investigator.Info(Target, Investigator.Type.Murder));
        }
        if (!Investigator.HasMurder(Target) && !Investigator.IsTrespassing(Target))
        {
            MiscUtils.ShowNotification(Investigator.Info(Target, Investigator.Type.NoCrime), Color.white, AUSAssets.InvestigatorRoleCard.LoadAsset());
            MiscUtils.AddFakeChat(Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Investigator Info"), Investigator.Info(Target, Investigator.Type.NoCrime));
        }
        MiscUtils.PostSuccessfulVisit(Player, Target, false, true);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }
}

#region Investigator_Options
#endregion
public sealed class Investigator_Options : AbstractOptionGroup<Investigator>
{
    public override string GroupName => "Investigator";

    [ModdedNumberOption("<color=#06E00C>Investigator</color> <color=#4a86e8>Investigate</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}