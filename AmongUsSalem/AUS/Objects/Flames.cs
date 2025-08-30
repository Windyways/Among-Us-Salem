using System.Collections;
using UnityEngine;
using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace ObjectWorkshop.LifeImprovement.Objects;

[RegisterInIl2Cpp]
public class Flames : MonoBehaviour
{
    public byte id;
    public PlayerControl PyreOwner;
    public PlayerControl PlayerOnFire;
    public SpriteRenderer myRend;

    public void Start()
    {
        ObjectWorkshopPlugin.DebugLogMessage(PlayerOnFire.Data.PlayerName + " has been light on fire!");
        myRend = gameObject.GetComponent<SpriteRenderer>();

        id = GetAvailableId();
        AllFlames.Add(this);

        Coroutines.Start(Death());
    }

    public void Update()
    {
        UpdateColor();
        Douse();

        /*if (PlayerOnFire.HasDied())
        {
            DestroyGameObject();
        }*/
    }

    public IEnumerator Death()
    {
        yield return new WaitForSeconds(OptionGroupSingleton<Pyre_Options>.Instance.Duration);

        if (PyreOwner.AmOwner || Debugger.IsDebuggerActive)
        {
            PyreOwner.RpcCustomMurder(PlayerOnFire, true, false, true, false);
        }

        DestroyGameObject();
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

    public void DestroyGameObject()
    {
        if (this != null)
        {
            AllFlames.Remove(this);
            Destroy(gameObject);
        }
    }

    public void Douse()
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (IsInRange(player))
            {
                Begin(PyreOwner, player);
            }
        }
    }

    public bool IsInRange(PlayerControl target)
    {
        if (target == null || gameObject == null) return false;
        if (target.HasDied()/* || target.IsUnderground()*/) return false;
        if (target == PlayerOnFire) return false;
        if (target.IsRole<Pyre>() || target.IsOnFire()) return false;
        
        float dist = Vector2.Distance(gameObject.transform.position, target.transform.position);
        return dist <= 0.5f;
    }

    #region Global
    #endregion
    public static void Begin(PlayerControl player, PlayerControl target)
    {
        GameObject obj = new GameObject("FireEffect");
        obj.transform.SetParent(target.transform);
        obj.AddSpriteRenderer(OWAssets.FlamesSprite.LoadAsset(), 0, 100, new Vector3(target.transform.position.x, target.transform.position.y + 0.4f, target.transform.position.z), ObjectExtentions.fullColor(), Vector3.one);

        Flames flames = obj.AddComponent<Flames>();
        flames.PyreOwner = player;
        flames.PlayerOnFire = target;
    }

    public static byte GetAvailableId()
    {
        byte id = 0;
        while (AllFlames.Any(x => x.id == id))
        {
            id++;
        }
        return id;
    }

    public static void CleanUp()
    {
        AllFlames.Clear();
    }

    public static List<Flames> AllFlames = new List<Flames>();
}