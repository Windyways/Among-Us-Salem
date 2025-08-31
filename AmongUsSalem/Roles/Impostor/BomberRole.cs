using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using AmongUsSalem.Events.TouEvents;
using AmongUsSalem.Modules;
using AmongUsSalem.Options.Roles.Impostor;
using AmongUsSalem.Roles.Crewmate;
using AmongUsSalem.Utilities;
using UnityEngine;

namespace AmongUsSalem.Roles.Impostor;

public sealed class BomberRole(IntPtr cppPtr)
    : ImpostorRole(cppPtr), IAUSRole, IDoomable, ICrewVariant
{
    public string revealText => "";
    [HideFromIl2Cpp] public Bomb? Bomb { get; set; }
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<TrapperRole>());
    public DoomableType DoomHintType => DoomableType.Relentless;

    public string RoleName => TouLocale.Get(TouNames.Bomber, "Bomber");
    public string RoleDescription => "Plant Bombs To Kill Multiple Crewmates At Once";
    public string RoleLongDescription => "Plant bombs to kill several crewmates at once";
    public Color RoleColor => AUSColors.Mafia;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public Alignment Alignment => Alignment.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TouRoleIcons.Bomber,
        CanUseVent = OptionGroupSingleton<BomberOptions>.Instance.CanVent
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is an Impostor Killing role that can drop a bomb on the map, which detonates after {OptionGroupSingleton<BomberOptions>.Instance.DetonateDelay} second(s)" +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Place",
            $"Place a bomb, showing the radius in which it'll kill, killing up to {(int)OptionGroupSingleton<BomberOptions>.Instance.MaxKillsInDetonation} player(s)",
            TouImpAssets.PlaceSprite)
    ];

    [MethodRpc((uint)AUSRpc.PlantBomb, SendImmediately = true)]
    public static void RpcPlantBomb(PlayerControl player, Vector2 position)
    {
        if (player.Data.Role is not BomberRole role)
        {
            Logger<AUSPlugin>.Error("RpcPlantBomb - Invalid bomber");
            return;
        }

        var touAbilityEvent = new TouAbilityEvent(AbilityType.BomberPlant, player);
        MiraEventManager.InvokeEvent(touAbilityEvent);

        if (player.AmOwner)
        {
            role.Bomb = Bomb.CreateBomb(player, position);
        }
        else if (OptionGroupSingleton<BomberOptions>.Instance.AllImpsSeeBomb && PlayerControl.LocalPlayer.IsImpostor())
        {
            Coroutines.Start(Bomb.BombShowTeammate(player, position));
        }
    }
}