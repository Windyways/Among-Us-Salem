using System;
using System.Collections;
using Il2CppSystem.Runtime.InteropServices;
using UnityEngine;

namespace AmongUsSalem.LifeImprovement;

public static class Statistics
{
    public static List<PlayerControl> IsTrespassing = new List<PlayerControl>();
    public static List<PlayerControl> HasMurder = new List<PlayerControl>();

    [MethodRpc((uint)AUSRpc.AddTrespassing, SendImmediately = true)]
    public static void RpcAddTrespassing(PlayerControl visitor)
    {
        IsTrespassing.Add(visitor);
    }
    
    [MethodRpc((uint)AUSRpc.AddMurder, SendImmediately = true)]
    public static void RpcAddMurder(PlayerControl visitor)
    {
        HasMurder.Add(visitor);
    }
}