using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ObjectWorkshop.Roles.Crewmate;

#region Duelist
#endregion
public sealed class Duelist(IntPtr cppPtr)
    : CrewmateRole(cppPtr), IOWRole, IWikiDiscoverable, IContinueGame
{
    public bool attractsMetal => true;
    public string RoleName => TouLocale.Get(TouNames.Duelist, "Duelist");
    public string revealText => "is a samurai in training.";
    public string RoleDescription => "Sharpen to increase your chance to win Duels!";
    public string RoleLongDescription => RoleDescription;
    public Color RoleColor => OWColors.Crewmate;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateKilling;

    public bool continueGame => true;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = OWAssets.Duelist,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IOWRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Killing role who can Duel players, killing them if they have a high enough win chance."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Sharpen",
            $"You can Sharpen your sword during the round. You will increase your Win Chance by {OptionGroupSingleton<Duelist_Options>.Instance.ChanceUp}%, up to 100%.",
            OWAssets.Duelist_Sharpen),

        new("Duel",
            "You can Duel a player during the round. You will both be immobile and unable to use any abilities. You and your target will dash towards each other. You will kill your target based on your Win Chance. If you lose, your target will kill you. Your Win Chance will reset afterwards. If you or your target die before the duel ends, the duel will be canceled, your Duel cooldown will reset, and your Win Chance will remain.",
            OWAssets.Duelist_Duel)
    ];

    [MethodRpc((uint)ObjectWorkshopRpc.ResetWinChance, SendImmediately = true)]
    public static void RpcResetWinChance(PlayerControl player)
    {
        if (player.Data.Role is not Duelist)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcResetWinChance - Invalid duelist");
            return;
        }

        var duelist = player.GetRole<Duelist>();
        duelist.WinChance = (int)OptionGroupSingleton<Duelist_Options>.Instance.InitialChance;
        duelist.DuelingPlayers.Clear();
    }

    [MethodRpc((uint)ObjectWorkshopRpc.Sharpen, SendImmediately = true)]
    public static void RpcSharpen(PlayerControl player)
    {
        if (player.Data.Role is not Duelist)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcSharpen - Invalid duelist");
            return;
        }

        var duelist = player.GetRole<Duelist>();
        duelist.WinChance += (int)OptionGroupSingleton<Duelist_Options>.Instance.ChanceUp;
        if (duelist.WinChance > 100)
        {
            duelist.WinChance = 100;
        }
    }

    [MethodRpc((uint)ObjectWorkshopRpc.Duel, SendImmediately = true)]
    public static void RpcDuel(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not Duelist)
        {
            Logger<ObjectWorkshopPlugin>.Error("RpcDuel - Invalid duelist");
            return;
        }

        var duelist = player.GetRole<Duelist>();
		duelist.RandomNumber = Random.Range(0, 100);
        
		Duel.Begin(duelist.Player, target, duelist.WinChance);
        
    }

    public void LobbyStart()
    {
        Duel.CleanUp();
    }
    
    public int WinChance = (int)OptionGroupSingleton<Duelist_Options>.Instance.InitialChance;
    public int RandomNumber;
    public List<byte> DuelingPlayers = new List<byte>();
}

#region Duelist_Sharpen
#endregion
public sealed class Duelist_Sharpen : ObjectWorkshopRoleButton<Duelist>
{
    public override string Name => "Sharpen";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => OWColors.Crewmate;
    public override float Cooldown => OptionGroupSingleton<Duelist_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Duelist_Sharpen;

    public override bool CanUse()
    { 
        return 
            base.CanUse() && Role.WinChance < 100;
    }

    protected override void OnClick()
    {
        Duelist.RpcSharpen(Role.Player);
        TouAudio.PlaySound(OWAssets.Duelist_Sharpen_SFX);
    }
}

#region Duelist_Duel
#endregion
public sealed class Duelist_Duel : ObjectWorkshopRoleButton<Duelist, PlayerControl>
{
    public override string Name => "Duel";
    public override Color TextOutlineColor => OWColors.Crewmate;
    public override float Cooldown => OptionGroupSingleton<Duelist_Options>.Instance.DuelCD;
    public override LoadableAsset<Sprite> Sprite => OWAssets.Duelist_Duel;

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        if (playerControl.IsRole<Duelist>())
        {
            var duelist = playerControl.GetRole<Duelist>();
            Button?.usesRemainingText.gameObject.SetActive(true);
            Button?.usesRemainingSprite.gameObject.SetActive(true);
            Button!.usesRemainingText.text = duelist.WinChance + "%";
        }

        base.FixedUpdate(playerControl);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override bool IsTargetValid(PlayerControl? target)
    {
        if (target == null) return base.IsTargetValid(target);
        return !target.IsDueling();
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Reactor.Utilities.Logger<ObjectWorkshopPlugin>.Error("Duelist Ability: Target is null");
            return;
        }

        Duelist.RpcDuel(Role.Player, Target);
    }
}

#region Duelist_Options
#endregion
public sealed class Duelist_Options : AbstractOptionGroup<Duelist>
{
    public override string GroupName => TouLocale.Get(TouNames.Duelist, "Duelist");

    [ModdedNumberOption("<color=#b3ffff>Duelist</color> <color=#4a86e8>Sharpen</color> Cooldown", 0, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25;

    [ModdedNumberOption("<color=#b3ffff>Duelist</color> <color=#4a86e8>Duel</color> Cooldown", 0, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float DuelCD { get; set; } = 25;

    [ModdedNumberOption("<color=#b3ffff>Duelist</color> <color=#4a86e8>Duel</color> Duration", 0.25f, 60f, 0.05f, MiraNumberSuffixes.Seconds, "0.00")]
    public float Duration { get; set; } = 0.5f;

    [ModdedNumberOption("<color=#b3ffff>Duelist</color> Win Chance Up", 0, 100f, 5f, MiraNumberSuffixes.Percent)]
    public float ChanceUp { get; set; } = 10f;

    [ModdedNumberOption("<color=#b3ffff>Duelist</color> Initial Win Chance", 0, 100f, 5f, MiraNumberSuffixes.Percent)]
    public float InitialChance { get; set; } = 50f;
}