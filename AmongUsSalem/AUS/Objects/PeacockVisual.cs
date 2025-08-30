using System.Collections;
using ObjectWorkshop.Patches;
using UnityEngine;
using Color = UnityEngine.Color;

namespace ObjectWorkshop.LifeImprovement.Objects;

[RegisterInIl2Cpp]
public class PeacockVisual(IntPtr ptr) : MonoBehaviour(ptr)
{
    public byte id;
    public PlayerControl Owner;
    public PlayerControl associate;
    public SpriteRenderer myRend;
    
    public static List<PlayerControl> PalePlayers = new();
    public static List<PlayerControl> ImmunePlayers = new();

    public static List<PeacockVisual> AllPeacockVisuals = new List<PeacockVisual>();
    public static bool PlayerIsPale(PlayerControl player) => PalePlayers.Contains(player);

    public void Start()
    {
        //Lockdown.AvailableObjects.Add(gameObject);

        myRend = gameObject.GetComponent<SpriteRenderer>();

        DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Bounce(transform, 0.7f, 0.45f));
        DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.ColorFade(myRend, new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f), 0.4f));

        id = GetAvailableId();
        AllPeacockVisuals.Add(this);
    }

    private void OnDestroy()
    {
        AllPeacockVisuals.Remove(this);
    }

    private void Update()
    {
        UpdateColor();
        if (myRend != null) myRend.flipX = Owner.cosmetics?.FlipX ?? false;

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (VentPatches.InVision(player))
            {
                ImmobilizePlayers(player);
            }
            else MobilizePlayers(player);
        }
    }

    public void UpdateColor()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        /*if (PlayerControl.LocalPlayer.IsUnderground())
        {
            sr.color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            sr.color = new Color(1f, 1f, 1f, 1f);
        }*/
        sr.color = new Color(1f, 1f, 1f, 1f);
    }

    public void ImmobilizePlayers(PlayerControl player)
    {
        if (PalePlayers.Contains(player))
            return;

        if (player.IsRole<UndeadReaper>() && !UndeadReaper.ReapersAlive())
            return;

        player.Immobilize();
        PalePlayers.Add(player);

        if (/*!player.IsSpider() && !player.IsPeacock() && !player.IsSpecterInvisible() && */!player.IsRole<UndeadReaper>())
        {
            
        }
        else
        {
            ApplyPaleVisual(player, true);
        }

        // Crewmates don’t trigger the flash
        if (player.IsFaction(Faction.Crewmate))
            return;

        if (PlayerControl.LocalPlayer == associate) Coroutines.Start(MiscUtils.CoFlash(OWColors.Peacock, 1f, 0.3f));
    }

    public void MobilizePlayers(PlayerControl player, bool removeFromList = false)
    {
        if (!PalePlayers.Contains(player))
            return;

        player.Mobilize();
        if (removeFromList) PalePlayers.Remove(player);

        if (/*!player.IsSpider() && !player.IsPeacock() && !player.IsSpecterInvisible() && */!player.IsRole<UndeadReaper>())
        {
            // Unmorph or smth.
        }
        else
        {
            ApplyPaleVisual(player, false);
        }
    }

    public void ForceMobilePlayers()
    {
        foreach (var player in PalePlayers.Take(2))
        {
            MobilizePlayers(player);
            ImmunePlayers.Add(player);
        }
        PalePlayers.Clear();
    }

    /// <summary>
    /// Applies visual changes for Spider/Reaper when immobilized/unimmobilized.
    /// </summary>
    private static void ApplyPaleVisual(PlayerControl player, bool makePale)
    {
        Color targetColor = makePale ? Color.white : new Color(1f, 1f, 1f, 1f);

        /*if (player.IsSpider() && SpiderHandler.CurrentSpider != null)
        {
            SpiderHandler.CurrentSpider.GetComponent<SpriteRenderer>().color = targetColor;
        }
        else */
        if (player.IsRole<UndeadReaper>())
        {
            var reaperVisual = ReaperVisual.GetObjectByOwner(player);
            var sr = reaperVisual.gameObject.GetComponent<SpriteRenderer>();
            sr.color = targetColor;
        }
    }

    #region Global
    #endregion
    public static void Begin(Peacock peacock)
    {
        var player = peacock.Player;

        var obj = new GameObject("PeacockVisual");
        obj.transform.SetParent(player.transform);
        obj.AddSpriteRenderer(OWAssets.PeacockVisual.LoadAsset(), 0, 100, player.transform.position, ObjectExtentions.fullColor(), Vector3.one);

        var peacockVisual = obj.AddComponent<PeacockVisual>();
        peacockVisual.Owner = player;
        peacockVisual.associate = peacock.Associate;
    }

    public static byte GetAvailableId()
    {
        byte id = 0;
        while (AllPeacockVisuals.Any(x => x.id == id))
        {
            id++;
        }
        return id;
    }

    public static void DestroyAll()
    {
        foreach (var clock in AllPeacockVisuals)
        {
            AllPeacockVisuals.Remove(clock);
            Destroy(clock.gameObject);
        }
    }

    public static PeacockVisual GetObjectByOwner(PlayerControl player)
    {
        foreach (var peacockVisual in AllPeacockVisuals)
        {
            if (peacockVisual.Owner == player) return peacockVisual;
        }
        return null;
    }

    public static void CleanUp()
    {
        AllPeacockVisuals.Clear();
    }
}