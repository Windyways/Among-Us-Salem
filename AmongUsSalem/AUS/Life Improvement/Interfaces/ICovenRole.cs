using System.Globalization;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using AmongUsSalem.Utilities;

namespace AmongUsSalem.Roles;

public interface ICovenRole : ICustomRole
{
    NecronomiconPriority NecronomiconPriority => NecronomiconPriority.None;
    bool Necronomicon { get; set; }
    PlayerControl Player { get; }

    void OnDeath()
    {
        CovenNecronomiconMechanic.OnRoleDeath(this);
    }
}