using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;
using Rewired.Utils;
using Random = UnityEngine.Random;

[RegisterInIl2Cpp]
public class Sandstorm(IntPtr ptr) : MonoBehaviour(ptr)
{
    public byte id;
    public PlayerControl Owner;
    public SpriteRenderer myRend;

	public float spawnInterval = 0.03f;
	public float particleSpeedMin = 8f;
	public float particleSpeedMax = 18f;
	private float spawnTimer;
	private float effectTimer;
	private GameObject overlay;
    private DateTime ApplicationTime { get; set; }
    private float SpeedCache { get; set; }
        
    public void Start()
    {
        myRend = gameObject.GetComponent<SpriteRenderer>();
        id = GetAvailableId();

        AllSandstorms.Add(this);

        SpeedCache = PlayerControl.LocalPlayer.MyPhysics.Speed;
        ApplicationTime = DateTime.UtcNow;
        effectTimer = OptionGroupSingleton<Oasis_Options>.Instance.SandstormDuration - 5;
        CreateOverlay();
    }

    private void Update()
    {
		PushObjects();
		WhileActive();
    }

    private void OnDestroy()
    {
        PlayerControl.LocalPlayer.MyPhysics.Speed = SpeedCache;
        AllSandstorms.Remove(this);
    }

    private void FixedUpdate()
    {
        if (PlayerControl.LocalPlayer.HasDied())
        {
            return;
        }

        var timeSpan = DateTime.UtcNow - ApplicationTime;
        var duration = OptionGroupSingleton<Oasis_Options>.Instance.SandstormDuration * 1000f;
        PlayerControl.LocalPlayer.MyPhysics.Speed = SpeedCache * 1 - (duration - (float)timeSpan.TotalMilliseconds) *
            (1 - (GameOptionsManager.Instance.currentNormalGameOptions.PlayerSpeedMod / 1.5f)) / duration;
    }

    public void WhileActive()
    {
        if (!this.IsNullOrDestroyed())
        {
            effectTimer -= Time.deltaTime;
            if (effectTimer <= 0f) Oasis.RpcStopSandstorm(Owner);
            else
            {
                spawnTimer -= Time.deltaTime;
                if (spawnTimer <= 0f)
                {
                    spawnTimer = spawnInterval;
                    SpawnSandParticle();
                }
            }
        }
    }

    private void SpawnSandParticle()
    {
        GameObject particle = new GameObject("SandParticle");
        SpriteRenderer sr = particle.AddComponent<SpriteRenderer>();
        sr.sprite = GetSandSprite();
        sr.sortingOrder = 100;

        Vector2 spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(1.2f, Random.Range(0.1f, 0.9f), Camera.main.nearClipPlane));
        particle.transform.position = spawnPos;
        particle.transform.eulerAngles = new Vector3(0f, 0f, Random.Range(-15f, 15f));

        SandParticle mover = particle.AddComponent<SandParticle>();
        mover.speed = Random.Range(particleSpeedMin, particleSpeedMax);
        mover.lifetime = 2f;
    }

    private void CreateOverlay()
    {
        overlay = new GameObject("SandstormOverlay");
        SpriteRenderer sr = overlay.AddComponent<SpriteRenderer>();
        sr.sprite = OWAssets.SandOverlay.LoadAsset();
        sr.color = new Color(1f, 0.95f, 0.5f, 0.4f);
        sr.sortingOrder = 999;
        overlay.transform.localScale = new Vector3(40f, 28f, 1f);
        overlay.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10f);
        overlay.AddComponent<FollowCamera>();
    }

    public void PushObjects()
    {
        if (!this.IsNullOrDestroyed())
        {
            float pushStrength = 0.5f;
            foreach (GameObject obj in Object.FindObjectsOfType<GameObject>())
            {
                Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 offset = Vector2.left * pushStrength * Time.deltaTime;
                    if (obj.name == "Barricade" || obj.name == "BrownCrate")
                    {
                        rb.MovePosition(rb.position + offset);
                    }
                }
            }
        }
    }

    public void Stop()
    {
        AllSandstorms.Remove(this);
        Destroy(overlay);
        Destroy(gameObject);
    }

    public Sprite GetSandSprite()
    {
        int num = Random.Range(0, 2);
        if (num == 0) return OWAssets.SandParticle1.LoadAsset();
        return OWAssets.SandParticle2.LoadAsset();
    }

    public static void Begin(PlayerControl player)
    {
        GameObject gameObject = new GameObject("SandstormEffect");

        Sandstorm sandstorm = gameObject.AddComponent<Sandstorm>();
        sandstorm.Owner = player;
    }

    public static byte GetAvailableId()
    {
        byte id = 0;
        while (AllSandstorms.Any(x => x.id == id))
        {
            id++;
        }
        return id;
    }

    public static void DestroyAll()
    {
        foreach (Sandstorm sandstorms in AllSandstorms)
        {
            Destroy(sandstorms.gameObject);
        }
        AllSandstorms.Clear();
    }

    public static Sandstorm GetObjectByPlayer(PlayerControl player)
    {
        foreach (Sandstorm sandstorm in AllSandstorms)
        {
            if (sandstorm.Owner == player) return sandstorm;
        }
        return null;
    }

    public static void CleanUp()
    {
        AllSandstorms.Clear();
    }
    public static List<Sandstorm> AllSandstorms = new List<Sandstorm>();
}
