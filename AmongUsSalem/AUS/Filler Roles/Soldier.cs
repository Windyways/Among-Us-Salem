using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Hud;
using MiraAPI.Networking;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using ObjectWorkshop.Buttons;
using ObjectWorkshop.Utilities;
using UnityEngine;

namespace ObjectWorkshop.Roles.Crewmate;

#region Soldier
#endregion
public sealed class Soldier(IntPtr cppPtr)
    : CrewmateRole(cppPtr), IOWRole, IContinueGame
{
    public string revealText => "";
    public bool continueGame =>
        CustomButtonSingleton<Soldier_Attack>.Instance.MaxUses > 0 ||
        (Player.GetTasksLeft() + TasksCompleted) >= OptionGroupSingleton<Soldier_Options>.Instance.Required;

    public override bool IsAffectedByComms => false;

    public string RoleName => TouLocale.Get(TouNames.Soldier, "Soldier");
    public string RoleDescription => "Attack a player and complete tasks";
    public string RoleLongDescription => "Can kill every couple of tasks!";
    public Color RoleColor => OWColors.Crewmate;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        //Icon = OWAssets.Soldier,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Killing role who can Attack a player to kill them, although only usable every couple of tasks completed."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Attack",
            "You can Attack a player during the round. You will kill your target.",
            TouCrewAssets.Placeholder)
    ];

    public int TasksCompleted;
}

#region Soldier_Attack
#endregion
public sealed class Soldier_Attack : ObjectWorkshopRoleButton<Soldier, PlayerControl>
{
    public override string Name => "Attack";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Crewmate;
    public override float Cooldown => OptionGroupSingleton<Soldier_Options>.Instance.Cooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.Placeholder;
    public override int MaxUses => 0;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Reactor.Utilities.Logger<ObjectWorkshopPlugin>.Error("Soldier Ability: Target is null");
            return;
        }

        var player = PlayerControl.LocalPlayer;
        var soldier = player.GetRole<Soldier>();
        if (soldier == null)
            return;

        player.RpcCustomMurder(Target);
        DecreaseUses();
    }

    public override bool CanUse()
    {
        return base.CanUse() && UsesLeft > 0;
    }

}

#region Soldier_Options
#endregion
public sealed class Soldier_Options : AbstractOptionGroup<Soldier>
{
    public override string GroupName => TouLocale.Get(TouNames.Soldier, "Soldier");

    [ModdedNumberOption("<color=#b3ffff>Soldier</color> <color=#4a86e8>Attack</color> Cooldown", 0, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25;

    [ModdedNumberOption("<color=#b3ffff>Soldier</color> Tasks Required", 1, 30, 1, MiraNumberSuffixes.None)]
    public float Required { get; set; } = 3;
}

#region Soldier_Events
#endregion
public static class Soldier_Events
{
    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        if (@event.Player.Data.Role is Soldier soldier)
        {
            var tasksReq = (int)OptionGroupSingleton<Soldier_Options>.Instance.Required;

            soldier.TasksCompleted++;
            if (@event.Player.AmOwner && soldier.TasksCompleted >= tasksReq)
            {
                var button = CustomButtonSingleton<Soldier_Attack>.Instance;
                button.IncreaseUses();

                soldier.TasksCompleted -= tasksReq;
            }
        }
    }
}