namespace TownOfUs.Roles;

public static class TouRoleGroups
{
    public static RoleOptionsGroup TI { get; } = new("Town Investigative Roles", RoleColors.Town);
    public static RoleOptionsGroup TE { get; } = new("Town Executive Roles", RoleColors.Town);
    public static RoleOptionsGroup TG { get; } = new("Town Government Roles", RoleColors.Town);
    public static RoleOptionsGroup TK { get; } = new("Town Killing Roles", RoleColors.Town);
    public static RoleOptionsGroup TO { get; } = new("Town Outlier Roles", RoleColors.Town);
    public static RoleOptionsGroup TP { get; } = new("Town Protective Roles", RoleColors.Town);
    public static RoleOptionsGroup TS { get; } = new("Town Support Roles", RoleColors.Town);

    public static RoleOptionsGroup NA { get; } = new("Neutral Apocalypse Roles", RoleColors.Apocalypse);
    public static RoleOptionsGroup NB { get; } = new("Neutral Benign Roles", RoleColors.Neutral);
    public static RoleOptionsGroup NC { get; } = new("Neutral Chaos Roles", RoleColors.Neutral);
    public static RoleOptionsGroup NE { get; } = new("Neutral Evil Roles", RoleColors.Neutral);
    public static RoleOptionsGroup NK { get; } = new("Neutral Killing Roles", RoleColors.Neutral);
    public static RoleOptionsGroup NO { get; } = new("Neutral Outlier Roles", RoleColors.Neutral);
    public static RoleOptionsGroup NP { get; } = new("Neutral Pariah Roles", RoleColors.Neutral);

    public static RoleOptionsGroup MD { get; } = new("Mafia Deception Roles", RoleColors.Mafia);
    public static RoleOptionsGroup MK { get; } = new("Mafia Killing Roles", RoleColors.Mafia);
    public static RoleOptionsGroup MS { get; } = new("Mafia Support Roles", RoleColors.Mafia);

    public static RoleOptionsGroup CD { get; } = new("Coven Deception Roles", RoleColors.Coven);
    public static RoleOptionsGroup CE { get; } = new("Coven Evil Roles", RoleColors.Coven);
    public static RoleOptionsGroup CK { get; } = new("Coven Killing Roles", RoleColors.Coven);
    public static RoleOptionsGroup CO { get; } = new("Coven Outlier Roles", RoleColors.Coven);
    public static RoleOptionsGroup CPow { get; } = new("Coven Power Roles", RoleColors.Coven);
    public static RoleOptionsGroup CU { get; } = new("Coven Utility Roles", RoleColors.Coven);
}