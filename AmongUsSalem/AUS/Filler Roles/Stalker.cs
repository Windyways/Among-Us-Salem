/*using System.Text;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace ObjectWorkshop.Roles.Crewmate;

#region Stalker
#endregion
public sealed class Stalker(IntPtr cppPtr)
    : CrewmateRole(cppPtr), IOWRole, IWikiDiscoverable
{
    public override bool IsAffectedByComms => false;

    public string RoleName => TouLocale.Get(TouNames.Stalker, "Stalker");
    public string RoleDescription => "Track a player to see where they are";
    public string RoleLongDescription => "Track a player to get an arrow pointing at their location!";
    public Color RoleColor => OWColors.Crewmate;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = OWAssets.Stalker,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = IOWRole.SetNewTabText(this);

        if (StalkingPlayer != null)
        {
            stringB.Append("\n<b>Stalking:</b> ");
            stringB.Append(CultureInfo.InvariantCulture,
                $"{Color.white.ToTextColor()}{StalkingPlayer.Data.PlayerName}</color>");
        }

        return stringB;
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Investigative role who can use their Track ability on a player to get an arrow pointing towards them in realtime. Selecting a new target overrides the previous one."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Track",
            "You can Track a player during the round. You will get an arrow pointing to your target until you select a new one.",
            OWAssets.Placeholder)
    ];

    public override void OnMeetingStart()
    {
        if (StalkingPlayer != null && (StalkingPlayer.Data.IsDead || StalkingPlayer.Data.Disconnected))
        {
            if (!Arrow.IsDestroyedOrNull())
            {
                Arrow?.gameObject.Destroy();
                Arrow?.Destroy();
            }
            StalkingPlayer = null;
        }
    }

    private readonly float _updateInterval = 0.1f;
    private DateTime _time = DateTime.UnixEpoch;
    public ArrowBehaviour? Arrow;
    public void FixedUpdate()
    {
        if (StalkingPlayer == null)
            return;
        
        if (_updateInterval <= 0 || _time <= DateTime.UtcNow.AddSeconds(-_updateInterval))
        {
            if (Arrow != null)
            {
                Arrow.target = StalkingPlayer.transform.position;
                Arrow.Update();
            }

            _time = DateTime.UtcNow;
        }
    }

    public PlayerControl StalkingPlayer; 
}

#region Stalker_Track
#endregion
public sealed class Stalker_Track : ObjectWorkshopRoleButton<Stalker, PlayerControl>
{
    public override string Name => "Track";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Crewmate;
    public override float Cooldown => OptionGroupSingleton<Stalker_Options>.Instance.Cooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Stalker_Track;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        var stalker = PlayerControl.LocalPlayer.GetRole<Stalker>();
        if (stalker != null)
        {
            if (target == null) return base.IsTargetValid(target);
            return base.IsTargetValid(target) && !target.IsStalked();
        }
        return base.IsTargetValid(target);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Reactor.Utilities.Logger<ObjectWorkshopPlugin>.Error("Stalker Ability: Target is null");
            return;
        }

        var player = PlayerControl.LocalPlayer;
        var stalker = player.GetRole<Stalker>();
        if (stalker == null)
            return;

        stalker.StalkingPlayer = Target;

        if (!stalker.Arrow.IsDestroyedOrNull())
        {
            stalker.Arrow?.gameObject.Destroy();
            stalker.Arrow?.Destroy();
        }

        Color color = Palette.PlayerColors[Target.GetDefaultAppearance().ColorId];
        stalker.Arrow = MiscUtils.CreateArrow(Target.transform, color);
        
        var Arrow = stalker.Arrow;
        var spr = Arrow.gameObject.GetComponent<SpriteRenderer>();
        var r = Arrow.gameObject.AddComponent<BasicRainbowBehaviour>();
        r.AddRend(spr, Target.cosmetics.ColorId);
    }
}*/