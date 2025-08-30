using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

[RegisterInIl2Cpp]
public class DarkCloud(IntPtr ptr) : MonoBehaviour(ptr)
{
    public PlayerControl Owner;
    public byte id;
    private readonly float extraRange = 0.1f;

    public void Start()
    {
        // Lockdown.AvailableObjects.Add(darkcloud);

        id = GetAvailableId();
        AllDarkClouds.Add(this);
        
        // Destroy after duration
        Destroy(gameObject, OptionGroupSingleton<Canopy_Options>.Instance.Duration);
        Coroutines.Start(PulseCloud(gameObject, 2f, 0.05f));
    }

    public IEnumerator PulseCloud(GameObject cloud, float pulseSpeed = 2f, float pulseAmount = 0.05f)
    {
        Vector3 baseScale = cloud.transform.localScale;

        while (cloud != null)
        {
            // Sin wave oscillation for smooth pulsing
            float scaleOffset = Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            cloud.transform.localScale = baseScale * (1f + scaleOffset);
            yield return null;
        }
    }

    public bool IsInRange(PlayerControl target)
    {
        if (target == null || gameObject == null) return false;
        if (target.HasDied()) return false;

        // Position of target relative to cloud
        Vector2 diff = target.transform.position - gameObject.transform.position;

        // Get current scale of cloud (since it's pulsing)
        Vector3 scale = gameObject.transform.localScale;

        // Normalize difference by scale (ellipse math)
        float normX = diff.x / (scale.x * 0.5f); // radius is half scale
        float normY = diff.y / (scale.y * 0.5f);

        // If inside ellipse, (x² + y² <= 1)
        return (normX * normX + normY * normY) <= 1f + extraRange;
    }

    public void DestroyGameObject()
    {
        AllDarkClouds.Remove(this);
        Destroy(gameObject);
    }
    
    #region Global
    #endregion
    public static void Begin(PlayerControl player)
    {
        var obj = new GameObject("DarkCloud");
        obj.AddSpriteRenderer(OWAssets.DarkCloudSprite.LoadAsset(), 200, 100, player.transform.position, ObjectExtentions.fullColor(), Vector3.one * OptionGroupSingleton<Canopy_Options>.Instance.Size);

        var darkcloud = obj.AddComponent<DarkCloud>();
        darkcloud.Owner = player;
    }

    public static void CleanUp()
    {
        foreach (var darkcloud in AllDarkClouds)
        {
            Destroy(darkcloud.gameObject);
        }
        AllDarkClouds.Clear();
    }

    public static List<DarkCloud> AllDarkClouds = new List<DarkCloud>();
    public static byte GetAvailableId()
    {
        byte id = 0;
        while (AllDarkClouds.Any(x => x.id == id))
        {
            id++;
        }
        return id;
    }

    public static DarkCloud GetAll()
    {
        foreach (var darkCloud in AllDarkClouds)
        {
            return darkCloud;
        }
        return null;
    }

    public static DarkCloud GetObjectById(byte id)
    {
        foreach (var darkCloud in AllDarkClouds)
        {
            if (darkCloud.id == id) return darkCloud;
        }
        return null;
    }
}