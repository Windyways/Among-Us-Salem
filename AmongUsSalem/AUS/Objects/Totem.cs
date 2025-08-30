using System.Collections;
using UnityEngine;
using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace ObjectWorkshop.LifeImprovement.Objects;

[RegisterInIl2Cpp]
public class Totem : MonoBehaviour
{
    public byte id;
    public PlayerControl Owner;
    public SpriteRenderer myRend;

    public void Start()
    {
        //Lockdown.AvailableObjects.Add(gameObject);
        DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Bounce(gameObject.transform, 0.7f, 0.45f));
        DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.ColorFade(myRend, new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f), 0.4f));

        myRend = gameObject.GetComponent<SpriteRenderer>();

        id = Totem.GetAvailableId();
        Totem.AllTotems.Add(this);
    }

    public void Update()
    {
        UpdateColor();
        if (Owner.IsRole<Totemist>() && Owner.AmOwner)
        {
            Totemist? totemist = Owner.GetRole<Totemist>();
            if (totemist.isWatching)
            {
                GameObject target = totemist.TotemOrder[totemist.totemViewing - 1].gameObject;
                if (target == gameObject)
                {
                    gameObject.SetCameraToObject(Owner);
                }
            }
        }
    }

    public void OnDestroy()
    {
        if (Owner.IsRole<Totemist>())
        {
            Totemist? totemist = Owner.GetRole<Totemist>();
            if (totemist.isWatching)
            {
                GameObject target = totemist.TotemOrder[totemist.totemViewing - 1].gameObject;
                if (target == gameObject)
                {
                    totemist.isWatching = false;
                    LightSource light = Owner.lightSource;
                    light.transform.SetParent(Owner.transform);
                    light.transform.localPosition = Owner.Collider.offset;
                }
            }
            totemist.TotemOrder.Remove(this);
            Totem.AllTotems.Remove(this);
        }
    }

    // Token: 0x06000199 RID: 409 RVA: 0x0000E2DC File Offset: 0x0000C4DC
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

    public void DestroyGameObject()
    {
        if (this != null)
        {
            AllTotems.Remove(this);
            Destroy(gameObject);
        }
    }

    #region Global
    #endregion
    public static Totem Begin(PlayerControl player)
    {
        GameObject gameObject = new GameObject("Totem");
        gameObject.AddSpriteRenderer(OWAssets.TotemSprite.LoadAsset(), 20, 100, player.transform.position, ObjectExtentions.fullColor(), Vector3.one);

        Totem totem = gameObject.AddComponent<Totem>();
        totem.Owner = player;
        return totem;
    }

    public static byte GetAvailableId()
    {
        byte id = 0;
        while (AllTotems.Any(x => x.id == id))
        {
            id++;
        }
        return id;
    }

    public static void CleanUp()
    {
        foreach (Totem totem in AllTotems)
        {
            Destroy(totem.gameObject);
        }
        AllTotems.Clear();
    }

    public static List<Totem> AllTotems = new List<Totem>();
}