using InnerNet;
using RCoroutines = Reactor.Utilities.Coroutines;
using System.Collections;
using UnityEngine;
using HarmonyLib;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using AmongUsSalem.Utilities;
using AmongUsSalem.LifeImprovement.MCI.SmartMCI;

namespace AmongUsSalem.LifeImprovement.MCI;

public static class InstanceControlPatches
{
    internal static Dictionary<int, ClientData> Clients = new();
    internal static Dictionary<byte, int> PlayerClientIDs = new();
    public static PlayerControl CurrentPlayerInPower { get; private set; }
    public static int AvailableId()
    {
        for (var i = 1; i < 128; i++)
        {
            if (!AmongUsClient.Instance.allClients.ToArray().Any(x => x.Id == i) && !Clients.ContainsKey(i) && PlayerControl.LocalPlayer.OwnerId != i)
                return i;
        }

        return -1;
    }

    public static void SwitchTo(byte playerId)
    {
        var savedPlayerId = PlayerControl.LocalPlayer.PlayerId;
        PlayerControl savedPlayer = MiscUtils.PlayerById(savedPlayerId)!;
        var savedPosition = savedPlayer.transform.position;

        PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(PlayerControl.LocalPlayer.transform.position);
        PlayerControl.LocalPlayer.moveable = false;

        var light = PlayerControl.LocalPlayer.lightSource;

        //Setup new player
        var newPlayer =  MiscUtils.PlayerById(playerId);

        if (newPlayer == null) return;

        var newPosition = newPlayer.transform.position;

        PlayerControl.LocalPlayer = newPlayer;
        PlayerControl.LocalPlayer.lightSource = light;
        PlayerControl.LocalPlayer.moveable = true;

        AmongUsClient.Instance.ClientId = PlayerControl.LocalPlayer.OwnerId;
        AmongUsClient.Instance.HostId = PlayerControl.LocalPlayer.OwnerId;

        DestroyableSingleton<HudManager>.Instance.SetHudActive(true);
        DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(!newPlayer.Data.IsDead);

        DestroyableSingleton<HudManager>.Instance.KillButton.transform.parent.GetComponentsInChildren<Transform>().ToList().ForEach(x =>
        {
            if (x.gameObject.name == "KillButton(Clone)")
            {
                UnityEngine.Object.Destroy(x.gameObject);
            }

        });

        DestroyableSingleton<HudManager>.Instance.KillButton.transform.GetComponentsInChildren<Transform>().ToList().ForEach(x =>
        {
            if (x.gameObject.name == "KillTimer_TMP(Clone)")
            {
                UnityEngine.Object.Destroy(x.gameObject);
            }

        });

        DestroyableSingleton<HudManager>.Instance.transform.GetComponentsInChildren<Transform>().ToList().ForEach(x =>
        {
            if (x.gameObject.name == "KillButton(Clone)")
            {
                UnityEngine.Object.Destroy(x.gameObject);
            }

        });

        var modsTab = MiraAPI.Modifiers.ModifierDisplay.ModifierDisplayComponent.Instance;
        if (modsTab != null && !modsTab.IsOpen && PlayerControl.LocalPlayer.GetModifiers<GameModifier>().Any(x => !x.HideOnUi && x.GetDescription() != string.Empty))
        {
            modsTab.ToggleTab();
        }

        light.transform.SetParent(newPlayer.transform);
        light.transform.localPosition = newPlayer.Collider.offset;
        Camera.main.GetComponent<FollowerCamera>().SetTarget(newPlayer);
        newPlayer.MyPhysics.ResetMoveState(true);
        KillAnimation.SetMovement(newPlayer, true);
        newPlayer.MyPhysics.inputHandler.enabled = true;
        CurrentPlayerInPower = newPlayer;

        newPlayer.NetTransform.SnapTo(newPosition);
        savedPlayer.NetTransform.SnapTo(savedPosition);

        if (MeetingHud.Instance)
        {
            if (newPlayer.Data.IsDead)
                MeetingHud.Instance.SetForegroundForDead();
            else
                MeetingHud.Instance.SetForegroundForAlive();
        }
    }

    public static void CleanUpLoad()
    {
        if (GameData.Instance.AllPlayers.Count == 1)
        {
            Clients.Clear();
            PlayerClientIDs.Clear();
        }
    }

    public static void CreatePlayerInstance()
    {
        RCoroutines.Start(_CreatePlayerInstanceEnumerator());
    }

    internal static IEnumerator _CreatePlayerInstanceEnumerator()
    {
        var sampleId = AvailableId();
        var sampleC = new ClientData(sampleId, $"Bot-{sampleId}", new()
        {
            Platform = Platforms.StandaloneWin10,
            PlatformName = "Bot"
        }, 1, "", "robotmodeactivate");

        AmongUsClient.Instance.GetOrCreateClient(sampleC);
        yield return AmongUsClient.Instance.CreatePlayer(sampleC);

        GetRandomPlayer(sampleC.Character, sampleId);

        Clients.Add(sampleId, sampleC);
        PlayerClientIDs.Add(sampleC.Character.PlayerId, sampleId);
        sampleC.Character.MyPhysics.ResetAnimState();
        sampleC.Character.MyPhysics.ResetMoveState();

        yield return sampleC.Character.MyPhysics.CoSpawnPlayer(LobbyBehaviour.Instance);
        yield break;
    }
    public static void RemovePlayer(byte id)
    {
        if (id == 0)
            return;

        var clientId = Clients.FirstOrDefault(x => x.Value.Character.PlayerId == id).Key;
        Clients.Remove(clientId, out var outputData);
        PlayerClientIDs.Remove(id);
        AmongUsClient.Instance.RemovePlayer(clientId, DisconnectReasons.Custom);
        AmongUsClient.Instance.allClients.Remove(outputData);
    }

    public static void RemoveAllPlayers()
    {
        PlayerClientIDs.Keys.ToList().ForEach(RemovePlayer);
        SwitchTo(0);
        Keyboard_Joystick.ControllingFigure = 0;
    }

    public static void SetForegroundForAlive(this MeetingHud __instance)
    {
        __instance.amDead = false;
        __instance.SkipVoteButton.gameObject.SetActive(true);
        __instance.SkipVoteButton.AmDead = false;
        __instance.Glass.gameObject.SetActive(false);
        if (CacheMeetingSprite.Cache) __instance.Glass.sprite = CacheMeetingSprite.Cache;
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.SetForegroundForDead))]
    public static class CacheMeetingSprite
    {
        public static Sprite Cache;
        public static void Prefix(MeetingHud __instance) => Cache ??= __instance.Glass.sprite;
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
    public static class SameVoteAll
    {
        public static void Postfix(MeetingHud __instance, ref byte suspectStateIdx)
        {
            if (!Debugger.IsDebuggerActive)
                return;

            if (true)
            {
                foreach (PlayerVoteArea playerVoteArea in MeetingHud.Instance.playerStates)
                {
                    playerVoteArea.UnsetVote();
                    MeetingHud.Instance.ClearVote();
                }
            }

            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (!player.HasDied())
                {
                    CalculatedVoting.SetKillerContagious(player);
                    CalculatedVoting.SetEvidenceAgainst(player);
                }

                if (!player.HasDied())
                {
                    if (player.Is(Faction.Town)) CalculatedVoting.RandomTownVoting(player, __instance);
                    else if (player.Is(Faction.Mafia)) CalculatedVoting.RandomMafiaVoting(player, __instance);
                    else if (player.Is(Faction.Neutral)) CalculatedVoting.RandomNeutralVoting(player, __instance);
                    else if (player.Is(Faction.Coven)) CalculatedVoting.RandomCovenVoting(player, __instance);
                }
            }
        }
    }

    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    public static class OnLobbyStart
    {
        public static void Postfix()
        {
            if (Debugger.IsDebuggerActive && AUSPlugin.Persistence && Clients.Count != 0)
            {
                var count = Clients.Count;
                Clients.Clear();
                PlayerClientIDs.Clear();

                for (var i = 0; i < count; i++)
                    CreatePlayerInstance();
            }
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGameHost))]
    public static class OnGameStart
    {
        public static void Prefix(AmongUsClient __instance)
        {
            if (!Debugger.IsDebuggerActive) return;

            foreach (var p in __instance.allClients)
            {
                p.IsReady = true;
                p.Character.gameObject.GetComponent<DummyBehaviour>().enabled = false;
            }
        }
    }

    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
    public static class AirshipSpawn
    {
        public static void Postfix(SpawnInMinigame __instance)
        {
            if (!Debugger.IsDebuggerActive)
                return;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!AUSPlugin.IsBot.Contains(player))
                {
                    var random = UnityEngine.Random.Range(0, __instance.Locations.Count);
                    player.gameObject.SetActive(true);
                    player.NetTransform.RpcSnapTo(__instance.Locations[random].Location);
                }

                if (!player.Data.PlayerName.Contains(AUSPlugin.RobotName))
                    continue;

                var rand = UnityEngine.Random.Range(0, __instance.Locations.Count);
                player.gameObject.SetActive(true);
                player.NetTransform.RpcSnapTo(__instance.Locations[rand].Location);
            }
        }
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class CountdownPatch
    {
        public static void Prefix(GameStartManager __instance) 
        {
            if (!Input.GetKeyDown(KeyCode.LeftShift))
                return;
        
            __instance.countDownTimer = 0;
            if (Reactor_Coroutines._ourCoroutineStore.Any(x => x.Value != null)) __instance.countDownTimer = 1;
        }
    }

    // Token: 0x06000CB0 RID: 3248 RVA: 0x0005F81C File Offset: 0x0005DA1C
    public static void GetRandomPlayer(PlayerControl bot, int sampleId)
    {
        if (bot.PlayerId == 1)
        {
            byte color = (byte)((int)bot.PlayerId % Palette.PlayerColors.Length);
            bot.SetColor(33);
            bot.SetName("Crim");
            bot.SetHat("hats0394", bot.Data.DefaultOutfit.ColorId);
            bot.SetPet("pet_EmptyPet");
            bot.SetSkin("skin_Hazmat-Whiteskin", (int)color);
            bot.SetVisor("visor_EmptyVisor", (int)color);
        }
        else if (bot.PlayerId == 2)
        {
            byte color2 = (byte)((int)bot.PlayerId % Palette.PlayerColors.Length);
            bot.SetColor(14);
            bot.SetName("CoolMafiay");
            bot.SetHat("hat_bearyCold", bot.Data.DefaultOutfit.ColorId);
            bot.SetPet("pet_EmptyPet");
            bot.SetSkin("skin_MilitarySnowskin", (int)color2);
            bot.SetVisor("visor_chimkin", (int)color2);
        }
        else if (bot.PlayerId == 3)
        {
            byte color3 = (byte)((int)bot.PlayerId % Palette.PlayerColors.Length);
            bot.SetColor(15);
            bot.SetName("Corpsy");
            bot.SetHat("hat_hl_gura", bot.Data.DefaultOutfit.ColorId);
            bot.SetPet("pet_Creb");
            bot.SetSkin("skin_rhm", (int)color3);
            bot.SetVisor("visor_Blush", (int)color3);
        }
        else if (bot.PlayerId == 4)
        {
            byte color4 = (byte)((int)bot.PlayerId % Palette.PlayerColors.Length);
            bot.SetColor(14);
            bot.SetName("Kitty");
            bot.SetHat("hats0213", bot.Data.DefaultOutfit.ColorId);
            bot.SetPet("pet_EmptyPet");
            bot.SetSkin("skin_SuitB", (int)color4);
            bot.SetVisor("visor_EmptyVisor", (int)color4);
        }
        else if (bot.PlayerId == 5)
        {
            byte color5 = (byte)((int)bot.PlayerId % Palette.PlayerColors.Length);
            bot.SetColor(29);
            bot.SetName("E");
            bot.SetHat("hat_lny_cat", bot.Data.DefaultOutfit.ColorId);
            bot.SetPet("pet_EmptyPet");
            bot.SetSkin("skin_w21_elf", (int)color5);
            bot.SetVisor("visor_lny_tiger", (int)color5);
        }
        else if (bot.PlayerId == 6)
        {
            byte color6 = (byte)((int)bot.PlayerId % Palette.PlayerColors.Length);
            bot.SetColor(10);
            bot.SetName("Reachel");
            bot.SetHat("stream0015", bot.Data.DefaultOutfit.ColorId);
            bot.SetPet("pet_napstamate");
            bot.SetSkin("skin_greedygrampaskin", (int)color6);
            bot.SetVisor("visor_Stickynote_Cyan", (int)color6);
        }
        else if (bot.PlayerId == 7)
        {
            byte color7 = (byte)((int)bot.PlayerId % Palette.PlayerColors.Length);
            bot.SetColor(13);
            bot.SetName("Cake");
            bot.SetHat("hats0246", bot.Data.DefaultOutfit.ColorId);
            bot.SetPet("pet_Strawb");
            bot.SetSkin("skin_None", (int)color7);
            bot.SetVisor("visor_Blush", (int)color7);
        }
        else if (bot.PlayerId == 8)
        {
            byte color8 = (byte)((int)bot.PlayerId % Palette.PlayerColors.Length);
            bot.SetColor(15);
            bot.SetName("Shadoww");
            bot.SetHat("hats0377", bot.Data.DefaultOutfit.ColorId);
            bot.SetPet("pet_EmptyPet");
            bot.SetSkin("skin_rhm", (int)color8);
            bot.SetVisor("visor_claws_knife", (int)color8);
        }
        else if (bot.PlayerId == 9)
        {
            byte color9 = (byte)((int)bot.PlayerId % Palette.PlayerColors.Length);
            bot.SetColor(34);
            bot.SetName("Le Killer");
            bot.SetHat("hats0111", bot.Data.DefaultOutfit.ColorId);
            bot.SetPet("pet_napstamate");
            bot.SetSkin("skin_SuitB", (int)color9);
            bot.SetVisor("visor_henry", (int)color9);
        }
        else if (bot.PlayerId == 10)
        {
            byte color10 = (byte)((int)bot.PlayerId % Palette.PlayerColors.Length);
            bot.SetColor(6);
            bot.SetName("WillowXD");
            bot.SetHat("hats0124", bot.Data.DefaultOutfit.ColorId);
            bot.SetPet("pet_EmptyPet");
            bot.SetSkin("skin_Capt", (int)color10);
            bot.SetVisor("visor_pk01_DumStickerVisor", (int)color10);
        }
        else
        {
            bot.SetName($"Bot #{bot.PlayerId}");
            bot.SetSkin(HatManager.Instance.allSkins[UnityEngine.Random.Range(0, HatManager.Instance.allSkins.Count)].ProdId, 0);
            bot.SetNamePlate(HatManager.Instance.allNamePlates[UnityEngine.Random.RandomRangeInt(0, HatManager.Instance.allNamePlates.Count)].ProdId);
            bot.SetPet(HatManager.Instance.allPets[UnityEngine.Random.RandomRangeInt(0, HatManager.Instance.allPets.Count)].ProdId);
            bot.SetColor(UnityEngine.Random.Range(0, Palette.PlayerColors.Length));
            bot.SetHat(HatManager.Instance.allHats[UnityEngine.Random.Range(0, HatManager.Instance.allHats.Count)].ProdId, 0);
            bot.SetVisor(HatManager.Instance.allVisors[UnityEngine.Random.Range(0, HatManager.Instance.allVisors.Count)].ProdId, 0);

        }
        AUSPlugin.IsBot.Add(bot);
    }
}
