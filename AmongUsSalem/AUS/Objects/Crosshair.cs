using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;
using Rewired.Utils;

[RegisterInIl2Cpp]
public class Crosshair(IntPtr ptr) : MonoBehaviour(ptr)
{
    public byte id;
    public PlayerControl Owner;
    public SpriteRenderer myRend;
    public bool isChronoNullified;
    public PlayerControl currentTarget;
    public Vector2 targetPos;
    public float moveSpeed = 5f;

    public void Start()
    {
        myRend = gameObject.GetComponent<SpriteRenderer>();
        id = GetAvailableId();

        AllCrosshairs.Add(this);
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
        if (!this.IsNullOrDestroyed()) // might fix the error at line 41. (now 44)
        {
            gameObject.SetCameraToObject(Owner);

            Aimsman.RpcMoveCrosshair(Owner);
            currentTarget = FindTarget();
        }
    }

    public PlayerControl FindTarget()
    {
        float killRadius = 0.5f;
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            if (!player.HasDied()/* || player.IsInvincible() || player.IsUnderground()*/ && !ConditionalTargeting(player) && !player.IsFaction(Faction.Infiltrator, true))
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

    public bool ConditionalTargeting(PlayerControl player)
    {
        return player.onLadder && OptionGroupSingleton<Aimsman_Options>.Instance.LadderImmunity;
    }

    public void TryShoot(Aimsman aimsman, PlayerControl overrideTarget = null)
    {
        if (overrideTarget != null)
        {
            aimsman.Player.RpcCustomMurder(overrideTarget, true, true, true, false);
            Aimsman.RpcDestroyCrosshair(Owner);
        }
        else if (currentTarget != null)
        {
            aimsman.Player.RpcCustomMurder(currentTarget, true, true, true, false);
            Aimsman.RpcDestroyCrosshair(Owner);
        }
    }

    public void DestroyGameObject()
    {
        if (Owner.IsRole<Aimsman>())
        {
            var aimsman = Owner.GetRole<Aimsman>();
            if (aimsman.Player.AmOwner || Debugger.IsDebuggerActive)
            {
                aimsman.isAiming = false;

                var button = CustomButtonSingleton<Aimsman_Aim>.Instance;
                button.EffectActive = false;
                button.Timer = OptionGroupSingleton<Aimsman_Options>.Instance.Cooldown;

                LightSource light = Owner.lightSource;
                light.transform.SetParent(Owner.transform);
                light.transform.localPosition = Owner.Collider.offset;
            }

            AllCrosshairs.Remove(this);
            Destroy(gameObject);
        }
    }

    #region Global
    #endregion
    public static void Begin(PlayerControl player)
    {
        DestroyGameObject(GetObjectByPlayer(player));

        GameObject gameObject = new GameObject("Crosshair");
        gameObject.AddSpriteRenderer(OWAssets.SnipeCrosshair.LoadAsset(), 70, 100, player.transform.position, ObjectExtentions.fullColor(), Vector3.one);

        Crosshair crosshair = gameObject.AddComponent<Crosshair>();
        crosshair.Owner = player;
        crosshair.targetPos = player.transform.position;
    }

    public static byte GetAvailableId()
    {
        byte id = 0;
        while (AllCrosshairs.Any(x => x.id == id))
        {
            id++;
        }
        return id;
    }

    public static void DestroyGameObject(Crosshair crosshair)
    {
        if (crosshair != null)
        {
            AllCrosshairs.Remove(crosshair);
            Destroy(crosshair.gameObject);
        }
    }

    public static void DestroyAll()
    {
        foreach (Crosshair crosshairs in AllCrosshairs)
        {
            Destroy(crosshairs.gameObject);
        }
        AllCrosshairs.Clear();
    }

    public static Crosshair GetObjectByPlayer(PlayerControl player)
    {
        foreach (Crosshair crosshair in AllCrosshairs)
        {
            if (crosshair.Owner == player) return crosshair;
        }
        return null;
    }

    public static void CleanUp()
    {
        AllCrosshairs.Clear();
    }

    public static List<Crosshair> AllCrosshairs = new List<Crosshair>();
}