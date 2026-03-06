using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Collections;
using System.Text;
using TownOfUs.Modules.Components;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Ritualist(IntPtr cppPtr) : CovenRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Ritualist";
    public string revealText => "casts deadly rituals with specific knowledge.";
    public string RoleDescription => "Town Of Salem 2";
    public string RoleLongDescription => "You are a sorcerer who sacrifices members of the town to the Old Ones.";
    public Color RoleColor { get; set; } = RoleColors.Coven;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;

    public Faction Faction { get; set; } = Faction.Coven;
    public Alignment Alignment => Alignment.CovenKilling;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseSabotage = OptionGroupSingleton<CovenOptions>.Instance.CanSabotage,
        CanUseVent = OptionGroupSingleton<CovenOptions>.Instance.CanVent,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = AUSAssets.RitualistRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ICustomAURole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return 
            $"Attack: Unstoppable\n" +
            $"Defense: {Defense}\n" +
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that punishes players that claim their role with death.\n" +
            "Kill all who would oppose the Coven." + 
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- With the Necronomicon, you may choose to deal a Basic Attack to a target.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Blood Ritual",
            "You can perform a Blood Ritual on a player at Night.\n" +
            "If you guess their role correctly, they will be dealt an Unstoppable Attack.\n" +
            "You cannot guess Town Investigatives or perform Blood Rituals on Illuminated players.",
            AUSAssets.Ritualist_BloodRitual),
    ];

    public bool WinConditionMet() => CovenGameOver.WinConditionMet(this);
    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet() || CovenGameOver.AnyCovenWon(gameOverReason);
    }

    public IEnumerator OpenMenu()
    {
        yield return new WaitForSeconds(0.4f);
        if (Minigame.Instance != null)
            yield break;

        var shapeMenu = GuesserMenu.Create();
        shapeMenu.Begin(IsRoleValid, ClickRoleHandle);

        void ClickRoleHandle(RoleBehaviour role)
        {
            if (currentTarget.Data.Role.NiceName == role.NiceName)
            {
                if (Player.CanKill(currentTarget, Attack.Unstoppable))
                {
                    Player.RpcCustomMurder(currentTarget, teleportMurderer: false);
                    VisitingMechanic.RpcAddDeathReason(currentTarget, (int)DeathReasonShow.KilledByARitualist);
                }
                else Player.Notify(Feedback.TooMuchDefense(Player, currentTarget), NotifyMode.InstantlyAndMeeting);
            }

            currentTarget = null;
            CustomButtonSingleton<Ritualist_BloodRitual>.Instance.DecreaseUses();

            CustomButtonSingleton<Ritualist_Attack>.Instance.ResetCooldownAndOrEffect();
            CustomButtonSingleton<Ritualist_BloodRitual>.Instance.ResetCooldownAndOrEffect();
            shapeMenu.Close();
        }
    }

    private bool IsRoleValid(RoleBehaviour role)
    {
        if (role.IsDead) return false;
        if (role is IGhostRole) return false;
        if (role is not ICustomAURole) return false;
        if (role is ICustomAURole customRole && customRole.Alignment == Alignment.TownInvestigative) return false;
        if (role is ICustomAURole customRole2 && customRole2.Faction == Faction.Coven) return false;
        return true;
    }

    public PlayerControl currentTarget;
}

public sealed class Ritualist_Attack : TownOfUsRoleButton<Ritualist, PlayerControl>
{
    public override string Name => "Attack";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Coven;
    public override float Cooldown => OptionGroupSingleton<CovenOptions>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Necronomicon;

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
            VisitingMechanic.RpcAddDeathReason(Target, (int)DeathReasonShow.KilledByTheCoven);
        }
        else Player.Notify(Feedback.TooMuchDefense(Player, Target), NotifyMode.InstantlyAndMeeting);

        CustomButtonSingleton<Ritualist_BloodRitual>.Instance.ResetCooldownAndOrEffect();
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance,
            predicate: x => !x.Is(Faction.Coven));
    }

    public override bool CanUse()
    {
        return base.CanUse() && Player.HasModifier<Necronomicon>();
    }
}

public sealed class Ritualist_BloodRitual : TownOfUsRoleButton<Ritualist>
{
    public override string Name => "Blood Ritual";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => RoleColors.Coven;
    public override float Cooldown => OptionGroupSingleton<Ritualist_Options>.Instance.Cooldown;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Ritualist_BloodRitual;
    public override int MaxUses => (int)OptionGroupSingleton<Ritualist_Options>.Instance.Charges;

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
            plr => (!plr.HasDied() && !plr.Is(Faction.Coven) && !plr.HasModifier<IlluminatedModifier>()),
            plr =>
            {
                player1Menu.ForceClose();

                if (plr == null)
                {
                    return;
                }

                // Stuff here.
                Role.currentTarget = plr;
                Coroutines.Start(Role.OpenMenu());
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

        IncreaseUses();
    }
}

public sealed class Ritualist_Options : AbstractOptionGroup<Ritualist>
{
    public override string GroupName => "Ritualist";

    [ModdedNumberOption("Ritualist Blood Ritual Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;

    [ModdedNumberOption("Ritualist Max Blood Rituals", 0f, 30f, 1f, MiraNumberSuffixes.None, zeroInfinity: true)]
    public float Charges { get; set; } = 2;
}