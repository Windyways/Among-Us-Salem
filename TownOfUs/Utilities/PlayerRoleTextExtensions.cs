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

        // Mafia -------------------------------------------------------------------------------------------------------
        if ((PlayerControl.LocalPlayer.Data.Role is Framer framer && player.HasModifier<FramedModifier>(x => x.Caster == framer.Player))
            || (player.HasModifier<FramedModifier>() && PlayerControl.LocalPlayer.Is(Faction.Mafia))
            || (player.HasModifier<FramedModifier>() && PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !hidden))
        {
            name += "<color=#DD0000> Ⓕ</color>";
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