using TownOfUs.Options;
using UnityEngine;

namespace TownOfUs.Utilities;

public static class PlayerRoleTextExtensions
{
    public static Color UpdateTargetColor(this Color color, PlayerControl player, bool hidden = false)
    {
        // Town
        if ((PlayerControl.LocalPlayer.Data.Role is Bodyguard bodyguard && player.HasModifier<GuardedModifier>(x => x.Caster == bodyguard.Player)))
        {
            color = RoleColors.Town;
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Catalyst catalyst && player.HasModifier<OverchargedModifier>(x => x.Caster == catalyst.Player)))
        {
            color = RoleColors.Town;
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Seer seer && player.HasModifier<ComparedModifier>(x => x.Caster == seer.Player)))
        {
            color = RoleColors.Town;
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Cleric cleric && player.HasModifier<BarrieredModifier>(x => x.Caster == cleric.Player)))
        {
            color = RoleColors.Town;
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Crusader crusader && player.HasModifier<FortifiedModifier>(x => x.Caster == crusader.Player)))
        {
            color = RoleColors.Town;
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Lookout lookout && player.HasModifier<WatchedModifier>(x => x.Caster == lookout.Player)))
        {
            color = RoleColors.Town;
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Tracker tracker && player.HasModifier<TrackedModifier>(x => x.Caster == tracker.Player)))
        {
            color = RoleColors.Town;
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Pacifist pacifist && player.HasModifier<RalliedModifier>(x => x.Caster == pacifist.Player)) ||
            (player.HasModifier<ProtestModifier>()))
        {
            color = RoleColors.Town;
        }

        // Neutral
        if ((PlayerControl.LocalPlayer.Data.Role is Starspawn starspawn && player.HasModifier<IsolatedModifier>(x => x.Caster == starspawn.Player)))
        {
            color = RoleColors.Starspawn;
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Werewolf werewolf && player.HasModifier<TrackedModifier>(x => x.Caster == werewolf.Player)))
        {
            color = RoleColors.Werewolf;
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Plaguebearer plaguebearer && player.HasModifier<InfectedModifier>(x => x.Caster == plaguebearer.Player))
            || (player.HasModifier<InfectedModifier>() && PlayerControl.LocalPlayer.Is(Alignment.NeutralApocalypse)))
        {
            color = RoleColors.Apocalypse;
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Warlock warlock && player.HasModifier<CursedModifier>(x => x.Caster == warlock.Player))
            || (player.HasModifier<CursedModifier>() && PlayerControl.LocalPlayer.Is(Alignment.NeutralApocalypse)))
        {
            color = RoleColors.Apocalypse;
        }

        // Mafia
        if ((PlayerControl.LocalPlayer.Data.Role is Framer framer && player.HasModifier<FramedModifier>(x => x.Caster == framer.Player))
            || (player.HasModifier<FramedModifier>() && PlayerControl.LocalPlayer.Is(Faction.Mafia)))
        {
            color = RoleColors.Mafia;
        }

        // Coven
        if ((PlayerControl.LocalPlayer.Data.Role is HexMaster hexMaster && player.HasModifier<HexedModifier>(x => x.Caster == hexMaster.Player))
            || (player.HasModifier<HexedModifier>() && PlayerControl.LocalPlayer.Is(Faction.Coven)))
        {
            color = RoleColors.Coven;
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Illusionist illusionist && player.HasModifier<IllusionedModifier>(x => x.Caster == illusionist.Player))
            || (player.HasModifier<IllusionedModifier>() && PlayerControl.LocalPlayer.Is(Faction.Coven)))
        {
            color = RoleColors.Coven;
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Jinx jinx && player.HasModifier<JinxedModifier>(x => x.Caster == jinx.Player))
            || (player.HasModifier<JinxedModifier>() && PlayerControl.LocalPlayer.Is(Faction.Coven)))
        {
            color = RoleColors.Coven;
        }

        if ((PlayerControl.LocalPlayer.Data.Role is PotionMaster PM && player.HasModifier<BarrieredModifier>(x => x.Caster == PM.Player))
            || (player.HasModifier<BarrieredModifier>(x => x.Caster.Is(Faction.Coven)) && PlayerControl.LocalPlayer.Is(Faction.Coven)))
        {
            color = RoleColors.Coven;
        }
        return color;
    }

    public static string UpdateTargetSymbols(this string name, PlayerControl player, bool hidden = false)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        return name;
    }

    public static string UpdateProtectionSymbols(this string name, PlayerControl player, bool hidden = false)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        return name;
    }

    public static string UpdateAllianceSymbols(this string name, PlayerControl player, bool hidden = false)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        return name;
    }

    public static string UpdateStatusSymbols(this string name, PlayerControl player, bool hidden = false)
    {
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;

        // Town -------------------------------------------------------------------------------------------------------
        if ((PlayerControl.LocalPlayer.Data.Role is Bodyguard bodyguard && player.HasModifier<GuardedModifier>(x => x.Caster == bodyguard.Player))
            || (player.HasModifier<GuardedModifier>() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#06e00c> Ⓖ</color>";
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Catalyst catalyst && player.HasModifier<OverchargedModifier>(x => x.Caster == catalyst.Player))
            || (player.HasModifier<OverchargedModifier>() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#06e00c> Ⓞ</color>";
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Seer seer && player.HasModifier<ComparedModifier>(x => x.Caster == seer.Player))
            || (player.HasModifier<ComparedModifier>() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#06e00c> Ⓒ</color>";
        }

        if (PlayerControl.LocalPlayer.Data.Role is Seer seer2 && (seer2.intuit == player || seer2.gaze == player))
        {
            name += "<color=#06e00c> *</color>";
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Cleric cleric && player.HasModifier<BarrieredModifier>(x => x.Caster == cleric.Player))
            || (player.HasModifier<BarrieredModifier>(x => x.Caster.Is(Faction.Town)) && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#06e00c> Ⓑ</color>";
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Crusader crusader && player.HasModifier<FortifiedModifier>(x => x.Caster == crusader.Player))
            || (player.HasModifier<FortifiedModifier>(x => x.Caster.Is(Faction.Town)) && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#06e00c> Ⓕ</color>";
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Lookout lookout && player.HasModifier<WatchedModifier>(x => x.Caster == lookout.Player))
            || (player.HasModifier<WatchedModifier>(x => x.Caster.Is(Faction.Town)) && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#06e00c> Ⓦ</color>";
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Tracker tracker && player.HasModifier<TrackedModifier>(x => x.Caster == tracker.Player))
            || (player.HasModifier<TrackedModifier>(x => x.Caster.Is(Faction.Town)) && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#06e00c> Ⓣ</color>";
        }

        // Neutral -------------------------------------------------------------------------------------------------------
        if ((PlayerControl.LocalPlayer.Data.Role is Starspawn starspawn && player.HasModifier<IsolatedModifier>(x => x.Caster == starspawn.Player))
            || (player.HasModifier<IsolatedModifier>() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#a4a4f4> Ⓘ</color>";
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Werewolf werewolf && player.HasModifier<TrackedModifier>(x => x.Caster == werewolf.Player))
            || (player.HasModifier<TrackedModifier>(x => x.Caster.IsRole<Werewolf>()) && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#aa6d06> ⓉⓈ</color>";
        }
        if ((PlayerControl.LocalPlayer.Data.Role is Warlock warlock && player.HasModifier<CursedModifier>(x => x.Caster == warlock.Player))
            || (player.HasModifier<CursedModifier>() && PlayerControl.LocalPlayer.Is(Alignment.NeutralApocalypse))
            || (player.HasModifier<CursedModifier>() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#ff004e> Ⓒ</color>";
        }

        // Mafia -------------------------------------------------------------------------------------------------------
        if ((PlayerControl.LocalPlayer.Data.Role is Framer framer && player.HasModifier<FramedModifier>(x => x.Caster == framer.Player))
            || (player.HasModifier<FramedModifier>() && PlayerControl.LocalPlayer.Is(Faction.Mafia))
            || (player.HasModifier<FramedModifier>() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#DD0000> Ⓕ</color>";
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Agent agent && player.HasModifier<WatchedModifier>(x => x.Caster == agent.Player))
            || (player.HasModifier<WatchedModifier>(x => x.Caster.Is(Faction.Mafia)) && PlayerControl.LocalPlayer.Is(Faction.Mafia))
            || (player.HasModifier<WatchedModifier>(x => x.Caster.Is(Faction.Mafia)) && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#DD0000> Ⓢ</color>";
        }

        // Coven -------------------------------------------------------------------------------------------------------
        if ((PlayerControl.LocalPlayer.Data.Role is HexMaster hexMaster && player.HasModifier<HexedModifier>(x => x.Caster == hexMaster.Player))
            || (player.HasModifier<HexedModifier>() && PlayerControl.LocalPlayer.Is(Faction.Coven))
            || (player.HasModifier<HexedModifier>() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#B545FF> Ⓗ</color>";
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Illusionist illusionist && player.HasModifier<IllusionedModifier>(x => x.Caster == illusionist.Player))
            || (player.HasModifier<IllusionedModifier>() && PlayerControl.LocalPlayer.Is(Faction.Coven))
            || (player.HasModifier<IllusionedModifier>() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#B545FF> Ⓘ</color>";
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Jinx jinx && player.HasModifier<JinxedModifier>(x => x.Caster == jinx.Player))
            || (player.HasModifier<JinxedModifier>() && PlayerControl.LocalPlayer.Is(Faction.Coven))
            || (player.HasModifier<JinxedModifier>() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#B545FF> Ⓙ</color>";
        }

        if ((PlayerControl.LocalPlayer.Data.Role is PotionMaster PM && player.HasModifier<BarrieredModifier>(x => x.Caster == PM.Player))
            || (player.HasModifier<BarrieredModifier>(x => x.Caster.Is(Faction.Coven)) && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#B545FF> Ⓑ</color>";
        }

        if ((PlayerControl.LocalPlayer.Data.Role is Wildling wildling && player.HasModifier<WatchedModifier>(x => x.Caster == wildling.Player))
            || (player.HasModifier<WatchedModifier>(x => x.Caster.Is(Faction.Coven)) && PlayerControl.LocalPlayer.Is(Faction.Coven))
            || (player.HasModifier<WatchedModifier>(x => x.Caster.Is(Faction.Coven)) && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#B545FF> Ⓢ</color>";
        }
        return name;
    }
}


/* Status Alphabet

Ⓐ
Ⓑ
Ⓒ
Ⓓ
Ⓔ
Ⓕ
Ⓖ
Ⓗ
Ⓘ
Ⓙ
Ⓚ
Ⓛ
Ⓜ
Ⓝ
Ⓞ
Ⓟ
Ⓠ
Ⓡ
Ⓢ
Ⓣ
Ⓤ
Ⓥ
Ⓦ
Ⓧ
Ⓨ
Ⓩ

*/