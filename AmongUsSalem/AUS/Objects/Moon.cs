using System.Collections;
using UnityEngine;
using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace ObjectWorkshop.LifeImprovement.Objects;

[RegisterInIl2Cpp]
public class Moon(IntPtr ptr) : MonoBehaviour(ptr)
{
    public PlayerControl Owner;
    public GameObject beam;
    public bool isAbductOccuring;
    public byte id;

    public void Start()
    {
        // Lockdown.AvailableObjects.Add(moon);

        id = GetAvailableId();
        AllMoons.Add(this);
    }

    public IEnumerator PlaySwirlAndTeleport(UFO ufo, PlayerControl target)
    {
        if (ufo.Player == null || target == null)
            yield break;

        if (!target.HasDied()) target.Immobilize();

        isAbductOccuring = true;

        // Create beam effect
        beam = CreateAbductionBeam(target);

        TouAudio.PlaySound(OWAssets.Abduct_SFX);

        yield return Coroutines.Start(AnimateBeam());

        // Teleport target to Moon position, or dead body if dead
        if (target.HasDied())
        {
            var targetBody = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == target.PlayerId);
            if (targetBody != null)
            {
                targetBody.transform.position = transform.position;
            }
        }
        else
        {
            target.transform.position = transform.position;
            target.Mobilize();
        }

        // Destroy effects after teleport
        if (beam != null) Object.Destroy(beam);

        isAbductOccuring = false;
    }

    private IEnumerator AnimateBeam()
    {
        float duration = OptionGroupSingleton<UFO_Options>.Instance.Duration;
        float time = 0f;

        while (time < duration)
        {
            // Animate beam alpha fade-in/out
            if (beam != null)
            {
                LineRenderer lr = beam.GetComponent<LineRenderer>();
                if (lr != null)
                {
                    float alpha = Mathf.PingPong(time * 2f, 1f); // shimmer
                    Color c = new Color(0f, 1f, 1f, alpha);
                    lr.startColor = c;
                    lr.endColor = new Color(c.r, c.g, c.b, 0f);
                }
            }

            time += Time.deltaTime;
            yield return null;
        }
    }

    private GameObject CreateAbductionBeam(PlayerControl player)
    {
        Transform? target = null;
        
        // Instantiate on the target, or target dead body if dead
        if (player.Data.IsDead)
        {
            var targetBody = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == player.PlayerId);
            if (targetBody != null)
            {
                target = targetBody.transform;
            }
        }
        else
        {
            target = player.transform;
        }

        GameObject newBeam = new GameObject("AbductionBeam");
        newBeam.transform.position = target.position;

        LineRenderer lr = newBeam.AddComponent<LineRenderer>();
        newBeam.AddComponent<SpriteRenderer>();
        
        lr.positionCount = 2;
        lr.widthMultiplier = 0.5f;
        lr.useWorldSpace = true;

        // Just set start/end colors directly
        lr.startColor = Color.cyan;
        lr.endColor = new Color(0f, 1f, 1f, 0f); // fades to transparent

        // Simple material (swap for glow shader if available)
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = Color.cyan;
        lr.material = mat;

        // Top point above player, bottom at player
        Vector3 topPos = target.position + Vector3.up * 4f;
        lr.SetPosition(0, topPos);
        lr.SetPosition(1, target.position);

        return newBeam;
    }

    private void Update()
    {
        UpdateColor();
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
        if (beam != null)
        {
            SpriteRenderer bsr = beam.GetComponent<SpriteRenderer>();
            bsr.color = new Color(1f, 1f, 1f, 1f);
        }
    }
    
    #region Global
    #endregion
    public static void Begin(PlayerControl player)
    {
        var obj = new GameObject("Moon");
        obj.AddSpriteRenderer(OWAssets.MoonSprite.LoadAsset(), 10, 0, player.transform.position, ObjectExtentions.fullColor(), Vector3.one);

        var moon = obj.AddComponent<Moon>();
        moon.Owner = player;
    }

    public static void DestroyAll()
    {
        var moon = GetMoon();
        if (moon != null)
        {
            Destroy(moon.beam);
            Destroy(moon.gameObject); // This prevents UFO from working in the future. why? i dunno.
        }
        AllMoons.Clear();
    }

    public static void CleanUp()
    {
        AllMoons.Clear();
    }

    public static List<Moon> AllMoons = new List<Moon>();
    public static byte GetAvailableId()
    {
        byte id = 0;
        while (AllMoons.Any(x => x.id == id))
        {
            id++;
        }
        return id;
    }

    public static Moon GetMoon()
    {
        if (AllMoons.Count == 0) return null;
        return AllMoons[0];
    }
}