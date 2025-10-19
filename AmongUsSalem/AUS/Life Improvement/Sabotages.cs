using System;
using System.Collections;
using AmongUsSalem.Events;
using Il2CppSystem.Runtime.InteropServices;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement;

public static class Sabotages
{
    public static bool AnyActive()
    {
        if (AUSPlugin.InGame())
        {
            if (DayNightMechanic.DayCount >= 1)
            {
                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                var specials = system.specials.ToArray();

                return specials.Any((IActivatable s) => s.IsActive);
            }
        }

        return false;
    }
}

public enum SabEnum
{
    Doors,
    Lights,
    Reactor,
    Oxygen,
    Communications,
    MushroomMixup
}