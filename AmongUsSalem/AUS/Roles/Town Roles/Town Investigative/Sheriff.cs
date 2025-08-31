using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using Color = UnityEngine.Color;
using UnityEngine;

namespace AmongUsSalem.Roles;

#region Sheriff
#endregion
public sealed class Sheriff(IntPtr cppPtr)
    : CrewmateRole(cppPtr), IAUSRole, IWikiDiscoverable
{
    public string RoleName => TouLocale.Get(TouNames.Sheriff, "Sheriff");
    public string revealText => "is a protector of the town.";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction RoleFaction => Faction.Town;
    public Color RoleColor => AUSColors.Town;
    public Alignment Alignment => Alignment.TownInvestigative;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;
    public Attack ogAttack => Attack;
    public Defense ogDefense => Defense;
    public EtherealDefense ogEtherealDefense => EtherealDefense;
    
    public DeathReasonShow deathReasonShow { get; set; } = DeathReasonShow.Alive;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.SheriffRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#06e00c>Sheriff</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#06e00c>Town</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#06e00c>Town</color> <color=#1e45d4>Investigative</color>" +
            "\n<color=#fdbc00>Goal:</color> Hang every criminal and evildoer." +
            $"\n\nAttributes:" +
            "\n<color=#ab42ef>Enchanters</color> can make their targets look suspicious." +
            "\n<color=#dd0000>Framers</color> can make their targets look suspicious." +
            "\n<color=#ab42ef>Illusionists</color> can make their targets look not suspicious." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Search",
            "Search one person each night for suspicious activity." +
            "\nYou will find Coven members without the Necronomicon, Executioners, Pirates, and Doomsayers as suspicious." +
            "\nThe Necronomicon protects its holder from Search, making them appear not suspicious." +
            "\nSerial Killers, Arsonists, Werewolves, Shrouds and Jesters will not appear suspicious." +
            "\nPlaguebearer, Baker, and Soul Collector will appear suspicious if the Four Horsemen modifier is enabled, otherwise they will not appear suspicious.",
            AUSAssets.Sheriff_Search)
    ];

    public override void OnMeetingStart()
    {
        SearchedPlayers.Clear();
    }

    public static bool IsSuspicious(PlayerControl target)
    {
        if (target.Data.Role is IAUSRole role)
        {
            if (target.AppearsEvil()) return true;
            else if (target.IsIllusioned() || role.Necronomicon || target.IsTownTraitor()/* || target.IsRole<Godfather>()*/) return false;
            else if (target.Is(Faction.Coven) || target.Is(Faction.Mafia) || target.Is(Alignment.NeutralEvil) || target.Is(Alignment.NeutralPariah)) return true;
            /*
            else if (target.Is(RoleEnum.BTOS2Vampire) && !Role.GetRole<BTOS2Vampire>(target).Solo) return true;
            else if (target.Is(RoleEnum.SerialKiller) && SerialKiller.Version == SerialKiller.V.TownOfSalem) return true;
            else if (target.Is(RoleEnum.Werewolf) && DayNightMechanic.FullMoon()) return true;
            */
        }
        return false;
    }

    public static string Info(PlayerControl target)
    {
        if (IsSuspicious(target))
        {
            return target.GetDefaultAppearance().PlayerName + " is <b><color=#ff0000>Suspicious</color></b> or <b><color=#ff0000>Framed</color></b>!";
        }

        return target.GetDefaultAppearance().PlayerName + " is innocent or great at hiding secrets!";
    }
    
	public List<byte> SuspiciousPlayers = new List<byte>();
	public List<byte> SearchedPlayers = new List<byte>();
	public List<byte> PastSearchedPlayers = new List<byte>();

    public enum Version
    {
        TraitorsInSalem,
        TownOfSalem2
    }
}

#region Sheriff_Search
#endregion
public sealed class Sheriff_Search : AmongUsSalemRoleButton<Sheriff, PlayerControl>
{
    public override string Name => "Search";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Town;
    public override float Cooldown => OptionGroupSingleton<Sheriff_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Sheriff_Search;

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

        var stats = Role.Player.GetPlayerStats();
        if (stats != null)
        {
            stats.ReceivedInformation = true;

            Role.SearchedPlayers.Add(Target.PlayerId);
            Role.PastSearchedPlayers.Add(Target.PlayerId);

            if (Sheriff.IsSuspicious(Target))
            {
                Role.SuspiciousPlayers.Add(Target.PlayerId);
                stats.EvidenceAgainst.Add(Target);
            }

            MiscUtils.ShowNotification(Sheriff.Info(Target), Color.white, AUSAssets.SheriffRoleCard.LoadAsset());
            MiscUtils.AddFakeChat(Role.Player.CachedPlayerData, "Sheriff Info", Sheriff.Info(Target));
        }
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }
}

#region Sheriff_Options
#endregion
public sealed class Sheriff_Options : AbstractOptionGroup<Sheriff>
{
    public override string GroupName => TouLocale.Get(TouNames.Sheriff, "Sheriff");

    [ModdedNumberOption("<color=#06e00c>Sheriff</color> <color=#4a86e8>Search</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}