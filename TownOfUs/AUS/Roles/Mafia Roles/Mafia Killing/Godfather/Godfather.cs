using AmongUsSalem.MafiaRoles;
using Il2CppInterop.Runtime.Attributes;
using Reactor.Utilities.Extensions;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Godfather(IntPtr cppPtr) : ImpostorRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Godfather";
    public string revealText => "is the leader of the Mafia.";
    public string RoleDescription => "Town Of Salem";
    public string RoleLongDescription => "You are the leader of organized crime.";
    public Color RoleColor { get; set; } = RoleColors.Mafia;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public Faction Faction { get; set; } = Faction.Mafia;
    public Alignment Alignment => Alignment.MafiaKilling;

    public Attack Attack { get; set; } = Attack.Basic;
    public Defense Defense { get; set; } = Defense.Basic;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.Basic;
    public Defense ogDefense { get; set; } = Defense.Basic;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        Icon = AUSAssets.GodfatherRoleCard
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
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can force Mafiosos to kill for them, being undetected. It may kill directly if there are no Mafiosos.\n" +
            "Kill anyone that will not submit to the Mafia." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Order",
            "You can Order a Mafioso to attack a player at Night.\n" +
            "You will order your Mafioso to deal a Basic Attack to your target. If the Mafioso fails, you will attack instead.\n" +
            "If there are no Mafiosos, you will directly attack your target.",
            AUSAssets.Godfather_Order),
    ];

    public string GetAttributes()
    {
        return
            $"- You appear innocent to Sheriffs.";
    }

    public static string Info()
    {
        return "You were promoted to a Godfather!";
    }

    [MethodRpc((uint)AUSRpc.RpcResetMafiosoCooldown)]
    public static void RpcResetMafiosoCooldown(PlayerControl player)
    {
        if (player.AmOwner())
        {
            CustomButtonSingleton<Mafioso_Kill>.Instance.ResetCooldownAndOrEffect();
        }
    }
}

public sealed class Godfather_Kill : TownOfUsRoleButton<Godfather, PlayerControl>
{
    public override string Name => "Kill";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Godfather_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Godfather_Order;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, true, true)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        if (Target == null)
            return;

        if (Player.CanKill(Target))
        {
            Player.RpcCustomMurder(Target);
            VisitingMechanic.RpcAddDeathReason(Target, (int)DeathReasonShow.KilledByAMemberOfTheMafia);
        }
        else Player.Notify(Feedback.TooMuchDefense(Player, Target), NotifyMode.InstantlyAndMeeting);
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(false, Distance);
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && MiscUtils.GetRoles<Mafioso>().Count == 0;
    }
}

public sealed class Godfather_Order : TownOfUsRoleButton<Godfather>
{
    public override string Name => "Order";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Mafia;
    public override float Cooldown => OptionGroupSingleton<Godfather_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Godfather_Order;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Player, false, false)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        var player1Menu = CustomPlayerMenu.Create();
        player1Menu.transform.FindChild("PhoneUI").GetChild(0).GetComponent<SpriteRenderer>().material =
            PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
        player1Menu.transform.FindChild("PhoneUI").GetChild(1).GetComponent<SpriteRenderer>().material =
            PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;

        player1Menu.Begin(
            plr => (!plr.HasDied() && !plr.Is(Faction.Mafia)),
            plr =>
            {
                player1Menu.ForceClose();

                if (plr == null)
                {
                    return;
                }

                // Stuff here.
                var target = plr;
                var mafioso = MiscUtils.GetRoles<Mafioso>().Random();
                var mafiosoButton = CustomButtonSingleton<Mafioso_Kill>.Instance;

                if (mafiosoButton.IsTargetingValid(mafioso.Player, target, true, true, true))
                {
                    if (mafioso.Player.CanKill(target))
                    {
                        mafioso.Player.RpcAddModifier<InvisibleStatus>();
                        mafioso.Player.RpcAddModifier<OrderedModifier>(Player);
                        mafioso.Player.RpcCustomMurder(target);
                        VisitingMechanic.RpcAddDeathReason(target, (int)DeathReasonShow.KilledByAMemberOfTheMafia);
                    }
                    else mafioso.Player.Notify(Feedback.TooMuchDefense(Player, target), NotifyMode.InstantlyAndMeeting);
                }
                else if (button.IsTargetingValid(Player, target, true, true, true))
                {
                    if (Player.CanKill(target))
                    {
                        Player.RpcAddModifier<InvisibleStatus>();
                        Player.RpcCustomMurder(target);
                        VisitingMechanic.RpcAddDeathReason(target, (int)DeathReasonShow.KilledByAMemberOfTheMafia);
                    }
                    else Player.Notify(Feedback.TooMuchDefense(Player, target), NotifyMode.InstantlyAndMeeting);
                }

                Godfather.RpcResetMafiosoCooldown(mafioso.Player);
            }
        );
        foreach (var panel in player1Menu.potentialVictims)
        {
            panel.PlayerIcon.cosmetics.SetPhantomRoleAlpha(1f);
            if (panel.NameText.text != PlayerControl.LocalPlayer.Data.PlayerName)
            {
                panel.NameText.color = Color.white;
            }
        }
    }

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && MiscUtils.GetRoles<Mafioso>().Count > 0;
    }
}

public static class Godfather_Events
{
    [RegisterEvent]
    public static void EjectionEvent(EjectionEvent @event)
    {
        NetworkedPlayerInfo exiled = @event.ExileController.initData.networkedPlayer;
        if (exiled != null)
        {
            PlayerControl player = exiled.Object;
            if (player.IsRole<Godfather>()) MafiosoPromotionMechanic.GodfatherDied = true;
        }
    }

    [RegisterEvent]
    public static void AfterMurderEvent(AfterMurderEvent @event)
    {
        var role = @event.Target.GetRoleWhenAlive();
        var aliveMafiosos = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.HasDied() && x.IsRole<Mafioso>());
        if (role is Godfather && aliveMafiosos > 0) MafiosoPromotionMechanic.GodfatherDied = true;
    }
}

public sealed class Godfather_Options : AbstractOptionGroup<Godfather>
{
    public override string GroupName => "Godfather";

    [ModdedNumberOption("Godfather Order Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}