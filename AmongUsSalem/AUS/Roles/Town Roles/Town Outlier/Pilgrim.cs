using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using Color = UnityEngine.Color;

namespace AmongUsSalem.Roles;

#region Pilgrim
#endregion
public sealed class Pilgrim(IntPtr cppPtr) 
    : CrewmateRole(cppPtr), IAUSRole, IWikiDiscoverable
{
    public string RoleName => TouLocale.Get(TouNames.Pilgrim, "Pilgrim");
    public string revealText => "placeholder.";
    public string RoleDescription => "Placeholder.";
    public string RoleLongDescription => RoleDescription;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    public Faction RoleFaction => Faction.Town;
    public Color RoleColor => AUSColors.Town;
    public Alignment Alignment => Alignment.TownOutlier;
    public Attack Attack => Attack.None;
    public Defense Defense => Defense.None;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = AUSAssets.PilgrimRoleCard,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return IAUSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "<color=#06e00c>Pilgram</color>" +
            "\n<color=#fdbc00>Faction:</color> <color=#06e00c>Town</color>" +
            "\n<color=#fdbc00>Sub-alignment:</color> <color=#06e00c>Town</color> <color=#1e45d4>Outlier</color>" +
            "\n<color=#fdbc00>Goal:</color> Hang every criminal and evildoer." +
            $"\n\nAttributes:" +
            "\nNone." +
            MiscUtils.AppendOptionsText(GetType());
    }
}