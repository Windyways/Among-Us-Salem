using Il2CppInterop.Runtime.Attributes;
using System.Text;
using UnityEngine;

namespace AmongUsSalem.Roles;

public sealed class Seer(IntPtr cppPtr) : CrewmateRole(cppPtr), ICustomAURole, IWikiDiscoverable
{
    public string RoleName { get; set; } = "Seer";
    public string revealText => "can see into the hearts of people to find out their intentions.";
    public string RoleDescription => "";
    public string RoleLongDescription => "You are able to see into the hearts of townies to find out their intentions.";
    public Color RoleColor { get; set; } = RoleColors.Town;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction Faction { get; set; } = Faction.Town;
    public Alignment Alignment => Alignment.TownInvestigative;

    public Attack Attack { get; set; } = Attack.None;
    public Defense Defense { get; set; } = Defense.None;
    public EtherealDefense EtherealDefense { get; set; } = EtherealDefense.None;

    public Attack ogAttack { get; set; } = Attack.None;
    public Defense ogDefense { get; set; } = Defense.None;
    public EtherealDefense ogEtherealDefense { get; set; } = EtherealDefense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.SeerRoleCard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var info = ICustomAURole.SetNewTabText(this);

        // Only show info if we have data
        if (Information.Count > 0)
        {
            info.AppendLine();
            foreach (var searched in Information)
            {
                string intuitName = searched.Item1.Item1.Name();
                string gazeName = searched.Item1.Item2.Name();

                string result = searched.Item2 ? "<color=#FF0000>Enemies!</color>" : "<color=#00FF00>Friends!</color>";
                info.AppendLine($"{intuitName} & {gazeName} - {result}");
            }
        }
        else
        {
            info.AppendLine();
            info.AppendLine("No Compare results yet.");
        }

        return info;
    }

    public string GetAdvancedDescription()
    {
        return
            $"Attack: {Attack}\n" +
            $"Defense: {Defense}\n" +
            $"The {RoleName} is a {Alignment.ToSpacedString()} role that can check if two players are on opposing factions or not, being a huge threat to Coven and Mafia.\n" +
            "Hang every criminal and evildoer." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public string GetAttributes()
    {
        return
            $"- Illusionists can make your target appear innocent.\n" +
            $"- Enchanters and Framers can make your target appear suspicious.";
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Intuit",
            "You can Intuit a player at Night.\n" +
            "You will see if your target and your Gaze target are in opposite or equal factions.\n" +
            "You can only Intuit/Gaze your target once.",
            AUSAssets.Seer_Intuit),

        new("Gaze",
            "You can Gaze a player at Night.\n" +
            "You will see if your target and your Intuit target are in opposite or equal factions.\n" +
            "You can only Intuit/Gaze your target once.",
            AUSAssets.Seer_Gaze),
    ];

    public static string Info(PlayerControl player, PlayerControl intuit, PlayerControl gaze)
    {
        player.AddModifier<TI>();
        if (IsFriends(intuit, gaze)) return $"You sense that {intuit.Name()} and {gaze.Name()} seem friendly with each other!";

        gaze.AddModifier<IncriminatingEvidence>();
        intuit.AddModifier<IncriminatingEvidence>();
        return $"You sense that {intuit.Name()} and {gaze.Name()} seem to be enemies!";
    }

    public static bool IsFriends(PlayerControl intuit, PlayerControl gaze)
    {
        // --- Intuit ---
        if (intuit.HasModifier<IllusionedModifier>())
        {
            if (gaze.Is(Faction.Town) || intuit.IsRole<Jester>()) return true;
            if (gaze.HasModifier<IllusionedModifier>()) return true;
            return false;
        }

        if (intuit.HasModifier<FramedModifier>())
        {
            if (gaze.Is(Faction.Mafia)) return true;
            if (gaze.HasModifier<FramedModifier>()) return true;
            return false;
        }

        // --- GAZE ---
        if (gaze.HasModifier<IllusionedModifier>())
        {
            if (intuit.Is(Faction.Town) || intuit.IsRole<Jester>()) return true;
            if (intuit.HasModifier<IllusionedModifier>()) return true;
            return false;
        }

        if (gaze.HasModifier<FramedModifier>())
        {
            if (intuit.Is(Faction.Mafia)) return true;
            if (intuit.HasModifier<FramedModifier>()) return true;
            return false;
        }

        if (intuit.Is(Faction.Town) && gaze.IsRole<Jester>()) return true;
        if (gaze.Is(Faction.Town) && intuit.IsRole<Jester>()) return true;
        return intuit.IsSameFaction(gaze);
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);
        intuit = null;
        gaze = null;
        fullCooldown = true;
    }

    public PlayerControl intuit;
    public PlayerControl gaze;
    public bool fullCooldown = true;

    public List<((PlayerControl, PlayerControl), bool)> Information = new List<((PlayerControl, PlayerControl), bool)>();
}

public sealed class Seer_Intuit : TownOfUsRoleButton<Seer, PlayerControl>
{
    public override string Name => "Intuit";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => Role.fullCooldown ? OptionGroupSingleton<Seer_Options>.Instance.Cooldown : 1;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Seer_Intuit;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, false, true)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        if (Target == null)
            return;

        Role.fullCooldown = false;
        Role.intuit = Target;
        if (Role.intuit != null && Role.gaze != null)
        {
            Role.Information.Add(((Role.intuit, Role.gaze), !Seer.IsFriends(Role.intuit, Role.gaze)));

            Role.fullCooldown = true;
            Role.intuit.AddModifier<ComparedModifier>(Player, Role.gaze, Seer.IsFriends(Role.intuit, Role.gaze));
            Role.gaze.AddModifier<ComparedModifier>(Player, Role.intuit, Seer.IsFriends(Role.intuit, Role.gaze));
            Player.Notify(Seer.Info(Player, Role.intuit, Role.gaze), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.SeerRoleCard.LoadAsset());

            Role.intuit = null;
            Role.gaze = null;
            CustomButtonSingleton<Seer_Intuit>.Instance.ResetCooldownAndOrEffect();
            CustomButtonSingleton<Seer_Gaze>.Instance.ResetCooldownAndOrEffect();
        }
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !x.HasModifier<ComparedModifier>(x => x.Caster == Player) && x != Role.gaze && x != Role.intuit && !x.HasModifier<GlobalReveal>());
    }
}

public sealed class Seer_Gaze : TownOfUsRoleButton<Seer, PlayerControl>
{
    public override string Name => "Gaze";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => RoleColors.Town;
    public override float Cooldown => Role.fullCooldown ? OptionGroupSingleton<Seer_Options>.Instance.Cooldown : 1;
    public override LoadableAsset<Sprite> Sprite => AUSAssets.Seer_Gaze;

    public override void ClickHandler()
    {
        if (button.IsTargetingValid(Player, Target, false, true)) base.ClickHandler();
    }

    protected override void OnClick()
    {
        if (Target == null)
            return;

        Role.fullCooldown = false;
        Role.gaze = Target;
        if (Role.intuit != null && Role.gaze != null)
        {
            Role.Information.Add(((Role.intuit, Role.gaze), !Seer.IsFriends(Role.intuit, Role.gaze)));

            Role.fullCooldown = true;
            Role.intuit.AddModifier<ComparedModifier>(Player, Role.gaze, Seer.IsFriends(Role.intuit, Role.gaze));
            Role.gaze.AddModifier<ComparedModifier>(Player, Role.intuit, Seer.IsFriends(Role.intuit, Role.gaze));
            Player.Notify(Seer.Info(Player, Role.intuit, Role.gaze), NotifyMode.InstantlyAndMeeting, sprite: AUSAssets.SeerRoleCard.LoadAsset());

            Role.intuit = null;
            Role.gaze = null;
            CustomButtonSingleton<Seer_Intuit>.Instance.ResetCooldownAndOrEffect();
            CustomButtonSingleton<Seer_Gaze>.Instance.ResetCooldownAndOrEffect();
        }
    }

    public override PlayerControl? GetTarget()
    {
        return Player.GetClosestLivingPlayer(true, Distance, predicate: x =>
            !x.HasModifier<ComparedModifier>(x => x.Caster == Player) && x != Role.gaze && x != Role.intuit && !x.HasModifier<GlobalReveal>());
    }
}

public sealed class Seer_Options : AbstractOptionGroup<Seer>
{
    public override string GroupName => "Seer";

    [ModdedNumberOption("Seer Intuit & Gaze Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 25f;
}