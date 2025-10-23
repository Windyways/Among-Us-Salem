using System.Text;
using AmongUsSalem.LifeImprovement.Roles;
using AmongUsSalem.Modules.Components;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using UnityEngine;
using Color = UnityEngine.Color;

namespace AmongUsSalem.Roles;

#region Oracle
#endregion
public sealed class Oracle(IntPtr cppPtr)
    : CrewmateRole(cppPtr), IWikiDiscoverable, ICustomAURole
{
    public string RoleName { get; set; } = "Oracle";
    public string revealText => "safeguards the town with an aegis.";
    public string RoleDescription => "Aegis townies to protect them.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Color RoleColor { get; set; } = AUSColors.Town;
    public Alignment Alignment => Alignment.TownProtective;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;


    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.OracleRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#06E00C>Oracle</color>" +
            $"\n<color=#e70052>Attack: {Attack}</color>" +
            $"\n<color=#0000ff>Defense: {Defense}</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#06E00C>Town</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#06E00C>Town</color> <color=#1e45d4>Protective</color>" +
            "\n<color=#fdbc00>Goal:</color> Hang every criminal and evildoer." +
            $"\n\nAttributes:" +
            "\nYou cannot Aegis the same roles two nights in a row." +
            "\nYou know if a player was attacked." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Aegis",
            "You can Aegis a town role at Night. You will grant Powerful Defense to all players with the selected role. You may only Aegis Oracle a few times per game.",
            AUSAssets.Oracle_Aegis)
    ];

    public override void OnMeetingStart()
    {
        PastAegisRoles.Clear(); // Clear past roles.

        PastAegisRoles.AddRange(AegisRoles); // Apply recent roles.

        // Clear recent roles.
        AegisRoles.Clear();
        AegisPlayers.Clear();
    }

    public enum Type { OasisNotif, TargetProtected }
    public static string Info(Type type)
    {
        if (type is Type.OasisNotif) return "Your <b><color=#4a86e8>Aegis</color></b> protected someone last night!";
        return "Someone attacked you, but a <b><color=#4a86e8>Barrier</color></b> protected you!";
    }

    [MethodRpc((uint)AUSRpc.RpcNotifyOracle, SendImmediately = true)]
    public static bool RpcNotifyOracle(PlayerControl visitor, PlayerControl target)
    {
        foreach (var oracles in MiscUtils.GetPlayersWithRole<Oracle>())
        {
            var oracle = oracles.GetRole<Oracle>();
            if (oracle.Player.AmOwner())
            {
                MiscUtils.AddFakeChat(oracle.Player.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Oracle Info"), Info(Type.OasisNotif));
            }

            if (oracle.AegisPlayers.Contains(target.PlayerId))
            {
                if (target.AmOwner()) // Always put AmOwner inside of Rpcs that have RpcCustomMurder, or for client side notifications!
                {
                    MiscUtils.AddFakeChat(target.CachedPlayerData, MiscUtils.GetTitle(AUSColors.Town, "Oracle Info"), Info(Type.TargetProtected));
                }
            }
        }

        return false;
    }

    public void AegisMenu()
    {
        if (Minigame.Instance != null)
        {
            return;
        }

        var shapeMenu = GuesserMenu.Create();
        shapeMenu.Begin(IsRoleValid, ClickRoleHandle);

        void ClickRoleHandle(RoleBehaviour role)
        {
            if (role is Oracle) Charges--;

            AegisRoles.Add(role);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.Role is ICustomAURole customRole && customRole.RoleName == role.NiceName)
                {
                    AegisPlayers.Add(player.PlayerId);
                    customRole.ApplyDefense(Defense.Powerful);
                }
            }

            if (Player.AmOwner) // Resets other ability cooldowns they have.
            {
                var buttons = CustomButtonManager.Buttons.Where(x => x.Enabled(Player.Data.Role) && x.Timer <= 0).ToList();
                foreach (var button in buttons) button.ResetCooldownAndOrEffect();
            }

            shapeMenu.Close();
        }
    }

    private bool IsRoleValid(RoleBehaviour role)
    {
        if (role.IsDead) return false;
        if (role is IGhostRole) return false;

        if (!role.IsCrewmate()) return false;

        if (AegisRoles.Contains(role)) return false;
        if (PastAegisRoles.Contains(role)) return false;
        if (role is Oracle && Charges == 0) return false;

        return true;
    }

    public int Charges = (int)OptionGroupSingleton<Oracle_Options>.Instance.Charges;
    public List<byte> AegisPlayers = new List<byte>();
    public List<RoleBehaviour> AegisRoles = new List<RoleBehaviour>();
    public List<RoleBehaviour> PastAegisRoles = new List<RoleBehaviour>();
}


#region Oracle_Aegis
#endregion
public sealed class Oracle_Aegis : AmongUsSalemRoleButton<Oracle>
{
    public override string Name => "Aegis";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => AUSColors.Town;
    public override float Cooldown => OptionGroupSingleton<Oracle_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Oracle_Aegis;

    public override void ClickHandler()
    {
        if (Timer <= 0)
        {
            if (MiscUtils.SuccessfulVisit(Player, Player, false, false))
            {
                base.ClickHandler();
            }
        }
    }

    protected override void OnClick()
    {
        Role.AegisMenu();

        MiscUtils.PostSuccessfulVisit(Player, Player, false, false);
    }
}

#region Oracle_Options
#endregion
public sealed class Oracle_Options : AbstractOptionGroup<Oracle>
{
    public override string GroupName => "Oracle";

    [ModdedNumberOption("<color=#06E00C>Oracle</color> <color=#4a86e8>Aegis</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#06E00C>Oracle</color> Max <color=#4a86e8>Self Aegis</color>", 1f, 30f, 1f)]
    public float Charges { get; set; } = 1;
}