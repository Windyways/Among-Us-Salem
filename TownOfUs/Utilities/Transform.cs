using UnityEngine;

namespace TownOfUs.Utilities;

public static class TransformExtensions
{
    public static Transform FindRecursive(this Transform self, string exactName)
    {
        return self.FindRecursive(child => child.name == exactName);
    }

    public static Transform FindRecursive(this Transform self, Func<Transform, bool> selector)
    {
        for (var i = 0; i < self.childCount; i++)
        {
            var child = self.GetChild(i);
            if (selector(child))
            {
                return child;
            }

            var finding = child.FindRecursive(selector);

            if (finding != null)
            {
                return finding;
            }
        }

        return null!;
    }
}