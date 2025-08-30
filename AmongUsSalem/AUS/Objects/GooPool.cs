using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

[RegisterInIl2Cpp]
public class GooPool(IntPtr ptr) : MonoBehaviour(ptr)
{
    public byte id;
    public PlayerControl Owner;
    public SpriteRenderer myRend;
    public PlayerControl nearbyTarget;
    public PlayerControl victim;
    public bool killedNearbyPlayer;
    public float returnTimer;

    public void Start()
    {
        myRend = gameObject.GetComponent<SpriteRenderer>();
        id = GetAvailableId();

        // This fades it in maybe?
        DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.ColorFade(myRend, ObjectExtentions.noColor(), ObjectExtentions.fullColor(), 3f));

        AllGooPools.Add(this);
    }

    private void OnDestroy()
    {
        AllGooPools.Remove(this);
    }

    public void UpdateColor()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        sr.color = new Color(1f, 1f, 1f, 1f);
        /*if (PlayerControl.LocalPlayer.IsUnderground())
        {
            sr.color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            sr.color = new Color(1f, 1f, 1f, 1f);
        }*/
    }

    private void Update()
    {
        UpdateColor();
        nearbyTarget = FindTarget();

        if (killedNearbyPlayer && Owner.AmOwner)
        {
            gameObject.SetCameraToObject(Owner);
            returnTimer -= Time.deltaTime;

            if (returnTimer <= 0) 
            {
                killedNearbyPlayer = false;
                
                LightSource light = Owner.lightSource;
                light.transform.SetParent(Owner.transform);
                light.transform.localPosition = Owner.Collider.offset;
            }
        }
    }

    public PlayerControl FindTarget()
    {
        float killRadius = 0.75f;
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            if (!player.HasDied()/* || player.IsInvincible() || player.IsUnderground()*/ && !player.IsFaction(Faction.Infiltrator, true))
            {
                float dist = Vector2.Distance(transform.position, player.transform.position);
                if (dist <= killRadius)
                {
                    return player;
                }
            }
        }
        return null;
    }

    #region Global
    #endregion
    public static void Begin(PlayerControl player, PlayerControl target)
    {
        GameObject gameObject = new GameObject("GooPool");
        gameObject.AddSpriteRenderer(OWAssets.GooPool.LoadAsset(), 0, 100, target.transform.position,
            new Color(0.2f, 0.2f, 0.2f, 1), Vector3.one);

        GooPool goopool = gameObject.AddComponent<GooPool>();
        goopool.Owner = player;
        goopool.victim = target;
    }

    public static byte GetAvailableId()
    {
        byte id = 0;
        while (AllGooPools.Any(x => x.id == id))
        {
            id++;
        }
        return id;
    }

    public static void DestroyAll()
    {
        foreach (GooPool goopools in AllGooPools)
        {
            Destroy(goopools.gameObject);
        }
        AllGooPools.Clear();
    }

    public static GooPool GetObjectByPlayer(PlayerControl player)
    {
        foreach (GooPool goopool in AllGooPools)
        {
            if (goopool.Owner == player) return goopool;
        }
        return null;
    }

    public static void CleanUp()
    {
        AllGooPools.Clear();
    }

    public static List<GooPool> AllGooPools = new List<GooPool>();
}