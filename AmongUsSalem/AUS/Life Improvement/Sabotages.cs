using System;
using System.Collections;
using Il2CppSystem.Runtime.InteropServices;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement;

public static class Sabotages
{
    public static bool AnyActive()
    {
        if (AUSPlugin.InGame())
        {
            if (OnGameStart.SequenceCheck >= 3)
            {
                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                var specials = system.specials.ToArray();

                return specials.Any((IActivatable s) => s.IsActive);
            }
        }

        return false;
    }

    public static void Start()
    {
        Sabotages.AvailableSabotages.Clear();
        if (CheckMap.MapSelected == CheckMap.CurrentMap.Skeld)
        {
            Sabotages.AvailableSabotages.Add(SabEnum.Doors);
            Sabotages.AvailableSabotages.Add(SabEnum.Oxygen);
            Sabotages.AvailableSabotages.Add(SabEnum.Reactor);
            Sabotages.AvailableSabotages.Add(SabEnum.Communications);
            Sabotages.AvailableSabotages.Add(SabEnum.Lights);
        }
        else if (CheckMap.MapSelected == CheckMap.CurrentMap.MiraHQ)
        {
            Sabotages.AvailableSabotages.Add(SabEnum.Communications);
            Sabotages.AvailableSabotages.Add(SabEnum.Reactor);
            Sabotages.AvailableSabotages.Add(SabEnum.Lights);
        }
        else if (CheckMap.MapSelected == CheckMap.CurrentMap.Polus)
        {
            Sabotages.AvailableSabotages.Add(SabEnum.Doors);
            Sabotages.AvailableSabotages.Add(SabEnum.Reactor);
            Sabotages.AvailableSabotages.Add(SabEnum.Communications);
            Sabotages.AvailableSabotages.Add(SabEnum.Lights);
        }
        else if (CheckMap.MapSelected == CheckMap.CurrentMap.Airship)
        {
            Sabotages.AvailableSabotages.Add(SabEnum.Doors);
            Sabotages.AvailableSabotages.Add(SabEnum.Reactor);
            Sabotages.AvailableSabotages.Add(SabEnum.Communications);
            Sabotages.AvailableSabotages.Add(SabEnum.Lights);
        }
        else if (CheckMap.MapSelected == CheckMap.CurrentMap.Fungle)
        {
            Sabotages.AvailableSabotages.Add(SabEnum.Doors);
            Sabotages.AvailableSabotages.Add(SabEnum.Reactor);
            Sabotages.AvailableSabotages.Add(SabEnum.MushroomMixup);
        }
    }

    public static List<SabEnum> AvailableSabotages = new List<SabEnum>();

    // Token: 0x0200026E RID: 622
    public static class TReactor
    {
        // Token: 0x06000A75 RID: 2677 RVA: 0x00048890 File Offset: 0x00046A90
        public static bool Fix()
        {
            bool flag = CheckMap.MapSelected == CheckMap.CurrentMap.Polus;
            if (flag)
            {
                ShipStatus.Instance.RpcUpdateSystem((SystemTypes)21, 16);
            }
            else
            {
                bool flag2 = CheckMap.MapSelected == CheckMap.CurrentMap.Airship;
                if (flag2)
                {
                    ShipStatus.Instance.RpcUpdateSystem((SystemTypes)58, 16);
                    ShipStatus.Instance.RpcUpdateSystem((SystemTypes)58, 17);
                }
                else
                {
                    ShipStatus.Instance.RpcUpdateSystem((SystemTypes)3, 16);
                }
            }
            return false;
        }

        public static void Call(PlayerControl caller)
        {
            bool flag = ShipStatus.Instance == null || Sabotages.AnyActive();
            if (!flag)
            {
                bool flag2 = CheckMap.MapSelected == CheckMap.CurrentMap.Polus;
                if (flag2)
                {
                    ShipStatus.Instance.RpcUpdateSystem((SystemTypes)17, 21);
                    DestroyableSingleton<AchievementManager>.Instance.SabotageCalledLocally();
                    DestroyableSingleton<DebugAnalytics>.Instance.Analytics.SabotageStart((SystemTypes)21);
                }
                else
                {
                    bool flag3 = CheckMap.MapSelected == CheckMap.CurrentMap.Airship;
                    if (flag3)
                    {
                        ShipStatus.Instance.RpcUpdateSystem((SystemTypes)17, 58);
                        DestroyableSingleton<AchievementManager>.Instance.SabotageCalledLocally();
                    }
                    else
                    {
                        ShipStatus.Instance.RpcUpdateSystem((SystemTypes)17, 3);
                        DestroyableSingleton<AchievementManager>.Instance.SabotageCalledLocally();
                        DestroyableSingleton<DebugAnalytics>.Instance.Analytics.SabotageStart((SystemTypes)3);
                    }
                }
                
                /*if (caller.IsRole<Culverin>())
                {
                    Role.GetRole<Culverin>(caller).GainGold();
                }*/
            }
        }
    }

    // Token: 0x0200026F RID: 623
    public static class TOxygen
    {
        // Token: 0x06000A78 RID: 2680 RVA: 0x000489E0 File Offset: 0x00046BE0
        public static bool Fix()
        {
            if (CheckMap.MapSelected == CheckMap.CurrentMap.Submerged)
            {
                /*SubmergedCompatibility.RepairOxygen();
                Utils.Rpc(new object[]
                {
                        CustomRPC.SubmergedFixOxygen,
                        PlayerControl.LocalPlayer.NetId
                });*/
            }
            else
            {
                ShipStatus.Instance.RpcUpdateSystem((SystemTypes)8, 16);
            }
            return false;
        }

        // Token: 0x06000A79 RID: 2681 RVA: 0x00048A44 File Offset: 0x00046C44
        public static void Call(PlayerControl caller)
        {
            bool flag = ShipStatus.Instance == null || Sabotages.AnyActive();
            if (!flag)
            {
                ShipStatus.Instance.RpcUpdateSystem((SystemTypes)17, 8);
                DestroyableSingleton<AchievementManager>.Instance.SabotageCalledLocally();
                DestroyableSingleton<DebugAnalytics>.Instance.Analytics.SabotageStart((SystemTypes)8);
                
                /*if (caller.IsRole<Culverin>())
                {
                    Role.GetRole<Culverin>(caller).GainGold();
                }*/
            }
        }
    }

    // Token: 0x02000270 RID: 624
    public static class TCommunications
    {
        // Token: 0x06000A7B RID: 2683 RVA: 0x00048AB8 File Offset: 0x00046CB8
        public static bool Fix()
        {
            bool flag = CheckMap.MapSelected == CheckMap.CurrentMap.MiraHQ;
            if (flag)
            {
                ShipStatus.Instance.RpcUpdateSystem((SystemTypes)14, 16);
                ShipStatus.Instance.RpcUpdateSystem((SystemTypes)14, 17);
            }
            else
            {
                ShipStatus.Instance.RpcUpdateSystem((SystemTypes)14, 0);
            }
            return false;
        }

        // Token: 0x06000A7C RID: 2684 RVA: 0x00048B0C File Offset: 0x00046D0C
        public static void Call()
        {
            bool flag = ShipStatus.Instance == null || Sabotages.AnyActive();
            if (!flag)
            {
                ShipStatus.Instance.RpcUpdateSystem((SystemTypes)17, 14);
                DestroyableSingleton<AchievementManager>.Instance.SabotageCalledLocally();
                DestroyableSingleton<DebugAnalytics>.Instance.Analytics.SabotageStart((SystemTypes)14);
            }
        }
    }

    // Token: 0x02000271 RID: 625
    public static class TMushroomMixup
    {
        // Token: 0x06000A7E RID: 2686 RVA: 0x00048B6C File Offset: 0x00046D6C
        public static bool Fix()
        {
            MushroomMixupSabotageSystem mushroomMixup = ShipStatus.Instance.Systems[(SystemTypes)57].Cast<MushroomMixupSabotageSystem>();
            mushroomMixup.currentSecondsUntilHeal = 0.1f;
            return false;
        }

        // Token: 0x06000A7F RID: 2687 RVA: 0x00048BA4 File Offset: 0x00046DA4
        public static void Call()
        {
            bool flag = ShipStatus.Instance == null || Sabotages.AnyActive();
            if (!flag)
            {
                ShipStatus.Instance.RpcUpdateSystem((SystemTypes)17, 57);
                DestroyableSingleton<AchievementManager>.Instance.SabotageCalledLocally();
            }
        }
    }

    // Token: 0x02000272 RID: 626
    public static class TLights
    {
        // Token: 0x06000A81 RID: 2689 RVA: 0x00048BF0 File Offset: 0x00046DF0
        public static bool Fix()
        {
            SwitchSystem lights = ShipStatus.Instance.Systems[(SystemTypes)7].Cast<SwitchSystem>();
            /*Utils.Rpc(new object[]
            {
                    CustomRPC.FixLights
            });*/
            lights.ActualSwitches = lights.ExpectedSwitches;
            return false;
        }

        // Token: 0x06000A82 RID: 2690 RVA: 0x00048C3C File Offset: 0x00046E3C
        public static void Call()
        {
            bool flag = ShipStatus.Instance == null || Sabotages.AnyActive();
            if (!flag)
            {
                ShipStatus.Instance.RpcUpdateSystem((SystemTypes)17, 7);
                DestroyableSingleton<AchievementManager>.Instance.SabotageCalledLocally();
                DestroyableSingleton<DebugAnalytics>.Instance.Analytics.SabotageStart((SystemTypes)7);
            }
        }
    }

    // Token: 0x02000273 RID: 627
    public static class TDoors
    {
        // Token: 0x06000A84 RID: 2692 RVA: 0x00048C98 File Offset: 0x00046E98
        public static void Start()
        {
            Sabotages.TDoors.AvailableDoorRooms.Clear();
            bool flag = CheckMap.MapSelected == CheckMap.CurrentMap.Skeld;
            if (flag)
            {
                Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)2);
                Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)1);
                Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)13);
                Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)4);
                Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)10);
                Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)11);
                Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)7);
            }
            else
            {
                bool flag2 = CheckMap.MapSelected == CheckMap.CurrentMap.Polus;
                if (flag2)
                {
                    Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)7);
                    Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)8);
                    Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)21);
                    Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)23);
                    Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)1);
                    Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)12);
                    Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)14);
                }
                else
                {
                    bool flag3 = CheckMap.MapSelected == CheckMap.CurrentMap.Airship;
                    if (flag3)
                    {
                        Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)40);
                        Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)45);
                        Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)42);
                        Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)46);
                        Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)33);
                        Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)14);
                    }
                    else
                    {
                        bool flag4 = CheckMap.MapSelected == CheckMap.CurrentMap.Fungle;
                        if (flag4)
                        {
                            Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)1);
                            Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)14);
                            Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)21);
                            Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)52);
                            Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)49);
                            Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)3);
                            Sabotages.TDoors.AvailableDoorRooms.Add((SystemTypes)33);
                        }
                    }
                }
            }
        }

        // Token: 0x06000A87 RID: 2695 RVA: 0x00048E64 File Offset: 0x00047064
        public static void Call(SystemTypes room)
        {
            bool flag = ShipStatus.Instance == null;
            if (!flag)
            {
                ShipStatus.Instance.RpcCloseDoorsOfType(room);
                DestroyableSingleton<DebugAnalytics>.Instance.Analytics.SabotageStart((SystemTypes)16);
            }
        }

        // Token: 0x04000708 RID: 1800
        public static List<SystemTypes> AvailableDoorRooms = new List<SystemTypes>();

        // Token: 0x04000709 RID: 1801
        public static float Cooldown = 20f;

        // Token: 0x0400070A RID: 1802
        public static bool DoorsSabotaged;
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