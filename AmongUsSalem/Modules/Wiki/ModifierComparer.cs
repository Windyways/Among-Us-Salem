using MiraAPI.Modifiers;

namespace TownOfUs.Modules.Wiki;

public class ModifierComparer(IEnumerable<uint> activeModifiers) : IComparer<BaseModifier>
{
    public int Compare(BaseModifier? x, BaseModifier? y)
    {
        if (x == null && y == null)
        {
            return 0;
        }

        if (x == null)
        {
            return -1;
        }

        if (y == null)
        {
            return 1;
        }

        var xActive = activeModifiers.Contains(x.TypeId);
        var yActive = activeModifiers.Contains(y.TypeId);

        if (xActive && !yActive)
        {
            return -1;
        }

        if (!xActive && yActive)
        {
            return 1;
        }

        return string.Compare(x.ModifierName, y.ModifierName, StringComparison.OrdinalIgnoreCase);
    }
}