using System.Collections;
using UnityEngine;
using Color = UnityEngine.Color;

namespace ObjectWorkshop.LifeImprovement.Objects;

[RegisterInIl2Cpp]
public class AlarmClock(IntPtr ptr) : MonoBehaviour(ptr)
{
    public byte id;
    public PlayerControl Owner;
    public SpriteRenderer myRend;
    public bool emitWaves;
    public GameObject aura;

    public static List<AlarmClock> AllClocks = new List<AlarmClock>();

    public void Start()
    {
        //Lockdown.AvailableObjects.Add(gameObject);

        myRend = gameObject.GetComponent<SpriteRenderer>();

        DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Bounce(transform, 0.7f, 0.45f));
        DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.ColorFade(myRend, new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f), 0.4f));

        id = GetAvailableId();
        AllClocks.Add(this);
    }

    private void Update()
    {
        var alarum = Owner.GetRole<Alarum>();

        if (/*!Utils.IsPowerDown() && */!emitWaves)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.IsDead)
                    continue;

                float dist = Vector2.Distance(player.GetTruePosition(), transform.position);
                if (dist > OptionGroupSingleton<Alarum_Options>.Instance.Radius)
                    continue;

                var sr = aura.GetComponent<SpriteRenderer>();

                if (player.IsFaction(Faction.Infiltrator, true))
                {
                    sr.color = new Color32(255, 80, 80, 85);

                    var stats = alarum.Player.GetPlayerStats();
                    if (stats != null)
                    {
                        stats.ReceivedInformation = true;
                        stats.SlightSuspicion.Add(player);
                    }

                    emitWaves = true;
                    Coroutines.Start(EmitSoundWaves(transform, 1f));
                }
            }
        }
    }

    public IEnumerator EmitSoundWaves(Transform origin, float interval = 1f)
    {
        while (true)
        {
            // Create wave object
            var waveObj = new GameObject("AlarmWave");
            var lr = waveObj.AddComponent<LineRenderer>();

            // Configure line renderer
            lr.useWorldSpace = false;
            lr.loop = true;
            lr.positionCount = 64; // circle points
            lr.startWidth = 0.05f;
            lr.endWidth = 0.05f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = lr.endColor = Color.white;

            if (this.IsDestroyedOrNull())
                yield break;
                
            waveObj.transform.position = origin.position;

            // Start the wave animation
            Coroutines.Start(AnimateWave(lr, OptionGroupSingleton<Alarum_Options>.Instance.Radius, 0.7f));

            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator AnimateWave(LineRenderer lr, float maxRadius, float duration)
    {
        float time = 0f;
        Color startColor = lr.startColor;

        while (time < duration)
        {
            float t = time / duration;
            float radius = Mathf.Lerp(0f, maxRadius, t);

            SetCirclePoints(lr, radius);

            // Fade alpha over time
            float alpha = Mathf.Lerp(1f, 0f, t);
            var color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            lr.startColor = lr.endColor = color;

            time += Time.deltaTime;
            yield return null;
        }

        Destroy(lr.gameObject);
    }

    private void SetCirclePoints(LineRenderer lr, float radius)
    {
        if (this.IsDestroyedOrNull())
           return;
           
        int points = lr.positionCount;
        for (int i = 0; i < points; i++)
        {
            float angle = Mathf.PI * 2f / points * i;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            lr.SetPosition(i, new Vector3(x, y, 0f));
        }
    }

    #region Global
    #endregion
    public static void Begin(PlayerControl player)
    {
        var bubble = new GameObject("AlarumBubble");
        bubble.AddSpriteRenderer(OWAssets.Bubble.LoadAsset(), 30, 100, player.transform.position, new Color32(179, 255, 255, 85), Vector3.one);
        bubble.transform.localScale = Vector3.one * OptionGroupSingleton<Alarum_Options>.Instance.Radius * 2f;

        var clockObj = new GameObject("AlarmClock");
        clockObj.AddSpriteRenderer(OWAssets.AlarmClock.LoadAsset(), 10, 0, player.transform.position, ObjectExtentions.fullColor(), Vector3.one);

        var clock = clockObj.AddComponent<AlarmClock>();
        clock.Owner = player;
        clock.aura = bubble;

        bubble.transform.SetParent(clockObj.transform);
    }

    public static byte GetAvailableId()
    {
        byte id = 0;
        while (AllClocks.Any(x => x.id == id))
        {
            id++;
        }
        return id;
    }

    public static void DestroyGameObject(AlarmClock alarmClock)
    {
        if (alarmClock == null)
            return;

        AllClocks.Remove(alarmClock);
        Destroy(alarmClock.aura);
        Destroy(alarmClock.gameObject);
    }

    public static void DestroyAll()
    {
        foreach (var clock in AllClocks)
        {
            AllClocks.Remove(clock);
            Destroy(clock.aura);
            Destroy(clock.gameObject);
        }
    }

    public static void CleanUp()
    {
        AllClocks.Clear();
    }
}