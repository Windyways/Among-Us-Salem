using System.Text;
using UnityEngine;
using Color = UnityEngine.Color;

namespace ObjectWorkshop.LifeImprovement.Roles;

#region Totemist
#endregion
public sealed class Totemist(IntPtr cppPtr) 
    : CrewmateRole(cppPtr), IOWRole, IWikiDiscoverable, IVisualAppearance
{
    public string RoleName => TouLocale.Get(TouNames.Totemist, "Totemist");
    public string revealText => "is protective of their prized possessions.";
    public string RoleDescription => "Install totems, and watch them!";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => OWColors.Crewmate;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = OWAssets.Totemist,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Investigative role that can install totems, and can watch them from a distance." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Install",
            "You can Install a totem into the ground during the round. You will place down a totem, visible to everyone.",
            OWAssets.Totemist_Install),
            
        new("Watch",
            "You can Watch your totems during the round. You will be immobile, and you will see around a selected totem.",
            OWAssets.Totemist_Watch),
            
        new("Cycle",
            "You can Cycle between your totems during the round. You will view a different totem by number.",
            OWAssets.Totemist_Cycle),
    ];

    public override void OnMeetingStart()
    {
        if (isWatching)
        {
            isWatching = false;
            LightSource light = Player.lightSource;
            light.transform.SetParent(Player.transform);
            light.transform.localPosition = Player.Collider.offset;
        }

        totemViewing = 1;
    }

    public void LobbyStart()
    {
        Totem.CleanUp();
    }

    #region RpcInstall
    #endregion
    [MethodRpc((uint)ObjectWorkshopRpc.Install, SendImmediately = true)]
    public static void RpcInstall(PlayerControl player)
    {
        if (player.Data.Role is not Totemist)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcInstall - Invalid Totemist");
            return;
        }

        var totemist = player.GetRole<Totemist>();

        Totem totem = Totem.Begin(totemist.Player);
		totemist.TotemOrder.Add(totem);
    }

    public VisualAppearance GetVisualAppearance()
    {
        var appearance = Player.GetDefaultAppearance();
        if (!isWatching) return appearance;

        appearance.Speed = 0;
        return appearance;
    }

    public bool isWatching;
	public List<Totem> TotemOrder = new List<Totem>();
	public int totemViewing = 1;
}

#region Totemist_Install
#endregion
public sealed class Totemist_Install : ObjectWorkshopRoleButton<Totemist>
{
    public override string Name => "Install";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Crewmate;
    public override float Cooldown => OptionGroupSingleton<Totemist_Options>.Instance.Cooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Totemist_Install;
    public override int MaxUses => (int)OptionGroupSingleton<Totemist_Options>.Instance.MaxInstalls;

    public override bool CanUse()
    { 
        return base.CanUse() && !Role.isWatching;
    }

    protected override void OnClick()
    {
        Totemist.RpcInstall(Role.Player);
    }
}

#region Totemist_Watch
#endregion
public sealed class Totemist_Watch : ObjectWorkshopRoleButton<Totemist>
{
    public override string Name => "Watch";
    public override Color TextOutlineColor => OWColors.Crewmate;
    public override float Cooldown => 1f;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Totemist_Watch;

    public override bool CanUse()
    { 
        return 
            base.CanUse() && Role.TotemOrder.Count > 0;
    }

    protected override void OnClick()
    {
        if (!Role.isWatching)
        {
            Role.isWatching = true;
        }
        else
        {
            Role.isWatching = false;

            LightSource light = Role.Player.lightSource;
            light.transform.SetParent(Role.Player.transform);
            light.transform.localPosition = Role.Player.Collider.offset;
        }
    }
}

#region Totemist_Cycle
#endregion
public sealed class Totemist_Cycle : ObjectWorkshopRoleButton<Totemist>
{
    public override string Name => "Cycle";
    public override Color TextOutlineColor => OWColors.Crewmate;
    public override float Cooldown => 0.25f;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Totemist_Cycle;

    public override bool CanUse()
    { 
        return 
            base.CanUse() && Role.isWatching && Role.TotemOrder.Count >= 2;
    }

    protected override void OnClick()
    {
        Role.totemViewing++;
        if (Role.totemViewing > Role.TotemOrder.Count) Role.totemViewing = 1;
        if (Role.isWatching)
        {
            GameObject target = Role.TotemOrder[Role.totemViewing - 1].gameObject;
            target.SetCameraToObject(Role.Player);
        }
    }
}

#region Totemist_Options
#endregion
public sealed class Totemist_Options : AbstractOptionGroup<Totemist>
{
    public override string GroupName => TouLocale.Get(TouNames.Totemist, "Totemist");

    [ModdedNumberOption("<color=#b3ffff>Totemist</color> <color=#4a86e8>Install</color> Cooldown", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("<color=#b3ffff>Totemist</color> Max <color=#4a86e8>Installs</color>", 1f, 15f, 1f)]
    public float MaxInstalls { get; set; } = 3f;
}